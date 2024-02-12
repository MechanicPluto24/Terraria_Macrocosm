using Macrocosm.Content.Biomes;
using Macrocosm.Content.NPCs.Global;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using ReLogic.Content;
using System;

namespace Macrocosm.Content.NPCs.Enemies.Moon
{
    public class Dweller : ModNPC, IMoonEnemy
    {
        private const float WalkCycleLength = MathHelper.TwoPi;
        private const int LegCount = 6;

        private float[] legRotations;
        private float walkCyclePosition;

        public Player TargetPlayer => Main.player[NPC.target];
        public bool HasTarget => TargetPlayer is not null;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            NPC.width = 50;
            NPC.height = 128;
            NPC.damage = 150;
            NPC.defense = 25;
            NPC.lifeMax = 6000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath2;
            NPC.knockBackResist = 0.5f;
            NPC.aiStyle = -1;

            SpawnModBiomes = new int[1] { ModContent.GetInstance<UndergroundMoonBiome>().Type };

            legRotations = new float[LegCount];  
            for (int i = 0; i < LegCount; i++)
                legRotations[i] = 0f;
        }

        public override void AI()
        {
            NPC.TargetClosest(faceTarget: false);

            if(HasTarget && NPC.DistanceSQ(TargetPlayer.Center) > 1f)
            {
                Vector2 direction = NPC.Center.DirectionTo(TargetPlayer.Center);
                NPC.velocity.X = direction.X * 5f;
            }

            if(NPC.collideX)
            {
                NPC.velocity.Y = -2;
                NPC.velocity.X = 5;
            }

            if(Math.Abs(NPC.velocity.X) > 0.01f)
            {
                walkCyclePosition += NPC.velocity.X * 0.1f;
                if (walkCyclePosition > WalkCycleLength)
                {
                    walkCyclePosition -= WalkCycleLength;
   
                }

                for (int i = 0; i < LegCount; i++)
                    legRotations[i] += 0.02f * i;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D head = ModContent.Request<Texture2D>(Texture, AssetRequestMode.ImmediateLoad).Value;
            Texture2D leg1 = ModContent.Request<Texture2D>(Texture + "_Leg1", AssetRequestMode.ImmediateLoad).Value;
            Texture2D leg2 = ModContent.Request<Texture2D>(Texture + "_Leg2", AssetRequestMode.ImmediateLoad).Value;
            Texture2D foot = ModContent.Request<Texture2D>(Texture + "_Foot", AssetRequestMode.ImmediateLoad).Value;

            SpriteEffects effects = NPC.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < LegCount; i++)
            {
                Vector2 offset = new Vector2(-45, 58) + new Vector2(i * 18, 0);
                spriteBatch.Draw(leg1, NPC.position + offset - Main.screenPosition, null, drawColor, legRotations[i], leg1.Size()/2f, NPC.scale, effects, 0);
            }

            spriteBatch.Draw(head, NPC.position - Main.screenPosition, NPC.frame, drawColor, NPC.rotation, head.Size() / 2f, NPC.scale, effects, 0);

            return false;
        }
    }
}