using Macrocosm.Common.Drawing;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Systems.UI;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets.LaunchPads;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Special
{
    public enum MarkerState
    {
        Inactive,
        Invalid,
        Occupied,
        Vacant
    };

    public class LaunchPadMarker : ModTile
    {
        private static Asset<Texture2D> glowmask;
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileLighted[Type] = true;

            TileID.Sets.AvoidedByMeteorLanding[Type] = true;
            TileID.Sets.PreventsSandfall[Type] = true;
            TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;

            DustType = -1;
            HitSound = SoundID.Mech;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.AnchorInvalidTiles = [
                TileID.MagicalIceBlock,
                TileID.Boulder,
                TileID.BouncyBoulder,
                TileID.LifeCrystalBoulder,
                TileID.RollingCactus
            ];
            TileObjectData.addTile(Type);

            LocalizedText name = CreateMapEntryName();

            AddMapEntry(new Color(200, 200, 200), name);

            RegisterItemDrop(ModContent.ItemType<Items.Special.LaunchPadMarker>(), 0, 1, 2, 3);
        }

        public static void SetState(int i, int j, MarkerState state)
        {
            Tile tile = Main.tile[i, j];
            if (tile.HasTile && tile.TileType == ModContent.TileType<LaunchPadMarker>())
            {
                tile.TileFrameX = (short)(18 * (short)state);
            }
        }

        public static void SetState(Point16 coords, MarkerState state) => SetState(coords.X, coords.Y, state);
        public static void SetState(Point coords, MarkerState state) => SetState(coords.X, coords.Y, state);

        public override bool RightClick(int i, int j)
        {
            if (Utility.CoordinatesOutOfBounds(i + LaunchPad.MaxWidth, j) || Utility.CoordinatesOutOfBounds(i - LaunchPad.MaxWidth, j))
                return false;

            if (LaunchPadManager.TryGetLaunchPadAtTileCoordinates(MacrocosmSubworld.CurrentID, new(i, j), out _))
                return false;

            int tileY = j;
            SetState(i, j, MarkerState.Invalid);

            bool tooCloseRight = false;
            bool foundObstructionRight = false;
            bool foundGapRight = false;
            bool foundMarkerRight = false;
            for (int tileX = i + 1; tileX < i + LaunchPad.MaxWidth; tileX++)
            {
                Tile tile = Main.tile[tileX, tileY];
                if (WorldGen.SolidOrSlopedTile(tileX, tileY))
                {
                    foundObstructionRight = true;
                }

                if (!WorldGen.SolidOrSlopedTile(tileX, tileY + 1))
                {
                    foundGapRight = true;
                }

                if (tile.TileType == Type)
                {
                    foundMarkerRight = true;

                    if (tileX - i < LaunchPad.MinWidth)
                    {
                        tooCloseRight = !foundObstructionRight && !foundGapRight;
                    }
                    else if (!tooCloseRight && !foundObstructionRight && !foundGapRight)
                    {
                        LaunchPad.Create(i, j, tileX, tileY);
                        return true;
                    }
                }
            }

            bool tooCloseLeft = false;
            bool foundObstructionLeft = false;
            bool foundGapLeft = false;
            bool foundMarkerLeft = false;
            for (int tileX = i - 1; tileX > i - LaunchPad.MaxWidth; tileX--)
            {
                Tile tile = Main.tile[tileX, tileY];
                if (WorldGen.SolidOrSlopedTile(tileX, tileY))
                {
                    foundObstructionLeft = true;
                }

                if (!WorldGen.SolidOrSlopedTile(tileX, tileY + 1))
                {
                    foundGapLeft = true;
                }

                if (tile.TileType == Type)
                {
                    foundMarkerLeft = true;

                    if (i - tileX < LaunchPad.MinWidth)
                    {
                        tooCloseLeft = !foundObstructionLeft && !foundGapLeft;
                    }
                    else if (!tooCloseLeft && !foundObstructionLeft && !foundGapLeft)
                    {
                        LaunchPad.Create(tileX, tileY, i, j);
                        return true;
                    }
                }
            }

            if (tooCloseLeft && foundMarkerLeft || tooCloseRight && foundMarkerRight)
            {
                // The launch pad markers are too close to each other.
                Main.NewText(Language.GetText("Mods.Macrocosm.StatusMessages.LaunchpadMarker.TooClose"), Color.Yellow);
                return false;
            }

            if (foundGapLeft && foundMarkerLeft && !foundObstructionLeft || foundGapRight && foundMarkerRight && !foundObstructionRight)
            {
                // The area is not flat, can't place launchpad here.
                Main.NewText(Language.GetText("Mods.Macrocosm.StatusMessages.LaunchpadMarker.FoundGap"), Color.Yellow);
                return false;
            }

            if (foundObstructionRight && foundMarkerRight || foundObstructionLeft && foundMarkerLeft)
            {
                // There is a solid block, can't place launch pad here.
                Main.NewText(Language.GetText("Mods.Macrocosm.StatusMessages.LaunchpadMarker.SolidBlock"), Color.Yellow);
                return false;
            }

            // Found no other valid launch pad marker in the vicinity.
            Main.NewText(Language.GetText("Mods.Macrocosm.StatusMessages.LaunchpadMarker.NoPair"), Color.Yellow);
            return false;
        }

        public override void MouseOver(int i, int j)
        {
            if (!LaunchPadManager.TryGetLaunchPadAtTileCoordinates(MacrocosmSubworld.CurrentID, new(i, j), out _) && !UISystem.Active)
            {
                Main.LocalPlayer.noThrow = 2;
                CursorIcon.Current = CursorIcon.Wrench;
            }
        }

        public override bool CanPlace(int i, int j)
        {
            return !LaunchPadManager.TryGetLaunchPadAtTileCoordinates(MacrocosmSubworld.CurrentID, new(i, j), out _);
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (LaunchPadManager.TryGetLaunchPadAtTileCoordinates(MacrocosmSubworld.CurrentID, new(i, j), out LaunchPad launchPad))
            {
                if (launchPad.HasRocket || !launchPad.Inventory.IsEmpty)
                {
                    fail = true;
                    return;
                }
            }
        }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 0 : num;
        public override bool KillSound(int i, int j, bool fail) => !fail;

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            glowmask ??= ModContent.Request<Texture2D>(Texture + "_Glow");
            Utility.DrawTileExtraTexture(i, j, spriteBatch, glowmask, applyPaint: true);

            /*
            float cycle = Utility.PositiveSineWave(130 + ((i % 10) + (j % 10)));
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    Vector2 offset = new Vector2(x, y) * (0.25f + 0.25f * cycle);
                    Utility.DrawTileGlowmask(i, j, spriteBatch, glowmask, drawOffset: offset);
                }
            }
            */
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Main.tile[i, j];
            if (tile.TileFrameX > (int)MarkerState.Inactive * 18)
            {
                switch (tile.TileFrameX / 18)
                {
                    case (int)MarkerState.Invalid:
                        r = 255f / 255f;
                        g = 25f / 255f;
                        b = 25f / 255f;
                        break;

                    case (int)MarkerState.Occupied:
                        r = 249f / 255f;
                        g = 181f / 255f;
                        b = 19f / 255f;
                        break;

                    case (int)MarkerState.Vacant:
                        r = 124f / 255f;
                        g = 249f / 255f;
                        b = 10f / 255f;
                        break;
                }
            }

            float mult = 0.6f + 0.2f * Utility.PositiveSineWave(130 + (i % 10 + j % 10));
            r *= mult;
            g *= mult;
            b *= mult;
        }
    }
}
