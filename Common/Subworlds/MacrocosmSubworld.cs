using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using SubworldLibrary;
using Macrocosm.Content.Subworlds;
using Macrocosm.Content.Rockets.Navigation.Checklist;
using Macrocosm.Common.UI;
using Macrocosm.Content.Systems;
using Macrocosm.Content.Rockets;
using Terraria.Utilities;

namespace Macrocosm.Common.Subworlds
{
    public abstract partial class MacrocosmSubworld : Subworld, IModType
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
			LoadingScreen?.Setup();
			OnEnterWorld();
		}

		public override void OnExit()
		{
			Earth.LoadingScreen.Setup();
			OnExitWorld();
		}

		public override void DrawMenu(GameTime gameTime)
		{
			if (AnyActive)
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

		public override void CopyMainWorldData()
		{
			WorldDataSystem.Instance.CopyMainWorldData();
			RocketManager.CopyRocketData();
		}

		public override void ReadCopiedMainWorldData() 
		{
			WorldDataSystem.Instance.ReadCopiedMainWorldData();
			RocketManager.ReadCopiedRocketData();
		}

		public override void CopySubworldData() 
		{
			WorldDataSystem.Instance.CopySubworldData();
			RocketManager.CopyRocketData();
		}

		public override void ReadCopiedSubworldData() 
		{
			WorldDataSystem.Instance.ReadCopiedSubworldData();
			RocketManager.ReadCopiedRocketData();
		}
	}
}
