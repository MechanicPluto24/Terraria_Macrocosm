using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Bases.Projectiles
{
    /// <summary>
    /// Base class for a bullet projectile that bounces from enemy to enemy. 
    /// </summary>
    public abstract class RicochetBullet : ModProjectile
    {
        /// <summary> The number of ricochets </summary>
        public virtual int RicochetCount { get; set; } = 2;

        /// <summary> The bullet speed after a ricochet </summary>
        public virtual float RicochetSpeed { get; set; } = 2;

        /// <summary> Whether the bullet can ricochet at any given time. Only called on the owner client </summary>
        public virtual bool CanRicochet() { return true; }

        /// <summary> Called on ricochet on the owner client, for functional effects such as changing stats </summary>
        // TODO: Entity abstraction when implementing player ricochets?
        public virtual void OnHitNPC(bool ricochet, NPC target, NPC.HitInfo hit, int damageDone) { }

        /// <summary> Called on ricochet on all clients, for visual effects </summary>
        public virtual void OnHitNPCEffect(bool ricochet, NPC target, NPC.HitInfo hit, int damageDone) { }

        /// <summary> Set projectile defaults here </summary>
        public virtual void SetProjectileDefaults() { }

        /// <summary> Used to keep track of every NPC hit </summary>
        protected readonly bool[] hitList = new bool[Main.maxNPCs];

        // Needed since OnHitEffect is only called on the owner
        bool scheduleOnHitEffect = false;

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Bullet);
            AIType = ProjectileID.Bullet;

            Projectile.penetrate = RicochetCount;
            Projectile.localNPCHitCooldown = -1;
            Projectile.usesLocalNPCImmunity = true;

            SetProjectileDefaults();
        }

        // Only called on the owner 
        public sealed override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            hitList[target.whoAmI] = true; 
            int targetId = Utility.FindNPC(Projectile.Center, 600f, hitList);
            bool didRicochet = targetId != -1 && CanRicochet();

            OnHitNPC(didRicochet, target, hit, damageDone);

            if (didRicochet)
            {
                Vector2 shootVel = Main.npc[targetId].Center - Projectile.Center;
                shootVel.Normalize();
                shootVel *= RicochetSpeed;
                Projectile.velocity = shootVel;
                Projectile.rotation = Main.npc[targetId].Center.ToRotation();

                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    scheduleOnHitEffect = true;
                    Projectile.netUpdate = true;
                }
            }

            if (scheduleOnHitEffect || Main.netMode == NetmodeID.SinglePlayer)
            {
                OnHitNPCEffect(didRicochet, target, hit, damageDone);
                scheduleOnHitEffect = false;
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(scheduleOnHitEffect);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            scheduleOnHitEffect = reader.ReadBoolean();
        }
    }
}
