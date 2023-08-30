using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
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

			state1 = state2 = state3 = null;

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

		private static void DrawParticles(ParticleDrawLayer layer)
		{
			List<Particle> alphaBlendDrawers = new();
			List<Particle> additiveOnlyPostDrawers = new();

			foreach (Particle particle in Particles)
			{
				if (particle.DrawLayer == layer)
				{
					if (particle.PreDrawAdditive(Main.spriteBatch, Main.screenPosition, Lighting.GetColor(particle.Position.ToTileCoordinates())))
 						alphaBlendDrawers.Add(particle);
 					else
 						additiveOnlyPostDrawers.Add(particle);
 				}
			}

			if (alphaBlendDrawers.Any())
			{
				Main.spriteBatch.End();
				Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);

				foreach (var particle in alphaBlendDrawers)
				{
					particle.Draw(Main.spriteBatch, Main.screenPosition, Lighting.GetColor(particle.Position.ToTileCoordinates()));
				}

				Main.spriteBatch.End();
				Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);
			}

			if (additiveOnlyPostDrawers.Any())
			{
				foreach (var particle in additiveOnlyPostDrawers)
				{
					particle.PostDrawAdditive(Main.spriteBatch, Main.screenPosition, Lighting.GetColor(particle.Position.ToTileCoordinates()));
				}
			}
		}


		private static SpriteBatchState state1, state2, state3;
		private void DrawParticles_NPCs(On_Main.orig_DrawNPCs orig, Main self, bool behindTiles)
		{
			SpriteBatch spriteBatch = Main.spriteBatch;
			state1 = spriteBatch.SaveState();
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);

			DrawParticles(ParticleDrawLayer.BeforeNPCs);

			spriteBatch.End();
			spriteBatch.Begin(state1);
				
			orig(self, behindTiles);
			
			state2 = spriteBatch.SaveState();
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

			state3 = spriteBatch.SaveState();
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);

			DrawParticles(ParticleDrawLayer.BeforeTiles);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(state3);
			
			orig(self, force);
		}
	}
}
