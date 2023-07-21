using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Dev;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SubworldLibrary;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Macrocosm.Common.Netcode;
using System.Collections.Generic;
using Macrocosm.Content.Rockets.Modules;
using Terraria.Localization;
using Macrocosm.Common.Subworlds;
using System.Linq;
using Terraria.UI.Chat;
using Terraria.GameContent;

namespace Macrocosm.Content.Rockets
{
    public partial class Rocket 
	{
		[NetSync] public bool Active;


		[NetSync] public Vector2 Position;

		[NetSync] public Vector2 Velocity;


		/// <summary> Rocket sequence timer </summary>
		[NetSync] public int FlightTime;

		[NetSync] public bool InFlight;

		[NetSync] public bool Descending;


		[NetSync] public float FuelCapacity = 1000f;

		[NetSync] public string CurrentSubworld;

		public bool ActiveInCurrentSubworld => Active && CurrentSubworld == MacrocosmSubworld.CurrentSubworld;

		/// <summary> The amount of fuel stored in the rocket </summary>
		[NetSync] public float Fuel;

		public int WhoAmI = -1;

		/// <summary> The rocket's width </summary>
		public int Width = DefaultWidth;

		/// <summary> The rocket's height </summary>
		public int Height = DefaultHeight;

		/// <summary> The size of the rocket's bounds </summary>
		public Vector2 Size => new(Width, Height);

		/// <summary> The rectangle occupied by the Rocket in the world </summary>
		public Rectangle Bounds => new((int)Position.X, (int)Position.Y, Width, Height);

		/// <summary> The rocket's center in world coordinates </summary>
		public Vector2 Center
		{
			get => Position + Size/2f;
			set => Position = value - Size/2f;
		}

		/// <summary> The layer this rocket is drawn in </summary>
		public RocketDrawLayer DrawLayer = RocketDrawLayer.BeforeNPCs;

		/// <summary> The Rocket's name, set by the user, defaults to a localized "Rocket" name </summary>
		public string DisplayName
			=> EngineModule.Nameplate.HasNoSupportedChars() ? Language.GetTextValue("Mods.Macrocosm.Common.Rocket") : EngineModule.Nameplate.Text;

		/// <summary> Whether the player can interact with the rocket </summary>
		public bool InInteractionRange
		{
			get
			{
				Point location = Bounds.ClosestPointInRect(Main.LocalPlayer.Center).ToTileCoordinates();
				return Main.LocalPlayer.IsInTileInteractionRange(location.X, location.Y, TileReachCheckSettings.Simple);
			}
		}

		/// <summary> The Rocket's command pod </summary>
		public CommandPod CommandPod;

		/// <summary> The Rocket's service module </summary>
		public ServiceModule ServiceModule;

		/// <summary> The rocket's reactor module </summary>
		public ReactorModule ReactorModule;

		/// <summary> The Rocket's engine module </summary>
		public EngineModule EngineModule;

		/// <summary> The rocket's left booster </summary>
		public BoosterLeft BoosterLeft;

		/// <summary> The rocket's right booster </summary>
		public BoosterRight BoosterRight;

		/// <summary> List of all the rocket's modules, in their order found in ModuleNames </summary>
		public List<RocketModule> Modules;

		#region Private vars

		// The world Y coordinate for entering the target subworld
		private float worldExitPosY = 20 * 16f;
		
		// Acceleration values 
		private float flightAcceleration = 0.1f;   // mid-flight
		private float liftoffAcceleration = 0.05f; // during liftoff
		private float startAcceleration = 0.02f;   // initial 

		private float maxFlightSpeed = 25f;

		// Number of ticks of the launch countdown (seconds * 60 ticks/sec)
		private int liftoffTime = 180;

		// Store the initial vertical position
		private float startYPosition;

		// Gets a tick count approximation needed to reach the sky (tough to get it exact since acceleration is not constant and speed is capped) 
		private float TimeToReachSky => (liftoffTime + 60) + MathF.Sqrt(2 * (startYPosition - worldExitPosY) / flightAcceleration);

