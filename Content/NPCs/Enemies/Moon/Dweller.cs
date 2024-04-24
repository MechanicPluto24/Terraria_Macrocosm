using Macrocosm.Common.Utils;
using Macrocosm.Content.Biomes;
using Macrocosm.Content.NPCs.Global;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.NPCs.Enemies.Moon
{
    public class Dweller : ModNPC, IMoonEnemy
    {
        private const int LegCount = 6;
        private readonly Leg[] Legs = new Leg[LegCount];

        private enum LegType
        {
            Back,
            Mid,
            Front
        }

        private class Leg
        {
            public const int SegmentCount = 3;

            public Vector2 Velocity;
            public Vector2 TipPosition;

            public Vector2 TargetPosition
            {
                get => targetPosition;
                set
                {
                    lastTargetPosition = targetPosition;
                    targetPosition = value;
                }
            }

            private Vector2 targetPosition;
            private Vector2 lastTargetPosition;

            public float SpeedTowardsTarget = 1f;
            public bool TargetOutOfRange;
            public bool AtTargetPosition;

            private readonly NPC npc;
            private readonly int index;
            private readonly Vector2 baseOffset;

            private float rotationLeg1, rotationLeg2, rotationFoot;
            private float prevRotationLeg1, prevRotationLeg2, prevRotationFoot;

            private Vector2 basePosition, joint1Position, joint2Position;
            private Vector2 prevBasePosition, prevJoint1Position, prevJoint2Position;

            private readonly Asset<Texture2D> leg1, leg2, leg3;
            private Vector2 leg1Length, leg2Length, leg3Length;

            private float lastExtensionDistance;

            private LegType legType;

            public Leg(NPC npc, int index, LegType legType, Vector2 baseOffset)
            {
                this.npc = npc;
                this.index = index;
                this.baseOffset = baseOffset;

                this.legType = legType;
                switch (legType)
                {
                    case LegType.Back:
                        leg1 = legBack1;
                        leg2 = legBack2;
                        leg3 = legBack3;
                        break;
                    case LegType.Mid:
                        leg1 = legMid1;
                        leg2 = legMid2;
                        leg3 = legMid3;
                        break;
                    case LegType.Front:
                        leg1 = legFront1;
                        leg2 = legFront2;
                        leg3 = legFront3;
                        break;
                }


                leg1Length = new Vector2(leg1.Height(), 0);
                leg2Length = new Vector2(leg2.Height(), 0);
                leg3Length = new Vector2(leg3.Height(), 0);
            }

            private bool firstUpdate;
            public void Update()
            {
                bool leftLeg = index >= 3;
                basePosition = new Vector2(npc.Center.X, npc.position.Y) + npc.velocity + baseOffset;

                if (!firstUpdate)
                {
                    prevBasePosition = prevJoint1Position = prevJoint2Position = TipPosition = basePosition;
                    prevRotationLeg1 = prevRotationLeg2 = prevRotationFoot = 0f;
                    firstUpdate = true;
                }

                float smoothingFactor = 0.35f;
                float totalLegLength = leg1Length.Length() + leg2Length.Length() + leg3Length.Length();

                Vector2 baseToTipVector = TipPosition - basePosition;
                Vector2 baseToTargetVector = targetPosition - basePosition;
                float baseToTipDistance = baseToTipVector.Length();
                float baseToTargetDistance = baseToTargetVector.Length();

                if (baseToTipDistance > totalLegLength)
                {
                    baseToTipVector.Normalize();
                    baseToTipVector *= totalLegLength;
                    TipPosition = basePosition + baseToTipVector;
                }

                TargetOutOfRange = baseToTargetDistance > totalLegLength;
                AtTargetPosition = baseToTargetDistance < 100;

                float tipToTargetDistance = Vector2.Distance(targetPosition, TipPosition);
                //float progress = MathHelper.Clamp(Vector2.Distance(TipPosition, lastTargetPosition) / Vector2.Distance(targetPosition, lastTargetPosition), 0f, 1f);
                //Velocity.Y -= (float)Math.Sin(Math.PI * progress) * 50f;

                if (tipToTargetDistance > 5f)
                    Velocity = Vector2.Lerp(Velocity, (targetPosition - TipPosition).SafeNormalize(default) * SpeedTowardsTarget, 0.4f);
                else
                    Velocity = Vector2.Zero;

                TipPosition += Velocity;

                Vector2 desiredTipPosition = prevJoint1Position + leg3Length.RotatedBy(prevRotationFoot);
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

                rotationFoot = (TipPosition - joint2Position).ToRotation();

                float extensionDist = Vector2.Distance(joint2Position, targetPosition) / leg3Length.Length();
                lastExtensionDistance = MathHelper.Lerp(lastExtensionDistance, extensionDist, smoothingFactor);
                lastExtensionDistance = MathHelper.Clamp(lastExtensionDistance * lastExtensionDistance, 0.0f, 1.0f);

                joint1Position = Vector2.Lerp(prevJoint1Position, joint1Position, 0.9f);
                joint2Position = Vector2.Lerp(prevJoint2Position, joint2Position, 0.8f);

                rotationLeg1 = Utility.WrapLerpAngle(prevRotationLeg1, rotationLeg1, smoothingFactor);
                rotationLeg2 = Utility.WrapLerpAngle(prevRotationLeg2, rotationLeg2, smoothingFactor);
                rotationFoot = Utility.WrapLerpAngle(prevRotationFoot, rotationFoot, smoothingFactor);

                prevJoint1Position = joint1Position;
                prevJoint2Position = joint2Position;
                prevRotationLeg1 = rotationLeg1;
                prevRotationLeg2 = rotationLeg2;
                prevRotationFoot = rotationFoot;
            }

            public void Draw(SpriteBatch spriteBatch, NPC npc, Vector2 screenPos, Color drawColor)
            {
                if (legType != LegType.Back)
                    return;

                SpriteEffects effects = index < 3 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

                Texture2D cross = ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "DebugCross").Value;

                Vector2 originLeg1 = new(leg1.Width() / 2f, 0);
                Vector2 originLeg2 = new(leg2.Width() / 2f, 0);
                Vector2 originFoot = new(leg3.Width() / 2f, 0);

                spriteBatch.Draw(leg1.Value, basePosition - screenPos, null, drawColor, rotationLeg1 - MathHelper.PiOver2, originLeg1, npc.scale, effects, 0f);
                spriteBatch.Draw(leg2.Value, joint1Position - screenPos, null, drawColor, rotationLeg2 - MathHelper.PiOver2, originLeg2, npc.scale, effects, 0f);
                spriteBatch.Draw(leg3.Value, joint2Position - screenPos, null, drawColor, rotationFoot - MathHelper.PiOver2, originFoot, npc.scale, effects, 0f);

                // Debug stuff
                /*
                spriteBatch.Draw(cross, basePosition - screenPos, null, Color.Red, 0f, cross.Size()/2f, 2f, SpriteEffects.None, 0);
                spriteBatch.Draw(cross, joint1Position - screenPos, null, Color.Yellow, 0f, cross.Size() / 2f, 2f, SpriteEffects.None, 0);
                spriteBatch.Draw(cross, joint2Position - screenPos, null, Color.LimeGreen, 0f, cross.Size() / 2f, 2f, SpriteEffects.None, 0);

                Vector2 visualTipPosition = joint2Position + footLength.RotatedBy(rotationFoot); 
                spriteBatch.Draw(cross, visualTipPosition - screenPos, null, Color.Blue, 0f, cross.Size() / 2f, 2f, SpriteEffects.None, 0);
                spriteBatch.Draw(cross, TipPosition - screenPos, null, Color.Cyan, 0f, cross.Size() / 2f, 2f, SpriteEffects.None, 0);
                */

                spriteBatch.Draw(cross, targetPosition - screenPos, null, Utility.HSLToRGB(new((index + 1) / 6f, 1f, 0.5f)), 0f, cross.Size() / 2f, 2f, SpriteEffects.None, 0);
                //spriteBatch.Draw(cross, lastTargetPosition - screenPos, null, Utility.HSLToRGB(new((index + 1) / 6f, 1f, 0.2f)), 0f, cross.Size() / 2f, 2f, SpriteEffects.None, 0);
            }
        }

        private static Asset<Texture2D> headBack;
        private static Asset<Texture2D> headJawLeft;
        private static Asset<Texture2D> headJawRight;
        private static Asset<Texture2D> headShellLeft;
        private static Asset<Texture2D> headShellRight;

        private static Asset<Texture2D> legBack1;
        private static Asset<Texture2D> legBack2;
        private static Asset<Texture2D> legBack3;

        private static Asset<Texture2D> legMid1;
        private static Asset<Texture2D> legMid2;
        private static Asset<Texture2D> legMid3;
                
        private static Asset<Texture2D> legFront1;
        private static Asset<Texture2D> legFront2;
        private static Asset<Texture2D> legFront3;


        public override string Texture => Macrocosm.EmptyTexPath;
        private string TexturePath => this.GetNamespacePath();

        public override void Load()
        {
            headBack = ModContent.Request<Texture2D>(TexturePath + "_Head_Back");
            headJawLeft = ModContent.Request<Texture2D>(TexturePath + "_Head_JawLeft");
            headJawRight = ModContent.Request<Texture2D>(TexturePath + "_Head_JawRight");
            headShellLeft = ModContent.Request<Texture2D>(TexturePath + "_Head_ShellLeft");
            headShellRight = ModContent.Request<Texture2D>(TexturePath + "_Head_ShellRight");

            legBack1 = ModContent.Request<Texture2D>(TexturePath + "_Leg_Back1");
            legBack2 = ModContent.Request<Texture2D>(TexturePath + "_Leg_Back2");
            legBack3 = ModContent.Request<Texture2D>(TexturePath + "_Leg_Back3");

            legMid1 = ModContent.Request<Texture2D>(TexturePath + "_Leg_Mid1");
            legMid2 = ModContent.Request<Texture2D>(TexturePath + "_Leg_Mid2");
            legMid3 = ModContent.Request<Texture2D>(TexturePath + "_Leg_Mid3");

            legFront1 = ModContent.Request<Texture2D>(TexturePath + "_Leg_Front1");
            legFront2 = ModContent.Request<Texture2D>(TexturePath + "_Leg_Front2");
            legFront3 = ModContent.Request<Texture2D>(TexturePath + "_Leg_Front3");
        }

        public enum AIState
        {
            Walk
        }

        public AIState ActionState
        {
            get => (AIState)NPC.ai[0];
            set => NPC.ai[0] = (float)value;
        }

        public ref float LegTimer => ref NPC.ai[1];

        public Player TargetPlayer => Main.player[NPC.target];
        public bool HasTarget => TargetPlayer is not null && TargetPlayer.active && !TargetPlayer.dead;

        private Rectangle collisionHitbox;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            NPC.damage = 150;
            NPC.defense = 25;
            NPC.lifeMax = 6000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath2;
            NPC.knockBackResist = 0.5f;
            NPC.aiStyle = -1;

            NPC.noGravity = true;
            NPC.noTileCollide = true;

            SpawnModBiomes = [ModContent.GetInstance<UndergroundMoonBiome>().Type];

            NPC.width = headBack.Width();
            NPC.height = headJawLeft.Height();

            InitializeLegs();
        }

        private void InitializeLegs()
        {
            for (int i = 0; i < LegCount; i++)
            {
                Legs[i] = new Leg(NPC, i, LegTypeFromIndex(i), new Vector2(18 - (8 * i), NPC.height * 0.8f));
            }
        }

        public override void AI()
        {
            NPC.TargetClosest(faceTarget: false);

            int legsTouchingGround = Legs.Count(leg => WorldGen.SolidOrSlopedTile(Main.tile[leg.TipPosition.ToTileCoordinates()]));
            bool anyLegTouchingGround = legsTouchingGround > 0;
            collisionHitbox = new((int)NPC.position.X, (int)NPC.position.Y, 4 * 16, 12 * 16);
            Rectangle tileCollisionHitbox = new(collisionHitbox.X / 16, collisionHitbox.Y / 16, collisionHitbox.Width / 16, collisionHitbox.Height / 16);
            bool midAir = Utility.EmptyTiles(tileCollisionHitbox);
            bool allowedVerticalMovement = false;

            float speed = 6f;
            if (HasTarget && NPC.DistanceSQ(TargetPlayer.Center) > 100f * 100f)
            {
                Vector2 direction = NPC.Center.DirectionTo(TargetPlayer.Center);
                NPC.velocity.X = direction.X * speed;

                if (midAir && !(Legs.Any(leg => leg.TipPosition.Y < (NPC.position.Y + NPC.height))))
                {
                    NPC.velocity.Y = Math.Abs(NPC.velocity.Y);
                    NPC.velocity.Y += NPC.gravity;
                }
                else if (anyLegTouchingGround)
                {
                    NPC.velocity.Y = direction.Y * speed;
                    allowedVerticalMovement = true;
                }
            }

            if (Math.Abs(NPC.velocity.X) > Math.Abs(NPC.velocity.Y))
                LegTimer += 0.015f * speed;
            else
                LegTimer += 0.006f * speed;

            float stepSize = 160f;
            float restDistance = 60f;
            for (int i = 0; i < LegCount; i++)
            {
                float legMultiplier = (i <= 2 ? 1 * (i + 1) : -1f * (i - LegCount / 2 + 1));
                bool isLegMoving = (Math.Truncate((LegTimer % 3) + 1)) == Math.Abs(legMultiplier);
                if (isLegMoving)
                {
                    float direction = Math.Sign(NPC.velocity.X);
                    float legDirection = Math.Sign(legMultiplier);
                    bool sameDirection = direction == legDirection;
                    float speedAmount = Math.Abs(NPC.velocity.X) / speed;

                    float legOffset;
                    if (sameDirection)
                        legOffset = MathHelper.Lerp(legMultiplier * restDistance, stepSize * legDirection, speedAmount);
                    else
                        legOffset = MathHelper.Lerp(legMultiplier * restDistance, stepSize * 0.5f * legDirection, speedAmount);

                    Vector2 targetPosition = new(NPC.Center.X + legOffset, NPC.position.Y + NPC.height + 122);

                    if (allowedVerticalMovement && NPC.velocity.Y < 0)
                        targetPosition.Y = NPC.position.Y - 20 * legMultiplier;

                    Legs[i].TargetPosition = FindSuitableGround(targetPosition, verticalRange: 10, horizontalRange: 5);
                }

                Legs[i].SpeedTowardsTarget = speed * 4f;
                Legs[i].Update();
            }

        }

        private Vector2 FindSuitableGround(Vector2 position, int verticalRange, int horizontalRange)
        {
            Point startTile = position.ToTileCoordinates();

            if (WorldGen.InWorld(startTile.X, startTile.Y))
            {
                for (int y = startTile.Y; y < startTile.Y + verticalRange && y < Main.maxTilesY; y++)
                    if (WorldGen.SolidTile(startTile.X, y) && !WorldGen.SolidTile(startTile.X, y - 1))
                        return new Vector2(startTile.X * 16, y * 16);

                for (int y = startTile.Y; y > startTile.Y - verticalRange && y >= 0; y--)
                    if (WorldGen.SolidTile(startTile.X, y) && !WorldGen.SolidTile(startTile.X, y - 1))
                        return new Vector2(startTile.X * 16, y * 16);

                for (int x = -horizontalRange; x <= horizontalRange; x++)
                {
                    if (x == 0)
                        continue;

                    for (int y = startTile.Y; y < startTile.Y + verticalRange && y < Main.maxTilesY; y++)
                    {
                        int checkX = startTile.X + x;
                        if (WorldGen.SolidTile(checkX, y) && !WorldGen.SolidTile(checkX, y - 1))
                            return new Vector2(checkX * 16, y * 16);
                    }

                    for (int y = startTile.Y; y > startTile.Y - verticalRange && y >= 0; y--)
                    {
                        int checkX = startTile.X + x;
                        if (WorldGen.SolidTile(checkX, y) && !WorldGen.SolidTile(checkX, y - 1))
                            return new Vector2(checkX * 16, y * 16);
                    }
                }
            }

            return Utility.GetClosestTile(startTile.X, startTile.Y, -1, 200, (t) => Main.tileSolid[t.TileType] && !t.IsActuated).ToVector2() * 16f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects effects = NPC.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Legs[2].Draw(spriteBatch, NPC, screenPos, drawColor);
            Legs[3].Draw(spriteBatch, NPC, screenPos, drawColor);

            spriteBatch.Draw(headBack.Value, new Vector2(NPC.Center.X, NPC.position.Y + headBack.Height() / 2) - Main.screenPosition, null, drawColor, NPC.rotation, headBack.Size() / 2f, NPC.scale, effects, 0);
            spriteBatch.Draw(headJawRight.Value, new Vector2(NPC.Center.X, NPC.position.Y + headJawRight.Height() / 2) - Main.screenPosition, null, drawColor, NPC.rotation, headJawRight.Size() / 2f, NPC.scale, effects, 0);
            spriteBatch.Draw(headJawLeft.Value, new Vector2(NPC.Center.X, NPC.position.Y + headJawLeft.Height() / 2) - Main.screenPosition, null, drawColor, NPC.rotation, headJawLeft.Size() / 2f, NPC.scale, effects, 0);
            spriteBatch.Draw(headShellRight.Value, new Vector2(NPC.Center.X, NPC.position.Y + headBack.Height() / 2) - Main.screenPosition, null, drawColor, NPC.rotation, headShellRight.Size() / 2f, NPC.scale, effects, 0);
            spriteBatch.Draw(headShellLeft.Value, new Vector2(NPC.Center.X, NPC.position.Y + headBack.Height() / 2) - Main.screenPosition, null, drawColor, NPC.rotation, headShellLeft.Size() / 2f, NPC.scale, effects, 0);

            Legs[1].Draw(spriteBatch, NPC, screenPos, drawColor);
            Legs[4].Draw(spriteBatch, NPC, screenPos, drawColor);

            Legs[0].Draw(spriteBatch, NPC, screenPos, drawColor);
            Legs[5].Draw(spriteBatch, NPC, screenPos, drawColor);

            // Debug collision hitbox
            /*
            Rectangle hitbox = collisionHitbox;
            hitbox.X -= (int)screenPos.X;
            hitbox.Y -= (int)screenPos.Y;
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, hitbox, Color.Purple * 0.5f);
            */

            return false;
        }

        private LegType LegTypeFromIndex(int index)
        {
            return
            index is 0 or 5 ? LegType.Front :
            index is 1 or 4 ? LegType.Mid :
                              LegType.Back;
        }
    }
}