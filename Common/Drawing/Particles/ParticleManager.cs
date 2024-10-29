using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
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
        BeforeTiles,
        PostInterface
    }

    /// <summary> Particle system by sucss, Nurby & Feldy @ PellucidMod (RIP) </summary>
    public class ParticleManager : ModSystem
    {
        public static List<Type> Types { get; private set; }
        public static List<Particle> Particles { get; private set; }
        public static List<Asset<Texture2D>> Textures { get; private set; }
        public static Dictionary<Type, Stack<Particle>> ParticlePools { get; private set; }

        public override void Load()
        {
            Types = new List<Type>();
            Particles = new List<Particle>();
            Textures = new List<Asset<Texture2D>>();
            ParticlePools = new Dictionary<Type, Stack<Particle>>();

            On_Main.DrawBlack += DrawParticles_Tiles;
            On_Main.DrawProjectiles += DrawParticles_Projectiles;
            On_Main.DrawNPCs += DrawParticles_NPCs;
        }

        public override void Unload()
        {
            Types = null;
            Particles = null;
            Textures = null;
            ParticlePools = null;

            On_Main.DrawBlack -= DrawParticles_Tiles;
            On_Main.DrawProjectiles -= DrawParticles_Projectiles;
            On_Main.DrawNPCs -= DrawParticles_NPCs;
        }

        /// <summary>
        /// Creates a new particle with the specified action. Sync only when absolutely necessary.
        /// </summary>
        /// <typeparam name="T">Type of the particle.</typeparam>
        /// <param name="particleAction">Action to invoke on the newly created particle.</param>
        /// <param name="shouldSync"> Whether to sync the particle spawn and its <see cref="Common.Netcode.NetSyncAttribute"> NetSync </see> fields </param>
        /// <returns> The particle instance </returns>
        public static T CreateParticle<T>(Action<T> particleAction, bool shouldSync = false) where T : Particle
        {
            T particle;

            if (ParticlePools.TryGetValue(typeof(T), out var pool) && pool.Count > 0)
                particle = (T)pool.Pop();
            else
                particle = (T)Activator.CreateInstance(typeof(T));

            particle.Reset();
            particle.Active = true;
            particleAction.Invoke(particle);

            if (!Particles.Contains(particle))
                Particles.Add(particle);

            if (shouldSync)
                particle.NetSync();

            return particle;
        }

        public static Particle CreateParticle(Type particleType, Action<Particle> particleAction = null, bool shouldSync = false)
        {
            Particle particle;

            if (ParticlePools.TryGetValue(particleType, out var pool) && pool.Count > 0)
                particle = pool.Pop();
            else
                particle = (Particle)Activator.CreateInstance(particleType);

            particle.Reset();
            particle.Active = true;
            particleAction?.Invoke(particle);

            if (!Particles.Contains(particle))
                Particles.Add(particle);

            if (shouldSync)
                particle.NetSync();

            return particle;
        }



        public override void ClearWorld()
        {
            ClearAllParticles();
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
                    particle.Kill();
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

        public static List<Particle> GetParticlesDrawnBy(object customDrawer)
        {
            var list = Particles.Where(p => p.CustomDrawer == customDrawer).ToList();
            return list;
        }

        public static void RemoveParticle(Particle particle)
        {
            Particles.Remove(particle);

            var type = particle.GetType();
            if (ParticlePools.TryGetValue(type, out var pool) && pool.Count < particle.MaxPoolCount)
                pool.Push(particle);
        }

        public override void PostSetupContent()
        {
            // Initialize particle pools after all content is loaded
            foreach (var particleType in Types)
            {
                var templateParticle = (Particle)Activator.CreateInstance(particleType);
                int poolCount = templateParticle.MaxPoolCount;

                if (poolCount > 0)
                {
                    Stack<Particle> pool = new(poolCount);

                    for (int i = 0; i < poolCount; i++)
                    {
                        var p = (Particle)Activator.CreateInstance(particleType);
                        pool.Push(p);
                    }

                    ParticlePools.Add(particleType, pool);
                }
            }
        }

        public override bool HijackSendData(int whoAmI, int msgType, int remoteClient, int ignoreClient, NetworkText text, int number, float number2, float number3, float number4, int number5, int number6, int number7)
        {
            if (Main.netMode == NetmodeID.Server && msgType == MessageID.FinishedConnectingToServer && remoteClient >= 0 && remoteClient < 255)
            {
                foreach (var particle in Particles)
                {
                    // TODO: add a NetImportant field?
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
                if (particle.DrawLayer == layer && !particle.HasCustomDrawer)
                {
                    if (particle.PreDrawAdditive(Main.spriteBatch, Main.screenPosition, Lighting.GetColor(particle.Position.ToTileCoordinates())))
                        alphaBlendDrawers.Add(particle);
                }
            }

            if (alphaBlendDrawers.Count > 0)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);

                foreach (var particle in alphaBlendDrawers)
                {
                    if (particle.DrawLayer == layer && !particle.HasCustomDrawer)
                        particle.Draw(Main.spriteBatch, Main.screenPosition, Lighting.GetColor(particle.Position.ToTileCoordinates()));
                }

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);
            }

            foreach (Particle particle in Particles)
            {
                if (particle.DrawLayer == layer && !particle.HasCustomDrawer)
                    particle.PostDrawAdditive(Main.spriteBatch, Main.screenPosition, Lighting.GetColor(particle.Position.ToTileCoordinates()));
            }
        }


        private static SpriteBatchState state1, state2, state3, state4;
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

        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            state4.SaveState(spriteBatch);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);

            DrawParticles(ParticleDrawLayer.PostInterface);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(state4);
        }
    }
}
