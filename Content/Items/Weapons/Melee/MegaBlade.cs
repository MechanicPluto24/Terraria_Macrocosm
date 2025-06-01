using Macrocosm.Common.Bases.Projectiles;
using Macrocosm.Common.CrossMod;
using Macrocosm.Content.Projectiles.Friendly.Melee;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Melee
{
    public class MegaBlade : GreatswordHeldProjectileItem
    {
        public override Vector2 SpriteHandlePosition => new(24, 88);
        public override UseBehavior LeftClickBehavior => UseBehavior.Charge;
        public override UseBehavior RightClickBehavior => UseBehavior.None;
        public override (float min, float max) ChargeBasedDamageRatio => base.ChargeBasedDamageRatio;
        public override int MaxCharge => 100;


        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaultsHeldProjectile()
        {
            Item.damage = 400;
            Item.DamageType = DamageClass.Melee;
            Item.width = 84;
            Item.height = 84;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.knockBack = 10;
            Item.value = Item.sellPrice(0, 20, 0, 0);
            Item.rare = ModContent.RarityType<MoonRarity2>();
            Item.UseSound = SoundID.DD2_MonkStaffSwing;
            Item.autoReuse = true;

            Redemption.SetSlashBonus(Item);
        }
    }
}