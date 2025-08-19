using Macrocosm.Common.CrossMod;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Biomes;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;


namespace Macrocosm.Content.NPCs.Enemies.Pollution;

public class SmogWisp : ModNPC
{
    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[Type] = 4;

        NPC.ApplyBuffImmunity
        (
            BuffID.Bleeding,
            BuffID.BloodButcherer,
            BuffID.Poisoned,
            BuffID.Venom
        );

        Redemption.AddElementToNPC(Type, Redemption.ElementID.Arcane);
        Redemption.AddElementToNPC(Type, Redemption.ElementID.Wind);
        Redemption.AddNPCToElementList(Type, Redemption.NPCType.Spirit);
    }

    public override void SetDefaults()
    {
        base.SetDefaults();

        NPC.width = 44;
        NPC.height = 38;
        NPC.damage = 12;
        NPC.defense = 10;
        NPC.lifeMax = 30;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath52;
        NPC.value = 60f;
        NPC.knockBackResist = 0f;
        SpawnModBiomes = [ModContent.GetInstance<PollutionBiome>().Type];
        NPC.Opacity = 0f;
        NPC.noTileCollide = true;
        NPC.noGravity = true;

    }

    public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
    {
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo) => spawnInfo.Player.InModBiome<PollutionBiome>() && (spawnInfo.SpawnTileY < spawnInfo.Player.Center.Y + 40) ? 1f : 0f;

    public override void ModifyNPCLoot(NPCLoot loot)
    {

    }


    public override void FindFrame(int frameHeight)
    {
        int frameSpeed = 8;

        NPC.frameCounter++;

        if (NPC.frameCounter >= frameSpeed)
        {
            NPC.frameCounter = 0;
            NPC.frame.Y += frameHeight;

            if (NPC.frame.Y >= Main.npcFrameCount[Type] * frameHeight)
            {
                NPC.frame.Y = 0;
            }
        }
    }
    int smogTimer = 0;
    public override void AI()
    {
        if (++smogTimer % 5 == 0)
        {
            Smoke smoke = Particle.Create<Smoke>((p) =>
                        {
                            p.Position = NPC.Center;
                            p.Velocity = new Vector2(0f, 1f).RotatedByRandom(MathHelper.TwoPi);
                            p.Acceleration = new Vector2(0f, 0f);
                            p.Scale = new(0.2f);
                            p.Rotation = 0f;
                            p.Color = (new Color(80, 80, 80) * Main.rand.NextFloat(0.75f, 1f)).WithAlpha(215);
                            p.VanillaUpdate = true;
                            p.Opacity = NPC.Opacity;
                            p.ScaleVelocity = new(0.0075f);
                            p.WindFactor = Main.windSpeedCurrent > 0 ? 0.035f : 0.01f;
                        });
        }
        Utility.AIFlier(NPC, ref NPC.ai, true, 0.2f, 0.2f, 2f, 2f, false);

        if (NPC.Opacity < 0.01f)
            NPC.dontTakeDamage = true;
        else
            NPC.dontTakeDamage = false;
        Player player = Main.player[NPC.target];
        if (player is not null)
        {
            NPC.rotation = NPC.Center.DirectionTo(Main.player[NPC.target].Center).ToRotation();
            if (Vector2.Distance(player.Center, NPC.Center) > 1000f)
                NPC.Opacity -= 0.01f;
            else if (Vector2.Distance(player.Center, NPC.Center) > 500f)
                NPC.Opacity += 0.001f;
            else
                NPC.Opacity += 0.01f;
        }
        if (NPC.Opacity > 0.7f)
            NPC.Opacity = 0.7f;
        if (NPC.Opacity < 0f)
            NPC.Opacity = 0f;
        NPC.direction = NPC.Center.X < player.Center.X ? 1 : -1;

    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        for (int i = 0; i < 5; i++)
        {
            int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<CoalDust>());
            Dust dust = Main.dust[dustIndex];
            dust.velocity.X *= dust.velocity.X * 1.25f * hit.HitDirection + Main.rand.Next(0, 100) * 0.015f;
            dust.velocity.Y *= dust.velocity.Y * 0.25f + Main.rand.Next(-50, 51) * 0.01f;
            dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (NPC.IsABestiaryIconDummy)
            NPC.Opacity = 0.7f;

        NPCID.Sets.TrailCacheLength[Type] = 25;
        NPCID.Sets.TrailingMode[Type] = 3;

        SpriteEffects effects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;
        int length = NPC.oldPos.Length;
        for (int i = 1; i < length; i++)
        {
            float progress = i / (float)length;
            Vector2 drawPos = NPC.oldPos[i] + NPC.frame.Size() / 2f - Main.screenPosition;
            Color trailColor = drawColor * Utility.QuadraticEaseIn(1f - progress) * 0.7f;
            float rotation = NPC.oldRot[i];
            float scale = NPC.scale * (1f - progress);

            int frameCount = Main.npcFrameCount[Type];
            int frameHeight = TextureAssets.Npc[Type].Height() / frameCount;
            int currentFrame = NPC.frame.Y / frameHeight;
            int trailFrameCounter = (currentFrame + i) % frameCount;
            Rectangle frame = TextureAssets.Npc[Type].Frame(verticalFrames: frameCount, frameY: trailFrameCounter);

            Main.EntitySpriteDraw(TextureAssets.Npc[Type].Value, drawPos, frame, trailColor * NPC.Opacity, rotation, frame.Size() / 2f, (float)scale, effects, 0f);
        }

        Main.EntitySpriteDraw(TextureAssets.Npc[Type].Value, NPC.position + NPC.frame.Size() / 2f - Main.screenPosition, NPC.frame, drawColor * NPC.Opacity, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0f);
        return false;
    }


}