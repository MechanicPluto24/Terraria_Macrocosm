using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Common.Utils
{
    public static partial class Utility
    {
        public static int GetFrameHeight(this NPC npc) => TextureAssets.Npc[npc.type].Height() / Main.npcFrameCount[npc.type];


        /// <summary> Scales this <paramref name="npc"/>'s health by the scale <paramref name="factor"/> provided. </summary>
        public static void ScaleHealthBy(this NPC npc, float factor, float balance, float bossAdjustment)
        {
            npc.lifeMax = (int)Math.Ceiling(npc.lifeMax * factor * balance * bossAdjustment);
        }

        public static void ApplyBuffImmunity(this NPC npc, params int[] buffIds) => ApplyImmunity(npc.type, buffIds);
        public static void ApplyImmunity(int type, params int[] buffIds)
        {
            foreach (int buff in buffIds)
            {
                NPCID.Sets.SpecificDebuffImmunity[type][buff] = true;
            }
        }

        public static void AddBuff<T>(this NPC npc, int time, bool quiet = false) where T : ModBuff
        {
            npc.AddBuff(ModContent.BuffType<T>(), time, quiet);
        }

        public static void RemoveBuff<T>(this NPC npc) where T : ModBuff => npc.RemoveBuff(ModContent.BuffType<T>());
        public static void RemoveBuff(this NPC npc, int type)
        {
            int idx = npc.FindBuffIndex(type);
            if (idx > 0) npc.DelBuff(idx);
        }

        public static void ClearBuffs(this NPC npc)
        {
            for (int i = 0; i < npc.buffType.Length; i++)
            {
                int type = npc.buffType[i];
                if (!Main.debuff[type])
                    npc.DelBuff(i);
            }
        }

        public static void ClearDebuffs(this NPC npc)
        {
            for (int i = 0; i < npc.buffType.Length; i++)
            {
                int type = npc.buffType[i];
                if (Main.debuff[type])
                    npc.DelBuff(i);
            }
        }

        public static bool SummonBossDirectlyWithMessage(Vector2 targetPosition, int type, float ai0 = 0f, float ai1 = 0f, float ai2 = 0f, float ai3 = 0f, int target = 255, SoundStyle? sound = null)
        {
            SoundEngine.PlaySound(sound, targetPosition);

            if (Main.netMode == NetmodeID.MultiplayerClient)
                return false;

            //Try to spawn the new NPC.  If that failed, then "npc" will be 200
            int npc = NPC.NewNPC(Entity.GetSource_NaturalSpawn(), (int)targetPosition.X, (int)targetPosition.Y, type, 0, ai0, ai1, ai2, ai3, target);

            //Only display the text if we could spawn the NPC
            if (npc < Main.npc.Length)
            {
                string name = Main.npc[npc].TypeName;

                //Display the "X has awoken!" text since we aren't using NPC.SpawnOnPlayer(int, int)
                Main.NewText(Language.GetTextValue("Announcement.HasAwoken", name), 175, 75, 255);
            }

            //Return false if we couldn't generate an NPC
            return npc != Main.maxNPCs;
        }

        public static void SummonBossOnPlayerWithMessage(Player player, int type, SoundStyle? sound = null)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                SoundEngine.PlaySound(sound, player.position);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.SpawnOnPlayer(player.whoAmI, type);
                else
                    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type);
            }
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

        /// <summary>
        /// Finds the closest valid NPC within range. 
        /// </summary>
        /// <param name="start">The origin point.</param>
        /// <param name="maxRange">Maximum range in coords.</param>
        /// <param name="exclude">Optional list of NPCs to ignore. If non-null, the chosen NPC will be marked as used.</param>
        public static int FindNPC(Vector2 start, float maxRange, bool[] exclude = null)
        {
            int target = -1;
            float maxRangeSq = maxRange * maxRange;
            float bestDistSq = maxRangeSq;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                if (npc.CanBeChasedBy() && (exclude == null || !exclude[i]))
                {
                    float distSq = Vector2.DistanceSquared(start, npc.Center);
                    if (distSq <= bestDistSq && Collision.CanHitLine(start, 0, 0, npc.Center, 0, 0))
                    {
                        target = i;
                        bestDistSq = distSq;
                    }
                }
            }

            if (exclude != null && target != -1)
                exclude[target] = true;

            return target;
        }

        public static int CountNPCs(int type)
        {
            int count = 0;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.type == type && npc.active)
                    count++;
            }

            return count;
        }

        public static void UpdateScaleAndHitbox(this NPC npc, int baseWidth, int baseHeight, float newScale)
        {
            Vector2 center = npc.Center;
            npc.width = (int)Math.Max(1f, baseWidth * newScale);
            npc.height = (int)Math.Max(1f, baseHeight * newScale);
            npc.scale = newScale;
            npc.Center = center;
        }


        public static void SpawnAndKillNPC(IEntitySource source, Vector2 position, int type, int start = 0, float ai0 = 0f, float ai1 = 0f, float ai2 = 0f, float ai3 = 0f, int target = 255, bool noPlayerInteraction = true)
        {
            NPC npc = NPC.NewNPCDirect(source, position, type, start, ai0, ai1, ai2, ai3, target);
            var hit = new NPC.HitInfo() { InstantKill = true };
            npc.StrikeNPC(hit, fromNet: false, noPlayerInteraction: noPlayerInteraction);
            if (Main.netMode != NetmodeID.SinglePlayer)
                NetMessage.SendStrikeNPC(npc, hit);
        }

        public static void BurnTownNPC(int npcType)
        {
            foreach (var npc in Main.ActiveNPCs)
            {
                if (npc.type == npcType && !npc.HasBuff(BuffID.OnFire))
                    npc.AddBuff(BuffID.OnFire, 60);
            }
        }
    }
}
