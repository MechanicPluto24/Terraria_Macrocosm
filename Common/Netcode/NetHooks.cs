using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Netcode;

public class NetHooks : ModSystem
{
    private const int JoinSyncDelayTicks = 30;

    private static bool[] syncedPlayers;
    private static int[] joinSyncTimers;

    public override void Load()
    {
        syncedPlayers = new bool[Main.maxPlayers];
        joinSyncTimers = new int[Main.maxPlayers];
    }

    public override void Unload()
    {
        syncedPlayers = null;
        joinSyncTimers = null;
    }

    public override void OnWorldLoad()
    {
        ResetJoinTracking();
    }

    public override void OnWorldUnload()
    {
        ResetJoinTracking();
    }

    public override void PreUpdatePlayers()
    {
        if (Main.netMode != NetmodeID.Server || syncedPlayers is null || joinSyncTimers is null)
            return;

        for (int i = 0; i < Main.maxPlayers; i++)
        {
            Player player = Main.player[i];

            if (player is null || !player.active)
            {
                syncedPlayers[i] = false;
                joinSyncTimers[i] = 0;
                continue;
            }

            if (syncedPlayers[i])
                continue;

            if (++joinSyncTimers[i] < JoinSyncDelayTicks)
                continue;

            foreach (var system in Macrocosm.Instance.GetContent<ModSystem>())
            {
                if (system is IOnPlayerJoining syncable)
                    syncable.OnPlayerJoining(playerIndex: i);
            }

            syncedPlayers[i] = true;
        }
    }

    private static void ResetJoinTracking()
    {
        if (syncedPlayers is not null)
            Array.Clear(syncedPlayers, 0, syncedPlayers.Length);

        if (joinSyncTimers is not null)
            Array.Clear(joinSyncTimers, 0, joinSyncTimers.Length);
    }
}
