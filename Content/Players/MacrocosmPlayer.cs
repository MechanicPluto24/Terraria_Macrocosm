using Macrocosm.Common.Config;
using Macrocosm.Common.Netcode;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Systems;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Biomes;
using Macrocosm.Content.Buffs.Debuffs;
using Macrocosm.Content.LoadingScreens;
using Macrocosm.Content.Subworlds;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using System;
using System.Collections.Generic;
using System.Drawing;
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
    public enum SpaceProtection
    {
        None,
        Tier1,
        Tier2,
        Tier3
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
        public SpaceProtection SpaceProtection { get; set; } = SpaceProtection.None;

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

        public override void ResetEffects()
        {
            SpaceProtection = SpaceProtection.None;
            RadNoiseIntensity = 0f;
            ChanceToNotConsumeAmmo = 0f;
            Player.buffImmune[BuffType<Depressurized>()] = false;

            if (Main.ingameOptionsWindow)
                TriggeredSubworldTravel = false;
        }

        public override bool CanConsumeAmmo(Item weapon, Item ammo)
        {
            bool consumeAmmo = true;

            if (Main.rand.NextFloat() < ChanceToNotConsumeAmmo)
                consumeAmmo = false;

            return consumeAmmo;
        }

        public override void PostUpdateBuffs()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
            {
                UpdateSpaceEnvironmentalDebuffs();
            }
        }

        public void UpdateSpaceEnvironmentalDebuffs()
        {
            if (!Player.GetModPlayer<RocketPlayer>().InRocket)
            {
                if (SubworldSystem.IsActive<Moon>())
                {
                    if (SpaceProtection == SpaceProtection.None)
                        Player.AddBuff(BuffType<Depressurized>(), 2);
                    //if (protectTier <= SpaceProtection.Tier1)
                    //Player.AddBuff(BuffType<Irradiated>(), 2);
                }
                //else if (SubworldSystem.IsActive<Mars>())
            }
        }

        public override void PostUpdateMiscEffects()
        {
            //UpdateGravity();
            UpdateFilterEffects();
        }

        public override void PostUpdateEquips()
        {
            UpdateSpaceArmourImmunities();
            if (Player.GetModPlayer<MacrocosmPlayer>().SpaceProtection > SpaceProtection.None)
                Player.setBonus = Language.GetTextValue("Mods.Macrocosm.Items.SetBonuses.SpaceProtection_" + SpaceProtection.ToString());
        }

        public void UpdateSpaceArmourImmunities()
        {
            if (SpaceProtection > SpaceProtection.None)
                Player.buffImmune[BuffType<Depressurized>()] = true;
            if (SpaceProtection > SpaceProtection.Tier1)
            {
            }
        }

        //private void UpdateGravity()
        //{
        //	if (MacrocosmSubworld.AnyActive)
        //		Player.gravity = Player.defaultGravity * MacrocosmSubworld.Current.GravityMultiplier;
        //}

        private void UpdateFilterEffects()
        {
            if (Main.dedServ)
                return;

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

        public bool HasVisitedSubworld(string subworldId) => visitedSubworlds.Contains(subworldId);

        public void SetReturnSubworld(string subworldId)
        {
            Guid guid = SubworldSystem.AnyActive<Macrocosm>() ? MacrocosmSubworld.Current.MainWorldUniqueId : Main.ActiveWorldFileData.UniqueId;
            lastSubworldIdByWorldUniqueId[guid] = subworldId;
        }

        public bool TryGetReturnSubworld(Guid worldUniqueId, out string subworldId) => lastSubworldIdByWorldUniqueId.TryGetValue(worldUniqueId, out subworldId);

        public override void OnEnterWorld()
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                LastSubworldCheck(Main.ActiveWorldFileData.UniqueId);
            }

            if (TriggeredSubworldTravel)
            {
                if (SubworldSystem.AnyActive<Macrocosm>())
                {
                    LoadingTitleSequence.Start(noTitle: HasVisitedSubworld(MacrocosmSubworld.CurrentMacrocosmID) && !MacrocosmConfig.Instance.AlwaysDisplayTitleScreens);
                    visitedSubworlds.Add(MacrocosmSubworld.CurrentMacrocosmID);
                }
                else
                {
                    LoadingTitleSequence.Start(noTitle: !MacrocosmConfig.Instance.AlwaysDisplayTitleScreens);
                }
            }
        }

        public static void LastSubworldCheck(BinaryReader reader, int whoAmI)
        {
            Guid mainWorldUniqueId = new(reader.ReadString());
            Main.LocalPlayer.GetModPlayer<MacrocosmPlayer>().LastSubworldCheck(mainWorldUniqueId);
        }

        private void LastSubworldCheck(Guid mainWorldUniqueId)
        {
            if (!SubworldSystem.AnyActive<Macrocosm>() && !TriggeredSubworldTravel && lastSubworldIdByWorldUniqueId.TryGetValue(mainWorldUniqueId, out string lastSubworldId))
            {
                if (lastSubworldId is not "Earth")
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

            if(visitedSubworlds.Any())
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
