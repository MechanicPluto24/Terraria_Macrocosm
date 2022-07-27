using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace Macrocosm.Base.BaseMod {
    public class BaseAI {
        //------------------------------------------------------//
        //-------------------BASE AI CLASS----------------------//
        //------------------------------------------------------//
        // Contains methods for various AI functions for both   //
        // NPCs and Projectiles, such as adding lighting,       //
        // movement, etc.                                       //
        //------------------------------------------------------//
        //  Author(s): Grox the Great, Yoraiz0r                 //
        //------------------------------------------------------//

        public static void Look(Projectile p, int lookType = 0, float rotAddon = 0f, float rotAmount = 0.1f, bool flipSpriteDir = false) {
            Look(p, ref p.rotation, ref p.spriteDirection, lookType, rotAddon, rotAmount, flipSpriteDir);
        }
        public static void Look(NPC npc, int lookType = 0, float rotAddon = 0f, float rotAmount = 0.1f, bool flipSpriteDir = false) {
            Look(npc, ref npc.rotation, ref npc.spriteDirection, lookType, rotAddon, rotAmount, flipSpriteDir);
        }
        /*
         * Makes the rotation value and sprite direction 'look' based on factors from the Entity.
         * lookType : the type of look code to run.
         *        0 -> Rotates the entity and changes spriteDirection based on velocity.
         *        1 -> changes spriteDirection based on velocity.
         *        2 -> Rotates the entity based on velocity.
         *        3 -> Smoothly rotate and change sprite direction based on velocity.
         *        4 -> Smoothly rotate based on velocity. 
         * rotAddon : the amount to add to the rotation. (only used by lookType 3/4)
         * rotAmount: the amount to rotate by. (only used by lookType 3/4)
         */
        public static void Look(Entity c, ref float rotation, ref int spriteDirection, int lookType = 0, float rotAddon = 0f, float rotAmount = 0.1f, bool flipSpriteDir = false) {
            LookAt(c.position + c.velocity, c.position, ref rotation, ref spriteDirection, lookType, rotAddon, rotAmount, flipSpriteDir);
        }

        public static void LookAt(Vector2 lookTarget, Entity c, int lookType = 0, float rotAddon = 0f, float rotAmount = 0.1f, bool flipSpriteDir = false) {
            int spriteDirection = (c is NPC ? ((NPC)c).spriteDirection : c is Projectile ? ((Projectile)c).spriteDirection : 0);
            float rotation = (c is NPC ? ((NPC)c).rotation : c is Projectile ? ((Projectile)c).rotation : 0f);
            LookAt(lookTarget, c.Center, ref rotation, ref spriteDirection, lookType, rotAddon, rotAmount, flipSpriteDir);
            if (c is NPC) {
                ((NPC)c).spriteDirection = spriteDirection;
                ((NPC)c).rotation = rotation;
            }
            else
            if (c is Projectile) {
                ((Projectile)c).spriteDirection = spriteDirection;
                ((Projectile)c).rotation = rotation;
            }
        }

        /*
         * Makes the rotation value and sprite direction 'look' at the given target.
         * lookType : the type of look code to run.
         *        0 -> Rotate the entity and change sprite direction based on the look target.
         *        1 -> change spriteDirection based on the look target.
         *        2 -> Rotate the entity based on the look target.
         *        3 -> Smoothly rotate and change sprite direction based on the look target.
         *        4 -> Smoothly rotate based on the look target.       
         * rotAddon : the amount to add to the rotation. (only used by lookType 3/4)
         * rotAmount: the amount to rotate by. (only used by lookType 3/4)
         */
        public static void LookAt(Vector2 lookTarget, Vector2 center, ref float rotation, ref int spriteDirection, int lookType = 0, float rotAddon = 0f, float rotAmount = 0.075f, bool flipSpriteDir = false) {
            if (lookType == 0) {
                if (lookTarget.X > center.X) { spriteDirection = -1; } else { spriteDirection = 1; }
                if (flipSpriteDir) { spriteDirection *= -1; }
                float rotX = lookTarget.X - center.X;
                float rotY = lookTarget.Y - center.Y;
                rotation = -((float)Math.Atan2((double)rotX, (double)rotY) - 1.57f + rotAddon);
                if (spriteDirection == 1) { rotation -= (float)Math.PI; }
            }
            else
            if (lookType == 1) {
                if (lookTarget.X > center.X) { spriteDirection = -1; } else { spriteDirection = 1; }
                if (flipSpriteDir) { spriteDirection *= -1; }
            }
            else
            if (lookType == 2) {
                float rotX = lookTarget.X - center.X;
                float rotY = lookTarget.Y - center.Y;
                rotation = -((float)Math.Atan2((double)rotX, (double)rotY) - 1.57f + rotAddon);
            }
            else
            if (lookType == 3 || lookType == 4) {
                int oldDirection = spriteDirection;
                if (lookType == 3 && lookTarget.X > center.X) { spriteDirection = -1; } else { spriteDirection = 1; }
                if (lookType == 3 && flipSpriteDir) { spriteDirection *= -1; }
                if (oldDirection != spriteDirection) {
                    rotation += (float)Math.PI * spriteDirection;
                }
                float pi2 = (float)Math.PI * 2f;
                float rotX = lookTarget.X - center.X;
                float rotY = lookTarget.Y - center.Y;
                float rot = ((float)Math.Atan2((double)rotY, (double)rotX) + rotAddon);
                if (spriteDirection == 1) { rot += (float)Math.PI; }
                if (rot > pi2) { rot -= pi2; } else if (rot < 0) { rot += pi2; }
                if (rotation > pi2) { rotation -= pi2; } else if (rotation < 0) { rotation += pi2; }
                if (rotation < rot) {
                    if ((double)(rot - rotation) > (float)Math.PI) { rotation -= rotAmount; } else { rotation += rotAmount; }
                }
                else
                if (rotation > rot) {
                    if ((double)(rotation - rot) > (float)Math.PI) { rotation += rotAmount; } else { rotation -= rotAmount; }
                }
                if (rotation > rot - rotAmount && rotation < rot + rotAmount) { rotation = rot; }
            }
        }

        public static void RotateTo(ref float rotation, float rotDestination, float rotAmount = 0.075f) {
            float pi2 = (float)Math.PI * 2f;
            float rot = rotDestination;
            if (rot > pi2) { rot -= pi2; } else if (rot < 0) { rot += pi2; }
            if (rotation > pi2) { rotation -= pi2; } else if (rotation < 0) { rotation += pi2; }
            if (rotation < rot) {
                if ((double)(rot - rotation) > (float)Math.PI) { rotation -= rotAmount; } else { rotation += rotAmount; }
            }
            else if (rotation > rot) {
                if ((double)(rotation - rot) > (float)Math.PI) { rotation += rotAmount; } else { rotation -= rotAmount; }
            }
            if (rotation > rot - rotAmount && rotation < rot + rotAmount) { rotation = rot; }
        }
    }
}