using Macrocosm.Common.Bases.NPCs;
using Macrocosm.Common.CrossMod;
using Macrocosm.Common.Enums;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Biomes;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Projectiles.Hostile;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.NPCs.Enemies.Pollution;

public enum WyrmwoodSizeTier
{
    Unset,
    Small,
    Medium,
    Large
}

public class WyrmwoodHead : WormHead
{
    private const int BestiaryFrameCount = 4;
    private const int BestiaryTicksPerFrame = 8;
    private const int BaseLifeMax = 1200;
    private const int BaseSegmentLength = 5;
    private const int LifePerExtraSegment = 100;

    public int WingsType => ModContent.NPCType<WyrmwoodWings>();
    public override int BodyType => ModContent.NPCType<WyrmwoodBody>();
    public override int TailType => ModContent.NPCType<WyrmwoodTail>();
    public override bool HasCustomBodySegments => true;
    public WyrmwoodSizeTier SizeTier
    {
        get => GetSizeTier(NPC.ai[2]);
        set => NPC.ai[2] = (int)value;
    }

    public override void SetStaticDefaults()
    {
        NPCID.Sets.NPCBestiaryDrawModifiers value = new()
        {
            Position = new Vector2(0, -12f),
        };
        NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);

        NPC.ApplyBuffImmunity
        (
            BuffID.Confused
        );

