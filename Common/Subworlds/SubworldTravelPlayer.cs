using Macrocosm.Common.Config;
using Macrocosm.Common.Utils;
using Macrocosm.Content.LoadingScreens;
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
        /// <br> (Player clicks the "Return" button from the in-game options menu, or is forced to a subworld on world enter) </br>
        /// </summary>
        public bool TriggeredSubworldTravel { get; set; }

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

        public override void OnEnterWorld()
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                currentMainWorldUniqueId = MacrocosmSubworld.MainWorldUniqueID;
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
                    if (MacrocosmConfig.Instance.AlwaysDisplayTitleCards)
                        TitleCard.Start();
                }
            }
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
                if (lastSubworldId is not "Macrocosm/Earth")
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
