using Terraria;
using Terraria.ModLoader;

namespace Macrocosm
{
    public class MPlayer : ModPlayer
    {
		public static bool useItem = false;

		public override void SetControls() 
		{
			if(useItem)
			{
				useItem = false;
				Player.delayUseItem = false;
				Player.controlUseItem = true;
			}
		}
    }
}