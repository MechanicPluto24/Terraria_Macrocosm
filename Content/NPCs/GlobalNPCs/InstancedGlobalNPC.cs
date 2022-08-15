using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.NPCs.GlobalNPCs
{
	public class InstancedGlobalNPC : GlobalNPC
	{
		public override bool InstancePerEntity => true;
		protected override bool CloneNewInstances => false;

		public bool[] targetedBy = new bool[Main.maxPlayers];

		public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			if (targetedBy[Main.LocalPlayer.whoAmI])
				DrawCrosshair(npc, spriteBatch, screenPos, drawColor);
		}

		private void DrawCrosshair(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			Texture2D crosshair = ModContent.Request<Texture2D>("Macrocosm/Assets/UI/Crosshair").Value;
			Color color = new Color(255, 255, 255, 64);
			Vector2 position = npc.Center - screenPos;
			float rotation = (float)(Main.timeForVisualEffects / 20);
			spriteBatch.Draw(crosshair, position, null, color, rotation, crosshair.Size() / 2, 1.5f, SpriteEffects.None, 0f);
		}
	}
}