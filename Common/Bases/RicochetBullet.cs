using Macrocosm.Content.Projectiles.Global;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Bases
{
    /// <summary>
    /// Base class for a bullet projectile that bounces from enemy to enemy. 
    /// </summary>
    public abstract class RicochetBullet : ModProjectile, IBullet
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

        private int newTarget = -1;
        private bool HasNewTarget => newTarget != -1;


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
            hitList[target.whoAmI] = true; //Make sure the projectile won't aim directly for this NPC
            newTarget = GetTarget(600, Projectile.Center); //Keeps track of the current target, set to -1 to ensure no NPC by default

            bool didRicochet = HasNewTarget && CanRicochet();

            OnHitNPC(didRicochet, target, hit, damageDone);

            if (didRicochet)
            {
                Vector2 shootVel = Main.npc[newTarget].Center - Projectile.Center;
                shootVel.Normalize();
                shootVel *= RicochetSpeed;
                Projectile.velocity = shootVel;
                Projectile.rotation = Main.npc[newTarget].Center.ToRotation();

                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    scheduleOnHitEffect = true;
                    Projectile.netUpdate = true;
                }
            }

            if (scheduleOnHitEffect || (Main.netMode == NetmodeID.SinglePlayer))
            {
                OnHitNPCEffect(didRicochet, target, hit, damageDone);
                scheduleOnHitEffect = false;
            }
        }

        private int GetTarget(float maxRange, Vector2 shootingSpot) //Function to find a NPC to target
        {
            int first = -1; //Used to keep track of the closest NPC, rather than the first one based on NPC whoAmI
            for (int j = 0; j < Main.maxNPCs; j++)
            {
                NPC npc = Main.npc[j];
                if (npc.CanBeChasedBy(this, false) && !hitList[j])
                {
                    float distance = Vector2.Distance(shootingSpot, npc.Center);
                    if (distance <= maxRange)
                    {
                        if ((first == -1 || distance < Vector2.Distance(shootingSpot, Main.npc[first].Center)) && Collision.CanHitLine(shootingSpot, 0, 0, npc.Center, 0, 0))
                            first = j;
                    }
                }
            }

            return first;
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
