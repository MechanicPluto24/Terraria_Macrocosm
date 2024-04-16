using Macrocosm.Common.Drawing;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Netcode;
using Macrocosm.Common.Storage;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Systems.UI;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Materials.Drops;
using Macrocosm.Content.Items.Materials.Tech;
using Macrocosm.Content.Particles;
using Macrocosm.Content.Players;
using Macrocosm.Content.Rockets.Customization;
using Macrocosm.Content.Rockets.LaunchPads;
using Macrocosm.Content.Rockets.Modules;
using Macrocosm.Content.Sounds;
using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using SubworldLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Golf;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using XPT.Core.Audio.MP3Sharp;

namespace Macrocosm.Content.Rockets
{
    public partial class Rocket : IInventoryOwner
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

        /// <summary> The target world to fly towards </summary>
        [NetSync] public string TargetWorld = "";

        /// <summary> The target landing position </summary>
        [NetSync] public Vector2 TargetLandingPosition;
        private LaunchPad targetLaunchPad;

        /// <summary> The amount of fuel currently stored in the rocket, as an absolute value </summary>
        [NetSync] public float Fuel;

        /// <summary> The rocket's fuel capacity as an absoulute value </summary>
        [NetSync] public float FuelCapacity = 1000f;

        /// <summary> The rocket's current world, "Earth" if active and not in a subworld. Other mod's subworlds have the mod name prepended </summary>
        [NetSync] public string CurrentWorld = "";

        /// <summary> Whether the rocket is active in the current world and should be updated and visible </summary>
        public bool ActiveInCurrentWorld => Active && CurrentWorld == MacrocosmSubworld.CurrentID;

        /// <summary> Number of ticks of the launch countdown (seconds * 60 ticks/sec) </summary>
        public int LiftoffTime = 3 * 60;

        public float MaxFlightSpeed = 25f;

