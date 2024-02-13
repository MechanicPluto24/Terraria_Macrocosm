using Macrocosm.Content.Biomes;
using Macrocosm.Content.NPCs.Global;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using ReLogic.Content;
using System;
using Macrocosm.Common.Utils;

namespace Macrocosm.Content.NPCs.Enemies.Moon
{
    public class Dweller : ModNPC, IMoonEnemy
    {
        private const float WalkCycleLength = MathHelper.TwoPi;
        private const int LegCount = 6;
        private const int SegmentCount = 3;

        private float[][] legSegmentRotations; // 3 segments per leg
        private Vector2[] legBasePositions; // Starting positions for each leg
        private Vector2[] legSegmentLengths; // Lengths of each leg segment

        private float walkCyclePosition;
        private int walkCycleDirection = 1;

        private Texture2D head;
        private Texture2D leg1;
        private Texture2D leg2;
        private Texture2D foot;

        public Player TargetPlayer => Main.player[NPC.target];
        public bool HasTarget => TargetPlayer is not null;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            NPC.width = 150;
            NPC.height = 220;
            NPC.damage = 150;
            NPC.defense = 25;
            NPC.lifeMax = 6000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath2;
            NPC.knockBackResist = 0.5f;
            NPC.aiStyle = -1;

            SpawnModBiomes = new int[1] { ModContent.GetInstance<UndergroundMoonBiome>().Type };

            InitializeLegs();
        }

        private void InitializeLegs()
        {
            head = ModContent.Request<Texture2D>(Texture, AssetRequestMode.ImmediateLoad).Value;
            leg1 = ModContent.Request<Texture2D>(Texture + "_Leg1", AssetRequestMode.ImmediateLoad).Value;
            leg2 = ModContent.Request<Texture2D>(Texture + "_Leg2", AssetRequestMode.ImmediateLoad).Value;
            foot = ModContent.Request<Texture2D>(Texture + "_Foot", AssetRequestMode.ImmediateLoad).Value;

            legBasePositions = new Vector2[LegCount];
            legSegmentLengths = new Vector2[SegmentCount];
            legSegmentRotations = new float[LegCount][];

            for (int i = 0; i < LegCount; i++)
            {
                legSegmentRotations[i] = new float[SegmentCount]; // 3 segments per leg

                legBasePositions[i] = new Vector2(28 - (12 * i), head.Height); // Adjust as necessary
            }

            legSegmentLengths[0] = new Vector2(0, leg1.Height); // Replace with actual length of segment 1
            legSegmentLengths[1] = new Vector2(0, leg2.Height); // Replace with actual length of segment 2
            legSegmentLengths[2] = new Vector2(0, foot.Height); // Replace with actual length of segment 3
        }

        public override void AI()
        {
            InitializeLegs();

            NPC.TargetClosest(faceTarget: false);

            if(HasTarget && NPC.DistanceSQ(TargetPlayer.Center) > 1f)
            {
                Vector2 direction = NPC.Center.DirectionTo(TargetPlayer.Center);
                NPC.velocity.X = direction.X * 5f;
            }

            if(NPC.collideX)
            {
                NPC.velocity.Y = -5;
            }
            else if(!NPC.collideY && NPC.velocity.Y < 0)
            {
                NPC.velocity.Y += 10;
            }

            walkCyclePosition -= NPC.velocity.X * 0.005f * walkCycleDirection;
            if (Math.Abs(walkCyclePosition) > MathHelper.Pi/8)
                walkCycleDirection *= -1;

            for (int i = 0; i < LegCount; i++)
            {
                int legIndexFactor = i - 3 + (i >= 3 ? 1 : 0);
                int legIndexSign = Math.Sign(legIndexFactor);

                legSegmentRotations[i][0] += ((walkCyclePosition % MathHelper.PiOver2) * -legIndexFactor) + ((MathHelper.Pi/8 * 0.8f) * legIndexFactor);
                legSegmentRotations[i][1] += ((MathHelper.Pi - MathHelper.PiOver4) + (legSegmentRotations[i][0] * 0.25f * legIndexSign)) * legIndexSign;
                legSegmentRotations[i][2] += (MathHelper.PiOver4 / 2 + (legSegmentRotations[i][0] * 0.15f * legIndexFactor)) * legIndexSign;
            }
        }

        private Vector2 GetSegmentEndPosition(Vector2 start, float rotation, Vector2 length)
        {
            return start + length.RotatedBy(rotation);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects effects = NPC.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < LegCount; i++)
            {
                Vector2 currentLegPosition = new Vector2(NPC.Center.X, NPC.position.Y) + legBasePositions[i]; // This is the base of the leg where it attaches to the body
      
                for (int j = 0; j < SegmentCount; j++)
                {
                    Texture2D segmentTexture = j switch
                    {
                        0 => leg1,
                        1 => leg2,
                        _ => foot,
                    };
                    
                    Vector2 segmentOrigin = new(segmentTexture.Width / 2f, 0);  

                    // Draw the current segment
                    spriteBatch.Draw(segmentTexture, currentLegPosition - Main.screenPosition, null, drawColor, legSegmentRotations[i][j], segmentOrigin, NPC.scale, effects, 0f);

                    // Update the current position for the next segment
                    currentLegPosition = GetSegmentEndPosition(currentLegPosition, legSegmentRotations[i][j], legSegmentLengths[j]);
                }
            }

            spriteBatch.Draw(head, new Vector2(NPC.Center.X, NPC.position.Y + head.Height/2) - Main.screenPosition, NPC.frame, drawColor, NPC.rotation, head.Size() / 2f, NPC.scale, effects, 0);

            return false;
        }
    }
}