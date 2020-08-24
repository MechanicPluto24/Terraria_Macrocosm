using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Mounts
{
    public class SpaceShuttle : ModMountData
    {
        public override void SetDefaults()
        {
            mountData.buff = mod.BuffType("SpaceShuttle");
            mountData.heightBoost = 20;          //how high is the mount from the ground
            mountData.fallDamage = 0f;
            mountData.runSpeed = 10f;
            mountData.dashSpeed = 8f;
            mountData.flightTimeMax = 1000000;
            mountData.fatigueMax = 0;
            mountData.jumpHeight = 5;
            mountData.acceleration = 1f;
            mountData.jumpSpeed = 5f;
            mountData.blockExtraJumps = false;
            mountData.totalFrames = 4;            //mount frame/animation
            mountData.constantJump = true;
            int[] offset = new int[mountData.totalFrames];
            for (int l = 0; l < offset.Length; l++)
            {
                offset[l] = 20;
            }
            mountData.playerYOffsets = offset;
            mountData.xOffset = 13;
            mountData.yOffset = -3;          //how high is the mount from the player
            mountData.bodyFrame = 3;          //player frame when it's on the mount
            mountData.playerHeadOffset = 22;
            mountData.standingFrameCount = 4;
            mountData.standingFrameDelay = 12;
            mountData.standingFrameStart = 0;
            mountData.runningFrameCount = 4;
            mountData.runningFrameDelay = 12;
            mountData.runningFrameStart = 0;
            mountData.flyingFrameCount = 4;
            mountData.flyingFrameDelay = 12;
            mountData.flyingFrameStart = 0;
            mountData.inAirFrameCount = 4;
            mountData.inAirFrameDelay = 12;
            mountData.inAirFrameStart = 0;
            mountData.idleFrameCount = 4;
            mountData.idleFrameDelay = 12;
            mountData.idleFrameStart = 0;
            mountData.idleFrameLoop = true;
            mountData.swimFrameCount = mountData.inAirFrameCount;
            mountData.swimFrameDelay = mountData.inAirFrameDelay;
            mountData.swimFrameStart = mountData.inAirFrameStart;
            if (Main.netMode != 2)
            {
                mountData.textureWidth = mountData.frontTexture.Width;
                mountData.textureHeight = mountData.frontTexture.Height;
            }
        }
    }
}