using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Enums;
using Macrocosm.Common.Players;
using Macrocosm.Common.Systems;
using Macrocosm.Content.Rockets;
using Macrocosm.Content.Rockets.Customization;
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
using Terraria.ModLoader.IO;

namespace Macrocosm.Common.Subworlds
{
    public abstract partial class MacrocosmSubworld : Subworld
    {
        public string ID => Mod.Name + "/" + Name;

        public override void SetStaticDefaults()
        {
            Subworlds.Add(ID, this);
        }

        #region Sublib options

        public override bool NormalUpdates => false;
        public override bool ShouldSave => true;
        public override bool NoPlayerSaving => false;

        #endregion

        #region Environment parameters

        /// <summary> Time rate of this subworld, compared to Earth's (1.0) </summary>
        public virtual double TimeRate { get; } = Earth.TimeRate;

        /// <summary> Day length of this subworld in ticks </summary>
        public virtual double DayLength { get; } = Earth.DayLength;

        /// <summary> Night length of this subworld in ticks </summary>
        public virtual double NightLength { get; } = Earth.NightLength;

        /// <summary> The gravity multiplier, measured in G (Earth has 1G) </summary>
        public virtual float GravityMultiplier { get; } = Earth.GravityMultiplier;

        /// <summary> 
        /// The atmospheric density, in terms of Earth's atmospheric density.
        /// 0f means vacuum, 1f means equal to Earth's, >1f means higher that Earth's.
        /// </summary>
        public virtual float AtmosphericDensity { get; } = Earth.AtmosphericDensity;

        /// <summary> The ambient temperature, expressed in °C </summary>
        public virtual float GetAmbientTemperature() => Earth.GetAmbientTemperature();

        /// <summary> Whether wiring should function in this subworld. Useful for solar storms :) </summary>
        public virtual bool ShouldUpdateWiring { get; set; } = true;

        /// <summary> Collection of LiquidIDs that should evaporate in this subworld </summary>
        public virtual int[] EvaporatingLiquidTypes => [];

        /// <summary> The map background color for each depth layer (Surface, Underground, Cavern, Underworld) </summary>
        public virtual Dictionary<MapColorType, Color> MapColors { get; } = null;
        #endregion

        #region Size

        /// <summary> Determine the size of this subworld </summary>
        /// <param name="earthWorldSize"> The Earth's world size </param>
        public virtual WorldSize GetSubworldSize(WorldSize earthWorldSize)
        {
            return earthWorldSize;
        }

        /// <summary> The width is determined in ReadCopiedMainWorldData using <see cref="GetSubworldSize(WorldSize)"> </summary>
        public sealed override int Width => GetSubworldSize(Earth.WorldSize).Width;

        /// <summary> The height is determined in ReadCopiedMainWorldData using <see cref="GetSubworldSize(WorldSize)"> </summary>
        public sealed override int Height => GetSubworldSize(Earth.WorldSize).Height;

        #endregion

        #region Travel

        /// <summary> Specifies the conditions for reaching this particular subworld </summary>
        public virtual ChecklistConditionCollection LaunchConditions { get; } = new();

        /// <summary> 
        /// Determines what <see cref="SubworldSystem.Exit"/> will do. 
        /// <br> If travelling using conventional methods, where <see cref="MacrocosmPlayer.TriggeredSubworldTravel"/> is set, will return to the main world (Earth). </br>
        /// <br> Otherwise, return to the main menu. This is used when clicking "Return" from the in-game settings menu, while in a subworld. </br>
        /// </summary>
        public override int ReturnDestination
        {
            get
            {
                // Return to the main world (Earth)
                //if (Main.LocalPlayer.GetModPlayer<SubworldTravelPlayer>().TriggeredSubworldTravel)
                return base.ReturnDestination;
                // Go to main menu
                //else
                //return int.MinValue;
            }
        }

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
        }

        public sealed override void OnExit()
        {
            OnExitSubworld();
            MapTileSystem.RestoreMapTileColors();
            Main.LocalPlayer.GetModPlayer<SubworldTravelPlayer>().OnExitSubworld();
        }

        #endregion

        #region Common updates

        public sealed override void Update()
        {
            SubworldSystem.hideUnderworld = true;
            SubworldSystem.noReturn = false;

            UpdateTime();

            UpdateWeather();
            UpdateInvasions();

            UpdateWiring();
            UpdateTileEntities();
            UpdateLiquids();

            CreditsRollEvent.UpdateTime();
        }

