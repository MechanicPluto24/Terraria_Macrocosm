using Macrocosm.Common.Utils;
using Terraria;

namespace Macrocosm.Common.Global.GlobalProjectiles
{
	public interface IExplosive
	{
		/// <summary> The explosion blast radius </summary>
		public float BlastRadius { get; }
		
		/// <summary> Called when the projectile hits anything. Calls <see cref="Utility.Explode(Projectile, float)"/>. Implement this only for otherwise special behaviour. </summary>
		public void OnCollide(Projectile projectile)
		{
			projectile.Explode(BlastRadius);
		}
 	}
}
