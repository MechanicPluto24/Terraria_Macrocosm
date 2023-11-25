using Macrocosm.Common.Bases;
using Macrocosm.Content.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Melee
{
    public class SteelZweihander : GreatswordHeldProjectileItem
    {
        public override Vector2 SpriteHandlePosition => new(12, 52);

        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaultsHeldProjectile()
        {
            Item.damage = 55;
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 5;
            Item.value = 10000;
            Item.rare = ItemRarityID.Orange;
        }

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient<SteelBar>(16);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}