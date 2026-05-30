using Macrocosm.Common.CrossMod;
using Macrocosm.Common.Enums;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Items.LiquidContainers;
using Macrocosm.Content.Sounds;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.NPCs.Enemies.Moon;

public class ZombieScientist : ModNPC
{

    public enum ActionState
    {
        walk,
        throwing
    }

    private readonly Range throwFrames = 0..10;
    private readonly Range idleFrames = 11..20;
    private readonly int airFrame = 21;

    public ref float AI_State => ref NPC.ai[0];
    public ref float AI_Timer => ref NPC.ai[1];

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[Type] = 22;

        NPCSets.MoonNPC[Type] = true;

        NPCSets.Material[Type] = NPCMaterial.Organic;
        Redemption.AddNPCToElementList(Type, Redemption.NPCType.Undead);
        Redemption.AddNPCToElementList(Type, Redemption.NPCType.Humanoid);
    }

    public override void SetDefaults()
    {
        NPC.width = 18;
        NPC.height = 44;
        NPC.damage = 65;
        NPC.defense = 40;
        NPC.lifeMax = 2000;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SFX.ZombieDeath;
        NPC.knockBackResist = 0.5f;
        NPC.aiStyle = -1;
    }

    public override bool PreAI()
    {
        if (NPC.velocity.Y < 0f)
            NPC.velocity.Y += 0.1f;

        return true;
    }

    public override void FindFrame(int frameHeight)
    {
        NPC.spriteDirection = NPC.velocity.X > 0f ? -1 : 1;
        int frameIndex = NPC.frame.Y / frameHeight;
        NPC.frameCounter++;
        switch (AI_State)
        {
            case (float)ActionState.walk:
                
                if (NPC.velocity.Y != 0)
                {
                    NPC.frame.Y = airFrame * frameHeight;
                }
                else
                {
                    if (!idleFrames.Contains(frameIndex))
                        NPC.frame.Y = frameHeight * idleFrames.Start.Value;

                    // Update frame
                    if (NPC.frameCounter > 5)
                    {
                        NPC.frame.Y += frameHeight;
                        NPC.frameCounter = 0.0;
                    }

                    if (frameIndex >= idleFrames.End.Value)
                        NPC.frame.Y = frameHeight * idleFrames.Start.Value;
                }
                break;
            case (float)ActionState.throwing:

                if (!throwFrames.Contains(frameIndex))
                    NPC.frame.Y = frameHeight * throwFrames.Start.Value;

                // Update frame
                if (NPC.frameCounter > 5)
                {
                    NPC.frame.Y += frameHeight;
                    NPC.frameCounter = 0.0;
                }

                if (frameIndex >= idleFrames.End.Value)
                    NPC.frame.Y = frameHeight * throwFrames.Start.Value;

                break;
        }
    }

    public override void AI()
    {
        NPC.TargetClosest();
        NPC.FaceTarget();
        Player target = Main.player[NPC.target];
        bool clearLineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, target.position, target.width, target.height);
        if (clearLineOfSight && Vector2.Distance(NPC.Center, target.Center) < 400f)
        {
            NPC.aiStyle = -1;
            AI_State = (float)ActionState.throwing;
        }
        else
        {
            AI_State = (float)ActionState.walk;
        }
        switch (AI_State)
        {
            case (float)ActionState.walk:
                walk();
                break;
            case (float)ActionState.throwing:
                throwing();
                break;
        }
    }

    private void walk()
    {
        NPC.aiStyle = NPCAIStyleID.Fighter;
    }

    public enum Flasks
    {
        acid,
        oil,
        prometheum,
        distortion,
        confetti
    }

    private float[] lastFlask = new float[5]; // stores the last 5 thrown flasks
    private void updateLastFlask(float justThrew) // updates lastFlask
    {
        for (int i = lastFlask.Length - 2; i > -1; i--)
        {
            lastFlask[i + 1] = lastFlask[i];
        }
        lastFlask[0] = justThrew;
    }

    private float calculateNextFlask() // decides what to throw next based on the last thrown projectiles
    {
        if (lastFlask[0] == (float)Flasks.oil)
        {
            return (float)Flasks.prometheum;
        }
        else
        {
            return (float)Flasks.acid;
        }
    }

    private float calculateSpeed(float flask) // changes speed based on what flask is being thrown
    {
        switch (flask)
        {
            case (float)Flasks.acid:
                return 6f;
            case (float)Flasks.oil:
                return 7f;
            case (float)Flasks.prometheum:
                return 10f;
            case (float)Flasks.distortion:
                return 5f;
            case (float)Flasks.confetti:
                return 4f;
        }
        return 6f;
    }

    private void throwing()
    {
        Player target = Main.player[NPC.target];
        if (NPC.velocity.Y == 0)
        {
            NPC.velocity.X = 0;
            AI_Timer++;
            int timeLimit = 60; // throw constantly at this interval

            if (AI_Timer == timeLimit - 20)
            {
                Vector2 playerDirection = target.Center - NPC.Center;
                float nextFlask = calculateNextFlask();
                float speed = calculateSpeed(nextFlask);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, playerDirection.SafeNormalize(Vector2.UnitX) * speed, ModContent.ProjectileType<Projectiles.Hostile.ZombieChemistVial>(), NPC.damage, 2, -1, nextFlask);
                updateLastFlask(nextFlask);

            }
            else if (AI_Timer >= timeLimit)
            {
                // if the throwing conditions arent met anymore then stop throwing
                AI_Timer = 0;
                // otherwise keep throwing
            }
        }
        else
        {
            AI_Timer = 0;
        }
    }

    // CURRENT ISSUES: flask things not done, flask not done, issue with direction and transitioning from walk to jump


    public override void ModifyNPCLoot(NPCLoot loot)
    {
        loot.Add(ItemDropRule.Common(ModContent.ItemType<RocketFuelCanister>(), 10, 1, 4));
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        if (NPC.life > 0)
        {
            for (int i = 0; i < 30; i++)
            {
                int dustType = Utils.SelectRandom<int>(Main.rand, ModContent.DustType<RegolithDust>(), DustID.Blood);

                Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, dustType);
                dust.velocity.X *= (dust.velocity.X + +Main.rand.Next(0, 100) * 0.015f) * hit.HitDirection;
                dust.velocity.Y = 3f + Main.rand.Next(-50, 51) * 0.01f;
                dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
                dust.noGravity = true;
            }
        }

        if (Main.dedServ)
            return; // don't run on the server

        if (NPC.life <= 0)
        {
            var entitySource = NPC.GetSource_Death();

            Gore.NewGore(entitySource, NPC.position, NPC.velocity, Mod.Find<ModGore>("ZombieScientistGoreHead").Type);
            Gore.NewGore(entitySource, NPC.position, NPC.velocity, Mod.Find<ModGore>("ZombieScientistGoreArm").Type);
            Gore.NewGore(entitySource, NPC.position, NPC.velocity, Mod.Find<ModGore>("ZombieScientistGoreGoggles").Type);
            Gore.NewGore(entitySource, NPC.position, NPC.velocity, Mod.Find<ModGore>("ZombieScientistGoreLeg1").Type);
            Gore.NewGore(entitySource, NPC.position, NPC.velocity, Mod.Find<ModGore>("ZombieScientistGoreLeg2").Type);
        }
    }
}
