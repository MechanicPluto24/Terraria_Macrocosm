using Macrocosm.Content.NPCs.Enemies.Moon;
using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Critters
{
    public class KyaniteScarabCritter : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.width = 18;
            Item.height = 10;
            Item.noUseGraphic = true;
            Item.makeNPC = ModContent.NPCType<NPCs.Critters.KyaniteScarabCritter>();
            Item.value = 150;
            Item.rare = ModContent.RarityType<MoonRarityT1>();
        }
    }
}