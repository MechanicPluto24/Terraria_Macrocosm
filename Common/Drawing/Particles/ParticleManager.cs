using System;
using System.Collections.Generic;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

	/// <summary> Particle system by sucss, Nurby & Feldy @ PellucidMod </summary>
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

			Terraria.On_Main.DrawBlack += DrawParticles_Tiles;
			Terraria.On_Main.DrawProjectiles += DrawParticles_Projectiles;
			Terraria.On_Main.DrawNPCs += DrawParticles_NPCs;
		}

		public override void Unload()
		{
			Types = null;
			Particles = null;
			Textures = null;

			Terraria.On_Main.DrawBlack -= DrawParticles_Tiles;
			Terraria.On_Main.DrawProjectiles -= DrawParticles_Projectiles;
			Terraria.On_Main.DrawNPCs -= DrawParticles_NPCs;
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
			foreach (Particle particle in Particles)
			{
				if (particle.DrawLayer == layer)
				{
					try
					{
						Color lightColor = Lighting.GetColor((int)(particle.Position.X / 16), (int)(particle.Position.Y / 16));
						particle.Draw(Main.spriteBatch, Main.screenPosition, lightColor);
					}
					catch (ReLogic.Content.AssetLoadException e)
					{
						Main.NewText(e, Color.Red);
					}
				}
			}
		}

		private void DrawParticles_NPCs(Terraria.On_Main.orig_DrawNPCs orig, Terraria.Main self, bool behindTiles)
		{
			SpriteBatch spriteBatch = Main.spriteBatch;
			SpriteBatchState state1 = spriteBatch.SaveState();
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);
			DrawParticles(ParticleDrawLayer.BeforeNPCs);
			spriteBatch.End();
			spriteBatch.Restore(state1);
				
			orig(self, behindTiles);
			
			SpriteBatchState state2 = spriteBatch.SaveState();
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);
			DrawParticles(ParticleDrawLayer.AfterNPCs);
			spriteBatch.End();
			spriteBatch.Restore(state2);
		}
		
		private void DrawParticles_Projectiles(Terraria.On_Main.orig_DrawProjectiles orig, Terraria.Main self)
		{
			SpriteBatch spriteBatch = Main.spriteBatch;
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);
			DrawParticles(ParticleDrawLayer.BeforeProjectiles);
			spriteBatch.End();
			
			orig(self);
			
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);
			DrawParticles(ParticleDrawLayer.AfterProjectiles);
			spriteBatch.End();
		}

		private void DrawParticles_Tiles(Terraria.On_Main.orig_DrawBlack orig, Terraria.Main self, bool force)
		{
			SpriteBatch spriteBatch = Main.spriteBatch;

			SpriteBatchState state = spriteBatch.SaveState();
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);
			DrawParticles(ParticleDrawLayer.BeforeTiles);
			Main.spriteBatch.End();
			Main.spriteBatch.Restore(state);
			
			orig(self, force);
		}
	}
}
