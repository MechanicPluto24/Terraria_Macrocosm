using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using System.Linq;
using System.Collections.Generic;
using Terraria.UI.Chat;
using Terraria.GameContent;
using Macrocosm.Common.Utils;
using Terraria.ID;
using Macrocosm.Content.Items.Wires;

namespace Macrocosm.Common.Systems.Power
{
    public enum WireType
    {
        None,
        Copper,
        // ...
    }

    public enum WireVisibility
    {
        Normal,
        Bright,
        Subtle,
        Hidden
    }

    public class PowerWiring : ModSystem
    {
        public static PowerWiring Map => ModContent.GetInstance<PowerWiring>();
        private Dictionary<Point16, WireData> wireMap = new();

        private Asset<Texture2D> wireTexture;

        public WireData this[Point16 point]
        {
            get
            {
                if (!wireMap.TryGetValue(point, out var data))
                    data = new();

                return data;
            }

            private set
            {
                wireMap[point] = value;
                CircuitSystem.SearchCircuits();
            }
        }

        public WireData this[int x, int y]
        {
            get => this[new Point16(x, y)];
            private set => this[new Point16(x, y)] = value;
        }

        public WireData this[Point point]
        {
            get => this[new Point16(point)];
            private set => this[new Point16(point)] = value;
        }

        public static void PlaceWire(Point coords, WireType type) => Map[coords] = new(type);
        public static void PlaceWire(int x, int y, WireType type) => Map[x, y] = new(type);

        public static void CutWire(Point coords) => CutWire(coords.X, coords.Y);
        public static void CutWire(int x, int y)
        {
            int itemDrop = -1;
            int dustType = -1;
            if (Map[x, y].CopperWire)
            {
                itemDrop = ModContent.ItemType<CopperWire>();
                dustType = DustID.Copper;
            }

            Map[x, y] = new();

            if (itemDrop > 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Item.NewItem(new EntitySource_TileBreak(x, x, "PowerWireCut"), new Vector2(x * 16 + 8, y * 16 + 8), itemDrop);
            }

            if (dustType > 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    Dust.NewDustDirect(new Vector2(x * 16 + 8, y * 16 + 8), 1, 1, DustID.Copper);
                }
            }
        }

        public override void Load()
        {
            wireTexture = ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "PowerWire");
        }

        public override void Unload()
        {
        }

        public override void ClearWorld()
        {
            wireMap = new();
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

        public WireVisibility CopperVisibility { get; set; } = WireVisibility.Normal;

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
                    WireData data = this[i, j];

                    if (data.AnyWire)
                    {
                        int frameY = 0;
                        if (data.CopperWire)
                        {
                            int frameX = 0;
                            if (this[i, j - 1].CopperWire)
                                frameX += 18;

                            if (this[i + 1, j].CopperWire)
                                frameX += 36;

                            if (this[i, j + 1].CopperWire)
                                frameX += 72;

                            if (this[i - 1, j].CopperWire)
                                frameX += 144;

                            frame.Y = frameY;
                            frame.X = frameX;

                            Color color = GetWireColor(i, j, CopperVisibility);

                            if (color != Color.Transparent)
                                spriteBatch.Draw(wireTexture.Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, frame, color, 0f, zero, 1f, SpriteEffects.None, 0f);
                        }
                    }
                }
            }

            spriteBatch.End();
        }

        private static Color GetWireColor(int i, int j, WireVisibility visibilty)
        {
            Color color = Lighting.GetColor(i, j);
            switch (visibilty)
            {
                case WireVisibility.Bright:
                    color = Color.White;
                    break;
                case WireVisibility.Subtle:
                    color *= 0.5f;
                    break;
                case WireVisibility.Hidden:
                    color = Color.Transparent;
                    break;
            }
            return color;
        }

        public static void MakeWireDust(Point coords)
        {
        }

        public override void SaveWorldData(TagCompound tag)
        {
            List<TagCompound> wireTags = new();

            foreach (var kvp in wireMap)
            {
                TagCompound entry = new();
                entry["coords"] = kvp.Key;
                entry["data"] = kvp.Value;
                wireTags.Add(entry);
            }

            tag[nameof(wireMap)] = wireTags;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            wireMap = new Dictionary<Point16, WireData>();

            if (tag.ContainsKey(nameof(wireMap)))
            {
                var wireTags = tag.GetList<TagCompound>(nameof(wireMap));
                foreach (var entry in wireTags)
                {
                    if (entry.ContainsKey("coords") && entry.ContainsKey("data"))
                    {
                        Point16 coords = entry.Get<Point16>("coords");
                        WireData data = entry.Get<WireData>("data");
                        wireMap[coords] = data;
                    }
                }
            }
        }
    }
}
