using Macrocosm.Common.Customization;
using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Enums;
using Macrocosm.Common.Global.Tiles;
using Macrocosm.Common.Systems;
using Macrocosm.Common.Systems.Flags;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets;
using Macrocosm.Content.Rockets.LaunchPads;
using Macrocosm.Content.Rockets.UI.Navigation.Checklist;
using Macrocosm.Content.Subworlds;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.GameContent.Events;
using Terraria.Graphics.Effects;
using Terraria.IO;
using Terraria.ModLoader.IO;

namespace Macrocosm.Common.Subworlds;

public abstract partial class MacrocosmSubworld : Subworld
{
    public string ID => Mod.Name + "/" + Name;

    #region Sublib options
    public override bool NormalUpdates => false;
    public override bool ShouldSave => true;
    public override bool NoPlayerSaving => false;
    public override int ReturnDestination => SubworldTravelPlayer.GetReturnDestination();
    #endregion

    #region Environment parameters

    /// <summary> Time rate of this subworld, compared to Earth's (1.0) </summary>
    protected virtual double TimeRate { get; } = Earth.TimeRate;

    /// <summary> The world update rate of this subworld, compared to Earth's (1.0) </summary>
    protected virtual double WorldUpdateRate { get; } = Earth.TimeRate;

    /// <summary> The tile update rate of this subworld, compared to Earth's (1.0) </summary>
    protected virtual double TileUpdateRate { get; } = Earth.TimeRate;

    /// <summary> Day length of this subworld in ticks </summary>
    protected virtual double DayLength { get; } = Earth.DayLength;

    /// <summary> Night length of this subworld in ticks </summary>
    protected virtual double NightLength { get; } = Earth.NightLength;

    /// <summary> Whether the subworld blocks enemy spawns or not </summary>
    public virtual bool PeacefulWorld { get; } = false;

    /// <summary> The gravity multiplier, measured in G (Earth has 1G) </summary>
    protected virtual float GravityMultiplier(Vector2 position) => Earth.GravityMultiplier;

    /// <summary> Get the atmospheric density at the specified position </summary>
    protected virtual float AtmosphericDensity(Vector2 position) => Earth.AtmosphericDensity;

    /// <summary> The ambient temperature, expressed in °C. Pass position only when you need temperature at a position different than the local player's </summary>
    protected virtual float AmbientTemperature(Vector2 position) => Earth.AmbientTemperature(position);

    /// <summary> Whether wiring should function in this subworld. Useful for solar storms :) </summary>
    public virtual bool ShouldUpdateWiring { get; set; } = true;

    /// <summary> Collection of LiquidIDs that should evaporate in this subworld </summary>
    public virtual int[] EvaporatingLiquidTypes => [];

    public virtual bool NoBackground => false;

    /// <summary> The map background color for each depth layer (Surface, Underground, Cavern, Underworld) </summary>
    public virtual Dictionary<MapColorType, Color> MapColors { get; } = null;

    public virtual string CustomSky { get; } = null;
    #endregion

    #region Size

    /// <summary> Determine the size of this subworld </summary>
    /// <param name="earthWorldSize"> The Earth's world size </param>
    public virtual WorldSize GetSubworldSize(WorldSize earthWorldSize) => earthWorldSize;

    public sealed override int Width => GetSubworldSize(Earth.WorldSize).Width;
    public sealed override int Height => GetSubworldSize(Earth.WorldSize).Height;

    #endregion

    #region Travel

    /// <summary> Specifies the conditions for reaching this particular subworld </summary>
    public virtual ChecklistConditionCollection LaunchConditions { get; } = new();

    #endregion

    #region Travel hooks

    /// <summary> Called when entering a subworld. </summary>
    public virtual void OnEnterSubworld() { }

    /// <summary> Called when exiting a subworld. </summary>
    public virtual void OnExitSubworld() { }

    public sealed override void OnEnter()
    {
        OnEnterSubworld();

        MapTileSystem.ApplyMapTileColors();

        if (!string.IsNullOrEmpty(CustomSky))
            SkyManager.Instance.Activate($"{Mod.Name}:{CustomSky}");
    }

    public sealed override void OnExit()
    {
        OnExitSubworld();

        MapTileSystem.RestoreMapTileColors();

        if (!string.IsNullOrEmpty(CustomSky))
            SkyManager.Instance.Deactivate($"{Mod.Name}:{CustomSky}");

        Main.LocalPlayer.GetModPlayer<SubworldTravelPlayer>().OnExitSubworld();
    }

    #endregion

    #region Common updates

