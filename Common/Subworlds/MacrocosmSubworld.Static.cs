using Macrocosm.Common.UI;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Dev;
using Macrocosm.Content.Rockets;
using Macrocosm.Content.Subworlds;
using Macrocosm.Content.UI.LoadingScreens;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using Terraria;
using Terraria.ID;

namespace Macrocosm.Common.Subworlds
{
	public enum MapColorType
	{
		SkyUpper,
		SkyLower,
		UndergroundUpper,
		UndergroundLower,
		CavernUpper,
		CavernLower,
		Underworld
	}

	public partial class MacrocosmSubworld 
	{
		/// <summary> Get the current <c>MacrocosmSubworld</c> active instance. 
		/// Earth returns null! You should check for <see cref="SubworldSystem.AnyActive"/> for <b>Macrocosm</b> before accessing this. </summary>
		public static MacrocosmSubworld Current => SubworldSystem.AnyActive<Macrocosm>() ? SubworldSystem.Current as MacrocosmSubworld : null;

		/// <summary> 
		/// Get the current active Macrocosm subworld string ID, matching the subworld class name. 
		/// Returns <c>Earth</c> if none active. 
		/// Use this for situations where we want other mods subworlds to behave like Earth.
		/// </summary>
		public static string CurrentPlanet => SubworldSystem.AnyActive<Macrocosm>() ? Current.Name : "Earth";

		/// <summary>
		/// Get the current active subworld string ID, matching the subworld class name. 
		/// If it's from another mod, not Macrocosm, returns the subworld name with the mod name prepended. 
		/// Returns <c>Earth</c> if none active.
		/// Use this for situations where other mods' subworlds will behave differently from Earth (the main world).
		/// </summary>
		public static string CurrentWorld { 
			get 
			{
				if (SubworldSystem.AnyActive<Macrocosm>())
					return Current.Name;
				else if (SubworldSystem.AnyActive())
					return SubworldSystem.Current.Mod.Name + "/" + SubworldSystem.Current.Name;
				else
					return "Earth";
			} 
		}

		public static bool IsValidWorldName(string name) => SubworldSystem.GetIndex(Macrocosm.Instance.Name + "/" + name) >= 0 || name is "Earth";

		// TODO: We could protect the original properties get them only via statics?
		public static double CurrentTimeRate => SubworldSystem.AnyActive<Macrocosm>() ? Current.TimeRate : Earth.TimeRate;
		public static double CurrentDayLenght => SubworldSystem.AnyActive<Macrocosm>() ? Current.DayLenght : Earth.DayLenght;
		public static double CurrentNightLenght => SubworldSystem.AnyActive<Macrocosm>() ? Current.NightLenght : Earth.NightLenght;
		public static float CurrentGravityMultiplier => SubworldSystem.AnyActive<Macrocosm>() ? Current.GravityMultiplier : Earth.GravityMultiplier;

		/// <summary> The loading screen. </summary>
		public static LoadingScreen LoadingScreen { get; set; }

		public static bool Travel(string targetWorld, Rocket rocket = null)
		{
			if(Main.netMode != NetmodeID.Server)
			{
				if (!SubworldSystem.AnyActive<Macrocosm>())
				{
					LoadingScreen = new EarthLoadingScreen();
				}
				else switch (Current.Name)
				{
					case "Moon": LoadingScreen = new MoonLoadingScreen(); break;
				}

				if (rocket is not null)
					LoadingScreen.SetRocket(rocket);

				if (targetWorld == "Earth")
				{
					SubworldSystem.Exit();
					LoadingScreen.SetTargetWorld("Earth");
					LoadingTitleSequence.SetTargetWorld("Earth");
					return true;
				}
			}

			string subworldId = Macrocosm.Instance.Name + "/" + targetWorld;
			bool entered = true;

			if(Main.netMode == NetmodeID.SinglePlayer)
			{
				entered = SubworldSystem.Enter(subworldId);
			} 
			else if(Main.netMode == NetmodeID.Server)
			{
				SubworldSystem.StartSubserver(SubworldSystem.GetIndex(subworldId));

				for (int i = 0; i < Main.maxPlayers; i++)
				{
					var player = Main.player[i];

					if (!player.active)
						continue;

					if(player.TryGetModPlayer(out RocketPlayer rocketPlayer))
					{
						if (rocketPlayer.InRocket && rocketPlayer.RocketID == rocket.WhoAmI)
							SubworldSystem.MovePlayerToSubworld(subworldId, i);
					}
				}
			}

			if (entered)
			{
				if(Main.netMode != NetmodeID.Server)
				{
					LoadingScreen.SetTargetWorld(targetWorld);
					LoadingTitleSequence.SetTargetWorld(targetWorld);
				}		
			}
			else
			{
				WorldTravelFailure("Error: Failed entering target subworld: " + targetWorld + ", staying on " + CurrentWorld);
			}

			return entered;
		}

		// Called if travel to the target subworld fails
		public static void WorldTravelFailure(string message)
		{
			Utility.Chat(message, Color.Red);
			Macrocosm.Instance.Logger.Error(message);
		}
	}
}
