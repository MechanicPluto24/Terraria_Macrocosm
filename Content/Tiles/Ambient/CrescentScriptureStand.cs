using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Systems;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Ambient
{
    public class CrescentScriptureStand : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileSolidTop[Type] = false;
            Main.tileTable[Type] = false;
            Main.tileNoAttach[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;

            MinPick = 1;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.Origin = new Point16(0, 5);
            TileObjectData.newTile.Width = 3;
            TileObjectData.newTile.Height = 6;
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 16, 16, 16];
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);

            TileID.Sets.DisableSmartCursor[Type] = true;
            AddMapEntry(new Color(100, 130, 110));
            RegisterItemDrop(ModContent.ItemType<Items.Weapons.Melee.CrescentScripture>(), 0);

        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (float f = 0f; f < 20f; f += 1f)
                {
                    int time = Main.rand.Next(20, 80);
                    Particle.Create<PrettySparkle>((p) =>
                    {
                        p.Position = new Vector2(i, j) * 16f + new Vector2(16f, 32f) + Main.rand.NextVector2Circular(16f, 16f);
                        p.Color = Main.rand.NextBool() ? new Color(154, 248, 224) : new Color(164, 101, 124);
                        p.Scale = new Vector2(1f + Main.rand.NextFloat() * 2f, 0.7f + Main.rand.NextFloat() * 0.7f);
                        p.Velocity = new Vector2(0f, -2f) * Main.rand.NextFloat();
                        p.FadeInNormalizedTime = 5E-06f;
                        p.FadeOutNormalizedTime = 0.95f;
                        p.TimeToLive = time;
                        p.FadeOutEnd = time;
                        p.FadeInEnd = time / 2;
                        p.FadeOutStart = time / 2;
                        p.AdditiveAmount = 0.35f;
                        p.DrawHorizontalAxis = true;
                        p.DrawVerticalAxis = true;
                    }, shouldSync: true);
                }

                WorldFlags.SetFlag(ref WorldFlags.LuminiteShrineUnlocked, true);
                NetMessage.SendData(MessageID.WorldData);
            }
        }
    }
}