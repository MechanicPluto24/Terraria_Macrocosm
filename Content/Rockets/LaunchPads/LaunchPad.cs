using Macrocosm.Common.Drawing;
using Macrocosm.Common.Netcode;
using Macrocosm.Common.Storage;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Systems.UI;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets.Modules;
using Macrocosm.Content.Tiles.Special;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets.LaunchPads
{
    public partial class LaunchPad : IInventoryOwner
    {
        public static int MinWidth => 18;
        public static int MaxWidth => 34;

        [NetSync] public bool Active;
        [NetSync] public Point16 StartTile;
        [NetSync] public Point16 EndTile;
        [NetSync] public int RocketID = -1;
        [NetSync] public string CompassCoordinates = "";

        public Rocket Rocket => HasRocket ? RocketManager.Rockets[RocketID] : unassembledRocket;
        public bool HasRocket => RocketID >= 0;

        private Rocket unassembledRocket;
        private Inventory assemblyInventory;

        public int Width => EndTile.X + 1 - StartTile.X;
        public Rectangle Hitbox => new((int)(StartTile.X * 16f), (int)(StartTile.Y * 16f), Width * 16, 16);
        public Vector2 Position => new(((StartTile.X + (EndTile.X - StartTile.X) / 2f) * 16f), StartTile.Y * 16f);

        public Vector2 InventoryItemDropLocation => Position;
        public int InventorySerializationIndex => ((StartTile.Y & 0xFFFF) << 16) | (StartTile.X & 0xFFFF);
        public Inventory Inventory
        {
            get => assemblyInventory;
            set => assemblyInventory = value;
        }

        private bool isMouseOver = false;
        private bool spawned = false;

        public LaunchPad()
        {
            StartTile = new();
            EndTile = new();

            unassembledRocket = new();
            foreach (var module in unassembledRocket.Modules)
                module.Value.IsBlueprint = true;

            assemblyInventory = new(CountRequiredAssemblyItemSlots(unassembledRocket), this);
        }

        public LaunchPad(int startTileX, int startTileY, int endTileX, int endTileY) : this()
        {
            StartTile = new(startTileX, startTileY);
            EndTile = new(endTileX, endTileY);
        }

        public LaunchPad(Point startTile, Point endTile) : this(startTile.X, startTile.Y, endTile.X, endTile.Y) { }
        public LaunchPad(Point16 startTile, Point16 endTile) : this(startTile.X, startTile.Y, endTile.X, endTile.Y) { }

        public static LaunchPad Create(int startTileX, int startTileY, int endTileX, int endTileY, bool shouldSync = true)
        {
            LaunchPad launchPad = new(startTileX, startTileY, endTileX, endTileY);

            launchPad.CompassCoordinates = Utility.GetCompassCoordinates(launchPad.Position);
            launchPad.Active = true;

            if (shouldSync)
                launchPad.NetSync(MacrocosmSubworld.CurrentID);

            LaunchPadManager.Add(MacrocosmSubworld.CurrentID, launchPad);

            return launchPad;
        }

        public static LaunchPad Create(int startTileX, int startTileY, bool shouldSync = true) => Create(startTileX, startTileY, startTileX, startTileY, shouldSync);
        public static LaunchPad Create(Point16 startTile, bool shouldSync = true) => Create(startTile.X, startTile.Y, shouldSync);
        public static LaunchPad Create(Point16 startTile, Point16 endTile, bool shouldSync = true) => Create(startTile.X, startTile.Y, endTile.X, endTile.Y, shouldSync);


        private int checkCounter;
        private const int checkCounterMax = 10;

        public void Update()
        {
            if(checkCounter++ >= checkCounterMax)
            {
                CheckMarkers();
                CheckRocket();
                //CheckTiles();

                checkCounter = 0;
            }
            
            if(spawned)
                Interact();

            if (!spawned)
            {
                CheckRocket();
                spawned = true;
            }
        }

        private void CheckMarkers()
        {
            if (Main.tile[StartTile].TileType != ModContent.TileType<LaunchPadMarker>())
            {
                Active = false;
                LaunchPadMarker.SetState(EndTile, MarkerState.Inactive);
                NetSync(MacrocosmSubworld.CurrentID);
                return;
            }

            if (Main.tile[EndTile].TileType != ModContent.TileType<LaunchPadMarker>())
            {
                Active = false;
                LaunchPadMarker.SetState(StartTile, MarkerState.Inactive);
                NetSync(MacrocosmSubworld.CurrentID);
                return;
            }
        }

        private void CheckRocket()
        {
            int prevRocketId = RocketID;
            RocketID = -1;

            for (int i = 0; i < RocketManager.MaxRockets; i++)
            {
                Rocket rocket = RocketManager.Rockets[i];
                if (rocket.ActiveInCurrentWorld && Hitbox.Intersects(rocket.Bounds))
                    RocketID = i;
            }

            if (RocketID != prevRocketId || !spawned)
            {
                if (RocketID < 0)
                {
                    LaunchPadMarker.SetState(StartTile, MarkerState.Vacant);
                    LaunchPadMarker.SetState(EndTile, MarkerState.Vacant);
                }
                else
                {
                    LaunchPadMarker.SetState(StartTile, MarkerState.Occupied);
                    LaunchPadMarker.SetState(EndTile, MarkerState.Occupied);
                }

                NetSync(MacrocosmSubworld.CurrentID);
            }
        }

        private void CheckTiles()
        {
            int tileY = StartTile.Y;
            bool foundObstruction = false;
            for (int tileX = StartTile.X; tileX <= EndTile.X; tileX++)
            {
                Tile tile = Main.tile[tileX, tileY];
                if (tile.HasTile)
                {
                    if (tile.TileType != ModContent.TileType<LaunchPadMarker>() && WorldGen.SolidOrSlopedTile(tileX, tileY))
                    {
                        foundObstruction = true;
                    }
                }
            }

            if (foundObstruction)
            {
                LaunchPadMarker.SetState(StartTile, MarkerState.Invalid);
                LaunchPadMarker.SetState(EndTile, MarkerState.Invalid);
                Active = false;
            }
        }

        private void Interact()
        {
            isMouseOver = Hitbox.Contains(Main.MouseWorld.ToPoint()) && Hitbox.InPlayerInteractionRange(TileReachCheckSettings.Simple);
            if (isMouseOver)
            {
                if (Main.mouseRight && Main.mouseRightRelease)
                {
                    UISystem.ShowAssemblyUI(this);
                }
                else
                {
                    if (!UISystem.Active)
                    {
                        Main.LocalPlayer.noThrow = 2;
                        CursorIcon.Current = CursorIcon.LaunchPad;
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 screenPosition)
        {
            Rectangle rect = Hitbox;
            rect.X -= (int)screenPosition.X;
            rect.Y -= (int)screenPosition.Y;

            if (isMouseOver)
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, rect, Color.Gold * 0.25f);
        }

        private int CountRequiredAssemblyItemSlots(Rocket rocket)
        {
            int count = 0;
            foreach (var kvp in rocket.Modules)
            {
                RocketModule module = kvp.Value;
                count += module.Recipe.Count();
            }

            return count;
        }
    }
}
