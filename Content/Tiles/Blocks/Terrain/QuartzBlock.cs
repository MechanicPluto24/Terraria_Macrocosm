using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace Macrocosm.Content.Tiles.Blocks.Terrain
{
    public class QuartzBlock : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBrick[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileShine2[Type] = true;
            Main.tileShine[Type] = 975;
            TileID.Sets.CanBeClearedDuringOreRunner[Type] = true;

            MinPick = 225;
            MineResist = 0.5f;
            AddMapEntry(new Color(255, 255, 255));
            HitSound = SoundID.Tink;
            DustType = ModContent.DustType<RegolithDust>();

            RegisterItemDrop(ModContent.ItemType<Items.Ores.QuartzFragment>());
        }

        public override bool CanExplode(int i, int j)
        {
            return false;
        }

        /*
        public void ModifyTileFrame(int i, int j, ref int up, ref int down, ref int left, ref int right, ref int upLeft, ref int upRight, ref int downLeft, ref int downRight)
        {
            WorldGen.TileMergeAttempt(-2, ModContent.TileType<Regolith>(), ref up, ref down, ref left, ref right, ref upLeft, ref upRight, ref downLeft, ref downRight);
            WorldGen.TileMergeAttempt(Type, ModContent.TileType<Protolith>(), ref up, ref down, ref left, ref right, ref upLeft, ref upRight, ref downLeft, ref downRight);
        }
        */
    }
}