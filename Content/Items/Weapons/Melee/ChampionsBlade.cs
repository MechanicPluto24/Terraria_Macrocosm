using Macrocosm.Content.Projectiles.Friendly.Melee;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Melee
{
    public class ChampionsBlade : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 185;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<MoonRarityT2>();
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shootSpeed = 2f;
            Item.shoot = ModContent.ProjectileType<ChampionsBladeHeldProjectile>();
            Item.noMelee = true; // This is set the sword itself doesn't deal damage (only the projectile does).
            Item.shootsEveryUse = true; // This makes sure Player.ItemAnimationJustStarted is set when swinging.
            Item.noUseGraphic = true;
        }

        public const int MaxStacks = 6;
        public float SwingDirection { get; private set; } = -1;
        public int HitStacks { get; set; }
        public int ResetTimer { get; set; }

        public override bool CanUseItem(Player player)
        {
            SwingDirection = -SwingDirection;
            return base.CanUseItem(player);
        }

        public override void UpdateInventory(Player player)
        {
            if (ResetTimer >= 320)
            {
                HitStacks = 0;
            }
            else
            {
                ResetTimer++;
            }
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            frame = new Rectangle(0, 24, 56, 54);
            origin = frame.Size() / 2f;
            spriteBatch.Draw(TextureAssets.Item[Type].Value, position, frame, drawColor, 0f, origin, scale * 1.3f, SpriteEffects.None, 0f);
            return false;
        }
    }
}