using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Items.Blocks.Terrain;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Rubble
{
    public abstract class ProtolithRockSmall : ModTile
    {
        // We want both tiles to use the same texture
        public override string Texture => this.GetNamespacePath().Replace(Name, nameof(ProtolithRockSmall));

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileSolidTop[Type] = false;
            Main.tileNoAttach[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileNoFail[Type] = true;
            TileID.Sets.BreakableWhenPlacing[Type] = true;

            DustType = ModContent.DustType<ProtolithDust>();

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);

            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);

            TileID.Sets.DisableSmartCursor[Type] = true;
            AddMapEntry(new Color(65, 65, 65));
        }
    }

    // This is the natural tile, this version is placed during world generation
    public class ProtolithRockSmallNatural : ProtolithRockSmall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
    }

    // This is the fake tile that will be placed by the Rubblemaker.
    public class ProtolithRockSmallFake : ProtolithRockSmall
    {
        public override void SetStaticDefaults()
        {
            // Call to base SetStaticDefaults. Must inherit static defaults from base type 
            base.SetStaticDefaults();

            // Add rubble variant, all existing styles, to Rubblemaker, allowing to place this tile by consuming Protolith
            FlexibleTileWand.RubblePlacementSmall.AddVariations(ModContent.ItemType<Protolith>(), Type, 0..9);

            // Tiles placed by Rubblemaker drop the item used to place them.
            RegisterItemDrop(ModContent.ItemType<Protolith>());
        }
    }

}