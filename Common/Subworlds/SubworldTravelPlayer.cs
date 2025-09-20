using Macrocosm.Common.Config;
using Macrocosm.Common.Netcode;
using Macrocosm.Common.Utils;
using Macrocosm.Content.LoadingScreens;
using Macrocosm.Content.Rockets;
using Macrocosm.Content.Subworlds;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Common.Subworlds;

public class SubworldTravelPlayer : ModPlayer
{
    private bool triggeredSubworldTravel = false;

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

    public bool HasVisitedSubworld(string subworldId) => visitedSubworlds.Contains(subworldId);

    public void SetReturnSubworld(string subworldId)
    {
        if (currentMainWorldUniqueId == Guid.Empty)
            Utility.LogChatMessage("Main world GUID not received from the server", Utility.MessageSeverity.Error);

        lastSubworldIdByWorldUniqueId[currentMainWorldUniqueId] = subworldId;
    }

    public bool TryGetReturnSubworld(Guid worldUniqueId, out string subworldId) => lastSubworldIdByWorldUniqueId.TryGetValue(worldUniqueId, out subworldId);


    public static int GetReturnDestination()
    {
        var player = Main.LocalPlayer.GetModPlayer<SubworldTravelPlayer>();

        if (!player.triggeredSubworldTravel && Main.netMode == NetmodeID.MultiplayerClient)
            return int.MinValue;

        return -1;
    }

    public override void ResetEffects()
    {
        triggeredSubworldTravel = false;
    }

    public override void PreUpdate()
    {
        if (exitedBySaveAndExit && Main.netMode == NetmodeID.SinglePlayer)
        {
            WorldGen.SaveAndQuit();
            exitedBySaveAndExit = false;
            return;
        }

        if (enteredWorld)
        {
            if (!triggeredSubworldTravel)
            {
                if (!SubworldSystem.AnyActive<Macrocosm>() && Main.netMode == NetmodeID.SinglePlayer)
                {
                    currentMainWorldUniqueId = MacrocosmSubworld.MainWorldUniqueID;
                    if (TryGetReturnSubworld(currentMainWorldUniqueId, out string subworldId) && subworldId != Earth.ID)
                    {
                        Travel(subworldId, trigger: false);
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

    public override void OnEnterWorld()
    {
        Main.BlackFadeIn = 255;
        enteredWorld = true;
    }

    internal void OnExitSubworld()
    {
        if (!triggeredSubworldTravel && Main.netMode == NetmodeID.SinglePlayer)
        {
            MacrocosmSubworld.SetupLoadingScreen(null, MacrocosmSubworld.CacheID);
            exitedBySaveAndExit = true;
        }
    }

    /// <summary> Travel to the specified subworld, using the specified rocket. </summary>
    /// <param name="targetWorldID"> The world to travel to, "Earth" for returning to the main world. </param>
    /// <param name="rocket"> The spacecraft used for travel, if applicable. Will display in the loading screen. </param>
    /// <param name="trigger"> Value set to the <see cref="MacrocosmPlayer.TriggeredSubworldTravel"/>. Normally true. </param>
    /// <returns> Whether world travel has been successful </returns>
    public static bool Travel(string targetWorldID, Rocket rocket = null, bool trigger = true, bool downwards = false)
    {
        if (Main.netMode != NetmodeID.Server)
        {
            if (!trigger)
                rocket = null;

            MacrocosmSubworld.SetupLoadingScreen(rocket, targetWorldID, downwards);
            TitleCard.SetTargetWorld(targetWorldID);

            Main.LocalPlayer.GetModPlayer<SubworldTravelPlayer>().triggeredSubworldTravel = trigger;
            Main.LocalPlayer.GetModPlayer<SubworldTravelPlayer>().SetReturnSubworld(targetWorldID);

            if (targetWorldID == Earth.ID)
            {
                SubworldSystem.Exit();
                return true;
            }

            bool entered = SubworldSystem.Enter(targetWorldID);
            if (!entered)
            {
                string message = "Error: Failed entering target subworld: " + targetWorldID + ", staying on " + MacrocosmSubworld.CurrentID;
                Utility.Chat(message, Color.Red);
                Macrocosm.Instance.Logger.Error(message);
            }

            return entered;
        }
        else
        {
            return false;
        }
    }

    public static void SendTravelRequest(int toPlayer, string targetWorld, int rocketId)
    {
        if (Main.netMode != NetmodeID.Server)
            return;

        ModPacket packet = Macrocosm.Instance.GetPacket();
        packet.Write((byte)MessageType.TravelRequest);
        packet.Write(targetWorld);
        packet.Write((byte)rocketId);
        packet.Send(toPlayer);
    }

    public static void ReceiveTravelRequest(BinaryReader reader, int whoAmI)
    {
        string targetWorld = reader.ReadString();
        int rocketId = reader.ReadByte();

        if (Main.netMode != NetmodeID.MultiplayerClient)
            return;

        Rocket rocket = (rocketId >= 0 && rocketId <= RocketManager.MaxRockets) ? RocketManager.Rockets[rocketId] : null;
        if (MacrocosmSubworld.IsValidID(targetWorld))
            Travel(targetWorld, rocket);
    }

    /// <summary>
    /// Send current main world GUID and ask player to travel to last known subworld  
    /// </summary>
    public static void SendLastSubworldCheck(int remoteClient)
    {
        if (Main.netMode != NetmodeID.Server)
            return;

        ModPacket packet = Macrocosm.Instance.GetPacket();
        packet.Write((byte)MessageType.LastSubworldCheck);
        Guid guid = MacrocosmSubworld.MainWorldUniqueID;
        packet.Write(guid.ToString());
        packet.Send(remoteClient);
    }

    public static void ReceiveLastSubworldCheck(BinaryReader reader, int whoAmI)
    {
        var player = Main.LocalPlayer.GetModPlayer<SubworldTravelPlayer>();
        Guid mainWorldUniqueId = new(reader.ReadString());

        if (Main.netMode != NetmodeID.MultiplayerClient)
            return;

        player.currentMainWorldUniqueId = mainWorldUniqueId;
        if
        (
            !SubworldSystem.AnyActive<Macrocosm>()
            && !player.triggeredSubworldTravel
            && player.lastSubworldIdByWorldUniqueId.TryGetValue(player.currentMainWorldUniqueId, out string lastSubworldId)
        )
        {
            if (lastSubworldId is not Earth.ID)
                Travel(lastSubworldId, trigger: false);
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

        tag[nameof(triggeredSubworldTravel)] = triggeredSubworldTravel;
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

        triggeredSubworldTravel = tag.ContainsKey(nameof(triggeredSubworldTravel));
    }
}
