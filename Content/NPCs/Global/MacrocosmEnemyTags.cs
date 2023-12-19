using Macrocosm.Content.Biomes;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Macrocosm.Content.NPCs.Global
{
	public interface IMacrocosmEnemy 
	{
	}

	public interface IMoonEnemy : IMacrocosmEnemy 
	{
		bool DropMoonstone => true;
    }
}
