using Macrocosm.Common.Bases.Projectiles;
using Macrocosm.Common.CrossMod;
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

        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaultsHeldProjectile()
        {
            Item.width = 58;
            Item.height = 58;
            Item.damage = 30;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 5;
            Item.value = 10000;
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item1;

            Redemption.SetSlashBonus(Item);
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            Redemption.Decapitation(target, ref damageDone, ref hit.Crit);
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