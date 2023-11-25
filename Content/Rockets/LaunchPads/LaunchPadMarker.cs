using Macrocosm.Common.Netcode;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Rockets.LaunchPads
{
    public enum MarkerState
    {
        Invalid,
        Occupied,
        Vacant,
        Inactive
    };

    public class LaunchPadMarker : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileLighted[Type] = true;

            DustType = -1;
            HitSound = SoundID.Mech;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<LaunchPadMarkerTE>().Hook_AfterPlacement, -1, 0, false);

            TileObjectData.addTile(Type);

            LocalizedText name = CreateMapEntryName();

            AddMapEntry(new Color(200, 200, 200), name);
        }

        public override bool CanPlace(int i, int j)
        {
            return LaunchPadManager.TryGetLaunchPadAtTileCoordinates(MacrocosmSubworld.CurrentID, new(i, j), out _);
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (LaunchPadManager.TryGetLaunchPadAtTileCoordinates(MacrocosmSubworld.CurrentID, new(i, j), out LaunchPad launchPad))
            {
                if (launchPad.HasRocket)
                {
                    fail = true;
                    return;
                }
            }

            ModContent.GetInstance<LaunchPadMarkerTE>().Kill(i, j);
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Utility.DrawTileGlowmask(i, j, spriteBatch, ModContent.Request<Texture2D>("Macrocosm/Content/Rockets/LaunchPads/LaunchPadMarker_Glow").Value, Color.White);
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Main.tile[i, j];
            if (tile.TileFrameX <= (int)MarkerState.Inactive * 18)
            {
                switch (tile.TileFrameX / 18)
                {
                    case ((int)MarkerState.Invalid):
                        r = 255f / 255f;
                        g = 25f / 255f;
                        b = 25f / 255f;
                        break;

                    case ((int)MarkerState.Occupied):
                        r = 249f / 255f;
                        g = 181f / 255f;
                        b = 19f / 255f;
                        break;

                    case ((int)MarkerState.Vacant):
                        r = 124f / 255f;
                        g = 249f / 255f;
                        b = 10f / 255f;
                        break;
                }
            }

            //float mult = 0.8f;
            //r *= mult;
            //g *= mult;
            //b *= mult;
        }
    }

    public class LaunchPadMarkerTE : ModTileEntity
    {
        public int CheckInterval { get; set; } = 30;
        public int MinCheckDistance { get; set; } = 20;
        public int CheckDistance { get; set; } = 40;

        public LaunchPad LaunchPad { get; set; }
        public bool HasLaunchPad => LaunchPad != null;


        public LaunchPadMarkerTE Pair { get; set; }
        public bool HasPair => Pair != null;
        public bool IsPair { get; set; }

        public MarkerState MarkerState { get; set; } = MarkerState.Inactive;
        private MarkerState lastMarkerState = MarkerState.Inactive;

        private int checkTimer;

        public override void Update()
        {
            if (IsPair)
                return;

            checkTimer++;

            if (checkTimer >= CheckInterval)
            {
                checkTimer = 0;

                MarkerState = MarkerState.Inactive;

                int x = Position.X;
                int y = Position.Y;

                if (CheckAdjacentMarkers(x, y, out LaunchPadMarkerTE pair))
                {
                    Pair = pair;
                    Pair.IsPair = true;

                    LaunchPad = LaunchPadManager.GetLaunchPadAtStartTile(MacrocosmSubworld.CurrentID, new(x, y));
                    LaunchPad ??= LaunchPad.Create(x, y, Pair.Position.X, Pair.Position.Y);
                    Pair.LaunchPad = LaunchPad;

                    if (LaunchPad.HasRocket)
                        MarkerState = MarkerState.Occupied;
                    else
                        MarkerState = MarkerState.Vacant;
                }

                if (HasPair)
                    Pair.MarkerState = MarkerState;

                if (MarkerState != lastMarkerState)
                    OnStateChanged();

                lastMarkerState = MarkerState;
            }

            Main.tile[Position.ToPoint()].TileFrameX = GetFrame();

            if (HasPair)
                Main.tile[Pair.Position.ToPoint()].TileFrameX = GetFrame();
        }

        private bool CheckAdjacentMarkers(int x, int y, out LaunchPadMarkerTE pair)
        {
            int startCheck = x;

            x = startCheck;
            while (x < startCheck + CheckDistance)
            {
                x++;

                Tile tile = Main.tile[x, y];
                if (tile.HasTile)
                {
                    if (tile.TileType == ModContent.TileType<LaunchPadMarker>())
                    {
                        bool result = TileEntity.ByPosition.TryGetValue(new(x, y), out TileEntity foundPair);
                        pair = foundPair as LaunchPadMarkerTE;
                        return result;
                    }
                    else if (HasPair && WorldGen.SolidOrSlopedTile(tile))
                    {
                        MarkerState = MarkerState.Invalid;
                        pair = Pair;

                        if (!TileEntity.ByPosition.TryGetValue(Pair.Position, out _))
                            Pair = null;

                        return false;
                    }
                }
            }

            // No valid adjacent markers found
            pair = null;
            return false;
        }

        private void OnStateChanged()
        {
            if (MarkerState is MarkerState.Invalid or MarkerState.Inactive)
            {
                if (HasLaunchPad)
                {
                    LaunchPadManager.Remove(MacrocosmSubworld.CurrentID, LaunchPad);
                    LaunchPad = null;
                }
            }

            Main.tile[Position.ToPoint()].TileFrameX = GetFrame();

            if (HasPair)
                Main.tile[Pair.Position.ToPoint()].TileFrameX = GetFrame();

            NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, ID, Position.X, Position.Y);
        }

        private short GetFrame() => (short)((int)MarkerState * 18);

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write((byte)MarkerState);

            writer.Write(HasPair);
            if (HasPair)
            {
                writer.WritePoint16(Pair.Position);
            }

            writer.Write(HasLaunchPad);
            if (HasLaunchPad)
            {
                writer.WritePoint16(LaunchPad.StartTile);
            }

        }

        public override void NetReceive(BinaryReader reader)
        {
            MarkerState = (MarkerState)reader.ReadByte();
            Utility.Chat(MarkerState.ToString(), sync: false);

            bool hasPair = reader.ReadBoolean();
            if (hasPair)
            {
                TileEntity.ByPosition.TryGetValue(reader.ReadPoint16(), out TileEntity foundPair);
                Pair = foundPair as LaunchPadMarkerTE;
            }

            bool hasLaunchPad = reader.ReadBoolean();
            if (hasLaunchPad)
            {
                LaunchPad = LaunchPadManager.GetLaunchPadAtStartTile(MacrocosmSubworld.CurrentID, reader.ReadPoint16());
            }


            Main.tile[Position.ToPoint()].TileFrameX = GetFrame();

            if (HasPair)
                Main.tile[Pair.Position.ToPoint()].TileFrameX = GetFrame();
        }

        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile && tile.TileType == ModContent.TileType<LaunchPadMarker>();
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
        {
            TileObjectData tileData = TileObjectData.GetTileData(type, style, alternate);

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                //Sync the entire multitile's area. 
                NetMessage.SendTileSquare(Main.myPlayer, i, j, tileData.Width, tileData.Height);

                //Sync the placement of the tile entity with other clients
                NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, i, j, Type);

                return -1;
            }

            int placedEntity = Place(i, j);

            return placedEntity;
        }

        public override void OnNetPlace()
        {
            NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, ID, Position.X, Position.Y);
        }

        public override void OnKill()
        {
            if (HasPair)
            {
                Pair.MarkerState = MarkerState.Inactive;
                Pair.IsPair = false;
                Pair.LaunchPad = null;
                NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, Pair.ID, Pair.Position.X, Pair.Position.Y);
            }
        }
    }
}
