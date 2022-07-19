using System.Collections.Generic;
using Terraria;
using SubworldLibrary;
using Terraria.WorldBuilding;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.Graphics.Effects;
using Macrocosm.Backgrounds.Moon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Macrocosm.Common.Utility.IO;
using Macrocosm.Common.Utility;
using Terraria.UI.Chat;
using Terraria.GameContent;
using ReLogic.Graphics;
using Terraria.GameInput;
using Macrocosm.Common.Drawing.Stars;

namespace Macrocosm.Content.Subworlds.Moon
{
    /// <summary>
    /// Moon terrain and crater generation by 4mbr0s3 2
    /// Why isn't anyone else working on this
    /// I have saved the day - Ryan
    /// </summary>
    public class Moon : Subworld
    {
        public const float TimeRate = 0.125f;
        public override bool NormalUpdates => true;
        public override int Width => 2000;
        public override int Height => 1200; // 200 tile padding for the hell-layer.
        public override bool ShouldSave => true;
        public override bool NoPlayerSaving => false;
        public override List<GenPass> Tasks => new()
        {
            new MoonGen("LoadingMoon", 1f, this)
        };

        private bool toEarth;
        private double animationTimer;
        private Texture2D lunaBackground;
        private Texture2D lunaAtmoBackground;
        private Texture2D earthBackground;
        private Texture2D earthAtmoBackground;
        private string chosenMessage;
        private StarsDrawing starsDrawing = new();
        private TextFileLoader textFileLoader = new();

        public override void OnEnter()
        {
            lunaBackground = ModContent.Request<Texture2D>("Macrocosm/Content/Subworlds/LoadingBackgrounds/Luna").Value;
            lunaAtmoBackground = ModContent.Request<Texture2D>("Macrocosm/Content/Subworlds/LoadingBackgrounds/LunaAtmo").Value;

            animationTimer = 0;
            toEarth = false;
            chosenMessage = ListRandom.Pick(textFileLoader.Parse("Content/Subworlds/Moon/MoonMessages"));

            starsDrawing.Clear();
            starsDrawing.SpawnStars(100, 175);

            SkyManager.Instance.Activate("Macrocosm:MoonSky");
            MoonSky.SpawnStarsOnMoon();
        }

        public override void OnExit()
        {
            earthBackground = ModContent.Request<Texture2D>("Macrocosm/Content/Subworlds/LoadingBackgrounds/Earth").Value;
            earthAtmoBackground = ModContent.Request<Texture2D>("Macrocosm/Content/Subworlds/LoadingBackgrounds/EarthAtmo").Value;

            animationTimer = 0;
            toEarth = true;
            chosenMessage = ListRandom.Pick(textFileLoader.Parse("Content/Subworlds/Earth/EarthMessages"));

            starsDrawing.Clear();
            starsDrawing.SpawnStars(100, 175);

            SkyManager.Instance.Deactivate("Macrocosm:MoonSky");
        }

        public override void Load()
        {
            // One Terraria day = 86400
            SubworldSystem.hideUnderworld = true;
            SubworldSystem.noReturn = true;
            Main.numClouds = 0;
            Main.raining = false;
        }

        public override void DrawSetup(GameTime gameTime)
        {
            PlayerInput.SetZoom_Unscaled();
            Main.instance.GraphicsDevice.Clear(Color.Black);
            this.DrawMenu(gameTime);
        }

        public override void DrawMenu(GameTime gameTime)
        {
            Color bodyColor = Color.White * (float)(animationTimer / 5) * 0.8f;  // color of the celestial body
            bodyColor.A = byte.MaxValue;                                         // keep it opaque

            if (toEarth)
            {
                Main.spriteBatch.Begin(0, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);
                Main.spriteBatch.Draw
                (
                    earthAtmoBackground,
                    new Rectangle(Main.screenWidth - earthAtmoBackground.Width, Main.screenHeight - earthAtmoBackground.Height + 50 - (int)(animationTimer * 10), earthAtmoBackground.Width, earthAtmoBackground.Height),
                    null,
                    bodyColor
                    
                );
                Main.spriteBatch.End();

                Main.spriteBatch.Begin(0, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);

                starsDrawing.Draw();

                Main.spriteBatch.Draw
                (
                    earthBackground,
                    new Rectangle(Main.screenWidth - earthBackground.Width, Main.screenHeight - earthBackground.Height + 50 - (int)(animationTimer * 10), earthBackground.Width, earthBackground.Height),
                    null,
                    bodyColor
                );
                string msgToPlayer = "Earth"; // Title
                Vector2 messageSize = FontAssets.DeathText.Value.MeasureString(msgToPlayer) * 1.2f;
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.DeathText.Value, msgToPlayer, new Vector2(Main.screenWidth / 2f - messageSize.X / 2f, messageSize.Y), new Color(94, 150, 255), 0f, Vector2.Zero, new Vector2(1.2f));
                Vector2 messageSize2 = FontAssets.DeathText.Value.MeasureString(chosenMessage) * 0.7f;
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.DeathText.Value, chosenMessage, new Vector2(Main.screenWidth / 2f - messageSize2.X / 2f, Main.screenHeight - messageSize2.Y - 20), Color.White, 0f, Vector2.Zero, new Vector2(0.7f));
            }
            else
            {
                Main.spriteBatch.Begin(0, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);
                Main.spriteBatch.Draw
                (
                    lunaAtmoBackground,
                    new Rectangle(Main.screenWidth - lunaAtmoBackground.Width, Main.screenHeight - lunaAtmoBackground.Height + 50 - (int)(animationTimer * 10), lunaAtmoBackground.Width, lunaAtmoBackground.Height),
                    null,
                    bodyColor
                );
                Main.spriteBatch.End();

                Main.spriteBatch.Begin(0, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);

                starsDrawing.Draw();

                Main.spriteBatch.Draw
                (
                    lunaBackground,
                    new Rectangle(Main.screenWidth - lunaBackground.Width, Main.screenHeight - lunaBackground.Height + 50 - (int)(animationTimer * 10), lunaBackground.Width, lunaBackground.Height),
                    null,
                    bodyColor
                );

                string msgToPlayer = "Earth's Moon"; // Title
                Vector2 messageSize = FontAssets.DeathText.Value.MeasureString(msgToPlayer) * 1.2f;
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.DeathText.Value, msgToPlayer, new Vector2(Main.screenWidth / 2f - messageSize.X / 2f, messageSize.Y), Color.White, 0f, Vector2.Zero, new Vector2(1.2f));
                Vector2 messageSize2 = FontAssets.DeathText.Value.MeasureString(chosenMessage) * 0.7f;
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.DeathText.Value, chosenMessage, new Vector2(Main.screenWidth / 2f - messageSize2.X / 2f, Main.screenHeight - messageSize2.Y - 20), Color.White, 0f, Vector2.Zero, new Vector2(0.7f));

            }

            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.DeathText.Value, Main.statusText, new Vector2((float)Main.screenWidth, (float)Main.screenHeight) / 2f - FontAssets.DeathText.Value.MeasureString(Main.statusText) / 2f, Color.White, 0f, Vector2.Zero, Vector2.One);

            Main.DrawCursor(Main.DrawThickCursor(false), false);

            Main.spriteBatch.End();

            animationTimer += 0.125;
            if (animationTimer > 5)
                animationTimer = 5;
        }
    }
}
