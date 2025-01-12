using Macrocosm.Common.Utils;
using Macrocosm.Content.Projectiles.Friendly.Magic;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Magic
{
    public class PhantasmalSkullTome : ModItem
    {
        private static Asset<Texture2D> heldTexture;

        public override void Load()
        {
            heldTexture = ModContent.Request<Texture2D>(Texture + "_Held");
        }

        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.damage = 280;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 8;
            Item.width = 28;
            Item.height = 30;
            Item.useTime = 13;
            Item.useAnimation = 13;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.channel = true;
            Item.knockBack = 8;
            Item.value = Item.sellPrice(0, 20, 0, 0);
            Item.rare = ModContent.RarityType<MoonRarityT2>();
            Item.UseSound = SoundID.Item8;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<PhantasmalSkullTomeProjectile>();
            Item.shootSpeed = 16f;

            Item.noUseGraphic = true;
            Item.CustomDrawData().CustomHeldTexture = heldTexture;
        }

        public override Vector2? HoldoutOffset() => new Vector2(0, 1);

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position += new Vector2(Main.rand.Next(-100, 201), Main.rand.Next(-100, 101));
        }
    }
}