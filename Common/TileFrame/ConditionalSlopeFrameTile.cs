using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Macrocosm.Common.Utils;

namespace Macrocosm.Common.TileFrame
{
	public interface IHasConditionalSlopeFrames
	{
		public void ApplySlopeFrames(int i, int j);
		public bool ShouldDrawAsSloped(TileDrawInfo drawInfo);
	}

	public class ConditionalSlopeFrameTile : GlobalTile
	{
		public override void Load()
		{
			On_TileDrawing.DrawBasicTile += On_TileDrawing_DrawBasicTile;
		}

		public override void Unload()
		{
			On_TileDrawing.DrawBasicTile -= On_TileDrawing_DrawBasicTile;
		}

		public override bool TileFrame(int i, int j, int type, ref bool resetFrame, ref bool noBreak)
		{
			ModTile modTile = TileLoader.GetTile(type);
			if (modTile is not null && Main.tile[i,j].IsSloped() && modTile is IHasConditionalSlopeFrames tileWithSlopeFrames)
				tileWithSlopeFrames.ApplySlopeFrames(i, j);

			return base.TileFrame(i, j, type, ref resetFrame, ref noBreak);
		}

		// This doesn't cut it. Replace with IL edit next to the "slope() > 0" check...
		private void On_TileDrawing_DrawBasicTile(On_TileDrawing.orig_DrawBasicTile orig, TileDrawing self, Vector2 screenPosition, Vector2 screenOffset, int tileX, int tileY, TileDrawInfo drawData, Rectangle normalTileRect, Vector2 normalTilePosition)
		{
			HideVisualSlope(drawData);
			orig(self, screenPosition, screenOffset, tileX, tileY, drawData, normalTileRect, normalTilePosition);
		}

		private static void HideVisualSlope(TileDrawInfo drawData)
		{
			ModTile modTile = TileLoader.GetTile(drawData.typeCache);
			if (modTile is not null && modTile is IHasConditionalSlopeFrames tileWithSlopeFrames && tileWithSlopeFrames.ShouldDrawAsSloped(drawData))
				drawData.tileCache.Slope = SlopeType.Solid;
		}
	}
}
