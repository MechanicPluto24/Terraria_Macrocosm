using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Macrocosm.Content.Gores
{
	public class RegolithDebris : ModGore
	{
		public override void OnSpawn(Gore gore, IEntitySource source)
		{
			gore.Frame = new SpriteFrame(1, 6);
			gore.Frame.CurrentRow = (byte)Main.rand.Next(6);
		}
	}
}
