using Macrocosm.Common.Bases.Tiles;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using System.Collections.Generic;
using System.IO;
using Terraria.ModLoader;


namespace Macrocosm.Content.Tiles.Blocks.Terrain
{
    public class QuartzBlock : ModTile, IModifyTileFrame
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;

            TileID.Sets.ChecksForMerge[Type] = true;
            Main.tileMerge[ModContent.TileType<Regolith>()][Type] = true;
            Main.tileMerge[ModContent.TileType<Protolith>()][Type] = true;

            TileID.Sets.CanBeClearedDuringOreRunner[Type] = true;

            MinPick = 225;
            MineResist = 0.5f;
            AddMapEntry(new Color(255, 255, 255));
            HitSound = SoundID.Dig;
            DustType = ModContent.DustType<RegolithDust>();
        }

        public override bool HasWalkDust() => Main.rand.NextBool(3);

        public override void WalkDust(ref int dustType, ref bool makeDust, ref Color color)
        {
            dustType = ModContent.DustType<RegolithDust>();
        }
        public override IEnumerable<Item> GetItemDrops(int i, int j) {
			Tile t = Main.tile[i, j];
			// It can be useful to share a single tile with multiple styles.
			yield return new Item(Mod.Find<ModItem>("QuartzFragment").Type);

		}

        public void ModifyTileFrame(int i, int j, ref int up, ref int down, ref int left, ref int right, ref int upLeft, ref int upRight, ref int downLeft, ref int downRight)
        {
            WorldGen.TileMergeAttempt(-2, ModContent.TileType<Regolith>(), ref up, ref down, ref left, ref right, ref upLeft, ref upRight, ref downLeft, ref downRight);
            WorldGen.TileMergeAttempt(Type, ModContent.TileType<Protolith>(), ref up, ref down, ref left, ref right, ref upLeft, ref upRight, ref downLeft, ref downRight);
        }
    }
}