using Macrocosm.Common.Netcode;
using Macrocosm.Content.Items.Connectors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Common.Systems.Connectors
{
    public class ConnectorSystem : ModSystem
    {
        public static ConnectorSystem Instance => ModContent.GetInstance<ConnectorSystem>();
        public static ConnectorSystem Map => Instance;

        private Dictionary<Point16, ConnectorData> map = new();

        private Asset<Texture2D> texture;

        public override void Load()
        {
            texture = ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "Connectors");
        }

        public override void Unload()
        {
        }

        public override void ClearWorld()
        {
            map = new();
        }

        public ConnectorData this[Point16 point]
        {
            get
            {
                if (!map.TryGetValue(point, out var data))
                    data = new();

                return data;
            }

            private set
            {
                map[point] = value;
            }
        }

        public ConnectorData this[int x, int y]
        {
            get => this[new Point16(x, y)];
            private set => this[new Point16(x, y)] = value;
        }

        public ConnectorData this[Point point]
        {
            get => this[new Point16(point)];
            private set => this[new Point16(point)] = value;
        }
        public bool ShouldDrawWires
        {
            get
            {
                Item item = Main.LocalPlayer.HeldItem;
                if (item.mech)
                    return true;

                return false;
            }
        }

        public ConnectorVisibility ConveyorVisibility { get; set; } = ConnectorVisibility.Normal;

        public static void PlaceConnector(Point coords, ConnectorType type, bool sync = true) => PlaceWire(coords.X, coords.Y, type, sync);
        public static void PlaceWire(int x, int y, ConnectorType type, bool sync = true)
        {
            Map[x, y] = new(type);

            if (sync && Main.netMode != NetmodeID.SinglePlayer)
                SyncConnector(x, y, type);
        }

        public static void CutWire(Point coords, bool sync = true) => CutWire(coords.X, coords.Y, sync);
        public static void CutWire(int x, int y, bool sync = true)
        {
            int itemDrop = -1;
            int dustType = -1;
            if (Map[x, y].Conveyor)
            {
                itemDrop = ModContent.ItemType<Conveyor>();
                dustType = DustID.Copper;
            }

            Map[x, y] = new();

            if (itemDrop > 0 && Main.netMode != NetmodeID.MultiplayerClient)
                Item.NewItem(new EntitySource_TileBreak(x, x, "ConnectorCut"), new Vector2(x * 16 + 8, y * 16 + 8), itemDrop);

            if (dustType > 0)
                for (int i = 0; i < 10; i++)
                    Dust.NewDustDirect(new Vector2(x * 16 + 8, y * 16 + 8), 1, 1, dustType);

            if (sync && Main.netMode != NetmodeID.SinglePlayer)
                SyncConnector(x, y, ConnectorType.None);
        }

        public override void PostDrawTiles()
        {
            if (!ShouldDrawWires)
                return;

            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, default, default, null, Main.GameViewMatrix.ZoomMatrix);

            Rectangle frame = new(0, 0, 16, 16);
            Vector2 zero = Vector2.Zero;
            Point screenOverdrawOffset = Main.GetScreenOverdrawOffset();

            int startX = (int)((Main.screenPosition.X - zero.X) / 16f - 1f);
            int endX = (int)((Main.screenPosition.X + Main.screenWidth + zero.X) / 16f) + 2;
            int startY = (int)((Main.screenPosition.Y - zero.Y) / 16f - 1f);
            int endY = (int)((Main.screenPosition.Y + Main.screenHeight + zero.Y) / 16f) + 5;
            if (startX < 0)
                startX = 0;

            if (endX > Main.maxTilesX)
                endX = Main.maxTilesX;

            if (startY < 0)
                startY = 0;

            if (endY > Main.maxTilesY)
                endY = Main.maxTilesY;

            for (int i = startX + screenOverdrawOffset.X; i < endX - screenOverdrawOffset.X; i++)
            {
                for (int j = startY + screenOverdrawOffset.Y; j < endY - screenOverdrawOffset.Y; j++)
                {
                    ConnectorData data = this[i, j];

                    if (data.AnyWire)
                    {
                        int frameY;
                        if (data.Conveyor)
                        {
                            frameY = 0;
                            int frameX = 0;
                            if (this[i, j - 1].Conveyor)
                                frameX += 18;

                            if (this[i + 1, j].Conveyor)
                                frameX += 36;

                            if (this[i, j + 1].Conveyor)
                                frameX += 72;

                            if (this[i - 1, j].Conveyor)
                                frameX += 144;

                            frame.Y = frameY;
                            frame.X = frameX;

                            Color color = GetColor(i, j, ConveyorVisibility);

                            if (color != Color.Transparent)
                                spriteBatch.Draw(texture.Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, frame, color, 0f, zero, 1f, SpriteEffects.None, 0f);
                        }
                    }
                }
            }

            spriteBatch.End();
        }

        private static Color GetColor(int i, int j, ConnectorVisibility visibilty)
        {
            Color color = Lighting.GetColor(i, j);
            switch (visibilty)
            {
                case ConnectorVisibility.Bright:
                    color = Color.White;
                    break;
                case ConnectorVisibility.Subtle:
                    color *= 0.5f;
                    break;
                case ConnectorVisibility.Hidden:
                    color = Color.Transparent;
                    break;
            }
            return color;
        }

        public static void SyncConnector(int x, int y, ConnectorType type, int toClient = -1, int ignoreClient = -1)
        {
            ModPacket packet = Macrocosm.Instance.GetPacket();

            packet.Write((byte)MessageType.SyncPowerWire);
            packet.Write((ushort)x);
            packet.Write((ushort)y);
            packet.Write((byte)type);

            packet.Send(toClient, ignoreClient);
        }

        public static void ReceiveSyncConnector(BinaryReader reader, int sender)
        {
            int x = reader.ReadUInt16();
            int y = reader.ReadUInt16();
            ConnectorType type = (ConnectorType)reader.ReadByte();

            if (type != ConnectorType.None)
                PlaceWire(x, y, type, sync: false);
            else
                CutWire(x, y, sync: false);

            if (Main.netMode == NetmodeID.Server)
                SyncConnector(x, y, type, ignoreClient: sender);
        }

        public override void SaveWorldData(TagCompound tag)
        {
            List<TagCompound> wireTags = new();

            foreach (var kvp in map)
            {
                TagCompound entry = new();
                entry["coords"] = kvp.Key;
                entry["data"] = kvp.Value;
                wireTags.Add(entry);
            }

            tag[nameof(map)] = wireTags;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            map = new Dictionary<Point16, ConnectorData>();

            if (tag.ContainsKey(nameof(map)))
            {
                var wireTags = tag.GetList<TagCompound>(nameof(map));
                foreach (var entry in wireTags)
                {
                    if (entry.ContainsKey("coords") && entry.ContainsKey("data"))
                    {
                        Point16 coords = entry.Get<Point16>("coords");
                        ConnectorData data = entry.Get<ConnectorData>("data");
                        map[coords] = data;
                    }
                }
            }
        }
    }
}
