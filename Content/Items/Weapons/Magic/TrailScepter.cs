using Macrocosm.Common.CrossMod;
using Macrocosm.Content.Projectiles.Friendly.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Magic
{
    public class TrailScepter : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;

            Redemption.AddElementToItem(Type, Redemption.ElementID.Arcane);
            Redemption.AddElementToItem(Type, Redemption.ElementID.Celestial, true);
        }

        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Magic;
            Item.damage = 220;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.useAnimation = 5;
            Item.crit = 6;
            Item.useTime = 5;
            Item.shoot = ModContent.ProjectileType<TrailStar>();
            Item.shootSpeed = 20f;
            Item.noMelee = true;
            Item.mana = 3;
            Item.width = 40;
            Item.height = 40;
            Item.knockBack = 4;
            Item.UseSound = SoundID.Item43;
        }

        //public override Vector2? HoldoutOrigin()
        //{
        //    return base.HoldoutOrigin();
        //}
    }
}
