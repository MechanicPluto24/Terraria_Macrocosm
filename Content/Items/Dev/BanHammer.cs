using Macrocosm.Content.Items.Global;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Dev
{
    public class BanHammer : ModItem, IDevItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.damage = 100000000;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 1000;
            Item.value = 10000;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = false;
        }

        public override void AddRecipes()
        {

        }
    }
}