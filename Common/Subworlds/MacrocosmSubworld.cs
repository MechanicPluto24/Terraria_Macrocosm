using Macrocosm.Common.Systems;
using Macrocosm.Common.UI;
using Macrocosm.Content.Rockets;
using Macrocosm.Content.Rockets.Customization;
using Macrocosm.Content.Rockets.LaunchPads;
using Macrocosm.Content.Rockets.Navigation.Checklist;
using Macrocosm.Content.Subworlds;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;
using static Terraria.WorldGen;

namespace Macrocosm.Common.Subworlds
{
    public abstract partial class MacrocosmSubworld : Subworld 
	{
		/// <summary> Time rate of this subworld, compared to Earth's (1.0) </summary>
		public virtual double TimeRate { get; set; } = Earth.TimeRate;
		
		/// <summary> Day lenght of this subworld in ticks </summary>
 		public virtual double DayLenght { get; set; } = Earth.DayLenght;
		
		/// <summary> Night lenght of this subworld in ticks </summary>
 		public virtual double NightLenght { get; set; } = Earth.NightLenght;

		/// <summary> The gravity multiplier, measured in G (Earth has 1G) </summary>
 		public virtual float GravityMultiplier { get; set; } = Earth.GravityMultiplier;

		/// <summary> Determine the size of this subworld </summary>
		/// <param name="earthWorldSize"> The Earth's world size </param>
		public virtual WorldSize GetWorldSize(WorldSize earthWorldSize) 
		{ 
			return earthWorldSize; 
		}	

		/// <summary> The width is determined in ReadCopiedMainWorldData using GetWorldSize </summary>
		public sealed override int Width => GetWorldSize(Earth.WorldSize).Width;

		/// <summary> The height is determined in ReadCopiedMainWorldData using GetWorldSize </summary>
		public sealed override int Height => GetWorldSize(Earth.WorldSize).Height;


		/// <summary> Modifiy color of the skies (applied to background, tiles, etc.) </summary>
		public virtual void ModifyColorOfTheSkies(ref Color colorOfTheSkies) { }

		/// <summary> Specifies the conditions for reaching this particular subworld </summary>
		public virtual ChecklistConditionCollection LaunchConditions { get; } = new();

		/// <summary> Called when entering a subworld. </summary>
		public virtual void OnEnterWorld() { }

		/// <summary> Called when exiting a subworld. </summary>
		public virtual void OnExitWorld() { }

		/// <summary> The map background color for each depth layer (Surface, Underground, Cavern, Underworld) </summary>
		public virtual Dictionary<MapColorType, Color> MapColors { get; } = null;

		public override void OnEnter()
		{
			SubworldSystem.noReturn = true;
			OnEnterWorld();
			LoadingScreen?.Setup();
		}

		public override void OnExit()
		{
			SubworldSystem.noReturn = false;
			OnExitWorld();
			LoadingScreen?.Setup();
		}

		public override void DrawMenu(GameTime gameTime)
		{
			if (LoadingScreen is not null)
				LoadingScreen.Draw(gameTime, Main.spriteBatch);
			else
				base.DrawMenu(gameTime);
		}

		public override float GetGravity(Entity entity)
		{
			if(entity is Player)
				return Player.defaultGravity * CurrentGravityMultiplier;

			// This is set using the new NPC.GravityMultiplier tML property in MacrocosmGlobalNPC instead
			if(entity is NPC)
				return base.GetGravity(entity);

			return base.GetGravity(entity);
		}

		private static void SaveData(TagCompound tag)
		{
			WorldDataSystem.Instance.SaveData(tag);
			RocketManager.SaveData(tag);
			LaunchPadManager.SaveData(tag);
			CustomizationStorage.SaveData(tag);
		}

		private static void LoadData(TagCompound tag)
		{
			WorldDataSystem.Instance.LoadData(tag);
			RocketManager.LoadData(tag);
			LaunchPadManager.LoadData(tag);
			CustomizationStorage.LoadData(tag);
		}

		TagCompound subworldDataTag;
		TagCompound mainWorldDataTag;
		public override void CopySubworldData()
		{
			subworldDataTag = new();

			SaveData(subworldDataTag);

			// This is to ensure the data is properly transfered by SubLib code to subservers in MP 
			SubworldSystem.CopyWorldData("Macrocosm:subworldDataTag", subworldDataTag);
		}

		public override void ReadCopiedSubworldData()
		{
			// The data is sent to a MP subserver as packets, and read here, as it doesn't exist in its memory otherwise
			// For some reason, this read returns wrong data when travelling from a subworld to the main world,
			//	in single player, possibly even in MP.
			// For single player, using the local tag in memory instead, awaiting SubLib fix
			if (Main.netMode != NetmodeID.SinglePlayer)
				subworldDataTag = SubworldSystem.ReadCopiedWorldData<TagCompound>("Macrocosm:subworldDataTag");

			// In SP they are read directly from the subworldDataCopyTag in memory
			LoadData(subworldDataTag);
		}

		public override void CopyMainWorldData()
		{
			mainWorldDataTag = new();

			SaveData(mainWorldDataTag);

			// Save Earth's world size for other subworlds to use 
			mainWorldDataTag[nameof(Earth.WorldSize)] = Earth.WorldSize;

			SubworldSystem.CopyWorldData("Macrocosm:mainWorldDataTag", mainWorldDataTag);
		}

		public override void ReadCopiedMainWorldData()
		{
			if (Main.netMode != NetmodeID.SinglePlayer)
				mainWorldDataTag = SubworldSystem.ReadCopiedWorldData<TagCompound>("Macrocosm:mainWorldDataTag");

			LoadData(mainWorldDataTag);	

			// Read world size and apply it here. 
			// In SubLib maxTiles are assigned before the data is read.
			// This is called before worldgen so it's safe to assign them here 
			if (mainWorldDataTag.ContainsKey(nameof(Earth.WorldSize)))
			{
				Earth.WorldSize = mainWorldDataTag.Get<WorldSize>(nameof(Earth.WorldSize));
				WorldSize subworldSize = GetWorldSize(Earth.WorldSize);
				Main.maxTilesX = subworldSize.Width;
				Main.maxTilesY = subworldSize.Height;
			}
		}

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

		/// <summary> Called before Invasions get updated. Not called for multiplayer clients. </summary>
		public virtual void PreUpdateInvasions() { }

		/// <summary> Called after Invasions get updated. Not called for multiplayer clients. </summary>
		public virtual void PostUpdateInvasions() { }

		/// <summary> Called after the Network got updated, this is the last hook that happens in a subworld update.
		/// </summary>
		public virtual void PostUpdateEverything() { }
	}
}
