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
using Newtonsoft.Json.Linq;
using Macrocosm.Common.Subworlds;
using System.Linq.Expressions;

namespace Macrocosm.Content.NPCs.Enemies.Moon
{
    public class Dweller : ModNPC, IMoonEnemy
    {
        private const int LegCount = 6;
        private readonly Leg[] Legs = new Leg[LegCount];

        private class Leg
        {
            public const int SegmentCount = 3;

            private readonly NPC npc;
            private readonly int index;

            public Vector2 velocity;
            public Vector2 tipPosition;

            public Vector2 targetPosition;

            public float rotationLeg1, rotationLeg2, rotationFoot;
            private float prevRotationLeg1, prevRotationLeg2, prevRotationFoot;

            private Vector2 basePosition, joint1Position, joint2Position;
            private Vector2 prevBasePosition, prevJoint1Position, prevJoint2Position;

            private Vector2 leg1Length, leg2Length, footLength;

            private float lastExtensionDistance;

            private readonly Texture2D head;
            private readonly Texture2D leg1;
            private readonly Texture2D leg2;
            private readonly Texture2D foot;

            public Leg(NPC npc, int index)
            {
                this.npc = npc;
                this.index = index;

                head = ModContent.Request<Texture2D>(npc.ModNPC.Texture, AssetRequestMode.ImmediateLoad).Value;
                leg1 = ModContent.Request<Texture2D>(npc.ModNPC.Texture + "_Leg1", AssetRequestMode.ImmediateLoad).Value;
                leg2 = ModContent.Request<Texture2D>(npc.ModNPC.Texture + "_Leg2", AssetRequestMode.ImmediateLoad).Value;
                foot = ModContent.Request<Texture2D>(npc.ModNPC.Texture + "_Foot", AssetRequestMode.ImmediateLoad).Value;

                leg1Length = new Vector2(leg1.Height, 0);
                leg2Length = new Vector2(leg2.Height, 0);
                footLength = new Vector2(foot.Height, 0);
            }

            private bool firstUpdate;

            public void Update()
            {
                bool leftLeg = index >= 3;
                basePosition = new Vector2(npc.Center.X, npc.position.Y) + new Vector2(18 - (8 * index), head.Height);

                if (!firstUpdate)
                {
                    prevBasePosition = prevJoint1Position = prevJoint2Position = tipPosition = basePosition;
                    prevRotationLeg1 = prevRotationLeg2 = prevRotationFoot = 0f;
                    firstUpdate = true;
                }

                float smoothingFactor = 0.35f;
                Vector2 baseToTipVector = tipPosition - basePosition;
                float baseToTipDistance = baseToTipVector.Length();
                float totalLegLength = leg1Length.Length() + leg2Length.Length() + footLength.Length();

                if (baseToTipDistance > totalLegLength)
                {
                    baseToTipVector.Normalize();
                    baseToTipVector *= totalLegLength;
                    tipPosition = basePosition + baseToTipVector;
                }

                if (leftLeg)
                {
                    if(tipPosition.X >= basePosition.X - 80)
                        tipPosition.X = basePosition.X - 80; 
                }
                else
                {
                    if (tipPosition.X <= basePosition.X + 80)
                        tipPosition.X = basePosition.X + 80;
                }

                velocity.Y += 1f * MacrocosmSubworld.CurrentGravityMultiplier;

                if (Vector2.Distance(targetPosition, tipPosition) > 10f)
                    velocity = Vector2.Lerp(velocity, (targetPosition - tipPosition).SafeNormalize(default) * 10, 0.6f);
                else
                    velocity = Vector2.Zero;

                velocity = Collision.TileCollision(tipPosition, velocity, 1, 1);
                tipPosition += velocity;

                Vector2 desiredTipPosition = prevJoint1Position + footLength.RotatedBy(prevRotationFoot);
                baseToTipVector = desiredTipPosition - basePosition;
                baseToTipDistance = baseToTipVector.Length();

                float length1 = leg1Length.Length();
                float length2 = leg2Length.Length();

                float cosAngleJoint1 = (baseToTipDistance * baseToTipDistance + length1 * length1 - length2 * length2) / (2 * baseToTipDistance * length1);
                float angleJoint1 = (float)Math.Acos(MathHelper.Clamp(cosAngleJoint1, -1f, 1f));

                rotationLeg1 = baseToTipVector.ToRotation() + angleJoint1;
   
                float cosAngleJoint2 = (length1 * length1 + length2 * length2 - baseToTipDistance * baseToTipDistance) / (2 * length1 * length2);
                float angleJoint2 = (float)Math.Acos(MathHelper.Clamp(cosAngleJoint2, -1f, 1f));

                rotationLeg2 = rotationLeg1 + angleJoint2;
                float extensionAdjustment = (leftLeg ? -MathHelper.Pi : MathHelper.Pi) * lastExtensionDistance;
                rotationLeg2 += extensionAdjustment;

                joint1Position = basePosition + leg1Length.RotatedBy(rotationLeg1);
                joint2Position = joint1Position + leg2Length.RotatedBy(rotationLeg2);

                rotationFoot = (tipPosition - joint2Position).ToRotation();

                float extensionDist = Vector2.Distance(prevJoint2Position, tipPosition) / footLength.Length();
                lastExtensionDistance += (extensionDist - lastExtensionDistance) * smoothingFactor;
                lastExtensionDistance = MathHelper.Clamp(lastExtensionDistance * lastExtensionDistance, 0.0f, 1.0f);

                basePosition = Vector2.Lerp(prevBasePosition, basePosition, 0.8f);
                joint1Position = Vector2.Lerp(prevJoint1Position, joint1Position, 0.75f);
                joint2Position = Vector2.Lerp(prevJoint2Position, joint2Position, 0.7f);

                rotationLeg1 = Utility.WrapLerpAngle(prevRotationLeg1, rotationLeg1, smoothingFactor);
                rotationLeg2 = Utility.WrapLerpAngle(prevRotationLeg2, rotationLeg2, smoothingFactor);
                rotationFoot = Utility.WrapLerpAngle(prevRotationFoot, rotationFoot, smoothingFactor);

                prevBasePosition = basePosition;
                prevJoint1Position = joint1Position;
                prevJoint2Position = joint2Position;
                prevRotationLeg1 = rotationLeg1;
                prevRotationLeg2 = rotationLeg2;
                prevRotationFoot = rotationFoot;
            }

