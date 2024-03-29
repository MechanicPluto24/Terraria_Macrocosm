using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Tiles.Blocks
{
    public class IrradiatedRock : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileMerge[Type][ModContent.TileType<Protolith>()] = true;
            Main.tileMerge[Type][ModContent.TileType<Regolith>()] = true;
            MinPick = 275;
            MineResist = 3f;
            AddMapEntry(new Color(199, 199, 184));
            HitSound = SoundID.Tink;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            //return TileBlend.BlendLikeDirt(i, j, new int[] { ModContent.TileType<Regolith>(), ModContent.TileType<Protolith>() });
            return true;
        }
    }
}