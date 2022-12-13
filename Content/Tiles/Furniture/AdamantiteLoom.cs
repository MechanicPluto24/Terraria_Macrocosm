using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Furniture
{
    public class AdamantiteLoom : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Width = 4;
			TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.addTile(Type);

            HitSound = SoundID.Dig;

			AnimationFrameHeight = 36;

			AddMapEntry(new Color(62, 62, 66), Language.GetText("Adamantite Loom"));
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, ModContent.ItemType<Items.Placeable.Furniture.AdamantiteLoom>());
        }

		public override void AnimateTile(ref int frame, ref int frameCounter)
		{
			/*const*/ int ticksPerFrame = 9;
            const int frameCount = 7;

			if (++frameCounter >= ticksPerFrame) {
				frameCounter = 0;
				frame = ++frame % frameCount;
			}
 		}
	}
}
