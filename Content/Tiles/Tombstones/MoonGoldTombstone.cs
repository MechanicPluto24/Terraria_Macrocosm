using Macrocosm.Common.Bases.Tiles;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Tombstones
{
    public class MoonGoldTombstone : ModTile, ITombstoneTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileSign[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            HitSound = SoundID.Dig;
            DustType = ModContent.DustType<RegolithDust>();

            AddMapEntry(new Color(148, 124, 22));

            RegisterItemDrop(ModContent.ItemType<Items.Tombstones.MoonGoldTombstone>(), 0, 1);
            RegisterItemDrop(ModContent.ItemType<Items.Tombstones.MoonGoldCrescent>(), 2, 3);
            RegisterItemDrop(ModContent.ItemType<Items.Tombstones.MoonGoldLunarCross>(), 4, 5);
        }
    }
}
