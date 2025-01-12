using Macrocosm.Common.Bases.Tiles;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.IO;
using Terraria.ModLoader;

namespace Macrocosm.Common.Hooks
{
    // Unused. Needs more attention (or tML integration)
    public interface IPreviewDrawTile
    {
        public bool PreDrawPreview(int i, int j, SpriteBatch spriteBatch, Color previewColor) { return true; }
        public void PostDrawPreview(int i, int j, SpriteBatch spriteBatch, Color previewColor) { }
        public void SpecialDrawPreview(int i, int j, SpriteBatch spriteBatch, Color previewColor) { }
    }

    internal class ObjectPreviewHooks : ILoadable
    {
        public void Load(Mod mod)
        {
            //On_TileObject.DrawPreview += On_TileObject_DrawPreview;
        }

        public void Unload()
        {
            //On_TileObject.DrawPreview -= On_TileObject_DrawPreview;
        }

        private void On_TileObject_DrawPreview(On_TileObject.orig_DrawPreview orig, SpriteBatch sb, TileObjectPreviewData op, Vector2 position)
        {
            if (TileLoader.GetTile(op.Type) is IPreviewDrawTile preview)
            {
                for (int i = 0; i < op.Size.X; i++)
                {
                    for (int j = 0; j < op.Size.Y; j++)
                    {
                        int data = op[i, j];
                        if (data == TileObjectPreviewData.None) continue;

                        Color color = (data == TileObjectPreviewData.InvalidSpot) ? Color.Red * 0.7f : Color.White;
                        int x = op.Coordinates.X + i;
                        int y = op.Coordinates.Y + j;

                        if (!preview.PreDrawPreview(x, y, sb, color))
                            return;
                    }
                }

                orig(sb, op, position);

                for (int i = 0; i < op.Size.X; i++)
                {
                    for (int j = 0; j < op.Size.Y; j++)
                    {
                        int data = op[i, j];

                        if (data == TileObjectPreviewData.None) 
                            continue;

                        Color color = (data == TileObjectPreviewData.InvalidSpot) ? Color.Red * 0.7f : Color.White;
                        int x = op.Coordinates.X + i;
                        int y = op.Coordinates.Y + j;

                        preview.PostDrawPreview(x, y, sb, color);
                        //preview.SpecialDrawPreview(x, y, sb, color);
                    }
                }
            }
            else
            {
                orig(sb, op, position);
            }
        }

    }
}
