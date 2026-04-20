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

public class WyrmwoodHead : WormHead
{
    private const int BestiaryFrameCount = 4;
    private const int BestiaryTicksPerFrame = 8;

    public int WingsType => ModContent.NPCType<WyrmwoodWings>();
    public override int BodyType => ModContent.NPCType<WyrmwoodBody>();
    public override int TailType => ModContent.NPCType<WyrmwoodTail>();
    public override bool HasCustomBodySegments => true;

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
        NPC.lifeMax = 1200;
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
        // Set the segment variance
        // If you want the segment length to be constant, set these two properties to the same value
        MinSegmentLength = 5;
        MaxSegmentLength = 9;
        CanFly = true;
        CommonWormInit(this);
    }

    public override int SpawnBodySegments(int segmentCount)
    {
        int latestNPC = NPC.whoAmI;
        int wingsSegmentIndex = 0;
        IEntitySource source = NPC.GetSource_FromAI();

        for (int i = 0; i < segmentCount; i++)
        {
            int type = i == wingsSegmentIndex ? WingsType : BodyType;
            latestNPC = SpawnSegment(source, type, latestNPC);
        }

        return latestNPC;
    }

    public static void CommonWormInit(Worm worm)
    {
        // These two properties handle the movement of the worm
        worm.MoveSpeed = 5.5f;
        worm.Acceleration = 0.04f;
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
        attackCounter = Main.rand.Next(400, 500);
    }

    public override void Init()
    {
        FlipSprite = true;
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
                attackCounter = Main.rand.Next(100, 200);
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
