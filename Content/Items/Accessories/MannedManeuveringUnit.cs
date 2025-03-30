using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Players;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Particles;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Accessories
{
    [AutoloadEquip(EquipType.Wings)]
    public class MannedManeuveringUnit : ModItem
    {
        public override void SetStaticDefaults()
        {
            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(180, 9f, 2.5f, hasHoldDownHoverFeatures: true, hoverFlySpeedOverride: -1, hoverAccelerationMultiplier: 1);
            ItemSets.WingTimeDependsOnAtmosphericDensity[Type] = false;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;
            Item.value = Item.sellPrice(gold: 15);
            Item.rare = ModContent.RarityType<MoonRarity1>();
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var macroPlayer = player.GetModPlayer<MacrocosmPlayer>();
            macroPlayer.ZeroGravitySpeed *= 2f;
        }

        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 0.85f;
            ascentWhenRising = 0.15f;
            maxCanAscendMultiplier = 1f;
            maxAscentMultiplier = 3f;
            constantAscend = 0.135f;
        }

        public override bool WingUpdate(Player player, bool inUse)
        {
            if (inUse)
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector2 position = player.Center;
                    Vector2 velocity = default;
                    if (Math.Abs(player.velocity.X) > Math.Abs(player.velocity.Y))
                    {
                        velocity.X = 0.4f * -player.direction;
                        position.Y += 10 * (i % 2 == 0 ? 1 : -1);
                    }
                    else if (player.velocity.Y < 0)
                    {
                        velocity.Y = 1;
                        position.X += 8 * (i % 2 == 0 ? 1 : -1);
                    }

                    velocity *= Main.rand.NextFloat(0.8f, 1.3f);

                    Particle.Create<Smoke>((p) =>
                    {
                        p.Position = position;
                        p.Velocity = velocity;
                        p.Scale = new Vector2(0.5f) * Main.rand.NextFloat();
                        p.Rotation = Utility.RandomRotation();
                        p.FadeInNormalizedTime = 0.9f;
                        p.FadeOutNormalizedTime = 0.4f;
                        p.ScaleVelocity = new Vector2(Main.rand.NextFloat(0.035f, 0.055f));
                        p.TimeToLive = 20;
                        p.Opacity = Main.rand.NextFloat(0.5f, 0.7f);
                    });
                }

                //TODO: add downwards flight
                //TODO: add sounds
            }

            // TODO: use these for the rocket trail, cool af
            /*
            
                        for(int i = 0; i < 2; i ++)
                Particle.Create<Smoke>((p) =>
                {
                    p.Position = player.Center + Main.rand.NextVector2Circular(10, 10);
                    p.Velocity = velocity;
                    p.Scale = new Vector2(Main.rand.NextFloat(0.2f, 0.3f));
                    p.Rotation = Utility.RandomRotation();
                    p.FadeInNormalizedTime = 0.2f;
                    p.FadeOutNormalizedTime = 0.3f;
                    p.ScaleVelocity = new Vector2(Main.rand.NextFloat(0.015f, 0.025f));
                    p.Opacity = Main.rand.NextFloat(0.1f, 0.2f);
                });
            */
            return false;
        }
    }
}