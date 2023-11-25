using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;

namespace Macrocosm.Common.Utils
{
    public static partial class Utility
    {
        public static int GetFrameHeight(this NPC npc) => TextureAssets.Npc[npc.type].Height() / Main.npcFrameCount[npc.type];


        /// <summary>  Scales this <paramref name="npc"/>'s health by the scale <paramref name="factor"/> provided. </summary>
        public static void ScaleHealthBy(this NPC npc, float factor, float balance, float bossAdjustment)
        {
            npc.lifeMax = (int)Math.Ceiling(npc.lifeMax * factor * balance * bossAdjustment);
        }


        public static bool SummonBossDirectlyWithMessage(Vector2 targetPosition, int type)
        {
            //Try to spawn the new NPC.  If that failed, then "npc" will be 200
            int npc = NPC.NewNPC(Entity.GetSource_NaturalSpawn(), (int)targetPosition.X, (int)targetPosition.Y, type);

            //Only display the text if we could spawn the NPC
            if (npc < Main.npc.Length)
            {
                string name = Main.npc[npc].TypeName;

                //Display the "X has awoken!" text since we aren't using NPC.SpawnOnPlayer(int, int)
                Main.NewText(Language.GetTextValue("Announcement.HasAwoken", name), 175, 75, 255);
            }

            return npc != Main.npc.Length;  //Return false if we couldn't generate an NPC
        }

        public static void Move(this NPC npc, Vector2 moveTo, Vector2 offset, float speed = 3f, float turnResistance = 0.5f)
        {
            moveTo += offset; // Gets the point that the NPC will be moving to.
            Vector2 move = moveTo - npc.Center;
            float magnitude = move.Length();

            if (magnitude > speed)
            {
                move *= speed / magnitude;
            }

            move = (npc.velocity * turnResistance + move) / (turnResistance + 1f);

            magnitude = move.Length();

            if (magnitude > speed)
            {
                move *= speed / magnitude;
            }

            npc.velocity = move;
        }

        public static void UpdateScaleAndHitbox(this NPC npc, int baseWidth, int baseHeight, float newScale)
        {
            Vector2 center = npc.Center;
            npc.width = (int)Math.Max(1f, baseWidth * newScale);
            npc.height = (int)Math.Max(1f, baseHeight * newScale);
            npc.scale = newScale;
            npc.Center = center;
        }
    }
}
