using Macrocosm.Common.Netcode;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Drawing.Particles
{
    /// <summary> Particle.Netcode by sucss, Nurby & Feldy @ PellucidMod (RIP) </summary>
    public partial class Particle
    {
        public const int MaxParticles = ushort.MaxValue;
        public const int MaxParticleTypes = ushort.MaxValue;

        /// <summary>
        /// Syncs the particle and particle fields with <see cref="NetSyncAttribute"/> across all clients and the server.
        /// </summary>
        public void NetSync(int toClient = -1, int ignoreClient = -1)
        {
            if (Main.netMode == NetmodeID.SinglePlayer || WhoAmI < 0)
                return;

            ModPacket packet = Macrocosm.Instance.GetPacket();

            packet.Write((byte)MessageType.SyncParticle);
            packet.Write((ushort)WhoAmI);
            packet.Write((ushort)Type);

            if (this.NetWriteFields(packet)) // Check if the writer was able to write all the fields.
                packet.Send(toClient, ignoreClient);

            packet.Dispose();
        }

        /// <summary>
        /// Syncs a particle with data from the <see cref="BinaryReader"/>. Don't use this method outside <see cref="PacketHandler.HandlePacket(BinaryReader, int)"/>
        /// </summary>
        /// <param name="reader"></param>
        public static void ReceiveSyncParticle(BinaryReader reader, int sender)
        {
            int particleIndex = reader.ReadUInt16(); // the Particle WhoAmI
            int particleType = reader.ReadUInt16();  // the Type int index

            Particle particle;
            if (ParticleManager.Particles.Count <= particleIndex)
            {
                particle = (Particle)Activator.CreateInstance(ParticleManager.Types[particleType]);
                ParticleManager.Particles.Add(particle);
            }
            else
            {
                particle = ParticleManager.Particles[particleIndex];
            }

            particle.NetReadFields(reader);

            if (Main.netMode == NetmodeID.Server)
                particle.NetSync(ignoreClient: sender);
        }
    }
}
