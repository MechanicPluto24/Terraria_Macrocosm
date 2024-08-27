using Macrocosm.Content.Skies.Moon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace Macrocosm.Content.Skies.CraterDemon
{
    // Currently unused
    public class CraterDemonSky : CustomSky, ILoadable
    {
        private struct Meteor
        {
            public Vector2 Position;
            public float Depth;
            public int FrameCounter;
            public float Scale;
            public float StartX;
        }

        private Asset<Texture2D> backgroundTexture;
        private Asset<Texture2D> meteorTexture;

        private bool isActive;
        private Meteor[] meteors;
        private float fadeOpacity;

        private const string Path = "Macrocosm/Content/Skies/CraterDemon/";

        public CraterDemonSky()
        {
            if (Main.dedServ)
                return;

            backgroundTexture = ModContent.Request<Texture2D>(Path + "Background");
            meteorTexture = ModContent.Request<Texture2D>(Path + "Meteor");
        }

        public void Load(Mod mod)
        {
            if (Main.dedServ)
                return;

            SkyManager.Instance["Macrocosm:CraterDemonSky"] = new CraterDemonSky();
        }

        public void Unload()
        {
        }

        public override void Update(GameTime gameTime)
        {
            if (isActive)
                fadeOpacity = Math.Min(1f, 0.01f + fadeOpacity);
            else
                fadeOpacity = Math.Max(0f, fadeOpacity - 0.01f);

            float speed = 12000f;
            for (int i = 0; i < meteors.Length; i++)
            {
                meteors[i].Position.X -= speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                meteors[i].Position.Y += speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if ((double)meteors[i].Position.Y > Main.worldSurface * 16.0)
                {
                    meteors[i].Position.X = meteors[i].StartX;
                    meteors[i].Position.Y = -10000f;
                }
            }
        }

        public override Color OnTileColor(Color inColor) => new Color(Vector4.Lerp(inColor.ToVector4(), Vector4.One, fadeOpacity * 0.5f));

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (minDepth < float.MaxValue)
            {
                spriteBatch.Draw(backgroundTexture.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White * Math.Min(1f, (Main.screenPosition.Y - 2600f) / 8000f * fadeOpacity));
            }

            int meteorStart = -1;
            int meteorEnd = 0;

            for (int i = 0; i < meteors.Length; i++)
            {
                float depth = meteors[i].Depth;
                if (meteorStart == -1 && depth < maxDepth)
                    meteorStart = i;

                if (depth <= minDepth)
                    break;

                meteorEnd = i;
            }

            if (meteorStart == -1)
                return;

            Rectangle meteorArea = new(-1000, -1000, 4000, 4000);
            Vector2 screenCenter = Main.screenPosition + new Vector2(Main.screenWidth >> 1, Main.screenHeight >> 1);

            float meteorOpacity = Math.Min(1f, (Main.screenPosition.Y - 1000f) / 1000f);

            for (int j = meteorStart; j < meteorEnd; j++)
            {
                Vector2 meteorScale = new(1f / meteors[j].Depth, 0.9f / meteors[j].Depth);
                Vector2 position = (meteors[j].Position - screenCenter) * meteorScale + screenCenter - Main.screenPosition;

                int frame = meteors[j].FrameCounter / 3;
                meteors[j].FrameCounter = (meteors[j].FrameCounter + 1) % 12;

                if (meteorArea.Contains((int)position.X, (int)position.Y))
                    spriteBatch.Draw(meteorTexture.Value, position, new Rectangle(0, frame * (meteorTexture.Height() / 4), meteorTexture.Width(), meteorTexture.Height() / 4), Color.White * meteorOpacity, 0f, Vector2.Zero, meteorScale.X * 5f * meteors[j].Scale, SpriteEffects.None, 0f);
            }
        }

        public override float GetCloudAlpha() => (1f - fadeOpacity) * 0.3f + 0.7f;

        public override void Activate(Vector2 position, params object[] args)
        {
            fadeOpacity = 0.002f;
            isActive = true;
            meteors = new Meteor[150];
            for (int i = 0; i < meteors.Length; i++)
            {
                float progress = (float)i / (float)meteors.Length;
                meteors[i].Position.X = progress * ((float)Main.maxTilesX * 16f) + Main.rand.NextFloat() * 40f - 20f;
                meteors[i].Position.Y = Main.rand.NextFloat() * (0f - ((float)Main.worldSurface * 4f + 100f)) - 100f;
                if (!Main.rand.NextBool(3))
                    meteors[i].Depth = Main.rand.NextFloat() * 3f + 1.8f;
                else
                    meteors[i].Depth = Main.rand.NextFloat() * 5f + 4.8f;

                meteors[i].FrameCounter = Main.rand.Next(12);
                meteors[i].Scale = Main.rand.NextFloat() * 0.5f + 1f;
                meteors[i].StartX = meteors[i].Position.X;
            }

            Array.Sort(meteors, SortMethod);
        }

        private int SortMethod(Meteor meteor1, Meteor meteor2) => meteor2.Depth.CompareTo(meteor1.Depth);

        public override void Deactivate(params object[] args)
        {
            isActive = false;
        }

        public override void Reset()
        {
            isActive = false;
        }

        public override bool IsActive()
        {
            if (!isActive)
                return fadeOpacity > 0.001f;

            return true;
        }
    }

}


// 