        // Updates the time 
        private void UpdateTime()
        {
            double timeRate = TimeRate;

            // Fast forward 60 times if using sun/moon-dials
            if (Main.IsFastForwardingTime())
                timeRate *= 60.0;

            // Apply current journey power time modifier
            timeRate *= CreativePowerManager.Instance.GetPower<CreativePowers.ModifyTimeRate>().TargetTimeRate;

            // Apply all players sleeping multiplier 
            if (Main.CurrentFrameFlags.SleepingPlayersCount == Main.CurrentFrameFlags.ActivePlayersCount && Main.CurrentFrameFlags.SleepingPlayersCount > 0)
                timeRate *= 5;

            // Don't pass time if disabled from the journey powers 
            if (CreativePowerManager.Instance.GetPower<CreativePowers.FreezeTime>().Enabled)
                timeRate = 0;

            // Update time
            Main.time += timeRate;

            // Set update rates
            Main.desiredWorldTilesUpdateRate = timeRate / 60.0;
            Main.desiredWorldEventsUpdateRate = timeRate;

            // We don't want slower updates if the time rate is less than Earth
            // TODO: adjust this for subworlds with a faster rate than Earth, while still letting vanilla time speedups increase the update rate
            if (timeRate < 1.0)
                Main.worldEventUpdates = 1;

            MacrocosmWorld.IsDusk = Main.dayTime && Main.time >= DayLength;
            MacrocosmWorld.IsDawn = !Main.dayTime && Main.time >= NightLength;

            if (MacrocosmWorld.IsDusk)
            {
                Main.time = 0;
                Main.dayTime = false;

                if (Main.fastForwardTimeToDusk)
                    Main.fastForwardTimeToDusk = false;
            }

            if (MacrocosmWorld.IsDawn)
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
            // TODO: make these per-subworld if using Terraria's weather system for future planets
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
            if (entity is Player)
                return Player.defaultGravity * CurrentGravityMultiplier;

            // This is instead modified using the new NPC.GravityMultiplier tML property in MacrocosmGlobalNPC 
            if (entity is NPC)
                return base.GetGravity(entity);

            return base.GetGravity(entity);
        }

        #region Saving and loading

        private void SaveData(TagCompound tag)
        {
            WorldFlags.SaveData(tag);
            RocketManager.SaveData(tag);
            LaunchPadManager.SaveData(tag);
            CustomizationStorage.SaveData(tag);
            TownNPCSystem.SaveData(tag);
        }

        private void LoadData(TagCompound tag)
        {
            WorldFlags.LoadData(tag);
            RocketManager.LoadData(tag);
            LaunchPadManager.LoadData(tag);
            CustomizationStorage.LoadData(tag);
            TownNPCSystem.LoadData(tag);
        }

        public override void CopySubworldData()
        {
            TagCompound subworldDataTag = new();
            SaveData(subworldDataTag);
            SubworldSystem.CopyWorldData("Macrocosm:subworldDataTag", subworldDataTag);
        }

        public override void ReadCopiedSubworldData()
        {
            TagCompound subworldDataTag = SubworldSystem.ReadCopiedWorldData<TagCompound>("Macrocosm:subworldDataTag");
            LoadData(subworldDataTag);
        }

        public override void CopyMainWorldData()
        {
            TagCompound mainWorldDataTag = new();
            SaveData(mainWorldDataTag);
            SaveEarthSpecificData(mainWorldDataTag);
            SubworldSystem.CopyWorldData("Macrocosm:mainWorldDataTag", mainWorldDataTag);
        }

        public override void ReadCopiedMainWorldData()
        {
            TagCompound mainWorldDataTag = SubworldSystem.ReadCopiedWorldData<TagCompound>("Macrocosm:mainWorldDataTag");
            LoadData(mainWorldDataTag);
            LoadEarthSpecificData(mainWorldDataTag);
        }

        private void SaveEarthSpecificData(TagCompound tag)
        {
            if (SubworldSystem.AnyActive() && !SubworldSystem.AnyActive<Macrocosm>())
                return;

            // Save Earth's world size for other subworlds to use 
            tag[nameof(Earth) + nameof(Earth.WorldSize)] = Earth.WorldSize;
        }

        private void LoadEarthSpecificData(TagCompound tag)
        {
            if (SubworldSystem.AnyActive() && !SubworldSystem.AnyActive<Macrocosm>())
                return;

            // Read world size and apply it here. 
            // In SubLib maxTiles are assigned before the data is read.
            // ReadCopiedMainWorldData is called before worldgen so it can be safely used there.
            if (tag.ContainsKey(nameof(Earth) + nameof(Earth.WorldSize)))
            {
                Earth.WorldSize = tag.Get<WorldSize>(nameof(Earth) + nameof(Earth.WorldSize));
                WorldSize subworldSize = GetSubworldSize(Earth.WorldSize);
                Main.maxTilesX = subworldSize.Width;
                Main.maxTilesY = subworldSize.Height;
            }
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

        /// <summary>  </summary>
        public virtual void RandomTileUpdate(int i, int j, bool underground) { }

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
}
