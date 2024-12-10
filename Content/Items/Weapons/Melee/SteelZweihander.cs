using Macrocosm.Common.Bases.Projectiles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Bars;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Melee
{
    public class SteelZweihander : GreatswordHeldProjectileItem
    {
        public override Vector2 SpriteHandlePosition => new(12, 52);

        public override bool RightClickUse => true;

        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaultsHeldProjectile()
        {
            Item.width = 58;
            Item.height = 58;
            Item.damage = 25;
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 5;
            Item.value = 10000;
            Item.rare = ItemRarityID.Orange;
        }

        public override bool CanUseItemHeldProjectile(Player player)
        {
            if (player.AltFunction())
            {
                Item.noUseGraphic = true;
                Item.noMelee = true;
                Item.useTime = 1;
                Item.useAnimation = 1;
                Item.UseSound = null;
            }
            else
            {
                Item.noUseGraphic = false;
                Item.noMelee = false;
                Item.useTime = 26;
                Item.useAnimation = 26;
                Item.UseSound = SoundID.Item1;
            }

            return base.CanUseItemHeldProjectile(player);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<SteelBar>(16)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }
}