using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.IO;
using SubworldLibrary;
using Macrocosm.Common.UI;
using Macrocosm.Content.Systems;
using Macrocosm.Content.Rockets;
using Macrocosm.Content.Subworlds;
using Macrocosm.Content.Rockets.LaunchPads;
using Macrocosm.Content.Rockets.Customization;
using Macrocosm.Content.Rockets.Navigation.Checklist;

namespace Macrocosm.Common.Subworlds
{
    internal abstract partial class MacrocosmSubworld : Subworld 
	{
		/// <summary> Time rate of this subworld, compared to Earth's (1.0) </summary>
 		public virtual double TimeRate { get; set; } = Earth.TimeRate;
		
		/// <summary> Day lenght of this subworld in ticks </summary>
 		public virtual double DayLenght { get; set; } = Earth.DayLenght;
		
		/// <summary> Night lenght of this subworld in ticks </summary>
 		public virtual double NightLenght { get; set; } = Earth.NightLenght;

		/// <summary> The gravity multiplier, measured in G (Earth has 1G) </summary>
 		public virtual float GravityMultiplier { get; set; } = Earth.GravityMultiplier;

		public virtual void SetDefaults() { }

		/// <summary> Pre Update logic for this subworld. Not called on multiplayer clients </summary>
		public virtual void PreUpdateWorld() { }

		/// <summary> Post Update logic for this subworld. Not called on multiplayer clients </summary>
		public virtual void PostUpdateWorld() { }

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

		/// <summary> The loading screen. Assign new instance in the constructor. </summary>
		protected LoadingScreen LoadingScreen;
		public override void OnEnter()
		{
			SubworldSystem.noReturn = true;
			LoadingScreen?.Setup();
			OnEnterWorld();
		}

		public override void OnExit()
		{
			SubworldSystem.noReturn = false;
			Earth.LoadingScreen.Setup();
			OnExitWorld();
		}

		public override void DrawMenu(GameTime gameTime)
		{
			if (SubworldSystem.AnyActive<Macrocosm>())
			{
				if(LoadingScreen is not null)
					LoadingScreen.Draw(Main.spriteBatch);
				else 
					base.DrawMenu(gameTime);
			}
			else
				Earth.LoadingScreen.Draw(Main.spriteBatch);
		}

		public override float GetGravity(Entity entity)
		{
			if(entity is Player)
				return Player.defaultGravity * CurrentGravityMultiplier;

			// This is set using the new NPC.GravityMultiplier property in MacrocosmGlobalNPC instead
			if(entity is NPC)
				return base.GetGravity(entity);

			return base.GetGravity(entity);
		}

		TagCompound dataCopyTag;
		public override void CopyMainWorldData()
		{
			dataCopyTag = new();

            WorldDataSystem.Instance.CopyWorldData(dataCopyTag);
			RocketManager.SaveRocketData(dataCopyTag);
			CustomizationStorage.SaveUnlockedStatus(dataCopyTag);
			LaunchPadManager.SaveLaunchPads(dataCopyTag);

			// This is to ensure the data is properly transfered by SubLib code to subservers in MP 
			SubworldSystem.CopyWorldData("Macrocosm:copiedData", dataCopyTag);
 		}

		public override void ReadCopiedMainWorldData()
		{
			// The data is sent to a MP subserver as packets, as it doesn't exist in its memory otherwise
			// For some reason, this returns wrong data in single player, possibly even in MP, using the local tag in memory
			if(Main.netMode != NetmodeID.SinglePlayer)
				dataCopyTag = SubworldSystem.ReadCopiedWorldData<TagCompound>("Macrocosm:copiedData");

			WorldDataSystem.Instance.ReadCopiedWorldData(dataCopyTag);
			RocketManager.ReadSavedRocketData(dataCopyTag);
			CustomizationStorage.LoadUnlockedStatus(dataCopyTag);
			LaunchPadManager.LoadLaunchPads(dataCopyTag);
        }

        // Should these be different?
        public override void CopySubworldData() => CopyMainWorldData();
		public override void ReadCopiedSubworldData() => ReadCopiedMainWorldData();
	}
}
