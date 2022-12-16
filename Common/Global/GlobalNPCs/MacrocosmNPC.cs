using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Common.Global.GlobalNPCs
{
	/// <summary>
	/// Global NPC for NPC instances
	/// </summary>
	public class MacrocosmNPC : GlobalNPC
	{
		public override bool InstancePerEntity => true;
		protected override bool CloneNewInstances => false;

		// this is only set on the local client
		// any projectiles who use this need syncing 
		public bool Targeted; 

		public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			if (Targeted)
				DrawCrosshair(npc, spriteBatch, screenPos, drawColor);
		}

		private void DrawCrosshair(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			Texture2D crosshair = ModContent.Request<Texture2D>("Macrocosm/Content/UI/Crosshair").Value;
			Color color = new Color(255, 255, 255, 64);
			Vector2 position = npc.Center - screenPos;
			float rotation = (float)(Main.timeForVisualEffects / 20);
			spriteBatch.Draw(crosshair, position, null, color, rotation, crosshair.Size() / 2, 1.5f, SpriteEffects.None, 0f);
		}
	}
}