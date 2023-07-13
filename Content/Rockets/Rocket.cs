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
using Macrocosm.Content.Rockets.Navigation;
using System.Collections.Generic;
using Macrocosm.Content.Rockets.Modules;
using Terraria.Localization;

namespace Macrocosm.Content.Rockets
{
    public partial class Rocket
	{
		[NetSync] public Vector2 Position;

		[NetSync] public Vector2 Velocity;

		[NetSync] public bool Active;

		/// <summary> Rocket sequence timer </summary>
		[NetSync] public int FlightTime;

		[NetSync] public bool Launching;

		[NetSync] public float FuelCapacity = 1000f;

		/// <summary> The amount of fuel stored in the rocket </summary>
		public float Fuel
		{
			get => fuel;
			set => fuel = MathHelper.Clamp(value, 0, FuelCapacity);
		}

		[NetSync] private float fuel;

		public int WhoAmI => RocketManager.Rockets.IndexOf(this);

		/// <summary> The rocket's width </summary>
		public int Width = DefaultWidth;

		/// <summary> The rocket's height </summary>
		public int Height = DefaultHeight;

		/// <summary> The size of the rocket's "hitbox" </summary>
		public Vector2 Size => new(Width, Height);

		/// <summary> The rocket's "hitbox" in the world, for interaction purposes </summary>
		public Rectangle Hitbox => new((int)Position.X, (int)Position.Y, Width, Height);

		/// <summary> The rocket's center in world coordinates </summary>
		public Vector2 Center
		{
			get => Position + Size * 0.5f;
			set => Position = value - Size * 0.5f;
		}

		/// <summary> The layer this rocket is drawn in </summary>
		public RocketDrawLayer DrawLayer = RocketDrawLayer.BeforeProjectiles;

		/// <summary> The Rocket's flavor name, set by the user, defaults to a localized "Rocket" name </summary>
		public string DisplayName
			=> EngineModule.Nameplate.HasNoSupportedChars() ? Language.GetTextValue("Mods.Macrocosm.Common.Rocket") : EngineModule.Nameplate.Text;

		/// <summary> Whether the player can interact with the rocket </summary>
		public bool InInteractionRange
		{
			get
			{
				Point location = Hitbox.ClosestPointInRect(Main.LocalPlayer.Center).ToTileCoordinates();
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

		// Not sure whether to keep this here or wrap it inside the EngineModule.
		// The boosters are built with the Engine Module, but they are customized on their own.
		/// <summary> The rocket's boosters </summary>
		public Boosters Boosters;

		/// <summary> List of all the rocket's modules </summary>
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
			startYPosition = Center.Y;

			CommandPod = new();
			ServiceModule = new();
			ReactorModule = new();
			EngineModule = new();
			Boosters = new();

			Modules = new()
			{
				EngineModule,
				Boosters,
				ReactorModule,
				ServiceModule,
				CommandPod
			};
		}

		/// <summary> Update the rocket </summary>
		public void Update()
		{
			Position += Velocity;

			Fuel = 1000f;

			SetModuleRelativePositions();

			SetDrawLayer(); 

			if (!Launching)
			{
				Interact();
				LookForCommander();
				return;
			}

			FlightTime++;

			if (FlightTime >= liftoffTime)
			{
				Movement();
				SetScreenshake();
				VisualEffects();
 			}

			if(Position.Y < worldExitPosY)
			{
				EnterDestinationSubworld();
				Despawn();
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
		}

		// Draw the rocket modules
		public void Draw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			// Positions set here, in the update method they lag behind 
			SetModuleRelativePositions();

			foreach (RocketModule module in Modules)
			{
				module.Draw(spriteBatch, screenPos, drawColor);
			}
		}

		// Set the rocket's modules positions in the world
		private void SetModuleRelativePositions()
		{
			CommandPod.Position = Position + new Vector2(CommandPod.Texture.Width/2, 0f);
			ServiceModule.Position = CommandPod.Position + new Vector2(-2, CommandPod.Texture.Height - 3f);
			ReactorModule.Position = ServiceModule.Position + new Vector2(0, ServiceModule.Texture.Height) + new Vector2(0, -2);
			EngineModule.Position = ReactorModule.Position + new Vector2(0, ReactorModule.Texture.Height);
			Boosters.Position = EngineModule.Position + new Vector2(0, 16);
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
			Launching = true;
			NetSync();
		}

		/// <summary> Checks whether the flight path is obstructed by solid blocks </summary>
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

		// Make the rocket draw over players while players are embarked 
		private void SetDrawLayer()
		{
			if (AnyEmbarkedPlayers(out _))
				DrawLayer = RocketDrawLayer.AfterProjectiles;
			else
				DrawLayer = RocketDrawLayer.BeforeProjectiles;
		}

		// Interaction logic
		private void Interact()
		{
			if (Main.netMode == NetmodeID.Server)
				return;

			bool hoveringMouse = Hitbox.Contains(Main.MouseWorld.ToPoint());

			if (hoveringMouse && InInteractionRange && !Launching)
			{
				if (Main.mouseRight)
				{
					Utility.UICloseOthers();

					bool noCommanderInRocket = (Main.netMode == NetmodeID.SinglePlayer) || !TryFindingCommander(out _);
					GetRocketPlayer(Main.myPlayer).InRocket = true;
					GetRocketPlayer(Main.myPlayer).AsCommander = noCommanderInRocket;
					GetRocketPlayer(Main.myPlayer).RocketID = WhoAmI;

					// Interaction is done locally. Send embark status to the server.
					if (Main.netMode == NetmodeID.MultiplayerClient)
						SendEmbarkedPlayer(Main.myPlayer, noCommanderInRocket);
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

		// Updates the commander player in real time, in multiplayer scenarios
		private void LookForCommander()
		{
			if (Main.netMode == NetmodeID.SinglePlayer)
				return;

			if (!TryFindingCommander(out _) && AnyEmbarkedPlayers(out int id))
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
				if (commander.TargetSubworldID == "Earth")
					SubworldSystem.Exit();
				else if (commander.TargetSubworldID != null && commander.TargetSubworldID != "")
					SubworldSystem.Enter(Macrocosm.Instance.Name + "/" + commander.TargetSubworldID);
			}
		}
	}
}