		// Approximated flight progress
		private float FlightProgress => Utils.GetLerpValue(liftoffTime, TimeToReachSky, FlightTime + 1, clamped: true);

		// The inverse value of the flight progress
		private float InvProgress => 1f - FlightProgress;

		#endregion

		/// <summary> Instatiates a rocket. Use <see cref="Create(Vector2)"/> for spawning in world and proper syncing. </summary>
		public Rocket()
		{
			CommandPod = new();
			ServiceModule = new();
			ReactorModule = new();
			EngineModule = new();
			BoosterLeft = new();
			BoosterRight = new();

			Modules = new() { CommandPod, ServiceModule, ReactorModule, EngineModule, BoosterLeft, BoosterRight };
		}

		public void OnSpawn()
		{
			CurrentSubworld = MacrocosmSubworld.CurrentSubworld;

			startYPosition = Center.Y;
		}

		/// <summary> Update the rocket </summary>
		public void Update()
		{
			Position += Velocity;

			// Testing
			Fuel = 1000f;

			SetModuleRelativePositions();

			if (!InFlight)
			{
				Interact();
				LookForCommander();

				Velocity.Y += 0.1f;
				Velocity = Collision.TileCollision(Position, Velocity, Width, Height);
			}
			else
			{
				FlightTime++;
			}

			if (FlightTime >= liftoffTime)
			{
				Movement();
				SetScreenshake();
				VisualEffects();
 			}

			if (Descending)
			{
				Velocity.Y *= 0.97f;

				if (Velocity.Length() < 0.01f)
					Descending = false;
			}

            if (InFlight && Position.Y < worldExitPosY)
			{
				InFlight = false;
				Descending = true;
				EnterDestinationSubworld();
				//Despawn();
			}
		}

		/// <summary> Safely despawn the rocket </summary>
		public void Despawn()
		{
			if(Main.netMode == NetmodeID.SinglePlayer)
			{
				if (CheckPlayerInRocket(Main.myPlayer))
				{
					GetRocketPlayer(Main.myPlayer).InRocket = false;
					GetRocketPlayer(Main.myPlayer).AsCommander = false;
				}
			}
 			else
			{
				for (int i = 0; i < Main.maxPlayers; i++)
				{
					if (CheckPlayerInRocket(i))
					{
						GetRocketPlayer(i).InRocket = false;
						GetRocketPlayer(i).AsCommander = false;
					}
				}
			}

			Active = false;
			NetSync();
		}

		/// <summary> Draw the rocket </summary>
		public void Draw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			// Positions (also) set here, in the update method only they lag behind 
			SetModuleRelativePositions();

			foreach (RocketModule module in Modules.OrderBy(module => module.DrawPriority))
			{
				module.Draw(spriteBatch, screenPos, drawColor);
			}

