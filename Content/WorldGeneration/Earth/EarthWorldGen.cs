using SubworldLibrary;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.GameContent.Events;
using Terraria.ModLoader;
using Macrocosm.Content.Subworlds;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.WorldBuilding;

namespace Macrocosm.Content
{
	class EarthWorldGen : ModSystem
	{
		public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
		{
			base.ModifyWorldGenTasks(tasks, ref totalWeight);
		}
	}
}
