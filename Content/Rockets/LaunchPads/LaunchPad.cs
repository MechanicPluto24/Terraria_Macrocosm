using Macrocosm.Common.Netcode;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Content.Rockets.LaunchPads
{
    public class LaunchPad : TagSerializable
    {
        [NetSync] public bool Active;
        [NetSync] public Point16 StartTile;
        [NetSync] public Point16 EndTile;
        [NetSync] public int RocketID = -1;
        [NetSync] public string CompassCoordinates = "";

        public bool HasRocket => RocketID >= 0;

        public int Width => EndTile.X + 1 - StartTile.X;
        public Rectangle Hitbox => new((int)(StartTile.X * 16f), (int)(StartTile.Y * 16f), Width * 16, 16);
        public Vector2 Position => new(((StartTile.X + (EndTile.X - StartTile.X) / 2f) * 16f), StartTile.Y * 16f);

        private bool isMouseOver;

        public LaunchPad()
        {
            StartTile = new();
            EndTile = new();
        }

        public LaunchPad(int startTileX, int startTileY, int endTileX, int endTileY)
        {
            StartTile = new(startTileX, startTileY);
            EndTile = new(endTileX, endTileY);
        }

        public LaunchPad(int startTileX, int startTileY) : this(startTileX, startTileY, startTileX, startTileY) { }

        public LaunchPad(Point16 startTile) : this(startTile.X, startTile.Y) { }

        public LaunchPad(Point16 startTile, Point16 endTile) : this(startTile.X, startTile.Y, endTile.X, endTile.Y) { }

        public static LaunchPad Create(int startTileX, int startTileY, int endTileX, int endTileY, bool shouldSync = true)
        {
            LaunchPad launchPad = new(startTileX, startTileY, endTileX, endTileY);

            launchPad.CompassCoordinates = Utility.GetCompassCoordinates(launchPad.Position);
            launchPad.Active = true;

            if (shouldSync)
                launchPad.NetSync(MacrocosmSubworld.CurrentID);

            LaunchPadManager.Add(MacrocosmSubworld.CurrentID, launchPad);

            return launchPad;
        }

        public static LaunchPad Create(int startTileX, int startTileY, bool shouldSync = true) => Create(startTileX, startTileY, startTileX, startTileY, shouldSync);
        public static LaunchPad Create(Point16 startTile, bool shouldSync = true) => Create(startTile.X, startTile.Y, shouldSync);
        public static LaunchPad Create(Point16 startTile, Point16 endTile, bool shouldSync = true) => Create(startTile.X, startTile.Y, endTile.X, endTile.Y, shouldSync);

        public void Update()
        {
            int prevRocketId = RocketID;
            RocketID = -1;

            if (Main.tile[StartTile.ToPoint()].TileType != ModContent.TileType<LaunchPadMarker>() || (Main.tile[EndTile.ToPoint()].TileType != ModContent.TileType<LaunchPadMarker>()))
            {
                Active = false;
                NetSync(MacrocosmSubworld.CurrentID);
                return;
            }

            for (int i = 0; i < RocketManager.MaxRockets; i++)
            {
                Rocket rocket = RocketManager.Rockets[i];

                if (rocket.ActiveInCurrentWorld && Hitbox.Intersects(rocket.Bounds))
                    RocketID = i;
            }

            if (RocketID != prevRocketId)
                NetSync(MacrocosmSubworld.CurrentID);

            isMouseOver = Hitbox.Contains(Main.MouseWorld.ToPoint()) && Hitbox.InPlayerInteractionRange(TileReachCheckSettings.Simple);

            if (isMouseOver)
            {
                Main.LocalPlayer.noThrow = 2;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 screenPosition)
        {
            Rectangle rect = Hitbox;
            rect.X -= (int)screenPosition.X;
            rect.Y -= (int)screenPosition.Y;

            if (isMouseOver)
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, rect, Color.Gold * 0.25f);
        }


        /// <summary>
        /// Syncs the launchpad fields with <see cref="NetSyncAttribute"/> across all clients and the server.
        /// </summary>
        public void NetSync(string subworldId, int toClient = -1, int ignoreClient = -1)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;

            ModPacket packet = Macrocosm.Instance.GetPacket();

            if (WriteToPacket(packet, subworldId))
                packet.Send(toClient, ignoreClient);

            packet.Dispose();
        }

        public bool WriteToPacket(ModPacket packet, string subworldId)
        {
            packet.Write((byte)MessageType.SyncLaunchPadData);

            packet.Write(subworldId);

            if (this.NetWriteFields(packet)) // Check if the writer was able to write all the fields.
                return true;

            return false;
        }

        /// <summary>
        /// Syncs a rocket with data from the <see cref="BinaryReader"/>. Don't use this method outside <see cref="PacketHandler.HandlePacket(BinaryReader, int)"/>
        /// </summary>
        /// <param name="reader"></param>
        public static void ReceiveSyncLaunchPadData(BinaryReader reader, int sender)
        {
            string subworldId = reader.ReadString();

            LaunchPad launchPad = new();
            launchPad.NetReadFields(reader);

            LaunchPad existingLaunchPad = LaunchPadManager.GetLaunchPadAtStartTile(subworldId, launchPad.StartTile);
            if (existingLaunchPad is null)
                LaunchPadManager.Add(subworldId, launchPad);

            if (Main.netMode == NetmodeID.Server)
            {
                launchPad.NetSync(subworldId, ignoreClient: sender);

                /*
				ModPacket packet = Macrocosm.Instance.GetPacket();
				launchPad.WriteToPacket(packet, subworldId);

				if (SubworldSystem.AnyActive())
					SubworldSystem.SendToMainServer(Macrocosm.Instance, packet.GetBuffer());
				else
					SubworldSystem.SendToAllSubservers(Macrocosm.Instance, packet.GetBuffer());
				*/
            }
        }

        public LaunchPad Clone() => DeserializeData(SerializeData());

        public static readonly Func<TagCompound, LaunchPad> DESERIALIZER = DeserializeData;

        public TagCompound SerializeData()
        {
            TagCompound tag = new()
            {
                [nameof(Active)] = Active,
                [nameof(StartTile)] = StartTile,
                [nameof(EndTile)] = EndTile,
                [nameof(RocketID)] = RocketID,
                [nameof(CompassCoordinates)] = CompassCoordinates,
            };

            return tag;
        }

        public static LaunchPad DeserializeData(TagCompound tag)
        {
            LaunchPad launchPad = new();

            launchPad.Active = tag.ContainsKey(nameof(Active));

            if (tag.ContainsKey(nameof(RocketID)))
                launchPad.RocketID = tag.GetInt(nameof(RocketID));

            if (tag.ContainsKey(nameof(StartTile)))
                launchPad.StartTile = tag.Get<Point16>(nameof(StartTile));

            if (tag.ContainsKey(nameof(EndTile)))
                launchPad.EndTile = tag.Get<Point16>(nameof(EndTile));

            if (tag.ContainsKey(nameof(CompassCoordinates)))
                launchPad.CompassCoordinates = tag.GetString(nameof(CompassCoordinates));

            return launchPad;
        }
    }
}
