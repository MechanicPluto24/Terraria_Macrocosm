using Macrocosm.Common.Netcode;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.CursorIcons;
using Macrocosm.Content.Rockets.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SubworldLibrary;
using System;
using System.IO;
using System.Net.Sockets;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Content.Rockets.LaunchPads
{
    public partial class LaunchPad 
    {
        [NetSync] public bool Active;
        [NetSync] public Point16 StartTile;
        [NetSync] public Point16 EndTile;
        [NetSync] public int RocketID = -1;
        [NetSync] public string CompassCoordinates = "";

        public bool HasRocket => RocketID >= 0;

        public int Width => EndTile.X + 1 - StartTile.X;
        public Rectangle Hitbox => new((int)(StartTile.X * 16f), (int)(StartTile.Y * 16f), Width * 16, 16);
        public Vector2 Position => new(((StartTile.X + (EndTile.X - StartTile.X) / 2f) * 16f), StartTile.Y * 16f);

        private bool isMouseOver;

        public LaunchPad()
        {
            StartTile = new();
            EndTile = new();
        }

        public LaunchPad(int startTileX, int startTileY, int endTileX, int endTileY)
        {
            StartTile = new(startTileX, startTileY);
            EndTile = new(endTileX, endTileY);
        }

        public LaunchPad(int startTileX, int startTileY) : this(startTileX, startTileY, startTileX, startTileY) { }

        public LaunchPad(Point16 startTile) : this(startTile.X, startTile.Y) { }

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

        public void Update()
        {
            int prevRocketId = RocketID;
            RocketID = -1;

            if (Main.tile[StartTile.ToPoint()].TileType != ModContent.TileType<LaunchPadMarker>() || (Main.tile[EndTile.ToPoint()].TileType != ModContent.TileType<LaunchPadMarker>()))
            {
                Active = false;
                NetSync(MacrocosmSubworld.CurrentID);
                return;
            }

            for (int i = 0; i < RocketManager.MaxRockets; i++)
            {
                Rocket rocket = RocketManager.Rockets[i];

                if (rocket.ActiveInCurrentWorld && Hitbox.Intersects(rocket.Bounds))
                    RocketID = i;
            }

            if (RocketID != prevRocketId)
                NetSync(MacrocosmSubworld.CurrentID);

            isMouseOver = Hitbox.Contains(Main.MouseWorld.ToPoint()) && Hitbox.InPlayerInteractionRange(TileReachCheckSettings.Simple);
            if (isMouseOver)
            {
                if (Main.mouseRight)
                {
                    RocketUISystem.ShowAssemblyUI(this);
                }
                else
                {
                    if (!RocketUISystem.AssemblyUIActive)
                    {
                        Main.LocalPlayer.noThrow = 2;
                        Main.LocalPlayer.cursorItemIconEnabled = true;
                        Main.LocalPlayer.cursorItemIconID = CursorIcon.GetType<Items.CursorIcons.QuestionMark>();
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
    }
}
