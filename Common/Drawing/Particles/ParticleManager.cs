using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Common.Drawing.Particles
{
    public enum ParticleDrawLayer
    {
        BeforeProjectiles,
        AfterProjectiles,
        BeforeNPCs,
        AfterNPCs,
        BeforeTiles
    }

    /// <summary> Particle system by sucss, Nurby & Feldy @ PellucidMod (RIP) </summary>
    public class ParticleManager : ModSystem
    {
        public static List<Type> Types { get; private set; }
        public static List<Particle> Particles { get; private set; }

        public static List<Texture2D> Textures;

        public override void Load()
        {
            Types = new List<Type>();
            Particles = new List<Particle>();
            Textures = new List<Texture2D>();

            On_Main.DrawBlack += DrawParticles_Tiles;
            On_Main.DrawProjectiles += DrawParticles_Projectiles;
            On_Main.DrawNPCs += DrawParticles_NPCs;
        }

        public override void Unload()
        {
            Types = null;
            Particles = null;
            Textures = null;

            On_Main.DrawBlack -= DrawParticles_Tiles;
            On_Main.DrawProjectiles -= DrawParticles_Projectiles;
            On_Main.DrawNPCs -= DrawParticles_NPCs;
        }

        public override void PreUpdateDusts()
        {
            UpdateParticles();
        }

        private static void UpdateParticles()
        {
            for (int i = 0; i < Particles.Count; i++)
            {
                Particle particle = Particles[i];
                particle.Update();

                if (!particle.Active)
                {
                    Particles.RemoveAt(i);
                    i--;
                }
            }
        }

        public static void ClearAllParticles()
        {
            for (int i = 0; i < Particles.Count; i++)
            {
                Particles[i].Kill();
            }
        }

        public override bool HijackSendData(int whoAmI, int msgType, int remoteClient, int ignoreClient, NetworkText text, int number, float number2, float number3, float number4, int number5, int number6, int number7)
        {
            if (Main.netMode == NetmodeID.Server && msgType == MessageID.FinishedConnectingToServer && remoteClient >= 0 && remoteClient < 255)
            {
                foreach (var particle in Particles)
                {
                    particle.NetSync(toClient: remoteClient);
                }
            }

            return false;
        }

        private static void DrawParticles(ParticleDrawLayer layer)
        {
            List<Particle> alphaBlendDrawers = new();

            foreach (Particle particle in Particles)
            {
                if (particle.DrawLayer == layer)
                {
                    if (particle.PreDrawAdditive(Main.spriteBatch, Main.screenPosition, Lighting.GetColor(particle.Position.ToTileCoordinates())))
                        alphaBlendDrawers.Add(particle);
                }
            }

            if (alphaBlendDrawers.Any())
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);

                foreach (var particle in alphaBlendDrawers)
                {
                    if (particle.DrawLayer == layer)
                        particle.Draw(Main.spriteBatch, Main.screenPosition, Lighting.GetColor(particle.Position.ToTileCoordinates()));
                }

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);
            }

            foreach (Particle particle in Particles)
            {
                if (particle.DrawLayer == layer)
                    particle.PostDrawAdditive(Main.spriteBatch, Main.screenPosition, Lighting.GetColor(particle.Position.ToTileCoordinates()));
            }
        }


        private static SpriteBatchState state1, state2, state3;
        private void DrawParticles_NPCs(On_Main.orig_DrawNPCs orig, Main self, bool behindTiles)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            state1.SaveState(spriteBatch);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);

            DrawParticles(ParticleDrawLayer.BeforeNPCs);

            spriteBatch.End();
            spriteBatch.Begin(state1);

            orig(self, behindTiles);

            state2.SaveState(spriteBatch);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);

            DrawParticles(ParticleDrawLayer.AfterNPCs);

            spriteBatch.End();
            spriteBatch.Begin(state2);
        }

        private void DrawParticles_Projectiles(On_Main.orig_DrawProjectiles orig, Main self)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);

            DrawParticles(ParticleDrawLayer.BeforeProjectiles);

            spriteBatch.End();

            orig(self);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);

            DrawParticles(ParticleDrawLayer.AfterProjectiles);

            spriteBatch.End();
        }

        private void DrawParticles_Tiles(On_Main.orig_DrawBlack orig, Main self, bool force)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;

            state3.SaveState(spriteBatch);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);

            DrawParticles(ParticleDrawLayer.BeforeTiles);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(state3);

            orig(self, force);
        }
    }
}
