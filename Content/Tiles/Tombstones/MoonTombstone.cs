using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Tombstones
{
    public class MoonTombstone : ModTile
    {

        public const int frameHeight = 36;

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileSign[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.StyleWrapLimit = 2;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            HitSound = SoundID.Dig;
            DustType = ModContent.DustType<RegolithDust>();

            AddMapEntry(new Color(180, 180, 180));
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, StyleToItem(frameY / frameHeight));
        }

        public static int StyleToItem(int style) => styleToItem[style];

        // since values are unique here, this is acceptable 
        public static int ItemToStyle(int item)
        {
            foreach (var pair in styleToItem)
            {
                if (pair.Value == item)
                    return pair.Key * 2 + 1;
            }

            return 0;
        }

        private static Dictionary<int, int> styleToItem = new()
        {
            { 0, ModContent.ItemType<Items.Placeable.Tombstones.MoonTombstone>() },
            { 1, ModContent.ItemType<Items.Placeable.Tombstones.MoonGoldTombstone>() }
        };

    }
}
