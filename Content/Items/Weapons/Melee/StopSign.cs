using Macrocosm.Common.Bases.Projectiles;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Melee
{
    public class StopSign : GreatswordHeldProjectileItem
    {
        public override Vector2 SpriteHandlePosition => new(24, 52);

        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaultsHeldProjectile()
        {
            Item.width = 72;
            Item.height = 72;
            Item.damage = 300;
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 16;
            Item.value = Item.sellPrice(gold: 20);
            Item.rare = ModContent.RarityType<MoonRarityT1>();
        }

        public override bool CanUseItemHeldProjectile(Player player)
        {
            return base.CanUseItemHeldProjectile(player);
        }

        public override void AddRecipes()
        {
        }
    }
}