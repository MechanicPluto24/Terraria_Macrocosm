using System;
using System.Linq;
using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Netcode;
using Macrocosm.Common.Systems.Flags;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets.Modules;
using Macrocosm.Content.Rockets.Modules.Top;
using Macrocosm.Common.Subworlds;
using Macrocosm.Content.Rockets.LaunchPads;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace Macrocosm.Content.Rockets;

public partial class Rocket
{
    public enum UnmannedMissionType
    {
        CreateSpaceStation
    }

    private const int UnmannedPreLaunchDuration = 120;
    private const int UnmannedObstructionTimeoutTicks = 30 * 60; //30s timeout then force land
    public const int DefaultUnmannedMissionDurationTicks = 30 * 60; // 30 seconds

    [NetSync] private bool unmannedMissionActive;
    [NetSync] private int unmannedMissionTicksRemaining;
    [NetSync] private string unmannedMissionParentId = "";
    [NetSync] private string unmannedMissionOrbitId = "";
    [NetSync] private byte unmannedMissionType;
    [NetSync] private Vector2 unmannedMissionReturnPos;

    private int unmannedSequenceTimer;
    private int unmannedObstructionTimer;

    public bool HasUnmannedMission => unmannedMissionActive;
    public bool ReservesLaunchpad => unmannedMissionReturnPos != default && (HasUnmannedMission || State is ActionState.Suspended or ActionState.UnmannedLaunch or ActionState.UnmannedFlight or ActionState.UnmannedLanding);
    public Vector2 ReservedLaunchpadCenter => unmannedMissionReturnPos;

    private LaunchPad ReservedLaunchPad => LaunchPadManager.GetLaunchPads(unmannedMissionParentId).FirstOrDefault(lp => lp.ReservedRocketID == WhoAmI || lp.CenterWorld == unmannedMissionReturnPos);

    public void StartUnmannedMission(string parentId, string orbitId, Vector2 returnWorldPosition, int durationTicks, UnmannedMissionType type)
    {
        if (HasUnmannedMission)
            return;

        unmannedMissionActive = true;
        unmannedMissionParentId = parentId;
        unmannedMissionOrbitId = orbitId ?? "";
        unmannedMissionTicksRemaining = durationTicks;
        unmannedMissionType = (byte)type;
        unmannedMissionReturnPos = returnWorldPosition;
        if (string.IsNullOrEmpty(unmannedMissionOrbitId) && type == UnmannedMissionType.CreateSpaceStation)
            unmannedMissionOrbitId = OrbitSubworld.GetFirstLockedForParent(parentId)?.ID ?? "";
        unmannedSequenceTimer = 0;
        unmannedObstructionTimer = 0;

        SyncCommonData();
    }

    public void TickUnmannedMission()
    {
        if (!HasUnmannedMission)
            return;
        if (State != ActionState.Suspended)
            return;

        if (unmannedMissionTicksRemaining > 0)
            unmannedMissionTicksRemaining--;

        if (unmannedMissionTicksRemaining <= 0)
            StartUnmannedReturn();
    }

    public bool TryStartUnmannedOrbitMission(string parentSubworldId, Vector2 launchPadCenterWorld, int durationTicks)
    {
        // unmanned iff no module explicitly Manned
        bool isUnmanned = true;
        if (Modules != null)
        {
            foreach (var m in Modules)
            {
                if (m.Configuration == RocketModule.ConfigurationType.Manned)
                {
                    isUnmanned = false;
                    break;
                }
            }
        }
        if (!isUnmanned)
            return false;

        StartUnmannedMission(parentSubworldId, orbitId: null, returnWorldPosition: launchPadCenterWorld, durationTicks: durationTicks, UnmannedMissionType.CreateSpaceStation);
        var launchPad = LaunchPadManager.GetLaunchPads(parentSubworldId).FirstOrDefault(lp => lp.CenterWorld == launchPadCenterWorld);
        if (launchPad is not null)
        {
            launchPad.ReserveRocket(WhoAmI);
            launchPad.NetSync(parentSubworldId);
        }
        StartUnmannedLaunch();
        return true;
    }

    public void StartUnmannedLaunch()
    {
        State = ActionState.UnmannedLaunch;
        FlightTime = 0;
        unmannedSequenceTimer = 0;
        unmannedObstructionTimer = 0;
        StartPositionY = Position.Y;
        Transparency = 1f;
        SyncCommonData();
    }

    public void StartUnmannedReturn()
    {
        if (!HasUnmannedMission || State != ActionState.Suspended)
            return;

        State = ActionState.UnmannedLanding;
        FlightTime = 0;
        unmannedSequenceTimer = 0;
        unmannedObstructionTimer = 0;
        LandingProgress = 0f;
        TargetTravelPosition = unmannedMissionReturnPos;
        Position = new Vector2(TargetTravelPosition.X - Width / 2f - 8f, FlightExitPosition);
        Transparency = 1f;
        ResetRenderTarget();
        SyncEverything();
    }

    private bool LaunchPadAreaClear()
    {
        Vector2 targetPos = unmannedMissionReturnPos - new Vector2(Width / 2f - 8, Height - 16);
        var futureBounds = new RotatedRectangle(targetPos, Size / 2f, Width, Height, 0f);
        for (int i = 0; i < RocketManager.MaxRockets; i++)
        {
            var r = RocketManager.Rockets[i];
            if (r == this || !r.ActiveInCurrentWorld)
                continue;
            if (futureBounds.Intersects(r.Bounds))
                return false;
        }
        return true;
    }

    public void TryCompleteUnmannedMission()
    {
        if (!HasUnmannedMission || State != ActionState.UnmannedLanding || LandingProgress < 1f - float.Epsilon)
            return;
        if (!LaunchPadAreaClear())
            return;

        Position = unmannedMissionReturnPos - new Vector2(Width / 2f - 8, Height - 16);
        Transparency = 1f;
        State = ActionState.Idle;
        ResetRenderTarget();

        int topIndex = (int)RocketModule.SlotType.Top;
        var topModule = Modules[topIndex];
        if (topModule is PayloadPod)
            topModule.IsBlueprint = true;

        if (!string.IsNullOrEmpty(unmannedMissionOrbitId))
            OrbitSubworld.Unlock(unmannedMissionOrbitId);

        if (ReservedLaunchPad is LaunchPad reservedLaunchPad)
        {
            reservedLaunchPad.ClearReservedRocket(WhoAmI);
            reservedLaunchPad.NetSync(unmannedMissionParentId);
        }

        Utility.Chat("Unmanned mission complete. Space station unlocked!", Color.LightSkyBlue);

        unmannedMissionActive = false;
        unmannedMissionTicksRemaining = 0;
        unmannedMissionParentId = "";
        unmannedMissionOrbitId = "";
        unmannedMissionReturnPos = Vector2.Zero;
        SyncCommonData();
        ResetAnimation();
    }
}
