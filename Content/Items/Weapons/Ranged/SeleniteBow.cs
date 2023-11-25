using Macrocosm.Content.Items.Materials;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Ranged
{
    public class SeleniteBow : ModItem
    {
        public override void SetStaticDefaults()
        {


        }
        public override void SetDefaults()
        {
            Item.DefaultToBow(18, 20, true);

            Item.damage = 320;
            Item.knockBack = 4;

            Item.width = 32;
            Item.height = 54;

            Item.value = 10000;
            Item.rare = ModContent.RarityType<MoonRarityT1>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(6, 0);
        }

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient(ModContent.ItemType<SeleniteBar>(), 12);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}