			//DrawDebugBounds();
			//DrawDebugModuleHitbox();
			ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.DeathText.Value, WhoAmI.ToString(), CommandPod.Center - new Vector2(0, 100) - Main.screenPosition, Color.White, 0f, Vector2.Zero, Vector2.One);
		}

		/// <summary> Draw the rocket as a dummy </summary>
		public void DrawDummy(SpriteBatch spriteBatch, Vector2 offset, Color drawColor)
		{
			// Passing Rocket world position as "screenPosition" cancels it out  
			Draw(spriteBatch, Position - offset, drawColor);
		}

		// Set the rocket's modules positions in the world
		private void SetModuleRelativePositions()
		{
			CommandPod.Position = Position + new Vector2(CommandPod.Texture.Width/4f + 1, 0);
			ServiceModule.Position = new Vector2(CommandPod.Position.X, CommandPod.Position.Y) + new Vector2(-20, CommandPod.Texture.Height - 2.1f);
			ReactorModule.Position = ServiceModule.Position + new Vector2(0, ServiceModule.Texture.Height) + new Vector2(0, -2);
			EngineModule.Position = ReactorModule.Position + new Vector2(0, ReactorModule.Texture.Height);
			BoosterLeft.Position = new Vector2(EngineModule.Center.X, EngineModule.Position.Y) - new Vector2(BoosterLeft.Texture.Width/2, 0) + new Vector2(-39, 14);
			BoosterRight.Position = new Vector2(EngineModule.Center.X, EngineModule.Position.Y)  -new Vector2(BoosterLeft.Texture.Width / 2, 0) + new Vector2(39, 14);
		}

		/// <summary> Gets the RocketPlayer bound to the provided player ID </summary>
		/// <param name="playerID"> The player ID </param>
		public RocketPlayer GetRocketPlayer(int playerID) => Main.player[playerID].RocketPlayer();

		/// <summary> Checks whether the provided player ID is on this rocket </summary>
		/// <param name="playerID"> The player ID </param>
		public bool CheckPlayerInRocket(int playerID) => Main.player[playerID].active && GetRocketPlayer(playerID).InRocket && GetRocketPlayer(playerID).RocketID == WhoAmI;

		/// <summary> Checks whether the provided player ID is a commander on this rocket </summary>
		/// <param name="playerID"> The player ID </param>
		public bool CheckPlayerCommander(int playerID) => Main.player[playerID].active && GetRocketPlayer(playerID).AsCommander && GetRocketPlayer(playerID).RocketID == WhoAmI;

		/// <summary> Checks whether this rocket has a commander </summary>
		/// <param name="playerID"> The commander player ID </param>
		public bool TryFindingCommander(out int playerID)
		{
			for (int i = 0; i < Main.maxPlayers; i++)
			{
				if (!Main.player[i].active)
					continue;

				RocketPlayer rocketPlayer = GetRocketPlayer(i);
				if (CheckPlayerInRocket(i) && rocketPlayer.AsCommander)
				{
					playerID = i;
					return true;
				}
			}

			playerID = -1;
			return false;
		}

		/// <summary> Check whether there are currently any players inside this rocket </summary>
		/// <param name="first"> The ID of the first player found in rocket, will be -1 if none found </param>
		public bool AnyEmbarkedPlayers(out int first)
		{
			if (Main.netMode == NetmodeID.SinglePlayer)
			{
				first = Main.myPlayer;
				return CheckPlayerInRocket(first);
			}

			for (int i = 0; i < Main.maxPlayers; i++)
			{
				if (!Main.player[i].active)
					continue;

				if (CheckPlayerInRocket(i))
				{
					first = i;
					return true;
				}
			}
			first = -1;
			return false;
		}

		/// <summary> Launches the rocket, with syncing </summary>
		public void Launch()
		{
			InFlight = true;
			NetSync();
		}

		/// <summary> Checks whether the flight path is obstructed by solid blocks </summary>
		// TODO: CHECK THIS AT A LOWER FREQUENCY - maybe once every second, and return a cached result otherwise
		// Will implement it in the Checklist provider rework
		public bool CheckFlightPathObstruction()
		{
			int startY = (int)((Center.Y - Height / 2) / 16);

			for (int offsetX = 0; offsetX < (Width / 16); offsetX++)
			{
				if (Utility.GetFirstTileCeiling((int)(Position.X / 16f) + offsetX, startY, solid: true) > 10)
					return false;
			}

			return true;
		}

		public bool CheckTileCollision()
		{
			foreach (RocketModule module in Modules)
				if (Math.Abs(Collision.TileCollision(module.Position, Velocity, module.Width, module.Height).Y) > 0.1f)
					return true;

			return false;
		}

		// Interaction logic
		private void Interact()
		{
			if (Main.netMode == NetmodeID.Server)
				return;

			if (MouseCanInteract() && InInteractionRange && !InFlight)
			{
				if (Main.mouseRight)
				{
					Utility.UICloseOthers();
					bool noCommanderInRocket = (Main.netMode == NetmodeID.SinglePlayer) || !TryFindingCommander(out _);
					GetRocketPlayer(Main.myPlayer).EmbarkPlayerInRocket(WhoAmI, noCommanderInRocket);
				}
				else
				{
					if(!RocketUISystem.Active)
					{
						Main.LocalPlayer.cursorItemIconEnabled = true;
						Main.LocalPlayer.cursorItemIconID = ModContent.ItemType<RocketPlacer>();
					}
				}
			}
		}

		private bool MouseCanInteract()
		{
			foreach (RocketModule module in Modules)
				if (module.Hitbox.Contains(Main.MouseWorld.ToPoint()))
					return true;

			return false;
 		}

		// Updates the commander player in real time, in multiplayer scenarios
		private void LookForCommander()
		{
			if (Main.netMode == NetmodeID.SinglePlayer)
				return;

			if (AnyEmbarkedPlayers(out int id) && !TryFindingCommander(out _))
				GetRocketPlayer(id).AsCommander = true;
		}

		// Handles the rocket's movement during flight
		private void Movement()
		{
			if (Math.Abs(Velocity.Y) > maxFlightSpeed)
				return;

			if (FlightTime >= liftoffTime + 60)
				Velocity.Y -= flightAcceleration;
			else if (FlightTime >= liftoffTime + 40)
				Velocity.Y -= liftoffAcceleration;
			else
				Velocity.Y -= startAcceleration;
		}

		// Sets the screenshake during flight 
		private void SetScreenshake()
		{
			float intenstity;

			if (FlightTime >= liftoffTime && FlightTime < liftoffTime + 5)
				intenstity = 80f;
			if (FlightTime >= liftoffTime + 5 && FlightTime < liftoffTime + 40)
				intenstity = 40f;
			if (FlightTime >= liftoffTime + 20 && FlightTime < liftoffTime + 60)
				intenstity = 20f;
			else
				intenstity = 15f * InvProgress;

			Main.LocalPlayer.AddScreenshake(intenstity, "RocketFlight");
		}

		// Handle visuals (dusts, particles)
		private void VisualEffects()
		{
			int dustCnt = FlightTime > liftoffTime + 40 ? 10 : 4;
 			for (int i = 0; i < dustCnt; i++)
			{
				Dust dust = Dust.NewDustDirect(Position + new Vector2(-10, Height - 15 - 50 * FlightProgress), (int)((float)Width + 20), 1, DustID.Flare, Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(20f, 200f) * FlightProgress, Scale: Main.rand.NextFloat(1.5f, 3f));
				dust.noGravity = false;
			}
 			
			for (int g = 0; g < 3; g++)
			{
				if (Main.rand.NextBool(2))
				{
					int type = Main.rand.NextFromList<int>(GoreID.Smoke1, GoreID.Smoke2, GoreID.Smoke3);
					Gore.NewGore(null, Position + new Vector2(Main.rand.Next(-25, Width), Height - 15 - 30 * FlightProgress), new Vector2(Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(0, 8f)), type);
				}
			}
		}

		// Handles the subworld transfer on each client, locally
		private void EnterDestinationSubworld()
		{
			if (Main.netMode == NetmodeID.Server)
				return;

			RocketPlayer commander;
			if (Main.netMode == NetmodeID.SinglePlayer)
 				commander = GetRocketPlayer(Main.myPlayer);
 			else  
			{
				if (TryFindingCommander(out int id))
					commander = GetRocketPlayer(id);
				else if (AnyEmbarkedPlayers(out id))
					commander = GetRocketPlayer(id);
				else
					return;
 			}

			if (CheckPlayerInRocket(Main.myPlayer))
			{
				CurrentSubworld = commander.TargetSubworldID;
				//NetSync();

				if (commander.TargetSubworldID == "Earth")
					SubworldSystem.Exit();
				else if (commander.TargetSubworldID != null && commander.TargetSubworldID != "")
					SubworldSystem.Enter(Macrocosm.Instance.Name + "/" + commander.TargetSubworldID);
			}
		}
	}
}
