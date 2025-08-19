using Macrocosm.Common.CrossMod;
using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Enums;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Biomes;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Projectiles.Hostile;
using Macrocosm.Content.Tiles.Blocks.Terrain;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
namespace Macrocosm.Content.NPCs.Enemies.Moon;

public class LuminiteElemental : ModNPC
{
    private static Asset<Texture2D> eyeTexture;
    private static Asset<Texture2D> pebbleTexture;

    public enum ActionState
    {
        Idle,
        Attacking,
        Panicking
    }

    public ActionState AI_State
    {
        get => (ActionState)NPC.ai[0];
        set => NPC.ai[0] = (float)value;
    }

    public bool Healing
    {
        get => NPC.ai[1] > 0f;
        set => NPC.ai[0] = value ? 1f : 0f;
    }

    public ref float AI_Timer => ref NPC.ai[2];
    public ref float AI_Speed => ref NPC.ai[3];

    // Extra AI
    public Vector2 TargetPosition { get; set; } = default;
    public int HealTimer { get; set; } = 0;

    public bool InTiles => NPC.active && Main.tile[NPC.Center.ToTileCoordinates()].HasTile;

    private int pebbleOrbitTimer;
    private float pebbleRotation;
    private float eyeScale;
    private int attackPeriod;
    private int panicAttackPeriod;

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 1;

        NPC.ApplyBuffImmunity
        (
            BuffID.Bleeding,
            BuffID.BloodButcherer,
            BuffID.Poisoned,
            BuffID.Venom
        );

        NPCSets.MoonNPC[Type] = true;

