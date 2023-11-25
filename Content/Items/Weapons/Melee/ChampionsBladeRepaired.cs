using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Melee
{
    public class ChampionsBladeRepaired : ModItem
    {
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            Item.damage = 300;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<MoonRarityT1>();
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shootSpeed = 0f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return false;
        }

        public override void AddRecipes()
        {
        }
    }
}