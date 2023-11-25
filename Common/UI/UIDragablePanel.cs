using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace Macrocosm.Common.UI
{
    /// <summary> Dragable UI panel, courtesy of ExampleMod </summary>
    public class UIDragablePanel : UIPanel
    {
        public bool IgnoreChildren = true;

        // Stores the offset from the top left of the UIPanel while dragging
        private Vector2 offset;

        // A flag that checks if the panel is currently being dragged
        private bool dragging;

        public override void LeftMouseDown(UIMouseEvent evt)
        {

            base.LeftMouseDown(evt);

            IgnoreChildren = true;

            // Start dragging on mouse down
            if (!IgnoreChildren || evt.Target == this)
                DragStart(evt);
        }

        public override void LeftMouseUp(UIMouseEvent evt)
        {

            base.LeftMouseUp(evt);

            IgnoreChildren = true;


            // Start dragging on mouse up
            if (!IgnoreChildren || evt.Target == this)
                DragEnd(evt);
        }

        private void DragStart(UIMouseEvent evt)
        {
            offset = new Vector2(evt.MousePosition.X - Left.Pixels, evt.MousePosition.Y - Top.Pixels);
            dragging = true;
        }

        private void DragEnd(UIMouseEvent evt)
        {
            Vector2 endMousePosition = evt.MousePosition;
            dragging = false;

            Left.Set(endMousePosition.X - offset.X, 0f);
            Top.Set(endMousePosition.Y - offset.Y, 0f);

            Recalculate();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (ContainsPoint(Main.MouseScreen))
            {
                Main.LocalPlayer.mouseInterface = true;
            }

            if (dragging)
            {
                Left.Set(Main.MouseScreen.X - offset.X, 0f);
                Top.Set(Main.MouseScreen.Y - offset.Y, 0f);
                Recalculate();
            }

            var parentSpace = Parent.GetDimensions().ToRectangle();
            if (!GetDimensions().ToRectangle().Intersects(parentSpace))
            {

                Left.Pixels = MathHelper.Clamp(Left.Pixels, 0, parentSpace.Right - Width.Pixels);
                Top.Pixels = MathHelper.Clamp(Top.Pixels, 0, parentSpace.Bottom - Height.Pixels);

                Recalculate();
            }
        }
    }
}
