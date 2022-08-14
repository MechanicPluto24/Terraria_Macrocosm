using Macrocosm.Common.Utility;
using Macrocosm.Content.Subworlds.Moon;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Common.Hooks
{
	public class GoreGravity : ILoadable
	{
		public void Load(Mod mod)
		{
			On.Terraria.Gore.Update += LowerGoreGravity;
		}

		public void Unload() { }

		private static void LowerGoreGravity(On.Terraria.Gore.orig_Update orig, Gore self)
		{
			orig(self);

			if (SubworldSystem.IsActive<Moon>())
			{
				if (self.velocity.Y >= 3f)
					self.velocity.Y = 3f;

				if (self.velocity.Y <= -10f)
					self.velocity.Y = -10f;

				if (self.velocity.X <= -10f)
					self.velocity.X = -10f;
				
				if (self.velocity.X >= 10f)
					self.velocity.X = 10f;
			}
		}
	}
}