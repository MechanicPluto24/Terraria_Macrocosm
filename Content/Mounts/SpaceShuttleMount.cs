using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Mounts
{
    public class SpaceShuttleMount : ModMount
    {
        public override void SetStaticDefaults()
        {
            MountData.buff = ModContent.BuffType<Buffs.GoodBuffs.MountBuffs.SpaceShuttle>();
            MountData.heightBoost = 20;          //how high is the mount from the ground
            MountData.fallDamage = 0f;
            MountData.runSpeed = 10f;
            MountData.dashSpeed = 8f;
            MountData.flightTimeMax = 1000000;
            MountData.fatigueMax = 0;
            MountData.jumpHeight = 5;
            MountData.acceleration = 1f;
            MountData.jumpSpeed = 5f;
            MountData.blockExtraJumps = false;
            MountData.totalFrames = 4;            //mount frame/animation
            MountData.constantJump = true;
            int[] offset = new int[MountData.totalFrames];
            for (int l = 0; l < offset.Length; l++)
            {
                offset[l] = 20;
            }
            MountData.playerYOffsets = offset;
            MountData.xOffset = 13;
            MountData.yOffset = -3;          //how high is the mount from the player
            MountData.bodyFrame = 3;          //player frame when it's on the mount
            MountData.playerHeadOffset = 22;
            MountData.standingFrameCount = 4;
            MountData.standingFrameDelay = 12;
            MountData.standingFrameStart = 0;
            MountData.runningFrameCount = 4;
            MountData.runningFrameDelay = 12;
            MountData.runningFrameStart = 0;
            MountData.flyingFrameCount = 4;
            MountData.flyingFrameDelay = 12;
            MountData.flyingFrameStart = 0;
            MountData.inAirFrameCount = 4;
            MountData.inAirFrameDelay = 12;
            MountData.inAirFrameStart = 0;
            MountData.idleFrameCount = 4;
            MountData.idleFrameDelay = 12;
            MountData.idleFrameStart = 0;
            MountData.idleFrameLoop = true;
            MountData.swimFrameCount = MountData.inAirFrameCount;
            MountData.swimFrameDelay = MountData.inAirFrameDelay;
            MountData.swimFrameStart = MountData.inAirFrameStart;
            if (Main.netMode != NetmodeID.Server)
            {
                MountData.textureWidth = MountData.frontTexture.Width();
                MountData.textureHeight = MountData.frontTexture.Height();
            }
        }
    }
}