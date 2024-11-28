using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Machines
{
    public class WindTurbineLarge : MachineTile
    {
        public override short Width => 3;
        public override short Height => 11;

        public override MachineTE MachineTE => ModContent.GetInstance<WindTurbineLargeTE>();

        private static Asset<Texture2D> blades;
        private static Asset<Texture2D> turbine;

        private static float rotationCounter;

        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileWaterDeath[Type] = true;
            Main.tileLavaDeath[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style6x3);
            TileObjectData.newTile.Width = Width;
            TileObjectData.newTile.Height = Height;

            TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16];
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;

            TileObjectData.newTile.DrawYOffset = 2;

            TileObjectData.newTile.Origin = new Point16(1, Height - 1);
            TileObjectData.newTile.AnchorTop = new AnchorData();
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop, Width, 0);

            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(MachineTE.Hook_AfterPlacement, -1, 0, false);
            TileObjectData.newTile.UsesCustomCanPlace = true;

            TileObjectData.addTile(Type);

            AddMapEntry(new Color(134, 137, 139), CreateMapEntryName());
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            Tile tile = Main.tile[i, j];
            if (tile.TileFrameX / 18 == 1 && tile.TileFrameY / 18 == 0)
            {
                Main.instance.TilesRenderer.AddSpecialLegacyPoint(i, j);
            }
        }

        // Called once an update, can use for the rotation animation
        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            if (Math.Abs(Utility.WindSpeedScaled) > 0.1f)
                rotationCounter += (MathHelper.Pi * 0.04f) * Utility.WindSpeedScaled;

            rotationCounter = MathHelper.WrapAngle(rotationCounter);
        }

        private SpriteBatchState state;
        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
        {
            blades ??= ModContent.Request<Texture2D>(Texture + "_Blades");
            turbine ??= ModContent.Request<Texture2D>(Texture + "_Turbine");

            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            Vector2 tileDrawPosition = new Vector2(i, j) * 16f + zero - Main.screenPosition;
            Color drawColor = Lighting.GetColor(i, j);

            float turbineScale = 1f;
            Vector2 turbineOffset = new(8, 11);
            Matrix turbineMatrix = GetTurbineMatrix(tileDrawPosition + turbineOffset, state.Matrix);
            SpriteEffects turbineEffects = Utility.WindSpeedScaled > 0f ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Vector2 bladeOffset = new(Utility.WindSpeedScaled < 0f ? (6 + 9.6f * -Utility.WindSpeedScaled) : (1 + 6f * (1f - Utility.WindSpeedScaled)), y: 11);
            Matrix bladeMatrix = GetBladesMatrix(tileDrawPosition + bladeOffset, state.Matrix);
            float bladeRotation = rotationCounter - (MathHelper.Pi / 32 * (i / 4));

            state.SaveState(spriteBatch, continuous: true);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, state.DepthStencilState, state.RasterizerState, null, turbineMatrix);

            spriteBatch.Draw(turbine.Value, tileDrawPosition + turbineOffset, null, drawColor, 0f, turbine.Size() / 2f, turbineScale, turbineEffects, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, state.DepthStencilState, state.RasterizerState, null, bladeMatrix);

            spriteBatch.Draw(blades.Value, tileDrawPosition + bladeOffset, null, drawColor, bladeRotation, blades.Size() / 2f + new Vector2(-0.11f, 0f), 1f, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(state);
        }

        private static Matrix GetTurbineMatrix(Vector2 drawPosition, Matrix baseMatrix)
        {
            float radians = MathHelper.Clamp(MathHelper.PiOver2 * (1f - Math.Abs(Utility.WindSpeedScaled)), 0f, 1f);
            Matrix transformationMatrix =
                Matrix.CreateTranslation(-drawPosition.X, -drawPosition.Y, 0f) * // Translate to screen origin
                Matrix.CreateRotationY(radians) *                                // Apply Y skew
                Matrix.CreateTranslation(drawPosition.X, drawPosition.Y, 0f) *   // Translate back to original position
                Matrix.CreateScale(baseMatrix.M11, baseMatrix.M22, 0f);          // Apply scale
            return transformationMatrix;
        }

        private static Matrix GetBladesMatrix(Vector2 drawPosition, Matrix baseMatrix)
        {

            float radians = MathHelper.PiOver2 * Utility.WindSpeedScaled * 0.66f;
            Matrix transformationMatrix =
                Matrix.CreateTranslation(-drawPosition.X, -drawPosition.Y, 0f) * // Translate to screen origin
                Matrix.CreateRotationY(radians) *                                // Apply Y skew
                Matrix.CreateTranslation(drawPosition.X, drawPosition.Y, 0f) *   // Translate back to original position
                Matrix.CreateScale(baseMatrix.M11, baseMatrix.M22, 0f);          // Apply scale
            return transformationMatrix;
        }
    }
}
