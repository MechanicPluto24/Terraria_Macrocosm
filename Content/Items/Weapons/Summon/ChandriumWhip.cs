using Macrocosm.Content.Items.Materials;
using Macrocosm.Content.Projectiles.Friendly.Summon;
using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Summon
{
    public class ChandriumWhip : ModItem
    {
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Item.DefaultToWhip(ModContent.ProjectileType<ChandriumWhipProjectile>(), 220, 2, 4);

            Item.shootSpeed = 4;
            Item.rare = ModContent.RarityType<MoonRarityT1>();

            Item.channel = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient<ChandriumBar>(12);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}
