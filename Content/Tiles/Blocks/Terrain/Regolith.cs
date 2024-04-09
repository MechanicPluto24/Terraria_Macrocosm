using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Tiles.Blocks.Terrain
{
    public class Regolith : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileMerge[Type][ModContent.TileType<Protolith>()] = true;
            Main.tileMerge[Type][ModContent.TileType<IrradiatedRock>()] = true;
            MinPick = 225;
            MineResist = 3f;
            AddMapEntry(new Color(220, 220, 220));
            HitSound = SoundID.Dig;
            DustType = ModContent.DustType<RegolithDust>();
        }

        public override bool HasWalkDust() => Main.rand.NextBool(3);

        public override void WalkDust(ref int dustType, ref bool makeDust, ref Color color)
        {
            dustType = ModContent.DustType<RegolithDust>();
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            return true;
        }
    }
}