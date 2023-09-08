using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Netcode;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.UI;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Dev;
using Macrocosm.Content.Particles;
using Macrocosm.Content.Rockets.Customization;
using Macrocosm.Content.Rockets.LaunchPads;
using Macrocosm.Content.Rockets.Modules;
using Macrocosm.Content.Subworlds;
using Macrocosm.Content.UI.LoadingScreens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SubworldLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets
{
	public partial class Rocket 
	{
		/// <summary> The rocket's identifier </summary>
		public int WhoAmI = -1;

		/// <summary> Whether the rocket is currently active </summary>
		[NetSync] public bool Active;

		/// <summary> The rocket's top-left coodrinates in the world </summary>
		[NetSync] public Vector2 Position;

		/// <summary> The rocket's velocity </summary>
		[NetSync] public Vector2 Velocity;

		/// <summary> The rocket's sequence timer </summary>
		[NetSync] public int FlightTime;

		/// <summary> Whether the rocket has been launched </summary>
		[NetSync] public bool Launched;

		/// <summary> Whether the rocket is landing </summary>
		[NetSync] public bool Landing;

		/// <summary> The initial vertical position </summary>
		[NetSync] public float StartPositionY;

		/// <summary> The target landing position </summary>
		[NetSync] public Vector2 TargetLandingPosition;

		/// <summary> The amount of fuel currently stored in the rocket, as an absolute value </summary>
		[NetSync] public float Fuel;

		/// <summary> The rocket's fuel capacity as an absoulute value </summary>
		[NetSync] public float FuelCapacity = 1000f;

		/// <summary> The rocket's current world, "Earth" if active and not in a subworld. Other mod's subworlds have the mod name prepended </summary>
		[NetSync] public string CurrentWorld;

		/// <summary> Whether the rocket is active in the current world and should be updated and visible </summary>
		public bool ActiveInCurrentWorld => Active && CurrentWorld == MacrocosmSubworld.CurrentWorld;

		/// <summary> Number of ticks of the launch countdown (seconds * 60 ticks/sec) </summary>
		public int LiftoffTime = 180;

		public float MaxFlightSpeed = 25f;

		/// <summary> Whether this rocket is currently in flight </summary>
		public bool InFlight => /*Launched && */FlightTime >= LiftoffTime;

		/// <summary> The world Y coordinate for entering the target subworld </summary>
		private const float WorldExitPositionY = 60 * 16f;

		/// <summary> The rocket's bounds width </summary>
		public const int Width = 276;

		/// <summary> The rocket's bounds height </summary>
		public const int Height = 594;

		/// <summary> The size of the rocket's bounds </summary>
		public static Vector2 Size => new(Width, Height);

		/// <summary> The rectangle occupied by the Rocket in the world </summary>
		public Rectangle Bounds => new((int)Position.X, (int)Position.Y, Width, Height);

		/// <summary> The rocket's center in world coordinates </summary>
		public Vector2 Center
		{
			get => Position + Size/2f;
			set => Position = value - Size/2f;
		}

		/// <summary> Whether the rocket is currently stationary </summary>
		public bool Stationary => Velocity.LengthSquared() < 0.1f;

		/// <summary> The layer this rocket is drawn in </summary>
		public RocketDrawLayer DrawLayer = RocketDrawLayer.AfterNPCs;

		/// <summary> This rocket's engine module nameplate </summary>
		public Nameplate Nameplate => EngineModule.Nameplate;

		/// <summary> The Rocket's name, if not set by the user, defaults to a localized "Rocket" name </summary>
		public string DisplayName
			=> Nameplate.IsValid() ? AssignedName : Language.GetTextValue("Mods.Macrocosm.Common.Rocket");

		/// <summary> The Rocket's name, set by the user </summary>
		public string AssignedName => EngineModule.Nameplate.Text;

		/// <summary> Dictionary of all the rocket's modules by name, in their order found in ModuleNames </summary>
		public Dictionary<string, RocketModule> Modules = new();

		/// <summary> List of the module names, in the customization access order </summary>
		public List<string> ModuleNames => Modules.Keys.ToList();

		/// <summary> The Rocket's command pod </summary>
		public CommandPod CommandPod => (CommandPod)Modules["CommandPod"];

		/// <summary> The Rocket's service module </summary>
		public ServiceModule ServiceModule => (ServiceModule)Modules["ServiceModule"];

		/// <summary> The rocket's reactor module </summary>
		public ReactorModule ReactorModule => (ReactorModule)Modules["ReactorModule"];

		/// <summary> The Rocket's engine module </summary>
		public EngineModule EngineModule => (EngineModule)Modules["EngineModule"];

		/// <summary> The rocket's left booster </summary>
		public BoosterLeft BoosterLeft => (BoosterLeft)Modules["BoosterLeft"];

		/// <summary> The rocket's right booster </summary>
		public BoosterRight BoosterRight => (BoosterRight)Modules["BoosterRight"];

		public int StaticFireBeginTime = 60;
		public bool StaticFire => StaticFireProgress > 0f;
		public float StaticFireProgress => Utility.InverseLerp(LiftoffTime - StaticFireBeginTime, LiftoffTime, FlightTime, clamped: true);

		/// <summary> The flight sequence progress </summary>
		public float FlightProgress => 1f - ((Position.Y - WorldExitPositionY) / (StartPositionY - WorldExitPositionY));

		/// <summary> The landing sequence progress </summary>
		public float LandingProgress => Position.Y / (TargetLandingPosition.Y - Height + 16);

		private bool forcedStationaryAppearance;
		private bool forcedFlightAppearance;

		private float worldExitSpeed;

		/// <summary> Whether this rocket is forced in a stationary (i.e. landed) state, visually </summary>
		public bool ForcedStationaryAppearance
		{
			get => forcedStationaryAppearance;
			set
			{
				forcedStationaryAppearance = value;

				if(value) 
					forcedFlightAppearance = false;

				ResetAnimation();
			}
		}

		/// <summary> Whether this rocket is forced in a full flight state, visually </summary>
		public bool ForcedFlightAppearance
		{
			get => forcedFlightAppearance;
			set
			{
				forcedFlightAppearance = value;

				if(value) 
					forcedStationaryAppearance = false;

				ResetAnimation();
			}
		}

		/// <summary> Instatiates a rocket. Use <see cref="Create(Vector2)"/> for spawning in world and proper syncing. </summary>
		public Rocket()
		{
			foreach(string moduleName in DefaultModuleNames)
				Modules[moduleName] = CreateModule(moduleName);
		}

		private RocketModule CreateModule(string moduleName)
		{
			return moduleName switch
			{
				"CommandPod" => new CommandPod(this),
				"ServiceModule" => new ServiceModule(this),
				"ReactorModule" => new ReactorModule(this),
				"EngineModule" => new EngineModule(this),
				"BoosterLeft" => new BoosterLeft(this),
				"BoosterRight" => new BoosterRight(this),
				_ => throw new ArgumentException($"Unknown module name: {moduleName}")
			};
		}

		public void OnCreation()
		{
			CurrentWorld = MacrocosmSubworld.CurrentWorld;
		}

		/// <summary> Called when spawning into a new world </summary>
		public void OnWorldSpawn()
		{
			ResetAnimation();

			if (Landing && ActiveInCurrentWorld)
			{
				// Travel to spawn point if a specific launchpad has not been set
				if (TargetLandingPosition == default)
 					TargetLandingPosition = Utility.SpawnWorldPosition;
 
				Center = new(TargetLandingPosition.X, Center.Y);
			}	
		}

		/// <summary> Called when a subworld is generated </summary>
		public void OnSubworldGenerated() 
		{
			if (Landing && ActiveInCurrentWorld)
			{
				// Target landing position always defaults to the spawn point just set on worldgen
				TargetLandingPosition = Utility.SpawnWorldPosition;
				Center = new(TargetLandingPosition.X, Center.Y);
			}
		}


		/// <summary> Update the rocket </summary>
		public void Update()
		{
			Velocity = GetCollisionVelocity();
			Position += Velocity;

			// Testing
			Fuel = 1000f;

			Movement();

			if (Stationary)
			{
				Interact();
				LookForCommander();
			}

            if (Launched && Position.Y < WorldExitPositionY)
			{
				worldExitSpeed = Velocity.Y;
				ResetAnimation();
				Launched = false;
				Landing = true;
				EnterDestinationSubworld();
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
			CurrentWorld = "";
			NetSync();
		}

		public void PreDrawBeforeTiles(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			foreach (RocketModule module in Modules.Values.OrderBy(module => module.DrawPriority))
			{
				module.PreDrawBeforeTiles(spriteBatch, screenPos, drawColor);
			}	
		}

		/// <summary> Draw the rocket in the configured draw layer </summary>
		public void Draw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			// Positions (also) set here, in the update method only they lag behind 
			SetModuleRelativePositions();

			foreach (RocketModule module in Modules.Values.OrderBy(module => module.DrawPriority))
			{
				module.Draw(spriteBatch, screenPos, drawColor);
			}

			//DrawDebugBounds();
			//DrawDebugModuleHitbox();
			//DisplayWhoAmI();
		}

		/// <summary> Draw the rocket as a dummy </summary>
		public void DrawDummy(SpriteBatch spriteBatch, Vector2 offset, Color drawColor)
		{
			// Passing Rocket world position as "screenPosition" cancels it out  
			PreDrawBeforeTiles(spriteBatch, Position - offset, drawColor);
			Draw(spriteBatch, Position - offset, drawColor);
 			DrawOverlay(spriteBatch, Position - offset);
 		}

		public void DrawOverlay(SpriteBatch spriteBatch, Vector2 screenPos)
		{
			foreach (RocketModule module in Modules.Values.OrderBy(module => module.DrawPriority))
			{
				module.DrawOverlay(spriteBatch, screenPos);
			}
		}

		// Set the rocket's modules positions in the world
		private void SetModuleRelativePositions()
		{
			CommandPod.Position = Position + new Vector2(Width/2f - CommandPod.Hitbox.Width/2f, 0);
			ServiceModule.Position = CommandPod.Position + new Vector2(-6, CommandPod.Hitbox.Height - 2.1f);
			ReactorModule.Position = ServiceModule.Position + new Vector2(-2, ServiceModule.Hitbox.Height - 2);
			EngineModule.Position = ReactorModule.Position + new Vector2(-18, ReactorModule.Hitbox.Height);
			BoosterLeft.Position = new Vector2(EngineModule.Center.X, EngineModule.Position.Y) - new Vector2(BoosterLeft.Texture.Width/2, 0) + new Vector2(-37, 16);
			BoosterRight.Position = new Vector2(EngineModule.Center.X, EngineModule.Position.Y)  -new Vector2(BoosterLeft.Texture.Width / 2, 0) + new Vector2(37, 16);
		}

		/// <summary> Gets the RocketPlayer bound to the provided player ID </summary>
		/// <param name="playerID"> The player ID </param>
		public RocketPlayer GetRocketPlayer(int playerID) => Main.player[playerID].RocketPlayer();

		/// <summary> Gets the commander of this rocket </summary>
		public RocketPlayer GetCommander()
		{
			if (Main.netMode == NetmodeID.SinglePlayer)
			{
				return GetRocketPlayer(Main.myPlayer);
			}
			else
			{
				if (TryFindingCommander(out int id))
					return GetRocketPlayer(id);
				else if (AnyEmbarkedPlayers(out id))
					return GetRocketPlayer(id);
				else
					return null;
			}
		}

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
			Launched = true;
			StartPositionY = Position.Y;
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
			foreach (RocketModule module in Modules.Values)
				if (Math.Abs(Collision.TileCollision(module.Position, Velocity, module.Hitbox.Width, module.Hitbox.Height).Y) > 0.1f)
					return true;

			return false;
		}

		// Interaction logic
		private void Interact()
		{
			if (Main.netMode == NetmodeID.Server)
				return;

			if (MouseCanInteract() && Bounds.InPlayerInteractionRange() && !Launched && !GetRocketPlayer(Main.myPlayer).InRocket)
			{
				if (Main.mouseRight)
				{
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

		public void ResetAnimation()
		{
			foreach (RocketModule module in Modules.Values)
			{
				if (module is AnimatedRocketModule animatedModule)
				{
					if(forcedStationaryAppearance) 
					{
						animatedModule.CurrentFrame = animatedModule.NumberOfFrames - 1;
					}
					else if (forcedFlightAppearance)
					{
						animatedModule.CurrentFrame = 0;
					}
					else
                    {
						if (Landing)
							animatedModule.CurrentFrame = 0;
						else
							animatedModule.CurrentFrame = animatedModule.NumberOfFrames - 1;

						animatedModule.ShouldAnimate = true;
					}
				}
			}
		}

		private Vector2 GetCollisionVelocity()
		{
			Vector2 minCollisionVelocity = new(float.MaxValue, float.MaxValue);

			foreach (RocketModule module in Modules.Values)
			{
				Vector2 collisionVelocity = Collision.TileCollision(module.Position, Velocity, module.Hitbox.Width, module.Hitbox.Height);
				if (collisionVelocity.LengthSquared() < minCollisionVelocity.LengthSquared())
					minCollisionVelocity = collisionVelocity;
 			}
			return minCollisionVelocity;
		}

		private bool MouseCanInteract()
		{
			foreach (RocketModule module in Modules.Values.Where((module) => !(module is BoosterRight) && !(module is BoosterLeft)))
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
			float gravityForce = 0.1f * MacrocosmSubworld.CurrentGravityMultiplier;

			if (Launched)
			{
				UpdateModuleAnimation();

				if (FlightProgress > 0.1f)
					SetModuleAnimation(landing: false);

				if (FlightProgress > 0.6f && !FadeEffect.IsFading && CheckPlayerInRocket(Main.myPlayer))
				{
					FadeEffect.ResetFade();
					FadeEffect.StartFadeOut(0.02f, selfDraw: true);
				}

				FlightTime++;

				if (InFlight)
				{
					
					float flightAcceleration = 0.1f;   // mid-flight
					float liftoffAcceleration = 0.05f; // during liftoff
					float startAcceleration = 0.01f;   // initial 

					if (Velocity.Y < MaxFlightSpeed)
						if (FlightTime >= LiftoffTime + 60)
							Velocity.Y -= flightAcceleration;
						else if (FlightTime >= LiftoffTime + 40)
							Velocity.Y -= liftoffAcceleration;
						else
							Velocity.Y -= startAcceleration;

					SpawnSmoke(countPerTick: 5);
					//SetScreenshake();
				}
				else if(StaticFire)
				{
					SpawnSmoke(10, 1.2f);
				}

				Light();
			}
			else if (Landing)
			{
				Velocity.Y = MathHelper.Lerp(-worldExitSpeed * 0.8f, 1.5f, Utility.QuadraticEaseOut(LandingProgress));
 
				UpdateModuleAnimation();

				if (LandingProgress > 0.7f)
				{
					SetModuleAnimation(landing: true);
				}

				if (LandingProgress >= 0.99f)
				{
					if(GetRocketPlayer(Main.myPlayer).InRocket)
						RocketUISystem.Show(this);

					Landing = false;
					ResetAnimation();
				}
			}
			else
			{
				Velocity.Y += gravityForce;
			}
		}

		// TODO: fix this
		private void Light()
		{
			if (FlightTime < LiftoffTime + 5)
			{
				Lighting.AddLight(new Vector2(Center.X + 50, Position.Y + Height + 25), new Color(215, 69, 0).ToVector3() * 5f);
				Lighting.AddLight(new Vector2(Center.X, Position.Y + Height + 25), new Color(215, 69, 0).ToVector3() * 15f);
				Lighting.AddLight(new Vector2(Center.X - 50, Position.Y + Height + 25), new Color(215, 69, 0).ToVector3() * 5f);

				Lighting.AddLight(new Vector2(Center.X + 50, Position.Y + Height + 75), new Color(215, 69, 0).ToVector3() * 5f);
				Lighting.AddLight(new Vector2(Center.X, Position.Y + Height + 75), new Color(215, 69, 0).ToVector3() * 15f);
				Lighting.AddLight(new Vector2(Center.X - 50, Position.Y + Height + 75), new Color(215, 69, 0).ToVector3() * 5f);

				Lighting.AddLight(new Vector2(Center.X + 50, Position.Y + Height + 150), new Color(215, 69, 0).ToVector3() * 5f);
				Lighting.AddLight(new Vector2(Center.X, Position.Y + Height + 125), new Color(215, 69, 0).ToVector3() * 15f);
				Lighting.AddLight(new Vector2(Center.X - 50, Position.Y + Height + 150), new Color(215, 69, 0).ToVector3() * 5f);
			}
			else
			{
				Lighting.AddLight(new Vector2(Center.X, Position.Y + Height + 75), new Color(215, 69, 0).ToVector3() * 10f);
			}
		}

		private void UpdateModuleAnimation()
		{
			foreach (RocketModule module in Modules.Values)
				if (module is AnimatedRocketModule animatedModule)
					animatedModule.UpdateAnimation();
 		}

		private void SetModuleAnimation(bool landing = false)
		{
			foreach (RocketModule module in Modules.Values)
			{
				if (module is AnimatedRocketModule animatedModule)
				{
					if (landing)
						animatedModule.StartAnimation();
					else 
						animatedModule.StartReverseAnimation();

					animatedModule.ShouldAnimate = false;
				}
			}
		}

		// Sets the screenshake during flight 
		private void SetScreenshake()
		{
			float intenstity;

			if (FlightTime >= LiftoffTime && FlightProgress < 0.05f)
				intenstity = 30f;
			else
				intenstity = 15f * (1f - Utility.QuadraticEaseOut(FlightProgress));

			Main.LocalPlayer.AddScreenshake(intenstity, "RocketFlight");
		}

		// Handle visuals (dusts, particles)
		private void SpawnSmoke(int countPerTick, float speedScale = 1f)
		{
			for (int i = 0; i < countPerTick; i++)
			{
				var smoke = Particle.CreateParticle<RocketExhaustSmoke>(p =>
				{
					p.Position = new Vector2(Center.X, Position.Y + Height - 28);
					p.Velocity = new Vector2(Main.rand.NextFloat(-0.6f, 0.6f), Main.rand.NextFloat(2, 10)) * speedScale;
					p.Scale = Main.rand.NextFloat(1.2f, 1.6f);
					p.Rotation = 0f;
					p.FadeIn = true;
					p.FadeOut = true;
					p.FadeInSpeed = 2;
					p.FadeOutSpeed = 15;
					p.TargetAlpha = 128;
					p.ScaleDownSpeed = 0.001f;
					p.Deceleration = 0.995f;
					p.DrawColor = Color.White.WithAlpha(150);
					p.Collide = true;
				}, shouldSync: false);
			}
		}

		// Handles the subworld transfer on each client, locally
		private void EnterDestinationSubworld()
		{
			if (Main.netMode == NetmodeID.Server)
				return;

			Velocity = Vector2.Zero;
			Landing = true;

			if (CheckPlayerInRocket(Main.myPlayer))
			{
				RocketPlayer commander = GetCommander();

				if(commander is null)
				{
					MacrocosmSubworld.WorldTravelFailure("Error: Could not find the commander of Rocket " + WhoAmI);
					return;
				}

				CurrentWorld = commander.TargetSubworldID;
				//NetSync();

				//LaunchPad launchPad = commander.SelectedLaunchPad;
				LaunchPad launchPad = null;


				if(launchPad is not null) 
					TargetLandingPosition = launchPad.Position;
				else
					TargetLandingPosition = default;

				if (commander.TargetSubworldID != null && commander.TargetSubworldID != "")
				{
					if (!MacrocosmSubworld.Travel(commander.TargetSubworldID, this))
						CurrentWorld = MacrocosmSubworld.CurrentWorld;
 				}
			}
		}
	}
}
