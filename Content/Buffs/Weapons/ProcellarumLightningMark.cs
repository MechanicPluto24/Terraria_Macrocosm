using Macrocosm.Common.Bases.Buffs;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Particles;
using Macrocosm.Content.Projectiles.Friendly.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Macrocosm.Content.Buffs.Weapons;

public class ProcellarumLightningMark : ComplexBuff
{
    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = true;
        Main.buffNoSave[Type] = true;
        Main.buffNoTimeDisplay[Type] = false;
        Main.pvpBuff[Type] = true;
    }

    public override void DrawEffects(NPC npc, ref Color drawColor)
    {
        if (!npc.dontTakeDamage)
            DustEffects(npc);
    }

    public override void DrawEffects(Player player, PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
    {
        int dustIndex = DustEffects(player);
        if (dustIndex > 0)
            drawInfo.DustCache.Add(dustIndex);
    }

    public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
    {
        if (projectile.type == ModContent.ProjectileType<ProcellarumHalberdProjectile>() && projectile.ai[0] == 1)
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
    }

    private int DustEffects(Entity entity)
    {
        for (int i = 0; i < 5; i++)
        {
            float rotation = Main.rand.NextFloat() * MathHelper.TwoPi + Main.rand.NextFloatDirection() * 0.25f;
            Particle.Create<LightningParticle>((p) =>
            {
                p.Position = entity.Center + Main.rand.NextVector2Circular(entity.width, entity.height) * 0.5f + entity.velocity * 4f;
                p.Rotation = (entity.Center - p.Position).ToRotation();
                p.Color = new Color(232, 243, 255, 255);
                p.OutlineColor = (Main.rand.NextBool() ? new Color(156, 174, 208, 127) : new Color(179, 171, 185, 127)) * 0.5f;
                p.Scale = new(Main.rand.NextFloat(0.4f, 0.8f));
                p.Velocity = Main.rand.NextVector2Circular(0.1f, 0.1f);
                p.Scale = new Vector2(1f, 1f) * Main.rand.NextFloat(0.3f, 0.8f);
                p.ScaleVelocity = new Vector2(-0.05f);
                p.TimeToLive = 10;
                p.FadeInNormalizedTime = 0.01f;
                p.FadeOutNormalizedTime = 0.5f;
            });
        }

        return -1;
    }
}
