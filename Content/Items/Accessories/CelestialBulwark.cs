using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Graphics;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Items.Drops;
using Macrocosm.Content.Particles;
using Macrocosm.Content.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Accessories
{
    [AutoloadEquip(EquipType.Shield)]
    public class CelestialBulwark : ModItem
    {
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            Item.damage = 120;
            Item.DamageType = DamageClass.Melee;
            Item.crit = 0;
            Item.knockBack = 9f;
            Item.width = 34;
            Item.height = 40;
            Item.value = Item.sellPrice(gold: 10);
            Item.rare = ItemRarityID.Expert;
            Item.expert = true;
            Item.accessory = true;

            Item.defense = 14;
        }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
            => !((equippedItem.type == Type && incomingItem.type == ItemID.EoCShield) || (incomingItem.type == Type && equippedItem.type == ItemID.EoCShield));

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            DashPlayer dashPlayer = player.GetModPlayer<DashPlayer>();

            dashPlayer.AccDashHorizontal = true;
            dashPlayer.AccDashVertical = true;

            dashPlayer.AccDashCooldown = 45;
            dashPlayer.AccDashDuration = 35;

            dashPlayer.AccDashSpeedX = 14f;
            dashPlayer.AccDashSpeedY = 8f;

            dashPlayer.AccDashPreserveVelocity = false;
            dashPlayer.AccDashAllowMovementDuringDashMultiplier = 0.5f;

            dashPlayer.AccDashDamage = Item.damage;
            dashPlayer.AccDashKnockback = Item.knockBack;
            dashPlayer.AccDashImmuneTime = 6;

            dashPlayer.AccDashHitboxIncrease = (int)(12 * Utility.CubicEaseIn(dashPlayer.DashProgress));

            dashPlayer.AccDashAfterImage = true;
            dashPlayer.AccDashStartVisuals = StartDashVisuals;
            dashPlayer.AccDashVisuals = DashVisuals;

            dashPlayer.OnCollisionWithNPC = OnNPCCollide;
        }

        public override void UpdateVanity(Player player)
        {
        }

        private void StartDashVisuals(Player player)
        {
            Particle.CreateParticle<CelestialBulwarkDashParticle>(p =>
            {
                p.Scale = new(0.35f);
                p.Position = player.Center;
                p.PlayerID = player.whoAmI;
                p.Rotation = player.velocity.ToRotation() - MathHelper.PiOver2;
            });
        }

        private void DashVisuals(Player player)
        {
            DashPlayer dashPlayer = player.GetModPlayer<DashPlayer>();
            float progress = (float)dashPlayer.DashTimer / dashPlayer.AccDashDuration;
            int count = (int)MathF.Floor(5 * progress);

            for (int i = 0; i < count; i++)
            {
                Dust dust = Dust.NewDustDirect(player.Center + new Vector2(35, 0).RotatedBy(player.velocity.ToRotation()) + Main.rand.NextVector2Circular(50, 50) * progress, 1, 1, ModContent.DustType<CelestialDust>(), 0f, 0f, 100, default, 1.1f);
                GetEffectColor(player, out dust.color, out Color? secondaryColor, out _, out _, out bool rainbow);

                if (secondaryColor.HasValue && dust.dustIndex % 2 == 0)
                    dust.color = secondaryColor.Value;

                if (rainbow)
                    dust.color = Utility.MultiLerpColor(Main.rand.NextFloat(), Color.Red, Color.Green, Color.Blue).WithAlpha(100);
            }
        }

        private void OnNPCCollide(Player player, NPC npc)
        {
            DashPlayer dashPlayer = player.GetModPlayer<DashPlayer>();
            int count = (int)(10 * dashPlayer.DashProgress);

            for (int i = 0; i < count; i++)
            {
                var star = Particle.CreateParticle<CelestialStar>(npc.Center + Main.rand.NextVector2Circular(npc.width / 2, npc.height / 2), npc.velocity + Main.rand.NextVector2Circular(2, 2), scale: 1.2f);
                GetEffectColor(player, out star.Color, out Color? secondaryColor, out star.BlendStateOverride, out _, out bool rainbow);

                if (secondaryColor.HasValue && ParticleManager.Particles.IndexOf(star) % 2 == 0)
                    star.Color = secondaryColor.Value;

                if (rainbow)
                {
                    star.Color = Utility.MultiLerpColor(Main.rand.NextFloat(), Color.Red, Color.Green, Color.Blue).WithAlpha(127);

                    if (star.BlendStateOverride == CustomBlendStates.Subtractive)
                        if (ParticleManager.Particles.IndexOf(star) % 2 == 0)
                            star.BlendStateOverride = null;
                        else
                            star.Color = new Color(70, 70, 70);
                }
            }

            for (int i = 0; i < 25; i++)
            {
                Vector2 dustVelocity = Main.rand.NextVector2Circular(2, 2);
                Dust dust = Dust.NewDustDirect(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, ModContent.DustType<CelestialDust>(), dustVelocity.X, dustVelocity.Y, 100, default, 1.5f);
                GetEffectColor(player, out dust.color, out Color? secondaryColor, out _, out _, out bool rainbow);

                if (secondaryColor.HasValue && dust.dustIndex % 2 == 0)
                    dust.color = secondaryColor.Value;

                if (rainbow)
                    dust.color = Utility.MultiLerpColor(Main.rand.NextFloat(), Color.Red, Color.Green, Color.Blue).WithAlpha(100);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<BrokenHeroShield>()
            .AddIngredient(ItemID.EoCShield)
            .AddIngredient(ItemID.FragmentNebula, 15)
            .AddIngredient(ItemID.FragmentStardust, 15)
            .AddIngredient(ItemID.FragmentVortex, 15)
            .AddIngredient(ItemID.FragmentSolar, 15)
            .AddTile(TileID.LunarCraftingStation)
            .Register();
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            DrawMask(spriteBatch, position, origin, scale);
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            DrawMask(spriteBatch, Item.Center - Main.screenPosition, Item.Size / 2f, scale, rotation);
            Lighting.AddLight(Item.Center, CelestialDisco.CelestialColor.ToVector3());
        }

        private static readonly Asset<Texture2D>[] celestialTextures =
            [
                ModContent.Request<Texture2D>("Macrocosm/Content/Items/Accessories/CelestialBulwark_Mask_Nebula"),
                ModContent.Request<Texture2D>("Macrocosm/Content/Items/Accessories/CelestialBulwark_Mask_Stardust"),
                ModContent.Request<Texture2D>("Macrocosm/Content/Items/Accessories/CelestialBulwark_Mask_Vortex"),
                ModContent.Request<Texture2D>("Macrocosm/Content/Items/Accessories/CelestialBulwark_Mask_Solar")
            ];


        private SpriteBatchState state;
        private void DrawMask(SpriteBatch spriteBatch, Vector2 position, Vector2 origin, float scale = 1f, float rotation = 0f)
        {
            Asset<Texture2D> currentTex = celestialTextures[(int)CelestialDisco.CelestialStyle];
            Asset<Texture2D> nextTex = celestialTextures[(int)CelestialDisco.NextCelestialStyle];
            Color currentColor = Color.White.WithOpacity(CelestialDisco.CelestialStyleProgress);
            Color nextColor = Color.White.WithOpacity(1f - CelestialDisco.CelestialStyleProgress);

            state.SaveState(spriteBatch);
            spriteBatch.EndIfBeginCalled();

            spriteBatch.Begin(BlendState.NonPremultiplied, state);

            spriteBatch.Draw(currentTex.Value, position, null, nextColor, rotation, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(nextTex.Value, position, null, currentColor, rotation, origin, scale, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(state);
        }

        public static bool GetEffectColor(Player player, out Color primaryColor) => GetEffectColor(player, out primaryColor, out _, out _, out _, out _);

        public static bool GetEffectColor(Player player, out Color primaryColor, out Color? secondaryColor, out BlendState blendStateOverride, out bool bypassShader, out bool rainbow)
        {
            int dyeItemType = -1;
            primaryColor = CelestialDisco.CelestialColor;
            secondaryColor = CelestialDisco.CelestialColor;
            blendStateOverride = null;
            bypassShader = false;
            rainbow = false;

            for (int i = 3; i <= 9; i++)
            {
                if (player.armor[i].type == ModContent.ItemType<CelestialBulwark>())
                    dyeItemType = player.dye[i].type;

                if (dyeItemType > 0)
                {
                    var shader = GameShaders.Armor.GetShaderFromItemId(dyeItemType);
                    Vector3 primary = typeof(ArmorShaderData).GetFieldValue<Vector3>("_uColor", shader);
                    primaryColor = primary != Vector3.One ? new(primary) : primaryColor;
                    Vector3 secondary = typeof(ArmorShaderData).GetFieldValue<Vector3>("_uSecondaryColor", shader);
                    secondaryColor = secondary != Vector3.One ? new(secondary) : null;

                    switch (dyeItemType)
                    {
                        // Bypass shader for "X and black" dyes for brighter glowmask
                        case ItemID.RedandBlackDye:
                        case ItemID.OrangeandBlackDye:
                        case ItemID.YellowandBlackDye:
                        case ItemID.LimeandBlackDye:
                        case ItemID.GreenandBlackDye:
                        case ItemID.TealandBlackDye:
                        case ItemID.CyanandBlackDye:
                        case ItemID.SkyBlueandBlackDye:
                        case ItemID.BlueandBlackDye:
                        case ItemID.PurpleandBlackDye:
                        case ItemID.VioletandBlackDye:
                        case ItemID.BrownAndBlackDye:
                        case ItemID.FlameAndBlackDye:
                        case ItemID.GreenFlameAndBlackDye:
                        case ItemID.BlueFlameAndBlackDye:
                            bypassShader = true;
                            break;

                        // Bypass shader for better shield glowmask for specific dyes
                        case ItemID.TwilightDye:
                        case ItemID.ChlorophyteDye:
                        case ItemID.ReflectiveObsidianDye:
                            bypassShader = true;
                            break;

                        // Make glowmask and effect white for some silver dyes
                        case ItemID.SilverDye:
                        case ItemID.BlackAndWhiteDye:
                        case ItemID.ReflectiveSilverDye:
                            primaryColor = Color.White;
                            break;

                        // Make glowmask actually silver 
                        case ItemID.SilverAndBlackDye:
                            primaryColor = Color.Silver;
                            bypassShader = true;
                            break;

                        // Special rainbow effect 
                        case ItemID.RainbowDye:
                        case ItemID.IntenseRainbowDye:
                        case ItemID.LivingRainbowDye:
                        case ItemID.HallowBossDye:
                            primaryColor = Main.DiscoColor;
                            rainbow = true;
                            break;

                        // Subtractive effect (black shadow dash)
                        case ItemID.BlackDye:
                        case ItemID.ShadowDye:
                        case ItemID.LokisDye:
                            blendStateOverride = CustomBlendStates.Subtractive;
                            primaryColor = new Color(25, 25, 25);
                            break;

                        // Subtractive red and black dash
                        case ItemID.GrimDye:
                            blendStateOverride = CustomBlendStates.Subtractive;
                            primaryColor = new Color(25, 25, 25);
                            secondaryColor = new Color(0, 65, 65);
                            break;

                        // Override color (Primary color was bypassed)
                        case ItemID.LivingOceanDye:
                            primaryColor = Color.Navy;
                            break;

                        // Override color (Glowmask was too dark)
                        case ItemID.ReflectiveMetalDye:
                            primaryColor = Color.White;
                            break;

                        // I have no clue what is going on but looks cool
                        case ItemID.NegativeDye:
                            blendStateOverride = CustomBlendStates.Subtractive;
                            secondaryColor = new(new Vector3(1f) - CelestialDisco.CelestialColor.ToVector3() * 0.8f);
                            primaryColor = CelestialDisco.CelestialColor * 0.8f;
                            break;

                        // Alternating black + rainbow color 
                        case ItemID.MidnightRainbowDye:
                            blendStateOverride = CustomBlendStates.Subtractive;
                            primaryColor = new(70, 70, 70);
                            secondaryColor = new(70, 70, 70);
                            rainbow = true;
                            break;

                        default:
                            break;
                    }

                    return true;
                }
            }

            return false;
        }
    }
}