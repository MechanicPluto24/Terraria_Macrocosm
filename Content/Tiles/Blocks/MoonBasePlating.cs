using Macrocosm.Common.TileFrame;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Tiles.Blocks
{
	public class MoonBasePlating : ModTile
    {
		public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileMergeDirt[Type] = false;
            //Main.tileBrick[Type] = true;
			//TileID.Sets.GemsparkFramingTypes[Type] = Type;

			DustType = ModContent.DustType<MoonBasePlatingDust>();

            MinPick = 225;
            MineResist = 4f;

            AddMapEntry(new Color(180, 180, 180));
        }

		public override bool Slope(int i, int j)
		{
			return true;
		}

		public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
		{
			if (Main.tile[i, j].IsSloped())
			{
				drawData.drawTexture = ModContent.Request<Texture2D>(Utility.GetPath(this) + "_Sloped").Value;
			}
		}

		public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
		{
		}

		public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
		{
			return TileFraming.PlatingStyle(i, j);
		}
	}
}