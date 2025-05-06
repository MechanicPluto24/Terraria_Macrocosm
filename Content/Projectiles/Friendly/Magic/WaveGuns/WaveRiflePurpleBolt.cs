using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Particles;
using Macrocosm.Content.Trails;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Magic.WaveGuns
{
    public class WaveRiflePurpleBolt : WaveGunBlueBolt
    {
        public override int MaxPenetrate => 10;
        public override bool UseLocalImmunity => true;
        public override int LocalImmunityCooldown => -1;

        public override Color BeamColor => new(255, 150, 255, 0);
        public override Vector3 LightColor => new(1f, 0f, 1f);

        public override int DustCount => 80;
        public override int ParticleLightningCount => 32;
        public override float ParticleLightningSpread => 3f;
        public override float ParticleFlashScale => 0.1f;
        public override int TrailLength => 100;


        private readonly bool[] hitList = new bool[Main.maxNPCs];
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ParticleEffects(0.5f);

            skipTrailUpdate = true;
            Projectile.extraUpdates = 10;
            Projectile.damage = (int)(Projectile.damage * 0.8f);
            hitList[target.whoAmI] = true;
            int targetIndex = Utility.FindNPC(Projectile.Center, maxRange: 600f, hitList);
            if (targetIndex != -1)
            {
                Vector2 direction = (Main.npc[targetIndex].Center - Projectile.Center).SafeNormalize(Vector2.Zero) * spawnSpeed;
                Projectile.velocity = direction;
                Projectile.rotation = direction.ToRotation();

                if (Main.netMode == NetmodeID.MultiplayerClient)
                    Projectile.netUpdate = true;
            }
            else
            {
                FalseKill();
            }
        }
    }
}

