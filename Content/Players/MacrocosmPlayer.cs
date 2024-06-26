﻿using Macrocosm.Common.Config;
using Macrocosm.Common.Netcode;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Systems;
using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Biomes;
using Macrocosm.Content.Debuffs;
using Macrocosm.Content.LoadingScreens;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Content.Players
{
    public enum SpaceHazard
    {
        None,
        Vacuum
    }

    /// <summary>
    /// Miscellaneous class for storing custom player data. 
    /// Complex, very specific systems should be implemented in a separate ModPlayer.
    /// </summary>
    public class MacrocosmPlayer : ModPlayer
    {
        /// <summary>
        /// Whether the player has voluntarily initiated subworld travel. Not synced. 
        /// <br> Value can be set in <see cref="MacrocosmSubworld.Travel"/>via the <c>trigger</c> parameter</br>
        /// <br> If true, the title sequence will display, and SubworldSystem.Exit() will move the player to Earth.</br> 
        /// <br> (Player took a Rocket, or used Teleporter)</br>
        /// <br> If false, the title sequence will display, and SubworldSystem.Exit() will return to the main menu. </br>
        /// <br> (Player clicks the "Return" button from the in-game options menu, or is forced to a subworld on world enter) </br>
        /// </summary>
        public bool TriggeredSubworldTravel { get; set; }

        /// <summary> 
        /// The player's space protection level.
        /// Not synced.
        /// </summary>
        public float SpaceProtection { get; set; } = 0f;

        /// <summary> 
        /// The radiation effect intensity for this player. 
        /// Not synced.
        /// </summary>
        public float RadNoiseIntensity = 0f;

        /// <summary> 
        /// Chandrium whip hit stacks. 
        /// Not synced.
        /// </summary>
        public int ChandriumWhipStacks = 0;

        /// <summary> 
        /// Whether this player is aware that they can use zombie fingers to unlock chests.
        /// Persistent. Not synced.
        /// </summary>
        public bool KnowsToUseZombieFinger = false;

        /// <summary> 
        /// Chance to not consume ammo from equipment and weapons, stacks additively with the vanilla chance 
        /// Not synced.
        /// </summary>
        public float ChanceToNotConsumeAmmo
        {
            get => chanceToNotConsumeAmmo;
            set => chanceToNotConsumeAmmo = MathHelper.Clamp(value, 0f, 1f);
        }

        private float chanceToNotConsumeAmmo = 0f;

        /// <summary>
        /// The subworlds this player has visited.
        /// Persistent. Local to the player. Not synced.
        /// </summary>
        //TODO: sync this if needed
        private List<string> visitedSubworlds = new();

        /// <summary>
        /// A dictionary of this player's last known subworld Id, by each Terraria main world file visited.
        /// Not synced.
        /// </summary>
        private readonly Dictionary<Guid, string> lastSubworldIdByWorldUniqueId = new();

        private Guid currentMainWorldUniqueId = Guid.Empty;

        public override void ResetEffects()
        {
            SpaceProtection = 0f;
            RadNoiseIntensity = 0f;
            ChanceToNotConsumeAmmo = 0f;
            Player.buffImmune[BuffType<Depressurized>()] = false;

            TriggeredSubworldTravel = false;
        }

        public override bool CanConsumeAmmo(Item weapon, Item ammo)
        {
            bool consumeAmmo = true;

            if (Main.rand.NextFloat() < ChanceToNotConsumeAmmo)
                consumeAmmo = false;

            return consumeAmmo;
        }

        public override void PreUpdate()
        {
            if(Main.netMode != NetmodeID.MultiplayerClient)
                currentMainWorldUniqueId = SubworldSystem.AnyActive<Macrocosm>() ? MacrocosmSubworld.Current.MainWorldUniqueId : Main.ActiveWorldFileData.UniqueId;
        }

        public override void PostUpdateBuffs()
        {
            if (!Player.GetModPlayer<RocketPlayer>().InRocket)
            {
                if (SubworldSystem.AnyActive<Macrocosm>())
                    Player.AddBuff(BuffType<Depressurized>(), 2);

                //if(Player.InModBiome<IrradiationBiome>())
                //    Player.AddBuff(BuffType<Irradiated>(), 2);
            }
        }

        public override void PostUpdateEquips()
        {
            if (SpaceProtection >= (float)SpaceHazard.Vacuum * 3)
                Player.buffImmune[BuffType<Depressurized>()] = true;

            //if (SpaceProtection >= (float)SpaceHazard.Radiation * 3)
            //    Player.buffImmune[BuffType<Irradiated>()] = true;

            if (SpaceProtection >= 3f)
                Player.setBonus = Language.GetTextValue("Mods.Macrocosm.Items.SetBonuses.SpaceProtection_" + (int)(SpaceProtection / 3f));
        }

        public override void PostUpdateMiscEffects()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                Main.SceneMetrics.GraveyardTileCount = 0;
            else
                Main.SceneMetrics.GraveyardTileCount += TileCounts.Instance.GraveyardModTileCount;

            if (!Main.dedServ)
            {
                if (Player.InModBiome<IrradiationBiome>())
                {
                    if (!Filters.Scene["Macrocosm:RadiationNoise"].IsActive())
                        Filters.Scene.Activate("Macrocosm:RadiationNoise");

                    RadNoiseIntensity += 0.189f * Utility.InverseLerp(400, 10000, TileCounts.Instance.IrradiatedRockCount, clamped: true);

                    Filters.Scene["Macrocosm:RadiationNoise"].GetShader().UseIntensity(RadNoiseIntensity);
                }
                else
                {
                    if (Filters.Scene["Macrocosm:RadiationNoise"].IsActive())
                        Filters.Scene.Deactivate("Macrocosm:RadiationNoise");
                }
            }
        }

        public bool HasVisitedSubworld(string subworldId) => visitedSubworlds.Contains(subworldId);

        public void SetReturnSubworld(string subworldId)
        {
            if (currentMainWorldUniqueId == Guid.Empty)
                Utility.LogChatMessage("Main world GUID not received from the server", Utility.MessageSeverity.Error);

            lastSubworldIdByWorldUniqueId[currentMainWorldUniqueId] = subworldId;
        }

        public bool TryGetReturnSubworld(Guid worldUniqueId, out string subworldId) => lastSubworldIdByWorldUniqueId.TryGetValue(worldUniqueId, out subworldId);

        public override void OnEnterWorld()
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                if (!SubworldSystem.AnyActive<Macrocosm>() && !TriggeredSubworldTravel && lastSubworldIdByWorldUniqueId.TryGetValue(currentMainWorldUniqueId, out string lastSubworldId))
                {
                    if (lastSubworldId is not "Macrocosm/Earth")
                        MacrocosmSubworld.Travel(lastSubworldId, trigger: false);
                }
            }

            if (TriggeredSubworldTravel)
            {
                if (SubworldSystem.AnyActive<Macrocosm>())
                {
                    if (!HasVisitedSubworld(MacrocosmSubworld.CurrentID) || MacrocosmConfig.Instance.AlwaysDisplayTitleCards)
                        TitleCard.Start();

                    visitedSubworlds.Add(MacrocosmSubworld.CurrentID);
                }
                else
                {
                    if(MacrocosmConfig.Instance.AlwaysDisplayTitleCards)
                        TitleCard.Start();
                }
            }

            CircuitSystem.SearchCircuits();
        }

        public static void ReceiveLastSubworldCheck(BinaryReader reader, int whoAmI)
        {
            var macrocosmPlayer = Main.LocalPlayer.GetModPlayer<MacrocosmPlayer>();
            
            Guid mainWorldUniqueId = new(reader.ReadString());
            macrocosmPlayer.currentMainWorldUniqueId = mainWorldUniqueId;

            if 
            (
                !SubworldSystem.AnyActive<Macrocosm>()
                && !macrocosmPlayer.TriggeredSubworldTravel
                && macrocosmPlayer.lastSubworldIdByWorldUniqueId.TryGetValue(macrocosmPlayer.currentMainWorldUniqueId, out string lastSubworldId)
            )
            {
                if (lastSubworldId is not "Macrocosm/Earth")
                    MacrocosmSubworld.Travel(lastSubworldId, trigger: false);
            }
        }

        public override void CopyClientState(ModPlayer clientClone)
        {
            // TODO: copy data that has to be netsynced
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            // TODO: SyncPlayer if netsynced data is different
        }

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)MessageType.SyncMacrocosmPlayer);
            packet.Write((byte)Player.whoAmI);

            // TODO: add netsynced data here 
        }

        public static void ReceiveSyncPlayer(BinaryReader reader, int whoAmI)
        {
            int playerWhoAmI = reader.ReadByte();
            MacrocosmPlayer macrocosmPlayer = Main.player[playerWhoAmI].GetModPlayer<MacrocosmPlayer>();

            // TODO: read netsynced data here
        }

        public override void SaveData(TagCompound tag)
        {
            if (KnowsToUseZombieFinger)
                tag[nameof(KnowsToUseZombieFinger)] = true;

            if (visitedSubworlds.Any())
                tag[nameof(visitedSubworlds)] = visitedSubworlds;

            TagCompound lastSubworldsByWorld = new();
            foreach (var kvp in lastSubworldIdByWorldUniqueId)
                lastSubworldsByWorld[kvp.Key.ToString()] = kvp.Value;

            tag[nameof(lastSubworldIdByWorldUniqueId)] = lastSubworldsByWorld;
        }

        public override void LoadData(TagCompound tag)
        {
            KnowsToUseZombieFinger = tag.ContainsKey(nameof(KnowsToUseZombieFinger));

            if (tag.ContainsKey(nameof(visitedSubworlds)))
                visitedSubworlds = tag.GetList<string>(nameof(visitedSubworlds)).ToList();

            if (tag.ContainsKey(nameof(lastSubworldIdByWorldUniqueId)))
            {
                TagCompound lastSubworldsByWorld = tag.GetCompound(nameof(lastSubworldIdByWorldUniqueId));
                foreach (var kvp in lastSubworldsByWorld)
                    lastSubworldIdByWorldUniqueId[new Guid(kvp.Key)] = lastSubworldsByWorld.GetString(kvp.Key);
            }
        }
    }
}
