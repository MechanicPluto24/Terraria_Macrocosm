using Macrocosm.Content.Subworlds;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Melee
{
    public abstract class ManisolBladeBase : ModProjectile
    {
        protected int npcStick = -1;
        protected bool tileStick = false;
        protected Vector2 stickPosition = Vector2.Zero;
        protected int penetrateCount = 0;
        protected int maxPenetrateCount = 0;
        protected int maxDropTime;

        public float DropTimer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public enum ActionState { Thrown, Stick, Returning }
        public ActionState AI_State
        {
            get => (ActionState)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 15;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 34;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            maxPenetrateCount = 0;
            maxDropTime = 20;
        }


        public virtual void OnStick()
        {
        }

        public virtual void OnRecalled()
        {
        }

        public virtual void OnReturnToPlayer()
        {
        }

        public void ForceRecall()
        {
            AI_State = ActionState.Returning;
            Projectile.velocity = new Vector2(10, 10);
            Projectile.timeLeft = 300;  
            OnRecalled();
            Projectile.netUpdate = true;
        }

        public override bool? CanHitNPC(NPC npc)
        {
            if ((npcStick > -1 && AI_State != ActionState.Returning) || (tileStick && AI_State != ActionState.Returning))
                return false;

            return null;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (npcStick == -1 && AI_State == ActionState.Thrown)
            {
                if (penetrateCount < maxPenetrateCount)
                {
                    penetrateCount++;
                }
                else
                {
                    Projectile.velocity = Vector2.Zero;
                    Projectile.rotation = Projectile.oldVelocity.ToRotation() + MathHelper.PiOver2;
                    npcStick = target.whoAmI;
                    stickPosition = Projectile.Center - target.position;
                    AI_State = ActionState.Stick;
                    OnStick();
                    Projectile.netUpdate = true;
                }
            }
        }

        public override bool PreAI()
        {
            Projectile.oldVelocity = Projectile.velocity;

            if (AI_State != ActionState.Returning)
                Projectile.timeLeft++;

            switch (AI_State)
            {
                case ActionState.Thrown:

                    DropTimer++;

                    if (DropTimer > maxDropTime)
                    {
                        Projectile.velocity.Y += 0.4f;
                        if (Projectile.velocity.Y > 16f)
                            Projectile.velocity.Y = 16f;

                        Projectile.rotation += Projectile.velocity.X * 0.02f;
                    }
                    else
                    {
                        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                    }

                    if (npcStick == -1)
                    {
                        if (Collision.SolidCollision(Projectile.Center, 8, 8))
                        {
                            if (!tileStick)
                            {
                                DropTimer = 20;
                                Projectile.velocity = Vector2.Zero;
                                Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
                                Projectile.rotation = Projectile.oldVelocity.ToRotation() + MathHelper.PiOver2;
                                SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
                                AI_State = ActionState.Stick;
                                OnStick();
                                tileStick = true;
                            }
                        }
                        else
                        {
                            tileStick = false;
                        }
                    }
                    break;

                case ActionState.Stick:

                    if (npcStick > -1)
                    {
                        if (Main.npc[npcStick].active)
                        {
                            Projectile.Center = Main.npc[npcStick].position + stickPosition;
                        }
                        else
                        {
                            npcStick = -1;
                            DropTimer = 11;
                            Projectile.velocity = new Vector2(10, 10);
                            Projectile.timeLeft = 300; 
                            AI_State = ActionState.Returning;
                            OnRecalled();
                        }
                    }
                    break;

                case ActionState.Returning:

                    if (Main.myPlayer == Projectile.owner)
                    {
                        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                        Player player = Main.player[Projectile.owner];
                        Vector2 direction = Projectile.DirectionTo(player.Center).SafeNormalize(Vector2.Zero);
                        Rectangle projectileHitbox = new((int)Projectile.position.X, (int)Projectile.position.Y, Projectile.width, Projectile.height);
                        Projectile.velocity = Projectile.velocity.MoveTowards(direction * 15f, 6f);
                        Rectangle playerHitbox = new((int)player.position.X, (int)player.position.Y, player.width, player.height);

                        if (projectileHitbox.Intersects(playerHitbox))
                        {
                            OnReturnToPlayer();
                            Projectile.Kill();
                        }
                    }
                    break;
            }

            return true;
        }
    }
}
