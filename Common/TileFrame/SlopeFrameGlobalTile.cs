using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using MonoMod.Cil;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.ModLoader;

namespace Macrocosm.Common.TileFrame
{
    /// <summary>
    /// Interface for ModTiles that have special slope frames under certain conditions 
    /// </summary>
    public interface IHasConditionalSlopeFrames
    {
        /// <summary> Apply the special slope frames here by modifying the TileFrameX and TileFrameY </summary>
        public void ApplySlopeFrames(int i, int j);

        /// <summary>
        /// Whether this tile should be still drawn as sloped using the vanilla code or not. 
        /// Normally, it should bypass the slope drawing when the special slope frames have been applied.
        /// </summary>
        public bool ShouldBypassSlopeDrawing(TileDrawInfo drawInfo);
    }

    public interface IHasCustomDraw
    {
        public bool CustomDraw(Vector2 screenPosition, Vector2 screenOffset, int tileX, int tileY, TileDrawInfo drawData, Rectangle normalTileRect, Vector2 normalTilePosition);
    }

    /// <summary>
    /// Global tile for ModTiles that implement the <see cref="IHasConditionalSlopeFrames"/> interface.
    /// </summary>
    public class SlopeFrameGlobalTile : GlobalTile
    {
        private static bool ilSuccess;

        public override void Load()
        {
            On_TileDrawing.DrawBasicTile += On_TileDrawing_DrawBasicTile;
            IL_TileDrawing.DrawBasicTile += IL_TileDrawing_DrawBasicTile;
        }

        public override void Unload()
        {
            ilSuccess = false;
            On_TileDrawing.DrawBasicTile -= On_TileDrawing_DrawBasicTile;
            IL_TileDrawing.DrawBasicTile -= IL_TileDrawing_DrawBasicTile;
        }

        private void On_TileDrawing_DrawBasicTile(On_TileDrawing.orig_DrawBasicTile orig, TileDrawing self, Vector2 screenPosition, Vector2 screenOffset, int tileX, int tileY, TileDrawInfo drawData, Rectangle normalTileRect, Vector2 normalTilePosition)
        {
            ModTile modTile = TileLoader.GetTile(drawData.typeCache);
            if (modTile is not null && modTile is IHasCustomDraw customDrawer)
            {
                if (!customDrawer.CustomDraw(screenPosition, screenOffset, tileX, tileY, drawData, normalTileRect, normalTilePosition))
                    return;
            }

            orig(self, screenPosition, screenOffset, tileX, tileY, drawData, normalTileRect, normalTilePosition);
        }

        // Special slope frames are applied here
        public override bool TileFrame(int i, int j, int type, ref bool resetFrame, ref bool noBreak)
        {
            ModTile modTile = TileLoader.GetTile(type);
            if (ilSuccess && modTile is not null && Main.tile[i, j].IsSloped() && modTile is IHasConditionalSlopeFrames tileWithSlopeFrames)
                tileWithSlopeFrames.ApplySlopeFrames(i, j);

            return base.TileFrame(i, j, type, ref resetFrame, ref noBreak);
        }

        // ILHook to allow bypassing the vanilla sloping drawcode
        private void IL_TileDrawing_DrawBasicTile(ILContext il)
        {
            var c = new ILCursor(il);
            ILLabel skipSlopeDrawing = c.MarkLabel();

            // Match the "drawData.tileCache.slope() > 0" check
            if (!c.TryGotoNext(
                i => i.MatchLdarg(5), // TileDrawInfo drawdata method parameter
                i => i.MatchLdflda(typeof(TileDrawInfo), "tileCache"),
                i => i.MatchCall<Tile>("slope"),
                i => i.MatchLdcI4(0),
                i => i.MatchBle(out skipSlopeDrawing) // get the branch label that skips the slope drawcode if the check is false
            ))
            {
                Macrocosm.Instance.Logger.Error("Failed to inject ILHook: SlopeFrame");
                ilSuccess = false;
                return;
            }

            c.Index += 5;

            // Bypass the code when the special slope frames have been applied 
            c.EmitLdarg(5); // TileDrawInfo drawdata method parameter
            c.EmitDelegate(HideVisualSlope);
            c.EmitBrtrue(skipSlopeDrawing);

            ilSuccess = true;
        }

        private static bool HideVisualSlope(TileDrawInfo drawData)
        {
            ModTile modTile = TileLoader.GetTile(drawData.typeCache);
            if (modTile is not null && modTile is IHasConditionalSlopeFrames tileWithSlopeFrames)
                return tileWithSlopeFrames.ShouldBypassSlopeDrawing(drawData);

            return false;
        }
    }
}
