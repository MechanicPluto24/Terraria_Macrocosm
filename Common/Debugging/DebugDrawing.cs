using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Common.Debugging
{
    // Did a few changes to make this feature more concise
    // The draw methods are no longer extensions, use DebugDrawing.DrawCross(...)
    public class DebugDrawing : ILoadable
    {
        private struct DebugDrawData
        {
            public DebugDrawData(Vector2 position, int drawTime)
            {
                this.position = position;
                DrawTime = drawTime;
                InitialDrawTime = drawTime;
            }

            public DebugDrawData(Func<Vector2> positionFunction, int drawTime, int initialDrawTime) : this()
            {
                this.position = positionFunction();
                this.positionFunction = positionFunction;
                DrawTime = drawTime;
                InitialDrawTime = initialDrawTime;
            }

            public Vector2 Position
            {
                get
                {
                    if (positionFunction is null)
                    {
                        return position;
                    }
                    else
                    {
                        position = positionFunction.Invoke();
                        return position;
                    }
                }
            }

            public int DrawTime { get; private set; }
            public int InitialDrawTime { get; }

            private Func<Vector2> positionFunction;
            private Vector2 position;

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

        private static List<DebugDrawData> DrawDatas { get; set; }
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

        public static void DrawCross(Vector2 position, int drawTime)
        {
            DrawDatas.Add(
                new(position, drawTime)
            );
        }

        public static void DrawCrossInWorld(Vector2 position, int drawTime)
        {
            DrawDatas.Add(
                new(position - Main.screenPosition, drawTime)
            );
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
