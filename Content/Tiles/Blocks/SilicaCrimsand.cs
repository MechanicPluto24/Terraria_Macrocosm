using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Projectiles.Environment.Sands;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Tiles.Blocks
{
    public class SilicaCrimsand : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBrick[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileBlockLight[Type] = true;
            Main.tileSand[Type] = false;

            //TileID.Sets.TouchDamageSands[Type] = 15;

            TileID.Sets.Conversion.Sand[Type] = true; // Allows Clentaminator solutions to convert this tile to their respective Sand tiles.
            TileID.Sets.ForAdvancedCollision.ForSandshark[Type] = true; // Allows Sandshark enemies to "swim" in this sand.
            TileID.Sets.Falling[Type] = true;
            AddMapEntry(new Color(155, 156, 134), CreateMapEntryName());

            DustType = ModContent.DustType<SilicaCrimsandDust>();
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
             => Utility.SandTileFrame(i, j, ModContent.ProjectileType<SilicaCrimsandProjectile>());

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    }
}