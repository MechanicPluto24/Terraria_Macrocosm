using Macrocosm.Common.Bases;
using Macrocosm.Content.Items.Materials;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Melee
{
    public class ArtemiteGreatsword : GreatswordHeldProjectileItem
    {
        public override Vector2 SpriteHandlePosition => new(23, 68);

        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaultsHeldProjectile()
        {
            Item.damage = 225;
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 5;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<MoonRarityT1>();
        }

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient<ArtemiteBar>(12);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}