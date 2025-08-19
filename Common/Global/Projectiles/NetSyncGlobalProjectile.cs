using Macrocosm.Common.Netcode;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Common.Global.Projectiles;

public class NetSyncGlobalProjectile : GlobalProjectile
{
    public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter binaryWriter)
    {
        if (projectile.ModProjectile is null)
            return;

        projectile.ModProjectile.NetWrite(binaryWriter, bitWriter);
    }

    public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader binaryReader)
    {
        if (projectile.ModProjectile is null)
            return;

        projectile.ModProjectile.NetRead(binaryReader, bitReader);
    }
}