        NPCSets.Material[Type] = NPCMaterial.Metal;
        Redemption.AddElementToNPC(Type, Redemption.ElementID.Arcane);
        Redemption.AddElementToNPC(Type, Redemption.ElementID.Celestial);
        Redemption.AddNPCToElementList(Type, Redemption.NPCType.Inorganic);
    }

    public override void SetDefaults()
    {
        NPC.width = 38;
        NPC.height = 38;
        NPC.damage = 55;
        NPC.defense = 80;
        NPC.lifeMax = 1600;
        NPC.HitSound = SoundID.NPCDeath6 with { Pitch = 0.5f, Volume = 0.2f };
        NPC.DeathSound = SoundID.NPCDeath6;
        NPC.value = 60f;
        NPC.knockBackResist = 0.2f;
        NPC.aiStyle = -1;
        NPC.noTileCollide = true;
        NPC.noGravity = true;

        attackPeriod = 180;
        panicAttackPeriod = 30;

        SpawnModBiomes = [ModContent.GetInstance<MoonUndergroundBiome>().Type];
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
        => spawnInfo.SpawnTileY > Main.rockLayer && spawnInfo.SpawnTileType == ModContent.TileType<Protolith>() && !spawnInfo.PlayerSafe && !spawnInfo.PlayerInTown ? 0.025f : 0f;

    public override void ModifyNPCLoot(NPCLoot loot)
    {
        loot.Add(ItemDropRule.Common(ItemID.LunarOre, chanceDenominator: 2, minimumDropped: 2, maximumDropped: 12));
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        SpawnDusts(3);

        if (NPC.life <= 0)
            SpawnDusts(15);
    }

    public override void AI()
    {
        NPC.TargetClosest(faceTarget: true);
        Player player = Main.player[NPC.target];

        if (Main.rand.NextBool(25))
            SpawnDusts();
        bool clearLineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height);
        if (NPC.HasPlayerTarget && clearLineOfSight)
            AI_State = ActionState.Attacking;
        else
            AI_State = ActionState.Idle;

        if (NPC.life < NPC.lifeMax / 3)
            AI_State = ActionState.Panicking;

        switch (AI_State)
        {
            case ActionState.Idle:
                Idle();
                break;
            case ActionState.Attacking:
                Attack();
                break;
            case ActionState.Panicking:
                Panic();
                break;
        }

        AI_Timer++;

        NPC.rotation = NPC.velocity.X * 0.04f;
        NPC.spriteDirection = NPC.direction;

        Lighting.AddLight(NPC.Center - NPC.velocity, new Vector3(0.36f, 0.89f, 0.64f) * (InTiles ? 0.2f : 1f));

        pebbleOrbitTimer += 2;
        if (pebbleOrbitTimer >= 180)
            pebbleOrbitTimer = 0;
    }

    public void Idle()
    {
        if (AI_Timer % 100 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
        {
            Vector2 offset = new Vector2(Main.rand.Next(-200, 200), Main.rand.Next(-200, 200));
            TargetPosition = NPC.Center + offset;
            AI_Timer = 0;
            NPC.netUpdate = true;
        }

        Vector2 direction = (TargetPosition - NPC.Center).SafeNormalize(Vector2.UnitX);
        NPC.velocity = ((NPC.velocity + (direction * 0.1f)).SafeNormalize(Vector2.UnitX)) * 0.6f;
        AI_Speed = 1f;
    }

    public void Attack()
    {
        Player target = Main.player[NPC.target];
        bool clearLineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, target.position, target.width, target.height);

        if (!clearLineOfSight)
            AI_Timer--;

        if (AI_Timer >= attackPeriod)
        {
            if (clearLineOfSight && target.active && !target.dead)
            {
                SoundEngine.PlaySound(SoundID.Item28, NPC.Center);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < Main.rand.Next(3, 6); i++)
                    {
                        Vector2 projVelocity = Utility.PolarVector(8f, Main.rand.NextFloat(0, MathHelper.Pi * 2));
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, projVelocity, ModContent.ProjectileType<LuminitePebble>(), Utility.TrueDamage((int)(NPC.damage * 0.9f)), 1f, Main.myPlayer, ai1: NPC.target, ai2: 25f);
                    }
                }
            }

            AI_Timer = 0;
        }

        Player player = Main.player[NPC.target];

        AI_Speed += 0.03f;
        if (AI_Speed > 6f)
            AI_Speed = 6f;

        Vector2 direction = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
        NPC.velocity = ((NPC.velocity + (direction * 0.8f)).SafeNormalize(Vector2.UnitX)) * AI_Speed;
    }

    public void Panic()
    {
        Player target = Main.player[NPC.target];
        bool clearLineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, target.position, target.width, target.height);

        if (!clearLineOfSight)
            AI_Timer--;

        if (AI_Timer >= panicAttackPeriod)
        {
            if (clearLineOfSight && target.active && !target.dead)
            {
                SoundEngine.PlaySound(SoundID.Item28, NPC.Center);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 projVelocity = Utility.PolarVector(12f, Main.rand.NextFloat(0, MathHelper.Pi * 2));
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, projVelocity, ModContent.ProjectileType<LuminitePebble>(), Utility.TrueDamage((int)(NPC.damage * 0.9f)), 1f, Main.myPlayer, ai1: NPC.target, ai2: 20f);
                }
            }

            AI_Timer = 0;
        }

        Point luminiteCoords = Utility.GetClosestTile(NPC.Center, TileID.LunarOre, 100);
        if (luminiteCoords != default)
        {
            Vector2 luminitePosition = luminiteCoords.ToWorldCoordinates();

            if (Healing)
                NPC.velocity *= 0.92f;
            else
                NPC.velocity = (luminitePosition - NPC.Center).SafeNormalize(Vector2.UnitX) * 5f;

            if (Vector2.DistanceSquared(NPC.Center, luminitePosition) < 30 * 30 && NPC.life < NPC.lifeMax)
            {
                Healing = true;

                if (HealTimer++ % 10 == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item4 with { Volume = 0.2f, PitchRange = (0f, 0.2f) }, NPC.Center);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NPC.life += 10;
                        NPC.HealEffect(10, broadcast: true);
                        NPC.netUpdate = true;
                    }
                }
            }
            else
            {
                Healing = false;
            }
        }
        else
        {
            Player player = Main.player[NPC.target];
            AI_Speed += 0.03f;

            if (AI_Speed > 9f)
                AI_Speed = 9f;

            Vector2 direction = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
            NPC.velocity = ((NPC.velocity + (direction * 0.8f)).SafeNormalize(Vector2.UnitX)) * AI_Speed;
        }

        SpawnDusts(1);
    }

    public void SpawnDusts(int count = 1)
    {
        for (int i = 0; i < count; i++)
        {
            Vector2 dustVelocity = Utility.PolarVector(0.01f, Utility.RandomRotation());
            Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<LuminiteBrightDust>(), dustVelocity.X, dustVelocity.Y, newColor: Color.White * 0.1f);
        }
    }


    SpriteBatchState state;
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        pebbleRotation += 0.001f;
        Vector2 orbit = new((float)Math.Cos(MathHelper.ToRadians(pebbleOrbitTimer * 2)) * 45f, -(float)Math.Sin(MathHelper.ToRadians(pebbleOrbitTimer * 2)) * 12f);

        if ((-(float)Math.Sin(MathHelper.ToRadians(pebbleOrbitTimer * 2)) * 12f) < 0f)
            DrawPebbles(spriteBatch, drawColor, orbit);

        if (AI_State != ActionState.Panicking)
        {
            for (int i = 0; i < 10; i++)
            {
                Color glowColor = new Color(100, 243, 172, 0) * (1f - i / 10f) * 0.1f;
                float glowScale = NPC.scale + (0.07f * i);
                spriteBatch.Draw(TextureAssets.Npc[Type].Value, NPC.Center - Main.screenPosition, NPC.frame, glowColor, NPC.rotation, TextureAssets.Npc[Type].Size() / 2, glowScale, NPC.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            }
        }

        spriteBatch.Draw(TextureAssets.Npc[Type].Value, NPC.Center - Main.screenPosition, NPC.frame, drawColor, NPC.rotation, TextureAssets.Npc[Type].Size() / 2, NPC.scale, NPC.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
        DrawEye(spriteBatch);

        if ((-(float)Math.Sin(MathHelper.ToRadians(pebbleOrbitTimer * 2)) * 12f) >= 0f)
            DrawPebbles(spriteBatch, drawColor, orbit);

        return NPC.IsABestiaryIconDummy;
    }

    private void DrawPebbles(SpriteBatch spriteBatch, Color drawColor, Vector2 orbit)
    {
        pebbleTexture ??= ModContent.Request<Texture2D>(Texture + "_Pebble");
        Rectangle frame1 = pebbleTexture.Frame(verticalFrames: 2, frameY: 0);
        Rectangle frame2 = pebbleTexture.Frame(verticalFrames: 2, frameY: 1);

        int trailLength = 50;
        float opacityFactor = 0.1f;
        for (int i = 0; i < trailLength; i++)
        {
            float lerpFactor = i / (float)trailLength;
            Vector2 previousOrbit = new
            (
                (float)Math.Cos(MathHelper.ToRadians((pebbleOrbitTimer - i) * 2)) * 45f,
                -(float)Math.Sin(MathHelper.ToRadians((pebbleOrbitTimer - i) * 2)) * 12f
            );

            Color trailColor = new Color(100, 243, 172, 50) * (1f - lerpFactor) * opacityFactor;

            spriteBatch.Draw(pebbleTexture.Value, NPC.Center + previousOrbit.RotatedBy(MathHelper.ToRadians(-30)) - Main.screenPosition, frame1, trailColor, pebbleRotation, frame1.Size() / 2, NPC.scale * (1f - lerpFactor), SpriteEffects.None, 0f);
            spriteBatch.Draw(pebbleTexture.Value, NPC.Center + previousOrbit.RotatedBy(MathHelper.ToRadians(46)) - Main.screenPosition, frame2, trailColor, pebbleRotation, frame2.Size() / 2, NPC.scale * (1f - lerpFactor), SpriteEffects.FlipHorizontally, 0f);
        }

        for (int i = 0; i < 10; i++)
        {
            Color glowColor = default;
            float glowScale = NPC.scale;
            if (AI_State == ActionState.Attacking)
            {
                glowColor = new Color(100, 243, 172, 50) * (AI_Timer / attackPeriod) * (1f - i / 10f);
                glowScale = NPC.scale + (0.075f * i);
            }

            if (AI_State == ActionState.Panicking)
            {
                glowColor = new Color(100, 243, 172, 50) * (AI_Timer / panicAttackPeriod) * (1f - i / 10f);
                glowScale = NPC.scale + (0.075f * i);
            }
            spriteBatch.Draw(pebbleTexture.Value, NPC.Center + orbit.RotatedBy(MathHelper.ToRadians(-30)) - Main.screenPosition, frame1, glowColor, pebbleRotation, frame1.Size() / 2, glowScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(pebbleTexture.Value, NPC.Center + orbit.RotatedBy(MathHelper.ToRadians(46)) - Main.screenPosition, frame2, glowColor, pebbleRotation, frame2.Size() / 2, glowScale, SpriteEffects.FlipHorizontally, 0f);
        }

        spriteBatch.Draw(pebbleTexture.Value, NPC.Center + orbit.RotatedBy(MathHelper.ToRadians(-30)) - Main.screenPosition, frame1, drawColor, pebbleRotation, frame1.Size() / 2, NPC.scale, SpriteEffects.None, 0f);
        spriteBatch.Draw(pebbleTexture.Value, NPC.Center + orbit.RotatedBy(MathHelper.ToRadians(46)) - Main.screenPosition, frame2, drawColor, pebbleRotation, frame2.Size() / 2, NPC.scale, SpriteEffects.FlipHorizontally, 0f);
    }

    private void DrawEye(SpriteBatch spriteBatch)
    {
        eyeTexture ??= ModContent.Request<Texture2D>(Texture + "_Eye");

        float targetScale = NPC.scale * 0.5f * Main.rand.NextFloat(0.8f, 1f);
        if (AI_State == ActionState.Attacking)
            targetScale += 0.5f * (AI_Timer / attackPeriod);

        if (AI_State == ActionState.Panicking)
            targetScale += 0.5f * (AI_Timer / panicAttackPeriod);

        eyeScale = MathHelper.Lerp(targetScale, eyeScale, 1f - 0.075f);
        float eyeRotation = NPC.rotation;
        Vector2 position = NPC.Center + new Vector2(3.5f * NPC.direction, -2) - Main.screenPosition;
        if (NPC.HasValidTarget)
        {
            float radians = (Main.player[NPC.target].Center - NPC.Center).ToRotation();
            position += new Vector2(2).RotatedBy(radians);
        }

        state.SaveState(spriteBatch);
        spriteBatch.End();
        spriteBatch.Begin(BlendState.Additive, state);

        for (int i = 0; i < 10; i++)
        {
            Color eyeColor = default;
            if (AI_State == ActionState.Attacking)
                eyeColor = new Color(100, 243, 172) * (1f - i / 10f) * (AI_Timer / attackPeriod);

            if (AI_State == ActionState.Panicking)
                eyeColor = new Color(100, 243, 172) * (1f - i / 10f) * (AI_Timer / panicAttackPeriod);

            spriteBatch.Draw(eyeTexture.Value, position, null, eyeColor, eyeRotation, eyeTexture.Size() / 2, eyeScale + (0.115f * i), SpriteEffects.None, 0f);
        }

        spriteBatch.End();
        spriteBatch.Begin(state);

        spriteBatch.Draw(eyeTexture.Value, position, null, Color.White, eyeRotation, eyeTexture.Size() / 2, eyeScale, SpriteEffects.None, 0f);
    }

    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.WriteVector2(TargetPosition);
        writer.Write(HealTimer);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        TargetPosition = reader.ReadVector2();
        HealTimer = reader.ReadInt32();
    }
}
