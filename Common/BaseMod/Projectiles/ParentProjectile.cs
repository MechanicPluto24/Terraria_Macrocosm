using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace Macrocosm
{
	public abstract class ParentProjectile : ModProjectile
	{
		public virtual void SetAI(float[] ai, int aiType) { }
		public virtual Vector4 GetFrameV4(){ return new Vector4(0, 0, 1, 1); }		
	}
}
