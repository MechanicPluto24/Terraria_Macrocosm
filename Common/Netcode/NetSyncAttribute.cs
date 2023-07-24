using System;

namespace Macrocosm.Common.Netcode
{
	/// <summary> 
	/// This attribute is used for network syncing of 
	/// <see cref="Drawing.Particles.Particle"> Particle </see>, 
	/// <see cref="Content.Rockets.Rocket"> Rocket </see> 
	/// and <b>(TO FIX!!!)</b> ModNPC and ModProjectile 
	/// <b>fields</b> (not auto-properties) marked with this attribute.
	/// For ModNPC and ModProjectile, this should be used only after filling up the entirety of the already existing ai[] array.
	/// The fields are synced whenever:  <list type="bullet">
	///		<item> <see cref="Drawing.Particles.Particle.NetSync(int)"> Particle.NetSync() </see> is called </item>
	///		<item> <see cref="Content.Rockets.Rocket.NetSync(int)"> Rocket.NetSync() </see> is called </item>
	///		<item> NPC.netUpdate or Projectile.netUpdate are set to true, or the corresponding NetMessage is sent </item>
	/// </list> </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class NetSyncAttribute : Attribute { }
}
