using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.Drawing;

namespace Macrocosm.Common.Drawing.Particles
{
	public partial class Particle
	{
		/// <summary>
		/// Creates a new particle with the specified position and velocity.
		/// </summary>
		/// <typeparam name="T">Type of the particle.</typeparam>
		/// <param name="position">Position of the newly created particle.</param>
		/// <param name="velocity">Velocity of the newly created particle.</param>s
		/// <returns></returns>
		public static T CreateParticle<T>(Vector2 position, Vector2 velocity, float rotation = 0f, float scale = 1f, bool shouldSyncOnSpawn = false) where T : Particle
		{
			return CreateParticle<T>(particle =>
			{
				particle.Position = position;
				particle.Velocity = velocity;
				particle.Rotation = rotation;
				particle.Scale = scale;
				particle.ShouldSyncOnSpawn = shouldSyncOnSpawn;
			});
		}

		/// <summary>
		/// Creates a new particle with the specified action.
		/// </summary>
		/// <typeparam name="T">Type of the particle.</typeparam>
		/// <param name="particleAction">Action to invoke on the newly created particle.</param>
		/// <returns></returns>
		public static T CreateParticle<T>(Action<T> particleAction) where T : Particle
		{
			T particle = (T)Activator.CreateInstance(typeof(T));
			ParticleManager.Particles.Add(particle);

			particleAction.Invoke(particle);

			if(particle.ShouldSyncOnSpawn)
				particle.NetSync();

			return particle;
		}

		/// <summary>
		/// Creates a new vanilla managed particle.
		/// </summary>
		/// <param name="type"> The type of the particle, found in <see cref="ParticleOrchestraType"/> </param>
		/// <param name="position"> The position of the particle in the world </param>
		/// <param name="velocity"> The initial velocity of this particle </param>
		public static void CreateParticle(ParticleOrchestraType type, Vector2 position, Vector2 velocity)
		{
			ParticleOrchestraSettings settings = new()
			{
				PositionInWorld = position,
				MovementVector = velocity
			};

			ParticleOrchestrator.SpawnParticlesDirect(type, settings);
		}
	}
}
