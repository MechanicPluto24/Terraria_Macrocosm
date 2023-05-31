using Macrocosm.Common.Utils;
using Macrocosm.Content.Subworlds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.UI;

namespace Macrocosm.Content.Rocket.Navigation
{
    public class UINavigationMap : UIElement
    {
        /// <summary> The displayed background texture </summary>
        public Texture2D Background;

        /// <summary> The next navigation map, switched on ZoomIn </summary>
        public UINavigationMap Next = null;

		/// <summary> The previous navigation map, switched on ZoomOut </summary>
		public UINavigationMap Prev = null;

        /// <summary> The next defaul navigation map, switched on non-targeted ZoomIn </summary>
        public UINavigationMap DefaultNext = null;

        public bool AnimationActive => showAnimationActive;

		private Dictionary<UIMapTarget, UINavigationMap> nextTargetChildMap = new();

        private bool showAnimationActive = false;
        private Texture2D animationPrevTexture;

        private float alpha = 1f;
        private float alphaSpeed = 0.02f;

        public UINavigationMap(Texture2D tex, UINavigationMap next = null, UINavigationMap prev = null, UINavigationMap defaultNext = null)
        {
            Background = tex;

            Next = next;
            Prev = prev;
            DefaultNext = defaultNext;
        }

        /// <summary>
        /// Adds a new target instance to the navigation map
        /// </summary>
        /// <param name="target"> The target instance </param>
        /// <param name="childMap"> The optionally linked map correspondent to the passed target, user can switch to it on ZoomIn </param>
        public void AddTarget(UIMapTarget target, UINavigationMap childMap = null)
        {
            if (childMap is not null)
                nextTargetChildMap.Add(target, childMap);

            Append(target);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (nextTargetChildMap.Count > 0)
            {
                UIMapTarget target = GetSelectedTarget();

                if (target != null && nextTargetChildMap.TryGetValue(target, out UINavigationMap navMap))
                    Next = navMap;
                else
                    Next = null;
            }

            if (showAnimationActive)
            {
				alpha += alphaSpeed;

                if (alpha >= 1f)
                {
                    showAnimationActive = false;
                    alpha = 1f;
                }
            } 
        }

        /// <summary>
        /// Attempts to find a target in this map by its ID
        /// </summary>
        /// <param name="ID"> The string ID </param>
        /// <param name="target"> The output target, null if not found </param>
        /// <returns> True if said target has been found, false otherwise </returns>
        public bool TryFindTargetBy(string ID, out UIMapTarget target)
        {
            target = FindTargetBy(ID);
            if (target is not null)
                return true;

            return false;
        }

        /// <summary>
        /// Find a target by its ID; may return null
        /// </summary>
        /// <param name="ID"> The string ID </param>
        /// <returns> The target instance, null if not found </returns>
        public UIMapTarget FindTargetBy(string ID)
        {
            foreach (UIElement element in Children)
            {
                if (element is UIMapTarget target && target.TargetID == ID)
                    return target;
            }

            return null;
        }

        /// <summary>
        /// Find the selected target in the current map, returns null if not found
        /// </summary>
        /// <returns> The selected target, null if not found </returns>
        public UIMapTarget GetSelectedTarget()
        {
           //if (showAnimationActive)
           //    return null;

            foreach (UIElement element in Children)
            {
                if (element is UIMapTarget target && target.Selected)
                    return target;
            }

            return null;
        }

        /// <summary> Clears the "selected" state from all the existing targets </summary>
        public void ClearAllTargets()
        {
            foreach (UIElement element in Children)
            {
                if (element is UIMapTarget target)
                    target.Selected = false;
            }
        }

        public override void OnInitialize()
        {
            Width.Set(Background.Width, 0);
            Height.Set(Background.Height, 0);
            HAlign = 0.5f;
            VAlign = 0.5f;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dimensions = GetDimensions();

            bool specialDraw = animationPrevTexture is not null && showAnimationActive;

            spriteBatch.Draw(TextureAssets.BlackTile.Value, GetDimensions().ToRectangle(), Color.Black);

            SpriteBatchState state = new();
            if (specialDraw)
            {
                state = spriteBatch.SaveState();
                spriteBatch.End();
                spriteBatch.Begin(BlendState.NonPremultiplied, state);

                spriteBatch.Draw(animationPrevTexture, dimensions.Position(), null, Color.White.NewAlpha(1 - alpha), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }

            spriteBatch.Draw(Background, dimensions.Position(), null, Color.White.NewAlpha(alpha), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            if (specialDraw)
                spriteBatch.Restore(state);
        }

        /// <summary>
        /// Show a fade transition animation towards this map's texture, from another's
        /// </summary>
        /// <param name="previousTexture"> The texture from which the current map's transitions from </param>
        public void ShowAnimation(Texture2D previousTexture)
        {
            showAnimationActive = true;
            animationPrevTexture = previousTexture;
            alpha = 0f;
            ClearAllTargets();
        }
    }
}