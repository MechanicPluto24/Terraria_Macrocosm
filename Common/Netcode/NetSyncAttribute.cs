using System;

namespace Macrocosm.Common.Netcode
{
	/// <summary> 
	/// This attribute is used for network syncing of Particle, (TO FIX) ModNPC and ModProjectile fields marked with this attribute.
	/// For ModNPC and ModProjectile, this should be used only after filling up the entirety of the already existing ai[] array.
	/// The fields are synced whenever:  <list type="">
	///		<item> - <see cref="Drawing.Particles.Particle.NetSync(int)"> Particle.NetSync() </see> is called </item>
	///		<item> - NPC.netUpdate or Projectile.netUpdate are set to true, or the corresponding NetMessage is sent </item>
	/// </list> </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class NetSyncAttribute : Attribute { }
}
