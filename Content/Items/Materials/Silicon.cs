using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Materials
{
    public class Silicon : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 750;
            Item.rare = ItemRarityID.White;
            Item.material = true;

            // TODO: if ever added as a block
            //Item.useStyle = ItemUseStyleID.Swing;
            //Item.useTurn = true;
            //Item.useAnimation = 15;
            //Item.useTime = 10;
            //Item.autoReuse = true;
            //Item.consumable = true;
            //Item.placeStyle = 0;
        }
    }
}