using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.DataStructures;

namespace Macrocosm.Content.Rocket
{
 	public class RocketMapLayer : ModMapLayer 
	{
 		public override void Draw(ref MapOverlayDrawContext context, ref string text) {

			var texture = ModContent.Request<Texture2D>("Macrocosm/Content/Rocket/RocketMap", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
			
			for(int i = 0; i < Main.maxNPCs; i++)
			{
				NPC npc = Main.npc[i];
				if (npc.type == ModContent.NPCType<RocketNPC>())
				{
					Vector2 position = new(
									      (npc.Center.X) / 16f,
									      (npc.position.Y + npc.height - 120) / 16f  
										);

					if (context.Draw(texture, position, Color.White, new SpriteFrame(1, 1, 0, 0), 1f, 1f, Alignment.Center).IsMouseOver)
 						text = "Rocket";
				}
 			}
		}
	}
}