            public void Draw(SpriteBatch spriteBatch, NPC npc, Vector2 screenPos, Color drawColor)
            {
                SpriteEffects effects = npc.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

                Texture2D cross = ModContent.Request<Texture2D>(Macrocosm.TextureAssetsPath + "DebugCross").Value;

                Vector2 originLeg1 = new Vector2(leg1.Width / 2f, 0);
                Vector2 originLeg2 = new Vector2(leg2.Width / 2f, 0);
                Vector2 originFoot = new Vector2(foot.Width / 2f, 0);

                spriteBatch.Draw(leg1, basePosition - screenPos, null, drawColor, rotationLeg1 - MathHelper.PiOver2, originLeg1, npc.scale, effects, 0f);
                spriteBatch.Draw(leg2, joint1Position - screenPos, null, drawColor, rotationLeg2 - MathHelper.PiOver2, originLeg2, npc.scale, effects, 0f);
                spriteBatch.Draw(foot, joint2Position - screenPos, null, drawColor, rotationFoot - MathHelper.PiOver2, originFoot, npc.scale, effects, 0f);

                spriteBatch.Draw(cross, basePosition - screenPos, null, Color.Red, 0f, cross.Size()/2f, 2f, SpriteEffects.None, 0);
                spriteBatch.Draw(cross, joint1Position - screenPos, null, Color.Yellow, 0f, cross.Size() / 2f, 2f, SpriteEffects.None, 0);
                spriteBatch.Draw(cross, joint2Position - screenPos, null, Color.LimeGreen, 0f, cross.Size() / 2f, 2f, SpriteEffects.None, 0);

                Vector2 visualTipPosition = joint2Position + footLength.RotatedBy(rotationFoot); 
                spriteBatch.Draw(cross, visualTipPosition - screenPos, null, Color.Blue, 0f, cross.Size() / 2f, 2f, SpriteEffects.None, 0);

                spriteBatch.Draw(cross, tipPosition - screenPos, null, Color.Cyan, 0f, cross.Size() / 2f, 2f, SpriteEffects.None, 0);
                spriteBatch.Draw(cross, targetPosition - screenPos, null, Color.Magenta, 0f, cross.Size() / 2f, 2f, SpriteEffects.None, 0);
            }
        }

        public Player TargetPlayer => Main.player[NPC.target];
        public bool HasTarget => TargetPlayer is not null;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            NPC.width = 150;
            NPC.height = 200;
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
            for (int i = 0; i < LegCount; i++)
            {
                Legs[i] = new Leg(NPC, i);
            }
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
                NPC.velocity.Y = -5;
            }
            else if(!NPC.collideY && NPC.velocity.Y < 0)
            {
                NPC.velocity.Y += 10;
            }

            for (int i = 0; i < LegCount; i++)
            {
                int legIndexFactor = i - 3 + (i >= 3 ? 1 : 0);
                int legIndexSign = Math.Sign(legIndexFactor);

                Legs[i].targetPosition = Main.MouseWorld + new Vector2(60, 0) * -legIndexFactor;
                Legs[i].Update();
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects effects = NPC.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < LegCount; i++)
            {
                Legs[i].Draw(spriteBatch, NPC, screenPos, drawColor);
            }

            Texture2D head = ModContent.Request<Texture2D>(Texture, AssetRequestMode.ImmediateLoad).Value;
            spriteBatch.Draw(head, new Vector2(NPC.Center.X, NPC.position.Y + head.Height/2) - Main.screenPosition, NPC.frame, drawColor, NPC.rotation, head.Size() / 2f, NPC.scale, effects, 0);

            return false;
        }
    }
}