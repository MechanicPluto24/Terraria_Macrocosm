using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Furniture
{
    public class IndustrialLoom : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Width = 4;
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.StyleWrapLimit = 2;
            TileObjectData.addTile(Type);

            HitSound = SoundID.Dig;
            DustType = -1;

            AnimationFrameHeight = 36;

            AddMapEntry(new Color(62, 62, 66), CreateMapEntryName());
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            int type;
            if (frameX == 0)
                type = ModContent.ItemType<Items.Placeable.Furniture.TitaniumLoom>();
            else
                type = ModContent.ItemType<Items.Placeable.Furniture.AdamantiteLoom>();

            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, type);
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            /*const*/
            int ticksPerFrame = 3;
            const int frameCount = 7;

            if (++frameCounter >= ticksPerFrame)
            {
                frameCounter = 0;
                frame = ++frame % frameCount;
            }
        }
    }
}
