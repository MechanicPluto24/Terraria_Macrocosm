using MonoMod.Cil;
using Terraria.ModLoader;
using Terraria.GameContent.Drawing;
using Macrocosm.Common.Utils;
using Mono.Cecil.Cil;
using System;
using Terraria;

namespace Macrocosm.Common.Hooks
{
	// Currently not working as expected
	internal class ForceTileRender : ILoadable
	{
		public void Load(Mod mod)
		{
			//IL_TileDrawing.DrawSingleTile += IL_TileDrawing_DrawSingleTile;
		}

		public void Unload()
		{
			//IL_TileDrawing.DrawSingleTile -= IL_TileDrawing_DrawSingleTile;
		}

		private void IL_TileDrawing_DrawSingleTile(ILContext il)
		{
			var c = new ILCursor(il);

			// Matches:
			//
			// bool flag = false;
			// if( ... || drawData.tileLight.B >= 1)
			//		flag = true;
			// 
			// "flag" means tile should be rendered
			if (!c.TryGotoNext(
					i => i.Match(OpCodes.Ldc_I4_1),
					i => i.Match(OpCodes.Blt_S),
					i => i.Match(OpCodes.Ldc_I4_1),
					i => i.Match(OpCodes.Stloc_3)
				))
			{
				Utility.LogILHookFail("Unable to match IL in ForceTileRender", il);
				return;
			}

			// Move cursor after Stloc_3
			c.Index += 4;

			// Load the current value of loc.3 (flag) onto the evaluation stack
			// A value of true means that the tile is sufficiently lit to be rendered 
			c.EmitLdloc3();

			// Load tile coordinates onto the evaluation stack
			c.EmitLdarg(6); // tileX
			c.EmitLdarg(7); // tileY

			// Call ForceRender. Sets flag to true if it was false before, forcing the tile render.
			c.EmitDelegate(ForceRender);

			// Store updated flag value
			c.EmitStloc3();
		}

		private bool ForceRender(bool rendered, int tileX, int tileY)
		{
			return rendered;
		}
	}
}