        NPCSets.Material[Type] = NPCMaterial.Slime;
        Redemption.AddElementToNPC(Type, Redemption.ElementID.Shadow);
        Redemption.AddNPCToElementList(Type, Redemption.NPCType.Dark);
    }
    public override float FallSpeed => 0f;
    public override void SetDefaults()
    {
        NPC.CloneDefaults(NPCID.DiggerHead);
        NPC.lifeMax = BaseLifeMax;
        NPC.damage = 40;
        NPC.defense = 10;
        NPC.width = 16;
        NPC.height = 16;
        SpawnModBiomes = [ModContent.GetInstance<PollutionBiome>().Type];
        NPC.aiStyle = -1;
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        return spawnInfo.Player.InModBiome<PollutionBiome>() && Main.hardMode ? 1f : 0f;
    }

    public override void ModifyNPCLoot(NPCLoot loot)
    {
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (NPC.IsABestiaryIconDummy)
            return DrawBestiary(screenPos, drawColor);

        return base.PreDraw(spriteBatch, screenPos, drawColor);
    }

    private bool DrawBestiary(Vector2 screenPos, Color drawColor)
    {
        Texture2D texture = ModContent.Request<Texture2D>(Texture.Replace("Head", "") + "_Bestiary").Value;
        int frame = (int)(Main.GameUpdateCount / BestiaryTicksPerFrame) % BestiaryFrameCount;
        Rectangle sourceRect = texture.Frame(verticalFrames: BestiaryFrameCount, frameY: frame);

        NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = NPCID.Sets.NPCBestiaryDrawOffset.TryGetValue(Type, out var modifiers) ? modifiers : new();

        Vector2 offset = drawModifiers.Position;
        int direction = drawModifiers.Direction ?? -1;
        int spriteDirection = drawModifiers.SpriteDirection ?? direction;
        SpriteEffects effects = spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        Vector2 drawPosition = NPC.Center + offset - screenPos;

        Main.EntitySpriteDraw(
            texture,
            drawPosition,
            sourceRect,
            NPC.GetAlpha(drawColor),
            drawModifiers.Rotation,
            sourceRect.Size() * 0.5f,
            NPC.scale * drawModifiers.Scale,
            effects,
            0f
        );

        return false;
    }

    public override void Init()
    {
        if (Main.netMode != NetmodeID.MultiplayerClient)
        {
            SizeTier = SelectSizeTier();
            NPC.netUpdate = true;
        }

        ApplySizeTier();
        CanFly = true;
        CommonWormInit(this);
    }

    public override int SpawnBodySegments(int segmentCount)
    {
        int latestNPC = NPC.whoAmI;
        int wingsSegmentIndex = 0;
        IEntitySource source = NPC.GetSource_FromAI();
        WyrmwoodSizeTier sizeTier = SizeTier;
        ApplyLifeForSegmentCount(segmentCount + 2);

        for (int i = 0; i < segmentCount; i++)
        {
            int type = i == wingsSegmentIndex ? WingsType : BodyType;
            latestNPC = SpawnSegment(source, type, latestNPC, sizeTier);
        }

        return latestNPC;
    }

    private WyrmwoodSizeTier SelectSizeTier()
    {
        int roll = Main.rand.Next(100);

        if (roll < 50)
            return WyrmwoodSizeTier.Small;

        if (roll < 85)
            return WyrmwoodSizeTier.Medium;

        return WyrmwoodSizeTier.Large;
    }

    private void ApplyLifeForSegmentCount(int segmentCount)
    {
        int extraSegments = segmentCount - BaseSegmentLength;

        if (extraSegments < 0)
            extraSegments = 0;

        float lifeScale = NPC.lifeMax / (float)BaseLifeMax;
        int scaledLifePerExtraSegment = (int)(LifePerExtraSegment * lifeScale);

        NPC.lifeMax += extraSegments * scaledLifePerExtraSegment;
        NPC.life = NPC.lifeMax;
    }

    private void ApplySizeTier()
    {
        switch (SizeTier)
        {
            case WyrmwoodSizeTier.Small:
                MinSegmentLength = 5;
                MaxSegmentLength = 6;
                break;
            case WyrmwoodSizeTier.Medium:
                MinSegmentLength = 7;
                MaxSegmentLength = 9;
                break;
            case WyrmwoodSizeTier.Large:
                MinSegmentLength = 10;
                MaxSegmentLength = 12;
                break;
        }
    }

    private int SpawnSegment(IEntitySource source, int type, int latestNPC, WyrmwoodSizeTier sizeTier)
    {
        int oldLatest = latestNPC;
        latestNPC = NPC.NewNPC(source, (int)NPC.Center.X, (int)NPC.Center.Y, type, NPC.whoAmI, 0f, latestNPC, (int)sizeTier);

        Main.npc[oldLatest].ai[0] = latestNPC;

        NPC latest = Main.npc[latestNPC];
        latest.realLife = NPC.whoAmI;

        return latestNPC;
    }

    public static WyrmwoodSizeTier GetSizeTier(NPC npc)
    {
        if (npc.realLife >= 0 && npc.realLife < Main.maxNPCs && npc.realLife != npc.whoAmI && Main.npc[npc.realLife].active)
            return GetSizeTier(Main.npc[npc.realLife].ai[2]);

        return GetSizeTier(npc.ai[2]);
    }

    private static WyrmwoodSizeTier GetSizeTier(float sizeTierValue)
    {
        int sizeTier = (int)sizeTierValue;

        if (sizeTier < (int)WyrmwoodSizeTier.Small || sizeTier > (int)WyrmwoodSizeTier.Large)
            return WyrmwoodSizeTier.Medium;

        return (WyrmwoodSizeTier)sizeTier;
    }

    public static int NextInitialAttackCooldown(WyrmwoodSizeTier sizeTier) => GetSizeTierAttackCooldown(sizeTier, initial: true);

    public static int NextAttackCooldown(WyrmwoodSizeTier sizeTier) => GetSizeTierAttackCooldown(sizeTier, initial: false);

    private static int GetSizeTierAttackCooldown(WyrmwoodSizeTier sizeTier, bool initial)
    {
        return sizeTier switch
        {
            WyrmwoodSizeTier.Small => initial ? Main.rand.Next(480, 620) : Main.rand.Next(150, 240),
            WyrmwoodSizeTier.Medium => initial ? Main.rand.Next(400, 500) : Main.rand.Next(110, 190),
            WyrmwoodSizeTier.Large => initial ? Main.rand.Next(300, 420) : Main.rand.Next(80, 150),
            _ => initial ? Main.rand.Next(400, 500) : Main.rand.Next(100, 200),
        };
    }

    public static void CommonWormInit(Worm worm)
    {
        switch (GetSizeTier(worm.NPC))
        {
            case WyrmwoodSizeTier.Small:
                worm.MoveSpeed = 6.35f;
                worm.Acceleration = 0.055f;
                break;
            case WyrmwoodSizeTier.Medium:
                worm.MoveSpeed = 5.5f;
                worm.Acceleration = 0.04f;
                break;
            case WyrmwoodSizeTier.Large:
                worm.MoveSpeed = 4.85f;
                worm.Acceleration = 0.035f;
                break;
        }
    }

    private int attackCounter;
    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.Write(attackCounter);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        attackCounter = reader.ReadInt32();
    }

    public override void AI()
    {
        if (Main.netMode != NetmodeID.MultiplayerClient)
        {
            // tick down the attack counter.
            if (attackCounter > 0)
                attackCounter--;
        }
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        for (int i = 0; i < 10; i++)
        {
            int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<CoalDust>());
            Dust dust = Main.dust[dustIndex];
            dust.velocity.X *= dust.velocity.X * 1.25f * hit.HitDirection + Main.rand.Next(0, 100) * 0.015f;
            dust.velocity.Y *= dust.velocity.Y * 0.25f + Main.rand.Next(-50, 51) * 0.01f;
            dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
        }
    }
}

