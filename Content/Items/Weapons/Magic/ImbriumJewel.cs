using Macrocosm.Content.Projectiles.Friendly.Magic;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Magic
{
    public class ImbriumJewel : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 280;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 8;
            Item.width = 28;
            Item.height = 28;
            Item.useTime = 13;
            Item.useAnimation = 13;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 8;
            Item.value = Item.sellPrice(0, 20, 0, 0);
            Item.rare = ModContent.RarityType<MoonRarityT2>();
            Item.UseSound = SoundID.Item8;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ImbriumJewelPhantasmalSkull>();
            Item.shootSpeed = 16f;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position += new Vector2(Main.rand.Next(-200, 201), Main.rand.Next(-200, 201));
        }
    }
}