        /// <summary> Whether this rocket is currently in flight </summary>
        public bool InFlight => FlightTime >= LiftoffTime;

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
            get => Position + Size / 2f;
            set => Position = value - Size / 2f;
        }

        /// <summary> Whether the rocket is currently stationary </summary>
        public bool Stationary => Velocity.LengthSquared() < 0.1f;

        /// <summary> The layer this rocket is drawn in </summary>
        public RocketDrawLayer DrawLayer = RocketDrawLayer.BeforeNPCs;

        /// <summary> This rocket's nameplate </summary>
        public Nameplate Nameplate { get; set; } = new();

        /// <summary> The Rocket's name, if not set by the user, defaults to a localized "Rocket" name </summary>
        public string DisplayName
            => Nameplate.IsValid() ? Nameplate.Text : Language.GetTextValue("Mods.Macrocosm.Common.Rocket");

        /// <summary> Dictionary of all the rocket's modules by name, in their order found in ModuleNames </summary>
        public Dictionary<string, RocketModule> Modules = new();

        public List<RocketModule> ModulesByDrawPriority => Modules.Values.OrderBy(module => module.DrawPriority).ToList();

        /// <summary> List of the module names, in the customization access order </summary>
        public List<string> ModuleNames => Modules.Keys.ToList();

        public int StaticFireBeginTime = 120;
        public bool StaticFire => StaticFireProgress > 0f && FlightTime < LiftoffTime;
        public float StaticFireProgress => Utility.InverseLerp(LiftoffTime - StaticFireBeginTime, LiftoffTime, FlightTime, clamped: true);

        /// <summary> The flight sequence progress </summary>
        public float FlightProgress { get; set; }
        private float EasedFlightProgress => Utility.QuartEaseIn(FlightProgress);

        /// <summary> The landing sequence progress </summary>
        public float LandingProgress { get; set; }
        private float EasedLandingProgress => Utility.QuadraticEaseOut(LandingProgress);

        private float worldExitSpeed;
        private bool ranFirstUpdate;
        private bool canAnimate = true;

        private bool forcedStationaryAppearance;
        /// <summary> Whether this rocket is forced in a stationary (i.e. landed) state, visually </summary>
        public bool ForcedStationaryAppearance
        {
            get => forcedStationaryAppearance;
            set
            {
                forcedStationaryAppearance = value;

                if (value)
                    forcedFlightAppearance = false;

                ResetAnimation();
            }
        }

        private bool forcedFlightAppearance;
        /// <summary> Whether this rocket is forced in a full flight state, visually </summary>
        public bool ForcedFlightAppearance
        {
            get => forcedFlightAppearance;
            set
            {
                forcedFlightAppearance = value;

                if (value)
                    forcedStationaryAppearance = false;

                ResetAnimation();
            }
        }

        public bool HasInventory => Inventory is not null;
        public Inventory Inventory { get; set; }

        public const int DefaultInventorySize = 50;

        public Vector2 InventoryItemDropLocation => Center;
        public int InventorySerializationIndex => WhoAmI;

        /// <summary> Instatiates a rocket. Use <see cref="Create(Vector2)"/> for spawning in world and proper syncing. </summary>
        public Rocket()
        {
            foreach (string moduleName in DefaultModuleNames)
            {
                Modules[moduleName] = CreateModule(moduleName);
                Modules[moduleName].SetRocket(this);
            }
        }

        private RocketModule CreateModule(string moduleName)
        {
            return moduleName switch
            {
                "CommandPod" => new CommandPod(),
                "ServiceModule" => new ServiceModule(),
                "ReactorModule" => new ReactorModule(),
                "EngineModule" => new EngineModule(),
                "BoosterLeft" => new BoosterLeft(),
                "BoosterRight" => new BoosterRight(),
                _ => throw new ArgumentException($"Unknown module name: {moduleName}")
            };
        }

        /// <summary> Called when initially creating the rocket </summary>
        public void OnCreation()
        {
            CurrentWorld = MacrocosmSubworld.CurrentID;
            Inventory = new(DefaultInventorySize, this);
            OnWorldSpawn();
        }

        /// <summary> Called when spawning into a new world </summary>
        public void OnWorldSpawn()
        {
            ResetAnimation();
            ResetRenderTarget();

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
            SetModuleWorldPositions();
            Velocity = GetCollisionVelocity();
            Position += Velocity;

            // Testing
            Fuel = 1000f;

            Movement();
            Effects();
            PlaySound();

            if (Stationary)
            {
                Interact();
                LookForCommander();
            }

            if (Launched && Position.Y < WorldExitPositionY + 1)
            {
                FlightTime = 0;
                FlightProgress = 0f;
                LandingProgress = 0f;
                Velocity = Vector2.Zero;
                Launched = false;
                Landing = true;
                ResetAnimation();
                Travel();
            }

            // reset render target after first update to fix reload issue
            if (!ranFirstUpdate)
            {
                ResetRenderTarget();
                ranFirstUpdate = true;
            }
        }

        /// <summary> Safely despawn the rocket </summary>
        public void Despawn()
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                if (CheckPlayerInRocket(Main.myPlayer))
                {
                    GetRocketPlayer(Main.myPlayer).InRocket = false;
                    GetRocketPlayer(Main.myPlayer).IsCommander = false;
                }
            }
            else
            {
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    if (CheckPlayerInRocket(i))
                    {
                        GetRocketPlayer(i).InRocket = false;
                        GetRocketPlayer(i).IsCommander = false;
                    }
                }
            }

            if (Active && HasInventory)
            {
                Inventory.Size = 0;
                Inventory.SyncSize();

                Inventory = null;
            }

            CurrentWorld = "";
            Active = false;
            NetSync();
        }

        private Vector2 GetModuleRelativePosition(RocketModule module, Vector2 origin)
        {
            var commandPod = Modules["CommandPod"];
            var serviceModule = Modules["ServiceModule"];
            var reactorModule = Modules["ReactorModule"];
            var engineModule = Modules["EngineModule"];

            return module switch
            {
                CommandPod => origin + new Vector2(Width / 2f - commandPod.Width / 2f, 0),
                ServiceModule => origin + new Vector2(Width / 2f - serviceModule.Width / 2f, commandPod.Height),
                ReactorModule => origin + new Vector2(Width / 2f - reactorModule.Width / 2f, commandPod.Height + serviceModule.Height - 2),
                EngineModule => origin + new Vector2(Width / 2f - engineModule.Width / 2f, commandPod.Height + serviceModule.Height + reactorModule.Height - 4),
                BoosterLeft => origin + new Vector2(78, commandPod.Height + serviceModule.Height + reactorModule.Height + 12),
                BoosterRight => origin + new Vector2(152, commandPod.Height + serviceModule.Height + reactorModule.Height + 12),
                _ => default,
            };
        }

        // Set the rocket's modules positions in the world
        private void SetModuleWorldPositions()
        {
            foreach (RocketModule module in Modules.Values)
            {
                module.Position = GetModuleRelativePosition(module, Position);
            }
        }

        /// <summary> Gets the RocketPlayer bound to the provided player ID </summary>
        /// <param name="playerID"> The player ID </param>
        public RocketPlayer GetRocketPlayer(int playerID) => Main.player[playerID].GetModPlayer<RocketPlayer>();

        /// <summary> Gets the commander of this rocket </summary>
        public RocketPlayer GetCommander()
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                if (GetRocketPlayer(Main.myPlayer).InRocket)
                    return GetRocketPlayer(Main.myPlayer);
                else
                    return null;
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
        public bool CheckPlayerCommander(int playerID) => Main.player[playerID].active && GetRocketPlayer(playerID).IsCommander && GetRocketPlayer(playerID).RocketID == WhoAmI;

        /// <summary> Checks whether this rocket has a commander </summary>
        /// <param name="playerID"> The commander player ID </param>
        public bool TryFindingCommander(out int playerID)
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (!Main.player[i].active)
                    continue;

                RocketPlayer rocketPlayer = GetRocketPlayer(i);
                if (CheckPlayerInRocket(i) && rocketPlayer.IsCommander)
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

        /// <summary> 
        /// Launches the rocket, with syncing. 
        /// For <paramref name="targetWorld"/>, use the ID format respective to <see cref="MacrocosmSubworld.CurrentID"/> (mod name prepended) 
        /// </summary>
        public void Launch(string targetWorld, LaunchPad targetLaunchPad = null)
        {
            Main.playerInventory = false;
            Launched = true;
            StartPositionY = Position.Y;
            TargetWorld = targetWorld;
            this.targetLaunchPad = targetLaunchPad;

            Fuel -= GetFuelCost(targetWorld);

            NetSync();
        }

        public float GetFuelCost(string targetWorld) => RocketFuelLookup.GetFuelCost(MacrocosmSubworld.CurrentID, targetWorld);

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

        public bool AtPosition(Vector2 position) => Vector2.Distance(Center, position) < Width;

        public bool AtCurrentLaunchpad(LaunchPad launchPad, string worldId)
        {
            if (launchPad == null)
            {
                // Check for position at world spawn disabled for now
                /*
                if (worldId == MacrocosmSubworld.CurrentID)
                    return AtPosition(Utility.SpawnWorldPosition);
                else
                */
                return false;
            }

            if (worldId == MacrocosmSubworld.CurrentID)
                return launchPad.RocketID == WhoAmI;
            else
                return false;
        }

        public bool CheckTileCollision()
        {
            foreach (RocketModule module in Modules.Values)
                if (Math.Abs(Collision.TileCollision(module.Position, Velocity, module.Width, module.Height).Y) > 0.1f)
                    return true;

            return false;
        }

        // Interaction logic
        private void Interact()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            if (MouseCanInteract() && Bounds.InPlayerInteractionRange(TileReachCheckSettings.Simple) && !Launched)
            {
                RocketPlayer rocketPlayer = GetRocketPlayer(Main.myPlayer);

                if (Main.mouseRight && Main.mouseRightRelease)
                {
                    if (!rocketPlayer.InRocket)
                    {
                        bool noCommanderInRocket = (Main.netMode == NetmodeID.SinglePlayer) || !TryFindingCommander(out _);
                        rocketPlayer.EmbarkPlayerInRocket(WhoAmI, noCommanderInRocket);
                    }
                    else if (!UISystem.Active)
                    {
                        Utility.UICloseOthers();
                        UISystem.ShowRocketUI(this);
                    }
                }
                else if (!UISystem.Active && !InFlight && !Landing)
                {
                    if (rocketPlayer.InRocket)
                    {
                        Main.LocalPlayer.cursorItemIconEnabled = true;
                        Main.LocalPlayer.cursorItemIconID = ModContent.ItemType<Computer>();
                    }
                    else
                    {
                        Main.LocalPlayer.noThrow = 2;
                        CursorIcon.Current = CursorIcon.Rocket;
                    }
                }
            }
        }

        public void ResetAnimation()
        {
            canAnimate = true;

            foreach (RocketModule module in Modules.Values)
            {
                if (module is AnimatedRocketModule animatedModule)
                {
                    if (forcedStationaryAppearance)
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
                    }
                }
            }
        }

        private Vector2 GetCollisionVelocity()
        {
            Vector2 minCollisionVelocity = new(float.MaxValue, float.MaxValue);

            foreach (RocketModule module in Modules.Values)
            {
                Vector2 collisionVelocity = Collision.TileCollision(module.Position, Velocity, module.Width, module.Height);
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
            {
                GetRocketPlayer(id).IsCommander = true;
                NetMessage.SendData(MessageID.SyncPlayer, number: id);
            }
        }

        // Handles the rocket's movement during flight
        private void Movement()
        {
            float gravity = MacrocosmSubworld.CurrentGravityMultiplier;
            float gravityFactor = 0.7f + 0.3f * gravity;

            if (Launched)
            {
                FlightTime++;

                if (InFlight)
                {
                    float launchDistance = Math.Abs(StartPositionY - WorldExitPositionY);
                    float launchDuration = 10f * 60f * gravityFactor;
                    float launchIncrement = launchDistance / launchDuration;

                    Position = new Vector2(Position.X, MathHelper.Lerp(StartPositionY, WorldExitPositionY, EasedFlightProgress));
                    FlightProgress += launchIncrement / launchDistance;
                    FlightProgress = MathHelper.Clamp(FlightProgress, 0f, 1f);

                    UpdateModuleAnimation();
                    if (EasedFlightProgress > 0.01f)
                        StartModuleAnimation(landing: false);

                    if (GetRocketPlayer(Main.myPlayer).InRocket)
                        Main.BlackFadeIn = (int)(255f * EasedFlightProgress);
                }
            }
            else if (Landing)
            {
                float landingDistance = Math.Abs(WorldExitPositionY - TargetLandingPosition.Y + Height);
                float landingDuration = 10f * 60f * (1f / gravityFactor);
                float landingIncrement = landingDistance / landingDuration;
                Position = new Vector2(Position.X, MathHelper.Lerp(WorldExitPositionY, TargetLandingPosition.Y - Height, EasedLandingProgress));
                LandingProgress += landingIncrement / landingDistance;
                LandingProgress = MathHelper.Clamp(LandingProgress, 0f, 1f);

                UpdateModuleAnimation();

                if (EasedLandingProgress > 0.95f)
                    StartModuleAnimation(landing: true);

                if (GetRocketPlayer(Main.myPlayer).InRocket)
                    Main.BlackFadeIn = (int)(255f * (1f - EasedLandingProgress));

                if (LandingProgress >= 1f - float.Epsilon)
                {
                    Landing = false;
                    ResetAnimation();
                }
            }
            else
            {
                Velocity.Y += 0.1f * gravity;
            }

        }
        private void UpdateModuleAnimation()
        {
            bool animationActive = false;
            foreach (RocketModule module in Modules.Values)
            {
                if (module is AnimatedRocketModule animatedModule)
                {
                    animatedModule.UpdateAnimation();
                    animationActive |= animatedModule.IsAnimationActive;
                }
            }

            if (animationActive)
                ResetRenderTarget();
        }

        private void StartModuleAnimation(bool landing = false)
        {
            if (canAnimate)
            {
                foreach (RocketModule module in Modules.Values)
                {
                    if (module is AnimatedRocketModule animatedModule)
                    {
                        if (landing)
                            animatedModule.StartAnimation();
                        else
                            animatedModule.StartReverseAnimation();
                    }
                }

                SoundEngine.PlaySound(SFX.RocketLandingLeg with
                {
                    Volume = 1f,
                    PlayOnlyIfFocused = true,
                    MaxInstances = 1,
                    SoundLimitBehavior = SoundLimitBehavior.IgnoreNew
                },
                Center, updateCallback: (sound) =>
                {
                    sound.Position = Center;
                    return true;
                });
            }

            canAnimate = false;
        }

        private void Effects()
        {
            if (!Launched && !Landing)
                return;

            float gravityFactor = 0.7f + 0.3f * MacrocosmSubworld.CurrentGravityMultiplier;
            float atmoDesityFactor = 0.5f + 0.5f * MacrocosmSubworld.CurrentAtmosphericDensity;


            Point tilePos = (Position + new Vector2(Width / 2f, Height)).ToTileCoordinates();
            Point closestTile = Utility.GetClosestTile(tilePos.X, tilePos.Y, -1, 15, (t) => Main.tileSolid[t.TileType] && !t.IsActuated);
            closestTile.Y += 1;

            float lightIntensity = 0f;
            float screenshakeIntensity = 0f;

            if (StaticFire)
            {
                lightIntensity = StaticFireProgress;
                screenshakeIntensity = 5f * (Utility.QuadraticEaseOut(StaticFireProgress));

                int count = MacrocosmSubworld.CurrentAtmosphericDensity < 1f ? 1 : (int)(3f * atmoDesityFactor * StaticFireProgress);
                SpawnSmokeExhaustTrail(countPerTick: count);

                if(Main.rand.NextFloat() < StaticFireProgress)
                    SpawnTileDust(closestTile, 1);
            }

            if (InFlight)
            {
                lightIntensity = 10f;
                screenshakeIntensity = FlightProgress < 0.05f ? 30f : 15f * (1f - Utility.QuadraticEaseOut(FlightProgress));

                int count = MacrocosmSubworld.CurrentAtmosphericDensity < 1f ? 2 : (int)(5f * atmoDesityFactor);
                SpawnSmokeExhaustTrail(countPerTick: count);

                if (EasedFlightProgress < 0.1f)
                     SpawnTileDust(closestTile, 2);
            }

            if (Landing)
            {
                lightIntensity = 10f;
                screenshakeIntensity = 2f * (Utility.QuadraticEaseOut(LandingProgress));

                int count = MacrocosmSubworld.CurrentAtmosphericDensity < 1f ? 1 : (int)(3f * atmoDesityFactor);
                SpawnSmokeExhaustTrail(countPerTick: count, speed: 2f * (1.2f - EasedLandingProgress));

                if(EasedLandingProgress > 0.9f)
                     SpawnTileDust(closestTile, 2);
 
                if (LandingProgress >= 1f - 0.03f)
                {
                    SpawnTileDust(closestTile, 15);
                    SpawnHitTileDust(closestTile);
                    screenshakeIntensity = 20f * gravityFactor;
                }
            }

            //Lighting.AddLight(new Vector2(Center.X, Position.Y + Height + 15), new Color(215, 69, 0).ToVector3() * lightIntensity);
            Main.LocalPlayer.AddScreenshake(screenshakeIntensity, $"Rocket{WhoAmI}");

            // Hack to force render the tiles otherwise completetly unlighted, so the trail does not draw in front of them 
            for (int i = (int)(Position.X / 16f); i < (int)(Position.X / 16f) + Width / 16; i++)
                for (int j = (int)(Position.Y / 16f) + Height / 16 - 5; j < (int)(Position.Y / 16f) + Height / 16 + 15; j++)
                    Lighting.AddLight(new Vector2(i * 16, j * 16), new Vector3(0.01f));
        }

        private void SpawnSmokeExhaustTrail(int countPerTick, float speed = 1f)
        {
            for (int i = 0; i < countPerTick; i++)
            {
                Vector2 position = new Vector2(Center.X, Position.Y + Height - 28);

                Vector2 velocity = new Vector2(Main.rand.NextFloat(-0.6f, 0.6f), Main.rand.NextFloat(2, 10));
                if (Landing)
                    velocity = new(Main.rand.NextFloat(-0.4f, 0.4f), Main.rand.NextFloat(8, 16));

                var smoke = Particle.CreateParticle<RocketExhaustSmoke>(p =>
                {
                    p.Position = position;
                    p.Velocity = velocity * speed;
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

            int smallSmokeCount = (int)(countPerTick * 2f);

            var boosterLeft = Modules["BoosterLeft"] as Booster;
            var boosterRight = Modules["BoosterRight"] as Booster;

            for (int i = 0; i < smallSmokeCount; i++)
            {
                Vector2 position = i % 2 == 0 ?
                                new Vector2(boosterLeft.Position.X + boosterLeft.ExhaustOffsetX, Position.Y + Height - 28) :
                                new Vector2(boosterRight.Position.X + boosterRight.ExhaustOffsetX, Position.Y + Height - 28);


                Vector2 velocity = new(Main.rand.NextFloat(-0.4f, 0.4f), Main.rand.NextFloat(2, 4));
                if(Landing)
                    velocity = new(Main.rand.NextFloat(-0.4f, 0.4f), Main.rand.NextFloat(8, 12));

                var smoke = Particle.CreateParticle<RocketExhaustSmoke>(p =>
                {
                    p.Position = position;
                    p.Velocity = velocity * speed;
                    p.Scale = Main.rand.NextFloat(0.6f, 0.9f);
                    p.Rotation = 0f;
                    p.FadeIn = true;
                    p.FadeOut = true;
                    p.FadeInSpeed = 2;
                    p.FadeOutSpeed = 12;
                    p.TargetAlpha = 128;
                    p.ScaleDownSpeed = 0.0015f;
                    p.Deceleration = 0.993f;
                    p.DrawColor = Color.White.WithAlpha(150);
                    p.Collide = true;
                }, shouldSync: false);
            }
        }

        private void SpawnTileDust(Point tileCoords, int countPerTick)
        {
            for (int i = 0; i < countPerTick; i++)
            {
                var smoke = Particle.CreateParticle<Smoke>(p =>
                {
                    p.Position = tileCoords.ToWorldCoordinates();
                    p.Velocity = new Vector2(Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-0.1f, -1f));
                    p.Scale = Main.rand.NextFloat(0.5f, 1.2f);
                    p.Rotation = Utility.RandomRotation();
                    p.DrawColor = Color.Lerp(Utility.GetTileColor(tileCoords), Color.Gray, 0.5f) * Main.rand.NextFloat(0.2f, 0.8f);
                    p.FadeIn = true;
                    p.Opacity = 0f;
                    p.ExpansionRate = 0.0075f;
                });
            }
        }

        private void SpawnHitTileDust(Point tileCoords)
        {
            for (int x = -8; x < 8; x++)
            {
                if(x is < -6 or > -2 and < 2 or > 6 && !Utility.CoordinatesOutOfBounds(tileCoords.X - x, tileCoords.Y))
                    WorldGen.KillTile_MakeTileDust(tileCoords.X - x, tileCoords.Y, Main.tile[tileCoords.X - x, tileCoords.Y]);
            }
        }

        public void PlaySound()
        {
            if (StaticFire)
            {
                float intensity = 1f;
                SoundEngine.PlaySound(SFX.RocketLoop with
                {
                    Volume = intensity,
                    MaxInstances = 1,
                    SoundLimitBehavior = SoundLimitBehavior.IgnoreNew
                },
                Center, updateCallback: (sound) =>
                {
                    sound.Pitch = MathHelper.Lerp(-1f, 0, StaticFireProgress);
                    sound.Volume = intensity;
                    sound.Position = Center;
                    return FlightTime < LiftoffTime;
                }); 
            }

            if (FlightTime == LiftoffTime)
            {
                SoundEngine.PlaySound(SFX.RocketLaunch with
                {
                    Volume = 1f,
                    MaxInstances = 1,
                    SoundLimitBehavior = SoundLimitBehavior.IgnoreNew,
                },
                Center, updateCallback: (sound) =>
                {
                    sound.Position = Center;
                    return InFlight;
                });
            }

            if (InFlight)
            {
                SoundEngine.PlaySound(SFX.RocketLoop with
                {
                    Volume = 1f,
                    IsLooped = true,
                    MaxInstances = 1,
                    SoundLimitBehavior = SoundLimitBehavior.IgnoreNew
                },
                Center, updateCallback: (sound) =>
                {
                    sound.Pitch = Main.rand.NextFloat(-0.1f, 0.1f);
                    sound.Position = Center;
                    return InFlight;
                });
            }

            if (Landing)
            {
                SoundEngine.PlaySound(SFX.RocketLoop with
                {
                    MaxInstances = 1,
                    IsLooped = true,
                    Volume = 1f,
                    SoundLimitBehavior = SoundLimitBehavior.IgnoreNew
                },
                Center, updateCallback: (sound) =>
                {
                    sound.Pitch = Main.rand.NextFloat(-0.1f, 0.1f);
                    sound.Volume = LandingProgress < 0.8f ? 1f : (1f - LandingProgress) * 5f;
                    sound.Position = Center;
                    return Landing;
                });
            }
        }

        // Handles the subworld travel
        private void Travel()
        {
            bool samePlanet = CurrentWorld == TargetWorld;
            CurrentWorld = TargetWorld;

            RocketPlayer commander = GetCommander();

            // Determine the landing location.
            // Set as default if no launchpad has been selected (i.e. the World Spawn option) 
            // For different world travel, the correct value is assigned when spawning in that world
            if (targetLaunchPad is not null)
                TargetLandingPosition = targetLaunchPad.CenterWorld + new Vector2(16, 16);
            else
                TargetLandingPosition = default;

            if (samePlanet)
            {
                // This failsafe logic could be extended to hiding the rocket for an amount of time, while remotely launching satellites
                // (no commander inside but wire triggered). Would mean also keeping the launchpad as occupied to avoid collisions on return
                if (!MacrocosmSubworld.IsValidID(TargetWorld) || commander is null || commander.Player.dead || !commander.Player.active)
                {
                    TargetLandingPosition = new(Center.X, StartPositionY + Height);
                    CurrentWorld = MacrocosmSubworld.CurrentID;
                }

                // For same world travel assign the correct position here
                if (TargetLandingPosition == default)
                    TargetLandingPosition = Utility.SpawnWorldPosition + new Vector2(16, 16);

                Center = new(TargetLandingPosition.X, Center.Y);

                /*
                FadeEffect.ResetFade();
                if (!FadeEffect.IsFading)
                    FadeEffect.StartFadeIn(0.02f, selfDraw: true);
                */
            }
            else
            {
                if (Main.netMode == NetmodeID.Server)
                    return;

                if (CheckPlayerInRocket(Main.myPlayer))
                {
                    if (!MacrocosmSubworld.Travel(TargetWorld, this))
                    {
                        CurrentWorld = MacrocosmSubworld.CurrentID;
                        NetSync();
                    }
                }
            }
        }
    }
}
