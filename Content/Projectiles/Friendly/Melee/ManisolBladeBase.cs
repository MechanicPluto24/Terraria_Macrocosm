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
        protected int dustType;
        protected Vector2 stickPosition = Vector2.Zero;
        protected bool tileStick = false;

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
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            dustType = DustID.OrangeTorch;
        }

        public override bool? CanHitNPC(NPC npc)
        {
            if ((npcStick > -1 && AI_State != ActionState.Returning) || (tileStick && AI_State != ActionState.Returning))
                return false;

            return base.CanHitNPC(npc);
        }

        public virtual void OnReturn()
        {
        }

        public virtual void Returning()
        {
        }

        public override void AI()
        {
            if (!Main.dedServ && AI_State != ActionState.Stick)
            {
                for(int i = 0; i < 5; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, dustType, new Vector2(-Projectile.velocity.X * 0.2f, -Projectile.velocity.Y * 0.2f).RotatedByRandom(MathHelper.Pi / 32f) * Main.rand.NextFloat(0.5f, 3.5f), Scale: 1.8f);
                    dust.noGravity = true;
                }
            }

            if (AI_State != ActionState.Returning)
                Projectile.timeLeft++;

            switch (AI_State)
            {
                case ActionState.Thrown:

                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                    DropTimer++;

                    if (DropTimer > 10)
                    {
                        Projectile.velocity.Y += 0.4f;
                        if (Projectile.velocity.Y > 16f)
                            Projectile.velocity.Y = 16f;
                    }

                    if (npcStick == -1)
                    {
                        if (Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                        {
                            if (!tileStick)
                            {
                                Projectile.ai[0] = 20;
                                Projectile.velocity = Vector2.Zero;
                                tileStick = true;
                                Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
                                SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
                                AI_State = ActionState.Stick;
                            }
                        }
                        else
                            tileStick = false;
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
                            Projectile.timeLeft = 300; //Failsafe
                            AI_State = ActionState.Returning;
                            OnReturn();
                        }
                    }
                    break;

                case ActionState.Returning:
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Returning();
                        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                        Player player = Main.player[Projectile.owner];
                        Vector2 unitVectorTowardsPlayer = Projectile.DirectionTo(player.Center).SafeNormalize(Vector2.Zero);
                        Rectangle projectileHitbox = new((int)Projectile.position.X, (int)Projectile.position.Y, Projectile.width, Projectile.height);
                        Projectile.velocity = Projectile.velocity.MoveTowards(unitVectorTowardsPlayer * 15f, 6f);
                        Rectangle playerHitbox = new((int)player.position.X, (int)player.position.Y, player.width, player.height);
                        if (projectileHitbox.Intersects(playerHitbox))
                        {
                            Projectile.Kill();
                        }
                    }
                    break;
            }
        }
    }
}
