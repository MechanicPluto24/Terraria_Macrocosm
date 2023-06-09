using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Macrocosm.Content.Sounds;
using Macrocosm.Content.Projectiles.Global;
using System.IO;

namespace Macrocosm.Content.Projectiles.Base
{
    public abstract class RicochetBullet : ModProjectile, IBullet
    {
        /// <summary> The number of ricochets </summary>
        public virtual int RicochetCount { get; set; } = 2;

        /// <summary> The bullet speed after a ricochet </summary>
        public virtual float RicochetSpeed { get; set; } = 2;

		/// <summary> Called on ricochet on the owner client, for functional effects </summary>
		public virtual void OnRicochet() { }

		/// <summary> Called on ricochet on all clients, for visual effects </summary>
		public virtual void OnRicochetEffect() { }

        /// <summary> Set projectile defaults here, or call base in SetDefaults </summary>
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

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            OnRicochet();

			hitList[target.whoAmI] = true; //Make sure the projectile won't aim directly for this NPC
            int newTarget = GetTarget(600, Projectile.Center); //Keeps track of the current target, set to -1 to ensure no NPC by default

            if (newTarget != -1)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    Vector2 shootVel = Main.npc[newTarget].Center - Projectile.Center;
                    shootVel.Normalize();
                    shootVel *= RicochetSpeed;
                    Projectile.velocity = shootVel;
                    Projectile.rotation = Main.npc[newTarget].Center.ToRotation();
                }

                scheduleOnHitEffect = true;
            }

            Projectile.netUpdate = true;
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

		public override void PostAI()
		{
			if (scheduleOnHitEffect)
			{
                if(!Main.dedServ)
				    SoundEngine.PlaySound(SFX.Ricochet with { Volume = 0.3f }, Projectile.position);

				OnRicochetEffect();

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
