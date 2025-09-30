using Macrocosm.Common.CrossMod;
using Macrocosm.Common.Enums;
using Macrocosm.Common.Sets;
using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Biomes;
using Macrocosm.Content.Items.Drops;
using Macrocosm.Content.Tiles.Blocks.Terrain;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Macrocosm.Content.Trails;

namespace Macrocosm.Content.NPCs.Enemies.SolarStorm;

public class SolarGranule : ModNPC
{
    private static Asset<Texture2D> glow;
    public enum ActionState
    {
        Idle,
        Attack
    }

    public ActionState AI_State
    {
        get => (ActionState)NPC.ai[0];
        set => NPC.ai[0] = (float)value;
    }

    public ref float AI_Speed => ref NPC.ai[1];

    public ref float AI_Rage => ref NPC.ai[2];

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[Type] = 1;

        NPC.ApplyBuffImmunity
        (
            BuffID.Bleeding,
            BuffID.BloodButcherer,
            BuffID.Poisoned,
            BuffID.Venom
        );

        NPCSets.SolarStormNPC[Type] = true;
        NPCSets.MoonNPC[Type] = true;

        NPCID.Sets.TrailCacheLength[NPC.type] = 25;
        NPCID.Sets.TrailingMode[NPC.type] = 1;

        NPCSets.Material[Type] = NPCMaterial.Supernatural;
        Redemption.AddElementToNPC(Type, Redemption.ElementID.Arcane);
        Redemption.AddElementToNPC(Type, Redemption.ElementID.Fire);
        Redemption.AddNPCToElementList(Type, Redemption.NPCType.Spirit);
    }
    private HorusTrail trail;

    public override void SetDefaults()
    {
        NPC.width = 32;
        NPC.height = 32;
        NPC.lifeMax = 2000;
        NPC.damage = 55;
        NPC.defense = 10;
        //NPC.HitSound = SoundID.NPCHit36 with { Volume = 0.5f }; //please find suitable sounds or maybe I'll do it, I guess I'll do it eventually
        //NPC.DeathSound = SoundID.NPCDeath6;
        NPC.value = 60f;
        NPC.knockBackResist = 0f;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        SpawnModBiomes = [ModContent.GetInstance<SolarStormBiome>().Type];
        AI_State = ActionState.Idle;
        trail = new();

        if (NPC.IsABestiaryIconDummy)
            NPC.Opacity = 1f;
    }
        private float glowTimer = 0;
    private int shineOrbitTimer = 0;
    private Vector2 shineOrbit = new(32f, 12f);
    public override void AI()
    {

        NPC.TargetClosest(true);

        Player player = Main.player[NPC.target];
        bool clearLineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height);
        NPC.direction = NPC.Center.X < player.Center.X ? 1 : -1;
        NPC.rotation = NPC.Center.DirectionTo(Main.player[NPC.target].Center).ToRotation();

        if (NPC.HasPlayerTarget && clearLineOfSight && AI_State == ActionState.Idle)
            AI_State = ActionState.Attack;//Slowly approach the player

        switch (AI_State)
        {
            case ActionState.Idle:
                Idle();
                break;
            case ActionState.Attack:
                Attack();
                break;
        }

        // Shine
        shineOrbitTimer += 8;
        if (shineOrbitTimer >= 180)
            shineOrbitTimer = 0;

        // Glow
        glowTimer++;

        // Light
        Lighting.AddLight(NPC.Center, new Color(255, 141, 114).ToVector3() * 0.48f);

    
        NPC.netUpdate = true;
    }

    public void Idle()
    {
        //Does nothing
        AI_Speed = 1f;
        NPC.velocity *= 0f;
    }

    public void Attack()
    {
        Player player = Main.player[NPC.target];
        Vector2 direction = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitX);

        NPC.velocity +=(player.Center - NPC.Center).SafeNormalize(Vector2.UnitX)*0.4f;
        if(NPC.velocity.Length()>17f)
            NPC.velocity=NPC.velocity.SafeNormalize(Vector2.UnitX)*17f;
    }


    public override float SpawnChance(NPCSpawnInfo spawnInfo)
        => spawnInfo.Player.InModBiome<SolarStormBiome>() ? 0.5f : 0f;

    public override void ModifyNPCLoot(NPCLoot loot)
    {
        loot.Add(ItemDropRule.Common(ModContent.ItemType<SpaceDust>(), 4));
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        if (NPC.life > 0)
        {
            for (int i = 0; i < 20; i++)
            {
                Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.SolarFlare);
                dust.velocity.X = (dust.velocity.X + Main.rand.Next(0, 100) * 0.02f) * hit.HitDirection;
                dust.velocity.Y = 1f + Main.rand.Next(-50, 51) * 0.01f;
                dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
                dust.noGravity = true;
            }
        }
    }

    private SpriteBatchState state;
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        Vector2 orbit = new((float)Math.Cos(MathHelper.ToRadians(shineOrbitTimer * 2)) * shineOrbit.X, -(float)Math.Sin(MathHelper.ToRadians(shineOrbitTimer * 2)) * shineOrbit.Y);

        state.SaveState(spriteBatch);
        spriteBatch.End();
        spriteBatch.Begin(BlendState.Additive, state);

        if ((-MathF.Sin(MathHelper.ToRadians(shineOrbitTimer * 2)) * shineOrbit.Y) < 0f)
            DrawShine(Main.spriteBatch, new Color(255, 170, 142), orbit);

        trail?.Draw(NPC, NPC.Size / 2f);

        spriteBatch.End();
        spriteBatch.Begin(state);
        return true;
    }
    public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        state.SaveState(spriteBatch);
        Vector2 orbit = new((float)Math.Cos(MathHelper.ToRadians(shineOrbitTimer * 2)) * shineOrbit.X, -(float)Math.Sin(MathHelper.ToRadians(shineOrbitTimer * 2)) * shineOrbit.Y);

        spriteBatch.End();
        spriteBatch.Begin(BlendState.Additive, state);

        glow ??= ModContent.Request<Texture2D>(Macrocosm.FancyTexturesPath + "Star5");
        float glowOpacity = 0.25f + (MathF.Sin(MathHelper.TwoPi * glowTimer / 90f) + 1f) * 0.5f * 0.75f; 
        Main.EntitySpriteDraw(glow.Value, NPC.Center - Main.screenPosition + new Vector2(0, 8), null, new Color(255, 170, 142) * glowOpacity, NPC.rotation, glow.Size() / 2, NPC.scale * Main.rand.NextFloat(0.19f, 0.21f), SpriteEffects.None, 0f);
        
        if ((-MathF.Sin(MathHelper.ToRadians(shineOrbitTimer * 2)) * shineOrbit.Y) >= 0f)
            DrawShine(spriteBatch, new Color(127, 200, 155), orbit);

        spriteBatch.End();
        spriteBatch.Begin(state);
    }
    private void DrawShine(SpriteBatch spriteBatch, Color drawColor, Vector2 orbit)
    {
        Texture2D shineTexture = TextureAssets.Extra[ExtrasID.SharpTears].Value;
        int trailLength = 80;
        float opacityFactor = 0.75f;
        float rotation = orbit.ToRotation() + MathHelper.Pi / 4f;
        Vector2 position = NPC.Center + new Vector2(0, 8) - Main.screenPosition;
        for (int i = 0; i < trailLength; i++)
        {
            float lerpFactor = i / (float)trailLength;
            Vector2 previousOrbit = new
            (
                (float)Math.Cos(MathHelper.ToRadians((shineOrbitTimer - i) * 2)) * shineOrbit.X,
                -(float)Math.Sin(MathHelper.ToRadians((shineOrbitTimer - i) * 2)) * shineOrbit.Y
            );

            Color trailColor = drawColor * (1f - lerpFactor) * opacityFactor;
            spriteBatch.Draw(shineTexture, position + previousOrbit.RotatedBy(MathHelper.ToRadians(-70)) , null, trailColor, rotation, shineTexture.Size() / 2, NPC.scale * (1f - lerpFactor) * 0.4f, SpriteEffects.None, 0f);
        }

        spriteBatch.Draw(shineTexture, position + orbit.RotatedBy(MathHelper.ToRadians(-70)), null, drawColor, rotation, shineTexture.Size() / 2, NPC.scale * 0.4f, SpriteEffects.None, 0f);
    }
}