using Macrocosm.Common.Config;
using Macrocosm.Common.Utils;
using Macrocosm.Content.LoadingScreens;
using Macrocosm.Content.Subworlds;
using SubworldLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Common.Subworlds
{
    public class SubworldTravelPlayer : ModPlayer
    {
        /// <summary>
        /// Whether the player has voluntarily initiated subworld travel. Not synced. 
        /// <br> Value can be set in <see cref="MacrocosmSubworld.Travel"/>via the <c>trigger</c> parameter</br>
        /// <br> If true, the title sequence will display, and SubworldSystem.Exit() will move the player to Earth.</br> 
        /// <br> (Player took a Rocket, or used Teleporter)</br>
        /// <br> If false, the title sequence will display, and SubworldSystem.Exit() will return to the main menu. </br>
        /// <br> (Player clicks the "Save & Exit"/"Return" button from the in-game options menu, or is forced to a subworld on world enter) </br>
        /// </summary>
        public bool TriggeredSubworldTravel { get; set; } = false;

        /// <summary> Whether the player has exited a subworld by clicking the "Save & Exit"/"Return" button or other means </summary>
        private bool exitedBySaveAndExit = false;

        /// <summary> Flag that runs when the player enters the world </summary>
        private bool enteredWorld = false;

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
            TriggeredSubworldTravel = false;
        }

        public bool HasVisitedSubworld(string subworldId) => visitedSubworlds.Contains(subworldId);

        public void SetReturnSubworld(string subworldId)
        {
            if (currentMainWorldUniqueId == Guid.Empty)
                Utility.LogChatMessage("Main world GUID not received from the server", Utility.MessageSeverity.Error);

            lastSubworldIdByWorldUniqueId[currentMainWorldUniqueId] = subworldId;
        }

        public bool TryGetReturnSubworld(Guid worldUniqueId, out string subworldId) => lastSubworldIdByWorldUniqueId.TryGetValue(worldUniqueId, out subworldId);

        public override void PreUpdate()
        {
            if (exitedBySaveAndExit)
            {
                WorldGen.SaveAndQuit();

                exitedBySaveAndExit = false;
                return;
            }

            if (enteredWorld)
            {
                if (!TriggeredSubworldTravel)
                {
                    currentMainWorldUniqueId = MacrocosmSubworld.MainWorldUniqueID;
                    if (!SubworldSystem.AnyActive() && Main.netMode == NetmodeID.SinglePlayer)
                    {
                        if (TryGetReturnSubworld(currentMainWorldUniqueId, out string subworldId) && subworldId != Earth.ID)
                        {
                            MacrocosmSubworld.Travel(subworldId, trigger: false);
                        }
                    }
                }
                else 
                {
                    if (SubworldSystem.AnyActive<Macrocosm>())
                    {
                        if (!HasVisitedSubworld(MacrocosmSubworld.CurrentID) || ClientConfig.Instance.AlwaysDisplayTitleCards)
                            TitleCard.Start();

                        visitedSubworlds.Add(MacrocosmSubworld.CurrentID);
                    }
                    else
                    {
                        if (ClientConfig.Instance.AlwaysDisplayTitleCards)
                            TitleCard.Start();
                    }
                }

                enteredWorld = false;
            } 
        }

        internal void OnExitSubworld()
        {
            if (!TriggeredSubworldTravel)
            {
                MacrocosmSubworld.SetupLoadingScreen(null, MacrocosmSubworld.Hacks.SubworldSystem_CacheID());
                exitedBySaveAndExit = true;
            }
        }

        public override void OnEnterWorld()
        {
            Main.BlackFadeIn = 255;
            enteredWorld = true;
        }

        public static void ReceiveLastSubworldCheck(BinaryReader reader, int whoAmI)
        {
            var macrocosmPlayer = Main.LocalPlayer.GetModPlayer<SubworldTravelPlayer>();

            Guid mainWorldUniqueId = new(reader.ReadString());
            macrocosmPlayer.currentMainWorldUniqueId = mainWorldUniqueId;

            if
            (
                !SubworldSystem.AnyActive<Macrocosm>()
                && !macrocosmPlayer.TriggeredSubworldTravel
                && macrocosmPlayer.lastSubworldIdByWorldUniqueId.TryGetValue(macrocosmPlayer.currentMainWorldUniqueId, out string lastSubworldId)
            )
            {
                if (lastSubworldId is not Earth.ID)
                    MacrocosmSubworld.Travel(lastSubworldId, trigger: false);
            }
        }

        public override void SaveData(TagCompound tag)
        {
            if (visitedSubworlds.Count > 0)
                tag[nameof(visitedSubworlds)] = visitedSubworlds;

            TagCompound lastSubworldsByWorld = new();
            foreach (var kvp in lastSubworldIdByWorldUniqueId)
                lastSubworldsByWorld[kvp.Key.ToString()] = kvp.Value;

            tag[nameof(lastSubworldIdByWorldUniqueId)] = lastSubworldsByWorld;
        }

        public override void LoadData(TagCompound tag)
        {
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
