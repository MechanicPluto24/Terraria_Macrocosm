using Macrocosm.Common.Utils;
using Macrocosm.Content.Projectiles.Friendly.Melee;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Melee
{
    public class ChampionsBlade : ModItem
    {
        private static Asset<Texture2D> repairedTexture;

        public override void Load()
        {
            repairedTexture = ModContent.Request<Texture2D>(Texture + "_Repaired");
        }

        public override void SetStaticDefaults()
        {

        }

        private int bladeCharge = 0;
        private int powerTime = 300;

        public override void SetDefaults()
        {
            Item.damage = 275;
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
            Item.shootSpeed = 0f;
            Item.shoot = ModContent.ProjectileType<ChampionsBladeSwing>();
            Item.noMelee = true; // This is set the sword itself doesn't deal damage (only the projectile does).
            Item.shootsEveryUse = true; // This makes sure Player.ItemAnimationJustStarted is set when swinging.
            Item.Glowmask().Texture = repairedTexture;
            Item.Glowmask().Color = null;
        }

        public override void HoldItem(Player player)
        {
            Item.Glowmask().Color = new Color(255, 255, 255, 255) * (float)(powerTime / 300f);

            if (powerTime > 0)
            {
                Item.shoot = ModContent.ProjectileType<ChampionsBladeSwingEmpowered>();
                powerTime--;
            }
            else
            {
                Item.shoot = ModContent.ProjectileType<ChampionsBladeSwing>();
            }
        }

        public override bool CanUseItem(Player player)
        {
            if (player.AltFunction())
            {
                if (bladeCharge >= 100)
                {
                    bladeCharge = 0;
                    powerTime = 300;
                    SoundEngine.PlaySound(SoundID.MaxMana, player.position);
                    for (int j = 0; j < 20; j++)
                    {
                        int num = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, DustID.GemAmethyst, 0f, 0f, 100, default(Color), 2f);
                        Dust dust = Main.dust[num];
                        dust.position.X = dust.position.X + (float)Main.rand.Next(-20, 21);
                        Dust dust2 = Main.dust[num];
                        dust2.position.Y = dust2.position.Y + (float)Main.rand.Next(-20, 21);
                        Main.dust[num].velocity *= 0.4f;
                        Main.dust[num].scale *= 1f + (float)Main.rand.Next(40) * 0.01f;
                        Main.dust[num].shader = GameShaders.Armor.GetSecondaryShader(player.cWaist, player);
                        Main.dust[num].noGravity = true;
                        if (Main.rand.NextBool(2))
                        {
                            Main.dust[num].scale *= 1f + (float)Main.rand.Next(40) * 0.01f;
                        }
                    }
                }

                return false;
            }

            return base.CanUseItem(player);

        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.AltFunction())
                return false;

            float adjustedItemScale = player.GetAdjustedItemScale(Item);

            if (powerTime > 0)
                damage = (int)(damage * 1.5f);

            Projectile.NewProjectile(source, player.MountedCenter, new Vector2(player.direction, 0f), type, damage, knockback, player.whoAmI, player.direction * player.gravDir, player.itemAnimationMax, adjustedItemScale);
            NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, player.whoAmI);
            bladeCharge += 100;

            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }
    }
}