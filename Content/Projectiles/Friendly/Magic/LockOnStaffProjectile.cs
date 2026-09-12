using Macrocosm.Common.Bases.Projectiles;
using Macrocosm.Common.Global.NPCs;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Magic;

public class LockOnStaffProjectile : ChargedHeldProjectile
{
    public ref float AI_UseCounter => ref Projectile.ai[1];

    private const int windupFrames = 5; // number of windup animaton frames
    private const int shootFrames = 1;  // number of shooting animaton frames

    private readonly int ManaUseAmount = 5;
    public int lockOnMax = 4;
    private readonly List<int> lockedTargets = new();

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

    public override bool PreDraw(Player player, ref Color lightColor)
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
        for (int i = 0; i < Main.maxNPCs; i++)
        {
            NPC npc = Main.npc[i];
            if (lockOnMax <= 0)
                return;

            if (npc.CanBeChasedBy() && Main.npc[i].getRect().Intersects(new Rectangle((int)(Main.MouseWorld.X - 10f), (int)(Main.MouseWorld.Y - 10f), 20, 20))
            && npc.GetGlobalNPC<MacrocosmNPC>().TargetedByHomingProjectile == false)
            {
                npc.GetGlobalNPC<MacrocosmNPC>().TargetedByHomingProjectile = true;
                lockedTargets.Add(i);
                SoundEngine.PlaySound(SoundID.Item29, npc.position);
                lockOnMax--;
            }
        }
    }

    private void ResetLockedTargets()
    {
        foreach (int targetIndex in lockedTargets)
        {
            if (targetIndex < 0 || targetIndex >= Main.maxNPCs)
                continue;

            NPC npc = Main.npc[targetIndex];
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

    private ProjectileAudioTracker tracker;
    private void PlaySounds()
    {
        if (!StillInUse)
            return;

        tracker ??= new(Projectile);

        SoundEngine.PlaySound(SFX.HandheldThrusterFlame with
        {
            Volume = 0.3f,
            PlayOnlyIfFocused = true,
            PauseBehavior = PauseBehavior.StopWhenGamePaused,
            MaxInstances = 1,
            SoundLimitBehavior = SoundLimitBehavior.IgnoreNew
        },
        Projectile.position, updateCallback: (sound) =>
        {
            sound.Position = Projectile.position;
            return FocusHelper.AllowGameplayInputs && tracker.IsActiveAndInGame();
        });

        if (!OwnerHasMana)
        {
            SoundEngine.PlaySound(SFX.HandheldThrusterOverheat with
            {
                Volume = 0.3f,
                PlayOnlyIfFocused = true,
                PauseBehavior = PauseBehavior.StopWhenGamePaused,
                MaxInstances = 1,
                SoundLimitBehavior = SoundLimitBehavior.IgnoreNew
            },
            Projectile.position, updateCallback: (sound) =>
            {
                sound.Position = Projectile.position;
                return FocusHelper.AllowGameplayInputs && tracker.IsActiveAndInGame();
            });
        }
    }

    public override void OnKill(int timeLeft)
    {
        if (lockedTargets.Count <= 0)
            return;

        int spawnedBolts = 0;
        foreach (int targetIndex in lockedTargets)
        {
            if (targetIndex < 0 || targetIndex >= Main.maxNPCs)
                continue;

            NPC npc = Main.npc[targetIndex];
            if (!npc.active || !npc.GetGlobalNPC<MacrocosmNPC>().TargetedByHomingProjectile)
                continue;

            Vector2 spawnOffset = new(-12f + spawnedBolts * 8f, 0f);
            int proj = Projectile.NewProjectile(
                Projectile.InheritSource(Projectile),
                Projectile.Center + spawnOffset,
                new Vector2(0, -16),
                ModContent.ProjectileType<LockOnStaffBolt>(),
                Projectile.damage,
                Projectile.knockBack,
                Main.player[Projectile.owner].whoAmI,
                ai2: targetIndex
            );

            if (proj >= 0 && proj < Main.maxProjectiles)
            {
                Player.CheckMana(Player.HeldItem.mana, true, false);
                spawnedBolts++;
            }
        }

        if (spawnedBolts == 0)
            ResetLockedTargets();
    }
}
