using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm
{
	public class MNPC : GlobalNPC
	{
		public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Color drawColor)
		{
			BaseArmorData.lastShaderDrawObject = npc;			
			return base.PreDraw(npc, spriteBatch, drawColor);
		}
	}
}