    public sealed override void Update()
    {
        SubworldSystem.hideUnderworld = true;
        SubworldSystem.noReturn = false;

        UpdateTime();

        UpdateBackground();
        UpdateWeather();

        UpdateInvasions();

        UpdateWiring();
        UpdateTileEntities();
        UpdateLiquids();

        TownNPCSystem.UpdateTownNPCSpawns();
        RandomUpdateGlobalTile.RandomTileUpdate();

        CreditsRollEvent.UpdateTime();
    }

    // Updates the time 
    private void UpdateTime()
    {
        double timeRate = TimeRate;
        double worldUpdateRate = WorldUpdateRate;
        double tileUpdateRate = TileUpdateRate;

        // Fast forward 60 times if using sun/moon-dials (except tile updates)
        if (Main.IsFastForwardingTime())
        {
            timeRate *= 60.0;
            worldUpdateRate *= 60.0;
        }
        else if (CreativePowerManager.Instance.GetPower<CreativePowers.FreezeTime>().Enabled)
        {
            // Don't pass time if disabled from the journey powers 
            timeRate = 0;
            worldUpdateRate = 0;
            tileUpdateRate = 0;
        }
        else
        {
            int targetTimeRate = CreativePowerManager.Instance.GetPower<CreativePowers.ModifyTimeRate>().TargetTimeRate;
            bool allPlayersSleeping = Main.CurrentFrameFlags.SleepingPlayersCount == Main.CurrentFrameFlags.ActivePlayersCount && Main.CurrentFrameFlags.SleepingPlayersCount > 0;

            // Apply current journey power time modifier
            timeRate *= targetTimeRate;
            worldUpdateRate *= targetTimeRate;
            tileUpdateRate *= targetTimeRate;

            // Apply all players sleeping multiplier 
            if (allPlayersSleeping)
            {
                timeRate *= 5;
                worldUpdateRate *= 5;
                tileUpdateRate *= 5;
            }
        }

        // Update time
        Main.time += timeRate;
        Main.desiredWorldTilesUpdateRate = tileUpdateRate;
        Main.desiredWorldEventsUpdateRate = worldUpdateRate;

        if (Utility.IsDusk)
        {
            Main.time = 0;
            Main.dayTime = false;

            if (Main.fastForwardTimeToDusk)
                Main.fastForwardTimeToDusk = false;
        }

        if (Utility.IsDawn)
        {
            Main.time = 0;
            Main.dayTime = true;

            if (Main.fastForwardTimeToDawn)
                Main.fastForwardTimeToDawn = false;
        }
    }

    // Updates wiring, TEs and liquids 
    private void UpdateWiring()
    {
        if (!Current.ShouldUpdateWiring)
            return;

        Wiring.UpdateMech();
    }

    private void UpdateTileEntities()
    {
        TileEntity.UpdateStart();
        foreach (TileEntity te in TileEntity.ByID.Values)
        {
            te.Update();
        }
        TileEntity.UpdateEnd();
    }

    private void UpdateLiquids()
    {
        if (++Liquid.skipCount > 1)
        {
            Liquid.UpdateLiquid();
            Liquid.skipCount = 0;
        }
    }

    private void UpdateInvasions()
    {
        Main.bloodMoon = false;
        Main.pumpkinMoon = false;
        Main.snowMoon = false;
        Main.eclipse = false;
        Main.invasionType = 0;
        DD2Event.StopInvasion();
    }

    // Freezes environment variables such as rain or clouds. 
    private void UpdateWeather()
    {
        // TODO: make hooks for these if we ever use vanilla's weather system for other planets
        Main.numClouds = 0;
        Main.windSpeedCurrent = 0;
        Main.windSpeedTarget = 0;
        Main.windCounter = 0;

        Main.weatherCounter = 0;

        Main.slimeRain = false;
        Main.slimeRainTime = 0;
        Main.StopSlimeRain(false);

        LanternNight.WorldClear();

        // Rain, rain, go away, come again another day
        Main.StopRain();
    }

    private void UpdateBackground()
    {
        if (!string.IsNullOrEmpty(CustomSky) && !SkyManager.Instance[$"{Mod.Name}:{CustomSky}"].IsActive())
            SkyManager.Instance.Activate($"{Mod.Name}:{CustomSky}");
    }

    #endregion

    public override void DrawMenu(GameTime gameTime)
    {
        if (LoadingScreen is not null)
            LoadingScreen.Draw(gameTime, Main.spriteBatch);
        else
            base.DrawMenu(gameTime);
    }

    public override float GetGravity(Entity entity)
    {
        if (entity is Player player)
        {
            float gravity = Player.defaultGravity * GetGravityMultiplier(player.Center);
            if (gravity == 0f)
                gravity = float.Epsilon;
            return gravity;
        }

        // This is instead modified using the new NPC.GravityMultiplier tML property in MacrocosmGlobalNPC 
        if (entity is NPC)
            return base.GetGravity(entity);

        return base.GetGravity(entity);
    }

