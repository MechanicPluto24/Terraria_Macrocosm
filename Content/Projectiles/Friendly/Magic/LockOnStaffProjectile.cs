using Macrocosm.Common.Bases.Projectiles;
using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Global.NPCs;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Debuffs.Weapons;
using Macrocosm.Content.Particles;
using Macrocosm.Content.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Magic
{
    public class LockOnStaffProjectile : ChargedHeldProjectile
    {
        public ref float AI_UseCounter => ref Projectile.ai[1];

        private const int windupFrames = 5; // number of windup animaton frames
        private const int shootFrames = 1;  // number of shooting animaton frames

        private readonly int ManaUseRate = 10;
        private readonly int ManaUseAmount = 5;
        public int lockOnMax = 4;

        //public NPC lockedOn[4] = -1;

        public override float CircularHoldoutOffset => 45;

        public override void SetProjectileStaticDefaults()
        {
            Main.projFrames[Type] = 6;
        }

        public override void SetProjectileDefaults()
        {
        }

        public override void OnSpawn(IEntitySource source)
        {
            //ResetTargets();
        }

        public void Visuals()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 - MathHelper.PiOver4 * Projectile.spriteDirection;
            if (CanShoot)
                Lighting.AddLight(Projectile.position + Utility.PolarVector(80f, Projectile.rotation), TorchID.Torch);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float xX = 5 * Projectile.spriteDirection;
            float yY = Projectile.spriteDirection == 1 ? 0 : 5;
            Projectile.DrawAnimated(lightColor, Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : Projectile.spriteDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None, new Microsoft.Xna.Framework.Vector2(xX, yY));
            return false;
        }

        public override bool? CanDamage() => false;

        private bool CanShoot => true;
        public override void ProjectileAI()
        {
            if (lockOnMax > 0)
                LockOn();
            Animate();
            //Shoot();
            //ComputeOverheat();
            Visuals();

          ///*if (!Main.dedServ && StillInUse)
          //      PlaySounds();

            AI_UseCounter++;
        }

        private void LockOn()
        {
            int id = -1;
            NPC[] numbers = new NPC[4];

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                //if (npc.TryGetGlobalNPC(out MacrocosmNPC macNpc) && macNpc.TargetedByHomingProjectile == true)
                //    lockOnMax--;

                if (lockOnMax <= 0)
                    return;
                if (npc.CanBeChasedBy() && Main.npc[i].getRect().Intersects(new Rectangle((int)(Main.MouseWorld.X - 10f), (int)(Main.MouseWorld.Y - 10f), 20, 20))
                && npc.GetGlobalNPC<MacrocosmNPC>().TargetedByHomingProjectile == false)
                {
                    id = i;
                    if (id > -1 && id < Main.maxNPCs)
                    {
                        Main.npc[id].GetGlobalNPC<MacrocosmNPC>().TargetedByHomingProjectile = true;
                        SoundEngine.PlaySound(SoundID.Item29, Main.npc[id].position);
                        lockOnMax--;
                    }
                }
                //lockOnMax = 4;
            }
        }

        private void ResetTargets()
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.TryGetGlobalNPC(out MacrocosmNPC macNpc))
                    macNpc.TargetedByHomingProjectile = false;
            }
        }

        private void Animate()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 8)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= Main.projFrames[Type])
                    Projectile.frame = 5;
            }
        }

        private bool OwnerHasMana => Player.CheckMana(ManaUseAmount);

        private SpriteBatchState state1, state2;

        private SlotId playingSoundId_1 = SlotId.Invalid;
        private SlotId playingSoundId_2 = SlotId.Invalid;
        private ProjectileAudioTracker tracker;
        private void PlaySounds()
        {
            if (!StillInUse)
                return;

            tracker ??= new(Projectile);

            SoundEngine.PlaySound(SFX.HandheldThrusterFlame with
            {
                Volume = 0.3f,
                MaxInstances = 1,
                SoundLimitBehavior = SoundLimitBehavior.IgnoreNew
            },
            Projectile.position, updateCallback: (sound) =>
            {
                sound.Position = Projectile.position;
                return tracker.IsActiveAndInGame();
            });

            if (!OwnerHasMana)
            {
                SoundEngine.PlaySound(SFX.HandheldThrusterOverheat with
                {
                    Volume = 0.3f,
                    MaxInstances = 1,
                    SoundLimitBehavior = SoundLimitBehavior.IgnoreNew
                },
                Projectile.position, updateCallback: (sound) =>
                {
                    sound.Position = Projectile.position;
                    return tracker.IsActiveAndInGame();
                });
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (lockOnMax < 4)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.GetGlobalNPC<MacrocosmNPC>().TargetedByHomingProjectile == true)
                    {
                        int proj = Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center + new Vector2(-12 + (i * 8), 0), new Microsoft.Xna.Framework.Vector2(0, -16), ModContent.ProjectileType<LockOnStaffBolt>(), (int)(Projectile.damage), Projectile.knockBack, Main.player[Projectile.owner].whoAmI, i);
                        Main.projectile[proj].localAI[0] = i;
                        Player.CheckMana(Player.HeldItem.mana, true, false);
                    }
                }
            }
            //ResetTargets();
        }
    }
}
