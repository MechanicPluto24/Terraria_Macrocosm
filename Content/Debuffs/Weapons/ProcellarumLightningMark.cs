using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Debuffs.Weapons
{
    public class ProcellarumLightningMark : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
            Main.pvpBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            DustEffects(player);
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (!npc.dontTakeDamage)
                DustEffects(npc);
        }

        private void DustEffects(Entity entity)
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
        }
    }
}
