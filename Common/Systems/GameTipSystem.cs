using Terraria;
using Macrocosm.Common.Subworlds;
using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader;

namespace Macrocosm.Common.Systems
{

	public class GameTipSystem : ModSystem
	{


		public override void ModifyGameTipVisibility(IReadOnlyList<GameTipData> gameTips)
		{
			foreach(var data in gameTips) 
			{
				//if (!data.FullName.Contains("Macrocosm/Moon"))
				//	data.Hide();
			}
		}
	}
}