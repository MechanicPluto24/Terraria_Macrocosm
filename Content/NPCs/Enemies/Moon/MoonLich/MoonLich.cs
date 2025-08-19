using Macrocosm.Common.CrossMod;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Items.Drops;
using Macrocosm.Content.Particles;
using Macrocosm.Content.Projectiles.Hostile;
using Macrocosm.Content.Tiles.Blocks.Terrain;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
namespace Macrocosm.Content.NPCs.Enemies.Moon.MoonLich;

public class MoonLich : ModNPC
{
    public override bool IsLoadingEnabled(Mod mod) => false;

    private static Asset<Texture2D> handTexture;

    private float offsetY = 0f;
    private int timer;
    private bool summoned = false;

    public ref float AI_Speed => ref NPC.ai[0];

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 4;

        NPCSets.MoonNPC[Type] = true;
        
        Redemption.AddElement(NPC, Redemption.ElementID.Celestial);
    }

    public override void SetDefaults()
    {
        NPC.width = 46;
        NPC.height = 46;
        NPC.lifeMax = 15000;
        NPC.damage = 55;
        NPC.defense = 90;
        NPC.HitSound = SoundID.NPCHit2;
        NPC.DeathSound = SoundID.NPCDeath2;
        NPC.aiStyle = -1;
        NPC.value = 100f;
        NPC.knockBackResist = 0f;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        AI_Speed = 5f;
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo) => spawnInfo.SpawnTileY > Main.rockLayer && spawnInfo.SpawnTileType == ModContent.TileType<Protolith>() ? 0.002f : 0f;

    public override void ModifyNPCLoot(NPCLoot loot)
    {
        loot.Add(ItemDropRule.Common(ModContent.ItemType<SpaceDust>(), 1, 3, 5));
    }

    public override void AI()
    {
        NPC.TargetClosest();
        Player player = Main.player[NPC.target];
        bool clearLineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height);
        Lighting.AddLight(NPC.Center, (new Vector3(0.4f, 1f, 1f)));
        offsetY = (float)(Math.Sin(timer / 10) * 7);
        timer++;
        if (timer % 5 == 0)
            Particle.Create<TintableFire>(p =>
            {
                p.Position = NPC.position;
                p.Velocity = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi);
                p.Color = new Color(34, 221, 151, 0);
                p.Scale = new(0.1f);
            });

        if (clearLineOfSight && player.active && !player.dead)
        {
            NPC.Move(player.Center, Vector2.Zero, AI_Speed, 0.1f);
            NPC.velocity += NPC.velocity.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2) * MathF.Sin(Main.GameUpdateCount * 0.05f);

            if (timer % 120 > 80)
            {
                Dust.NewDust(NPC.Center + new Vector2(35f, 50f + offsetY), 0, 0, ModContent.DustType<LuminiteBrightDust>());
                Dust.NewDust(NPC.Center + new Vector2(-25f, 50f + offsetY), 0, 0, ModContent.DustType<LuminiteBrightDust>());
            }

            if (timer % 120 == 119)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (Main.rand.NextBool(5))
                    {
                        for (int i = -1; i < 2; i++)
                        {
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + new Vector2(35f, 50f + offsetY), (Main.player[NPC.target].Center - (NPC.Center + new Vector2(35f, 50f + offsetY))).SafeNormalize(Vector2.UnitX).RotatedBy((MathHelper.PiOver4 / 2) * i) * 14f, ModContent.ProjectileType<LichBolt>(), Utility.TrueDamage((int)(NPC.damage * 1.15f)), 1f, Main.myPlayer, ai1: NPC.target);
                        }
                        for (int i = -1; i < 2; i++)
                        {
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + new Vector2(-25f, 50f + offsetY), (Main.player[NPC.target].Center - (NPC.Center + new Vector2(-25f, 50f + offsetY))).SafeNormalize(Vector2.UnitX).RotatedBy((MathHelper.PiOver4 / 2) * i) * 14f, ModContent.ProjectileType<LichBolt>(), Utility.TrueDamage((int)(NPC.damage * 1.15f)), 1f, Main.myPlayer, ai1: NPC.target);
                        }
                    }
                    else
                    {
                        Vector2 projVelocity = Main.player[NPC.target].Center - (NPC.Center + new Vector2(35f, 50f + offsetY));
                        projVelocity = projVelocity.SafeNormalize(Vector2.UnitX);
                        projVelocity = (projVelocity + Main.player[NPC.target].velocity * 0.1f).SafeNormalize(Vector2.UnitX);
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + new Vector2(35f, 50f + offsetY), projVelocity * 14f, ModContent.ProjectileType<LichBolt>(), Utility.TrueDamage((int)(NPC.damage * 1.15f)), 1f, Main.myPlayer, ai1: NPC.target);
                        Vector2 projVelocity2 = Main.player[NPC.target].Center - (NPC.Center + new Vector2(-25f, 50f + offsetY));
                        projVelocity2 = projVelocity2.SafeNormalize(Vector2.UnitX);
                        projVelocity2 = (projVelocity2 + Main.player[NPC.target].velocity * 0.1f).SafeNormalize(Vector2.UnitX);
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + new Vector2(-25f, 50f + offsetY), projVelocity2 * 14f, ModContent.ProjectileType<LichBolt>(), Utility.TrueDamage((int)(NPC.damage * 1.15f)), 1f, Main.myPlayer, ai1: NPC.target);
                    }
                }
            }
        }
        else
        {
            NPC.velocity *= 0.8f;
        }

        if (Vector2.Distance(NPC.Center, player.Center) < 300f)
            AI_Speed *= 0.95f;
        else
            AI_Speed *= 1.05f;

        if (AI_Speed > 5f)
            AI_Speed = 5f;

        if (AI_Speed < 0.05f)
            AI_Speed = 0.05f;

        if (clearLineOfSight && player.active && !player.dead && summoned == false)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Vector2 projVelocity = Utility.PolarVector(1.5f, Main.rand.NextFloat(0, MathHelper.Pi * 2));
                Projectile proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, projVelocity, ModContent.ProjectileType<MoonLichNPCSummon>(), Utility.TrueDamage((int)(0)), 1f, Main.myPlayer, NPC.target);
                proj.netUpdate = true;
            }
            summoned = true;
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        Vector2 drawPos = (NPC.Center + new Vector2(35f, 50f + offsetY)) - Main.screenPosition;
        Color colour = NPC.GetAlpha(drawColor);

        //Right
        handTexture ??= ModContent.Request<Texture2D>("Macrocosm/Content/NPCs/Enemies/Moon/MoonLich/MoonLichHand");

        spriteBatch.Draw(handTexture.Value, drawPos, null, colour, NPC.rotation, NPC.Size / 2, NPC.scale, SpriteEffects.FlipHorizontally, 0f);

        Vector2 drawPos2 = (NPC.Center + new Vector2(-25f, 50f + offsetY)) - Main.screenPosition;
        spriteBatch.Draw(handTexture.Value, drawPos2, null, colour, NPC.rotation, NPC.Size / 2, NPC.scale, SpriteEffects.None, 0f);

        return true;
    }

    public override void FindFrame(int frameHeight)
    {
        NPC.spriteDirection = -NPC.direction;
        int frameSpeed = 9;

        NPC.frameCounter++;

        if (NPC.frameCounter >= frameSpeed)
        {
            NPC.frameCounter = 0;
            NPC.frame.Y += frameHeight;

            if (NPC.frame.Y >= Main.npcFrameCount[Type] * frameHeight)
                NPC.frame.Y = 0;
        }
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        for (int i = 0; i < 8; i++)
        {
            int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<RegolithDust>());
            Dust dust = Main.dust[dustIndex];
            dust.velocity.X *= dust.velocity.X * 1.25f * hit.HitDirection + Main.rand.Next(0, 100) * 0.015f;
            dust.velocity.Y *= dust.velocity.Y * 0.25f + Main.rand.Next(-50, 51) * 0.01f;
            dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
        }

        if (Main.dedServ)
            return;

        if (NPC.life <= 0)
        {

            for (int i = 0; i < 50; i++)
            {
                int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<RegolithDust>());
                Dust dust = Main.dust[dustIndex];
                dust.velocity.X *= dust.velocity.X * 1.25f * hit.HitDirection + Main.rand.Next(0, 100) * 0.015f;
                dust.velocity.Y *= dust.velocity.Y * 0.25f + Main.rand.Next(-50, 51) * 0.01f;
                dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
            }
        }
    }
}