    #region Saving and loading

    protected virtual void PostCopyData() { }

    protected virtual void PostReadCopiedData() { }

    public override void CopySubworldData()
    {
    }

    public override void ReadCopiedSubworldData()
    {
    }

    public override void CopyMainWorldData()
    {
        CopyMacrocosmData();
        CopyVanillaData();
        PostCopyData();
    }

    public override void ReadCopiedMainWorldData()
    {
        ReadCopiedMacrocosmData();
        ReadCopiedVanillaData();
        PostReadCopiedData();
    }

    private static void CopyMacrocosmData()
    {
        TagCompound data = new();

        WorldData.SaveData(data);
        RocketManager.SaveData(data);
        LaunchPadManager.SaveData(data);
        TownNPCSystem.SaveData(data);
        PatternManager.SaveData(data);
        DecalManager.SaveData(data);

        SubworldSystem.CopyWorldData($"{nameof(Macrocosm)}:{nameof(data)}", data);
    }

    private static void ReadCopiedMacrocosmData()
    {
        TagCompound data = SubworldSystem.ReadCopiedWorldData<TagCompound>($"{nameof(Macrocosm)}:{nameof(data)}");

        WorldData.LoadData(data);
        RocketManager.LoadData(data);
        LaunchPadManager.LoadData(data);
        TownNPCSystem.LoadData(data);
        PatternManager.LoadData(data);
        DecalManager.LoadData(data);
    }

    private void CopyVanillaData()
    {
        SubworldSystem.CopyWorldData(nameof(Main.moonPhase), Main.moonPhase);
    }

    private void ReadCopiedVanillaData()
    {
        SetMoonPhase(SubworldSystem.ReadCopiedWorldData<int>(nameof(Main.moonPhase)));
    }

    private void SetMoonPhase(int moonPhase)
    {
        Main.moonPhase = moonPhase;
        typeof(WorldFile).SetFieldValue("_tempMoonPhase", moonPhase);
    }

    #endregion

    #region Update hooks

    /// <summary> 
    /// Use this if you want to do something before anything in the World gets updated.
    /// Called after UI updates, but before anything in the World (Players, NPCs, Projectiles,
    /// Tiles) gets updated.
    /// When Terraria.Main.autoPause is true or Terraria.Main.FrameSkipMode is 0 or 2,
    /// the game may do a partial update. This means that it only updates menus and some
    /// animations, but not the World or Entities. This hook - and every hook after it
    /// - only gets called on frames with a full update.
    /// </summary>
    public virtual void PreUpdateEntities() { }

    /// <summary> Called before Players get updated . </summary>
    public virtual void PreUpdatePlayers() { }

    /// <summary> Called after Players get updated . </summary>
    public virtual void PostUpdatePlayers() { }

    /// <summary> Called before NPCs get updated. </summary>
    public virtual void PreUpdateNPCs() { }

    /// <summary> Called after NPCs get updated. </summary>
    public virtual void PostUpdateNPCs() { }

    /// <summary> Called before Gores get updated. </summary>
    public virtual void PreUpdateGores() { }

    /// <summary> Called after Gores get updated. </summary>
    public virtual void PostUpdateGores() { }

    /// <summary> Called before Projectiles get updated. </summary>
    public virtual void PreUpdateProjectiles() { }

    /// <summary> Called after Projectiles get updated. </summary>
    public virtual void PostUpdateProjectiles() { }

    /// <summary> Called before Items get updated. </summary>
    public virtual void PreUpdateItems() { }

    /// <summary> Called after Items get updated. </summary>
    public virtual void PostUpdateItems() { }

    /// <summary> Called before Dusts get updated. </summary>
    public virtual void PreUpdateDusts() { }

    /// <summary> Called after Dusts get updated. </summary>
    public virtual void PostUpdateDusts() { }

    /// <summary> Called before Time gets updated. </summary>
    public virtual void PreUpdateTime() { }

    /// <summary> Called after Time gets updated. </summary>
    public virtual void PostUpdateTime() { }

    /// <summary> Called before the subworld is updated. Not called on multiplayer clients </summary>
    public virtual void PreUpdateWorld() { }

    /// <summary> Called after the subworld is updated. Not called on multiplayer clients </summary>
    public virtual void PostUpdateWorld() { }

    /// <summary> Called before Invasions get updated. Not called on multiplayer clients. </summary>
    public virtual void PreUpdateInvasions() { }

    /// <summary> Called after Invasions get updated. Not called on multiplayer clients. </summary>
    public virtual void PostUpdateInvasions() { }

    /// <summary> Called after the Network got updated, this is the last hook that happens in a subworld update. </summary>
    public virtual void PostUpdateEverything() { }

    #endregion
}
