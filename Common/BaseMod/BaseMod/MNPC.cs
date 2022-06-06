using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm
{
	public class MNPC : GlobalNPC
	{
		public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			BaseArmorData.lastShaderDrawObject = npc;			
			return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
		}
	}
}