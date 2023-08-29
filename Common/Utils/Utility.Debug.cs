using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Common.Utils
{
	internal partial class Utility
    {
        public static void Draw(this Vector2 position, int drawTime)
        {
            DebugDrawing.DrawDatas.Add(
                new(position, drawTime)
            );
        }

        public static void DrawInWorld(this Vector2 position, int drawTime)
        {
            DebugDrawing.DrawDatas.Add(
                new(position - Main.screenPosition, drawTime)
            );
        }
    }

    internal class DebugDrawData
    {
        public DebugDrawData(Vector2 position, int drawTime)
        {
            Position = position;
            DrawTime = drawTime;
            InitialDrawTime = drawTime;
        }

        public Vector2 Position { get; }
        public int DrawTime { get; private set; }
        public int InitialDrawTime { get; }

        public bool Tick()
        {
            DrawTime--;
            if (DrawTime <= 0)
            {
                return false;
            }

            return true;
        }
    }

    internal class DebugDrawing : ILoadable
    {
        public static List<DebugDrawData> DrawDatas { get; private set; }
        private static Texture2D DebugCrossTexture { get; set; }
        public void Load(Mod mod)
        {
            DrawDatas = new();
            DebugCrossTexture = ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/DebugCross").Value;
            On_Main.Draw += Draw;
        }

        public void Unload()
        {
            On_Main.Draw -= Draw;
            DrawDatas = null;
        }

        private void Draw(On_Main.orig_Draw orig, Main self, GameTime gameTime)
        {
            orig(self, gameTime);

            Main.spriteBatch.Begin();
            for (int i = 0; i < DrawDatas.Count; i++)
            {
                DebugDrawData drawData = DrawDatas[i];
                if (drawData.Tick())
                {
                    Main.spriteBatch.Draw(
                        DebugCrossTexture,
                        drawData.Position - Vector2.One * 6,
                        Color.Lerp(Color.Red, Color.GhostWhite, MathF.Sin(Main.GameUpdateCount * 0.1f))
                    );
                }
                else
                {
                    DrawDatas.RemoveAt(i--);
                }
            }
            Main.spriteBatch.End();
        }
    }
}