public class WyrmwoodBody : WormBody
{
    public WyrmwoodSizeTier SizeTier
    {
        get => WyrmwoodHead.GetSizeTier(NPC);
        set => NPC.ai[2] = (int)value;
    }

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[Type] = 1;

        NPC.ApplyBuffImmunity
        (
            BuffID.Confused
        );

        NPCID.Sets.NPCBestiaryDrawModifiers value = new() { Hide = true };
        NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
    }
    private int attackCounter;
    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.Write(attackCounter);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        attackCounter = reader.ReadInt32();
    }
    public override void SetDefaults()
    {
        NPC.CloneDefaults(NPCID.DiggerBody);
        NPC.damage = 20;
        NPC.defense = 20;
        NPC.npcSlots = 0f;
        NPC.width = 16;
        NPC.height = 18;
        NPC.aiStyle = -1;
    }

    public override void Init()
    {
        FlipSprite = true;
        attackCounter = WyrmwoodHead.NextInitialAttackCooldown(SizeTier);
        WyrmwoodHead.CommonWormInit(this);
    }
    public override void CustomBodyAI(Worm worm)
    {
        if (Main.netMode != NetmodeID.MultiplayerClient)
        {
            // tick down the attack counter.
            if (attackCounter > 0)
                attackCounter--;

            Player target = Main.player[NPC.target];
            if (attackCounter <= 0 && Vector2.Distance(NPC.Center, target.Center) < 1000f)
            {
                Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, 5), ModContent.ProjectileType<WyrmwoodProjectile>(), Utility.TrueDamage((int)(NPC.damage * 0.9f)), 1f, Main.myPlayer);
                attackCounter = WyrmwoodHead.NextAttackCooldown(SizeTier);
            }
        }
    }
    public override void OnSpawn(IEntitySource source)
    {
    }

    public override void FindFrame(int frameHeight)
    {
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        for (int i = 0; i < 10; i++)
        {
            int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<CoalDust>());
            Dust dust = Main.dust[dustIndex];
            dust.velocity.X *= dust.velocity.X * 1.25f * hit.HitDirection + Main.rand.Next(0, 100) * 0.015f;
            dust.velocity.Y *= dust.velocity.Y * 0.25f + Main.rand.Next(-50, 51) * 0.01f;
            dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
        }
    }
}

public class WyrmwoodWings : WyrmwoodBody
{
    private const int TicksPerFrame = 6;

    public override float SegmentSpacing => 12f;
    public override Vector2 SpriteDrawOffset => new Vector2(-4, 6);

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.npcFrameCount[Type] = 4;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 16;
        NPC.height = 16;
    }

    public override void FindFrame(int frameHeight)
    {
        int frame = (int)(NPC.frameCounter / TicksPerFrame) % Main.npcFrameCount[Type];
        NPC.frame = TextureAssets.Npc[Type].Frame(verticalFrames: Main.npcFrameCount[Type], frameY: frame);

        NPC.frameCounter++;
        if (NPC.frameCounter >= TicksPerFrame * Main.npcFrameCount[Type])
            NPC.frameCounter = 0;
    }

}

public class WyrmwoodTail : WormTail
{
    public override void SetStaticDefaults()
    {
        NPC.ApplyBuffImmunity
        (
            BuffID.Confused
        );

        NPCID.Sets.NPCBestiaryDrawModifiers value = new() { Hide = true };
        NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
    }

    public override void SetDefaults()
    {
        NPC.CloneDefaults(NPCID.DiggerTail);
        NPC.damage = 5;
        NPC.defense = 30;
        NPC.width = 16;
        NPC.height = 26;
        NPC.npcSlots = 0f;
        NPC.aiStyle = -1;
    }

    public override void Init()
    {
        FlipSprite = true;
        WyrmwoodHead.CommonWormInit(this);
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        for (int i = 0; i < 10; i++)
        {
            int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<CoalDust>());
            Dust dust = Main.dust[dustIndex];
            dust.velocity.X *= dust.velocity.X * 1.25f * hit.HitDirection + Main.rand.Next(0, 100) * 0.015f;
            dust.velocity.Y *= dust.velocity.Y * 0.25f + Main.rand.Next(-50, 51) * 0.01f;
            dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
        }
    }
}
