using Macrocosm.Common.Utils;
using Macrocosm.Content.Debuffs.Weapons;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Projectiles.Friendly.Melee;
using Macrocosm.Content.Projectiles.Friendly.Summon;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Common.Global.NPCs
{
    public class DebuffGlobalNPC : GlobalNPC
    {
        public override void Load()
        {
        }

        public override void AI(NPC npc)
        {
            if (npc.HasBuff<Stasis>())
            {
                npc.velocity.X = MathHelper.Lerp(npc.velocity.X * 0.5f, npc.velocity.X, 0.01f);
                npc.velocity.Y = MathHelper.Lerp(0f, npc.velocity.Y, 0.01f);
            }
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (npc.HasBuff<ProcellarumLightningMark>() && projectile.type == ModContent.ProjectileType<ProcellarumHalberdProjectile>())
            {
                //modifiers.FinalDamage *= 1.5f;
            }
        }

        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (npc.HasBuff<ProcellarumLightningMark>() && projectile.type == ModContent.ProjectileType<ProcellarumHalberdProjectile>() && projectile.ai[0] == 1)
            {
                int damage = (int)(hit.Damage * 0.5f);
                Vector2 position = new(Main.screenPosition.X + 0.5f * Main.screenWidth + Main.rand.Next(-128, 128), Main.screenPosition.Y);
                float angle = (npc.position - position).ToRotation();

                for (int i = 0; i < 1; i++)
                {
                    Projectile.NewProjectile(
                        projectile.GetSource_FromAI(),
                        position,
                        Utility.PolarVector(40 + i * 4, angle),
                        ModContent.ProjectileType<ProcellarumLightBolt>(),
                        damage,
                        0,
                        projectile.owner,
                        ai0: npc.whoAmI + 1,
                        ai1: i
                    );
                }
            }

            if (npc.HasBuff<TotalityTag>() && projectile.DamageType == DamageClass.Summon)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 position = npc.Center + Main.rand.NextVector2CircularEdge(npc.width, npc.height);
                    Vector2 velocity = new Vector2(5).RotatedByRandom(MathHelper.TwoPi);

                    float rotation = (Main.rand.Next(2) * 2 - 1) * ((float)Math.PI / 5f + (float)Math.PI * 4f / 5f * Main.rand.NextFloat()) * 0.5f;
                    velocity = velocity.RotatedBy(MathHelper.PiOver4);

                    position += npc.velocity * 5;
                    Projectile.NewProjectile(projectile.GetSource_OnHit(npc), position, velocity, ModContent.ProjectileType<TotalitySmallSlashProjectile>(), (int)(damageDone * 0.25f), 0f, projectile.owner, rotation);
                }

                Color color = new List<Color>() {
                        new(44, 210, 91),
                        new(201, 125, 205),
                        new(114, 111, 207)
                    }.GetRandom();
                color.A = (byte)Main.rand.Next(120, 200);

                for (float f = 0f; f < 1f; f += 1f / 4f)
                {
                    Vector2 velocity = new Vector2(3f).RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat();
                    Dust dust = Dust.NewDustPerfect(npc.Center, ModContent.DustType<ElectricSparkDust>(), velocity, Scale: Main.rand.NextFloat(0.4f, 0.6f));
                    dust.noGravity = true;
                    dust.color = color.WithLuminance(0.1f);
                    dust.alpha = 15;
                }

                npc.RemoveBuff<TotalityTag>();
            }
        }

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (npc.HasBuff<Stasis>())
                drawColor = new Color(104, 245, 220, 220);
        }

    }
}
