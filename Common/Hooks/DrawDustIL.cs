using Terraria.ModLoader;
using MonoMod.Cil;
using Microsoft.Xna.Framework.Graphics;
using System;
using Mono.Cecil.Cil;
using Terraria;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;

namespace Macrocosm.Common.Hooks
{
	public class DrawDustIL : ILoadable
	{
		public void Load(Mod mod)
		{
			IL.Terraria.Main.DrawDust += Main_DrawDust;
		}
		public void Unload()
		{
			IL.Terraria.Main.DrawDust -= Main_DrawDust;
		}

		private void Main_DrawDust(ILContext il)
		{
			var c = new ILCursor(il);


			if (!c.TryGotoNext(
				i => i.MatchLdsfld<Main>(nameof(Main.dust)),
				i => i.MatchLdloc(3),
				i => i.MatchLdelemRef(),
				i => i.MatchStloc(4),
				i => i.MatchLdloc(4),
				i => i.MatchLdfld<Dust>(nameof(Dust.active))
 			)) return;

			c.Index += 7;
			c.Emit(OpCodes.Ldloc_3); // load dust index of loop on the stack
			c.EmitDelegate(DrawDustExtra);
		}

		public static void DrawDustExtra(int dustIndex) 
		{
			Dust dust = Main.dust[dustIndex];
			if(dust.type == ModContent.DustType<LuminiteSparkDust>())
			{
				float count = Math.Abs(dust.velocity.X) + Math.Abs(dust.velocity.Y);
				count *= 0.3f;
				count *= 10f;

				if (count > 10f)
					count = 10f;

				for (int n = 0; n < count; n++)
				{
					Vector2 velocity = dust.velocity;
					Vector2 vector = dust.position - velocity * n;
					float scale = dust.scale * (1f - n / 10f);
					Color color = Lighting.GetColor((int)((double)dust.position.X + 4.0) / 16, (int)((double)dust.position.Y + 4.0) / 16);
					color = dust.GetAlpha(color);
 					Main.spriteBatch.Draw(ModContent.GetModDust(dust.type).Texture2D.Value, vector - Main.screenPosition, dust.frame, color, dust.rotation, new Vector2(4f, 4f), scale, SpriteEffects.None, 0f);
 				}
			}
		}
	}
}