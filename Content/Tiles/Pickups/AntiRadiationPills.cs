using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Pickups
{
    public class AntiRadiationPills : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinateHeights = [18];
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.Table | AnchorType.SolidWithTop, 1, 0);
            TileObjectData.addTile(Type);

            FlexibleTileWand.RubblePlacementSmall.AddVariations(ModContent.ItemType<Items.Consumables.Potions.AntiRadiationPills>(), Type, 0);
            RegisterItemDrop(ModContent.ItemType<Items.Consumables.Potions.AntiRadiationPills>(), 0);

            DustType = -1;

            AddMapEntry(new Color(51, 29, 17), CreateMapEntryName());
        }
    }
}
