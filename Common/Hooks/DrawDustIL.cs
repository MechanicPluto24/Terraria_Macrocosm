using System;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using Terraria;
using Terraria.ModLoader;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Macrocosm.Common.Drawing.Dusts;
using Macrocosm.Common.Utils;

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

			// matches :
			//	Dust dust = Main.dust[i];
			//		if (dust.active)
			if (!c.TryGotoNext(
				i => i.MatchLdsfld<Main>(nameof(Main.dust)),
				i => i.MatchLdloc(3),
				i => i.MatchLdelemRef(),
				i => i.MatchStloc(4),
				i => i.MatchLdloc(4),
				i => i.MatchLdfld<Dust>(nameof(Dust.active)) // <-
 			)) return;

			// move cursor to (ldfld bool Terraria.Dust::active)
			// target instruction gets the dust active status on the stack
			// next instruction will be a branch instruction that
			//	bypasses the dust drawing if false
			c.Index += 6;

			// load dust loop index on the stack
			c.Emit(OpCodes.Ldloc_3);

			// call method that reads the active field and the dust index from the stack 
			// the method calls the custom drawing logic, then pushes the return value on the stack
			// if it returns true, regular vanilla drawing will still happen
			// if false, dust will not be drawn, happens when:
			//		- dust is not active (same as vanilla)
			//		- vanilla dust drawing is disabled for this type in the custom drawing logic
			c.EmitDelegate(DustCustomDraw);
		}

		public static bool DustCustomDraw(bool active, int dustIndex) 
		{
			if (!active)
				return false;

			Dust dust = Main.dust[dustIndex];
			ModDust modDust = ModContent.GetModDust(dust.type);

			if (modDust is IDustCustomDraw dustDrawer)
				return dustDrawer.DrawDust(dust, Main.spriteBatch, Main.screenPosition, dust.GetTexture(), dust.frame);
 
			return true;
		}
	}
}