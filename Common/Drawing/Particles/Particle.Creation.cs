using Microsoft.Xna.Framework;
using System;
using Terraria.GameContent.Drawing;

namespace Macrocosm.Common.Drawing.Particles
{
    public partial class Particle
    {
        /// <summary>
        /// Creates a new particle with the specified parameters. Sync only when absolutely necessary.
        /// </summary>
        /// <typeparam name="T">Type of the particle.</typeparam>
        /// <param name="position">Position of the newly created particle.</param>
        /// <param name="velocity">Velocity of the newly created particle.</param>
        /// <param name="rotation">Rotation of the newly created particle</param>
        /// <param name="scale">Scale of the newly created particle, as a 2-dimensional vector </param>
        /// <param name="shouldSync"> Whether to sync the particle spawn and its <see cref="Common.Netcode.NetSyncAttribute"> NetSync </see> fields </param>
        /// <returns> The particle instance </returns>
        public static T Create<T>(Vector2 position, Vector2 velocity, Vector2 scale, float rotation = 0f, bool shouldSync = false) where T : Particle
        {
            return ParticleManager.CreateParticle((Action<T>)(p =>
            {
                p.Position = position;
                p.Velocity = velocity;
                p.Rotation = rotation;
                p.Scale = scale;
            }), shouldSync);
        }

        /// <summary>
        /// Creates a new particle with the specified action. Sync only when absolutely necessary.
        /// </summary>
        /// <typeparam name="T">Type of the particle.</typeparam>
        /// <param name="particleAction">Action to invoke on the newly created particle.</param>
        /// <param name="shouldSync"> Whether to sync the particle spawn and its <see cref="Common.Netcode.NetSyncAttribute"> NetSync </see> fields </param>
        /// <returns> The particle instance </returns>
        public static T Create<T>(Action<T> particleAction, bool shouldSync = false) where T : Particle
            => ParticleManager.CreateParticle(particleAction, shouldSync);

        /// <summary>
        /// Creates a new vanilla managed particle.
        /// </summary>
        /// <param name="type"> The type of the particle, found in <see cref="ParticleOrchestraType"/> </param>
        /// <param name="position"> The position of the particle in the world </param>
        /// <param name="velocity"> The initial velocity of this particle </param>
        public static void Create(ParticleOrchestraType type, Vector2 position, Vector2 velocity, int uniqueInfoPiece = 0)
        {
            ParticleOrchestraSettings settings = new()
            {
                PositionInWorld = position,
                MovementVector = velocity,
                UniqueInfoPiece = uniqueInfoPiece
            };

            ParticleOrchestrator.SpawnParticlesDirect(type, settings);
        }
    }
}