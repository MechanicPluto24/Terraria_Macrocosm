using Macrocosm.Common.Global.NPCs;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Biomes;
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
    public class Dweller : ModNPC
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
            }

            private bool firstUpdate;
            public void Update()
            {
                switch (legType)
                {
                    case LegType.Back:
                        leg1Length = new Vector2(MathF.Sqrt(MathF.Pow(leg1.Width(), 2) + MathF.Pow(leg1.Height(), 2)), 0) * 0.9f;
                        leg2Length = new Vector2(MathF.Sqrt(MathF.Pow(leg2.Width(), 2) + MathF.Pow(leg2.Height(), 2)), 0) * 2.2f;
                        leg3Length = new Vector2(MathF.Sqrt(MathF.Pow(leg3.Width(), 2) + MathF.Pow(leg3.Height(), 2)), 0) * 0.8f;
                        break;
                    case LegType.Mid:
                        leg1Length = new Vector2(MathF.Sqrt(MathF.Pow(leg1.Width(), 2) + MathF.Pow(leg1.Height(), 2)), 0) * 0.8f;
                        leg2Length = new Vector2(MathF.Sqrt(MathF.Pow(leg2.Width(), 2) + MathF.Pow(leg2.Height(), 2)), 0) * 0.8f;
                        leg3Length = new Vector2(MathF.Sqrt(MathF.Pow(leg3.Width(), 2) + MathF.Pow(leg3.Height(), 2)), 0) * 0.75f;
                        break;
                    case LegType.Front:
                        leg1Length = new Vector2(MathF.Sqrt(MathF.Pow(leg1.Width(), 2) + MathF.Pow(leg1.Height(), 2)), 0) * 0.9f;
                        leg2Length = new Vector2(MathF.Sqrt(MathF.Pow(leg2.Width(), 2) + MathF.Pow(leg2.Height(), 2)), 0) * 0.9f;
                        leg3Length = new Vector2(MathF.Sqrt(MathF.Pow(leg3.Width(), 2) + MathF.Pow(leg3.Height(), 2)), 0) * 0.7f;
                        break;
                }

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
                bool leftLeg = index > 2;

                SpriteEffects effectsLeg1 = default;
                SpriteEffects effectsLeg2 = default;
                SpriteEffects effectsLeg3 = default;
                Vector2 originLeg1 = default;
                Vector2 originLeg2 = default;
                Vector2 originFoot = default;
                float rotationOffsetLeg1 = 0f;
                float rotationOffsetLeg2 = 0f;
                float rotationOffsetLeg3 = 0f;

                switch (legType)
                {
                    case LegType.Back:
                        effectsLeg1 = leftLeg ? SpriteEffects.None : SpriteEffects.FlipVertically;
                        effectsLeg2 = leftLeg ? SpriteEffects.FlipVertically : SpriteEffects.None;
                        effectsLeg3 = leftLeg ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                        originLeg1 = leftLeg ? new(0, 0) : new(-4, 32);
                        originLeg2 = leftLeg ? new(12, 24) : new(20, 8);
                        originFoot = leftLeg ? new(24, 0) : new(22, 4);
                        rotationOffsetLeg1 = leftLeg ? -MathHelper.Pi / 4 + MathHelper.Pi / 12 : MathHelper.Pi / 8;
                        rotationOffsetLeg2 = leftLeg ? MathHelper.Pi / 2 + MathHelper.Pi / 8 : -MathHelper.Pi / 2 - MathHelper.Pi / 4;
                        rotationOffsetLeg3 = leftLeg ? -MathHelper.Pi / 2 - MathHelper.Pi / 16 : -MathHelper.Pi / 2;
                        break;
                    case LegType.Mid:
                        effectsLeg1 = leftLeg ? SpriteEffects.FlipVertically : SpriteEffects.None;
                        effectsLeg2 = leftLeg ? SpriteEffects.FlipVertically : SpriteEffects.None;
                        effectsLeg3 = leftLeg ? SpriteEffects.FlipVertically : SpriteEffects.None;
                        originLeg1 = leftLeg ? new(8, 42) : new(6, 4);
                        originLeg2 = leftLeg ? new(6, 6) : new(0, 32);
                        originFoot = leftLeg ? new(14, 72) : new(10, 14);
                        rotationOffsetLeg1 = leftLeg ? MathHelper.Pi / 4 : -MathHelper.Pi / 4;
                        rotationOffsetLeg2 = leftLeg ? -MathHelper.Pi / 6 : MathHelper.Pi / 8;
                        rotationOffsetLeg3 = leftLeg ? MathHelper.Pi / 6 : -MathHelper.Pi / 6;
                        break;
                    case LegType.Front:
                        effectsLeg1 = leftLeg ? SpriteEffects.FlipVertically : SpriteEffects.None;
                        effectsLeg2 = leftLeg ? SpriteEffects.FlipVertically : SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically;
                        effectsLeg3 = leftLeg ? SpriteEffects.FlipVertically : SpriteEffects.None;
                        originLeg1 = leftLeg ? new(6, 54) : new(14, 0);
                        originLeg2 = leftLeg ? new(4, -4) : new(42, -12);
                        originFoot = leftLeg ? new(16, 80) : new(-2, 12);
                        rotationOffsetLeg1 = leftLeg ? MathHelper.Pi / 2 - MathHelper.Pi / 8 : -MathHelper.Pi / 4 - MathHelper.Pi / 4;
                        rotationOffsetLeg2 = leftLeg ? -MathHelper.Pi / 4 : -MathHelper.Pi / 2 - MathHelper.Pi / 8;
                        rotationOffsetLeg3 = leftLeg ? MathHelper.Pi / 4 + MathHelper.Pi / 12 : -MathHelper.Pi / 4 - MathHelper.Pi / 12;
                        break;
                    default:

                        break;
                }

                spriteBatch.Draw(leg1.Value, basePosition - screenPos, null, drawColor, rotationLeg1 + rotationOffsetLeg1, originLeg1, npc.scale, effectsLeg1, 0f);
                spriteBatch.Draw(leg2.Value, joint1Position - screenPos, null, drawColor, rotationLeg2 + rotationOffsetLeg2, originLeg2, npc.scale, effectsLeg2, 0f);
                spriteBatch.Draw(leg3.Value, joint2Position - screenPos, null, drawColor, rotationFoot + rotationOffsetLeg3, originFoot, npc.scale, effectsLeg3, 0f);

                //DebugDraw(spriteBatch, screenPos);
            }

            private void DebugDraw(SpriteBatch spriteBatch, Vector2 screenPos)
            {
                Texture2D cross = ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "DebugCross").Value;

                Utils.DrawLine(spriteBatch, basePosition, joint1Position, Color.Red);
                Utils.DrawLine(spriteBatch, joint1Position, joint2Position, Color.Green);
                Utils.DrawLine(spriteBatch, joint2Position, TipPosition, Color.Blue);

                /*
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
            NPCSets.MoonNPC[Type] = true;
            NPCSets.DropsMoonstone[Type] = true;
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
                float posX = i switch
                {
                    0 => 16,
                    1 => 16,
                    2 => 32,
                    3 => -24,
                    4 => -8,
                    5 => -8,
                    _ => 0
                };

                Legs[i] = new Leg(NPC, i, LegTypeFromIndex(i), new Vector2(posX, NPC.height * 0.74f));
            }
        }

        public override void AI()
        {
            NPC.TargetClosest(faceTarget: false);

            int legsTouchingGround = Legs.Count(leg => WorldGen.SolidOrSlopedTile(Main.tile[leg.TipPosition.ToTileCoordinates()]));
            bool anyLegTouchingGround = legsTouchingGround > 0;
            collisionHitbox = new((int)NPC.position.X, (int)NPC.position.Y, 4 * 16, 10 * 16);
            Rectangle tileCollisionHitbox = new(collisionHitbox.X / 16, collisionHitbox.Y / 16, collisionHitbox.Width / 16, collisionHitbox.Height / 16);
            bool midAir = Utility.EmptyTiles(tileCollisionHitbox);
            bool allowedVerticalMovement = false;

            float speed = 4f;
            if (HasTarget && NPC.DistanceSQ(TargetPlayer.Center) > 100f * 100f)
            {
                Vector2 direction = NPC.Center.DirectionTo(TargetPlayer.Center);
                NPC.velocity.X = direction.X * speed;

                if (midAir && !(Legs.Any(leg => leg.TipPosition.Y < (NPC.position.Y + NPC.height))))
                {
                    NPC.velocity.Y = 0;
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
                int legMovementIdentifier = (i <= 2 ? 1 * (i + 1) : -1 * (i - LegCount / 2 + 1));

                float legMultiplier = i switch
                {
                    0 => 2f,
                    1 => 4f,
                    2 => 1.1f,
                    3 => -1.1f,
                    4 => -4f,
                    5 => -2f,
                    _ => 0f,
                };

                bool isLegMoving = (Math.Truncate((LegTimer % 3) + 1)) == Math.Abs(legMovementIdentifier);
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

            Legs[1].Draw(spriteBatch, NPC, screenPos, drawColor);
            Legs[4].Draw(spriteBatch, NPC, screenPos, drawColor);

            spriteBatch.Draw(headBack.Value, new Vector2(NPC.Center.X, NPC.position.Y + headBack.Height() / 2) - Main.screenPosition, null, drawColor, NPC.rotation, headBack.Size() / 2f, NPC.scale, effects, 0);
            spriteBatch.Draw(headJawRight.Value, new Vector2(NPC.Center.X, NPC.position.Y + headJawRight.Height() / 2) - Main.screenPosition, null, drawColor, NPC.rotation, headJawRight.Size() / 2f, NPC.scale, effects, 0);
            spriteBatch.Draw(headJawLeft.Value, new Vector2(NPC.Center.X, NPC.position.Y + headJawLeft.Height() / 2) - Main.screenPosition, null, drawColor, NPC.rotation, headJawLeft.Size() / 2f, NPC.scale, effects, 0);
            spriteBatch.Draw(headShellRight.Value, new Vector2(NPC.Center.X, NPC.position.Y + headBack.Height() / 2) - Main.screenPosition, null, drawColor, NPC.rotation, headShellRight.Size() / 2f, NPC.scale, effects, 0);
            spriteBatch.Draw(headShellLeft.Value, new Vector2(NPC.Center.X, NPC.position.Y + headBack.Height() / 2) - Main.screenPosition, null, drawColor, NPC.rotation, headShellLeft.Size() / 2f, NPC.scale, effects, 0);

            Legs[0].Draw(spriteBatch, NPC, screenPos, drawColor);
            Legs[5].Draw(spriteBatch, NPC, screenPos, drawColor);

            /*
            // Debug collision hitbox
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