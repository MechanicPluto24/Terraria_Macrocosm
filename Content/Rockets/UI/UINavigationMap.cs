using Macrocosm.Common.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria.UI;

namespace Macrocosm.Content.Rockets.UI
{
	public class UINavigationMap : UIElement, IConsistentUpdateable
	{
		/// <summary> The next navigation map, switched on zoom in </summary>
		public UINavigationMap Next = null;

		/// <summary> The previous navigation map, switched on zoom out </summary>
		public UINavigationMap Prev = null;

		/// <summary> The next default navigation map, switched on non-targeted zoom in </summary>
		public UINavigationMap DefaultNext = null;

		/// <summary> The displayed background texture </summary>
		public Texture2D Texture;

		/// <summary> Whether this animation map is undergoing a transition animation </summary>
		public bool AnimationActive => showAnimationActive;

		/// <summary> All active targets found on this map </summary>
		public List<UINavigationTarget> Targets => Children.OfType<UINavigationTarget>().ToList();

		/// <summary> Whether this map has a next navigation map </summary>
		public bool HasNext => Next != null;

		/// <summary> Whether this map has a previous navigation map </summary>
		public bool HasPrev => Prev != null;

		/// <summary> Whether this map has a navigation map it should default to on non-targeted zoom in </summary>
		public bool HasDefaultNext => DefaultNext != null;

		// Each target of this map can link to another map on zoom in  
		private Dictionary<UINavigationTarget, UINavigationMap> nextTargetMap = new();

		private Texture2D animationPrevTexture;
		private bool showAnimationActive = false;

		private float opacity = 1f;
		private float transitionSpeed = 0.03f;

		public UINavigationMap(Texture2D texture, UINavigationMap next = null, UINavigationMap prev = null, UINavigationMap defaultNext = null)
		{
			Texture = texture;

			Next = next;
			Prev = prev;
			DefaultNext = defaultNext;
		}

		public override void OnInitialize()
		{
			Width.Set(0, 0.86f);
			Height.Set(0, 0.9f);
			HAlign = 0.49f;
			VAlign = 0.5f;
			SetPadding(0f);
		}

		// Use for animation
		public void Update()
		{
			if (showAnimationActive)
			{
				opacity += transitionSpeed;

				if (opacity >= 1f)
				{
					showAnimationActive = false;
					opacity = 1f;
				}
			}
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			if (nextTargetMap.Count > 0)
			{
				UINavigationTarget target = GetSelectedTarget();

				if (target != null && nextTargetMap.TryGetValue(target, out UINavigationMap navMap))
					Next = navMap;
				else
					Next = null;
			}
		}

		/// <summary>
		/// Adds a new target instance to the navigation map, nothing happens if a target with the same name already exists
		/// </summary>
		/// <param name="target"> The target instance </param>
		/// <param name="nextMap"> The optionally linked map correspondent to the passed target, user can switch to it on ZoomIn </param>
		public void AddTarget(UINavigationTarget target, UINavigationMap nextMap = null)
		{
			// Avoid having duplicate target names
			foreach (UINavigationTarget existingTarget in Targets)
				if (existingTarget.Name == target.Name)
					return;

			if (nextMap is not null)
				nextTargetMap.Add(target, nextMap);

			Append(target);
		}

		/// <summary>
		/// Removes a target with the specified name ID from the map, if found
		/// </summary>
		/// <param name="targetName"></param>
		public void RemoveTarget(string targetName)
		{
			if (TryFindTarget(targetName, out UINavigationTarget target))
				RemoveChild(target);
		}

		/// <summary>
		/// Attempts to find a target in this map by its name ID
		/// </summary>
		/// <param name="name"> The string name ID </param>
		/// <param name="target"> The output target, null if not found </param>
		/// <returns> True if said target has been found, false otherwise </returns>
		public bool TryFindTarget(string name, out UINavigationTarget target)
		{
			target = FindTarget(name);
			if (target is not null)
				return true;

			return false;
		}

		/// <summary>
		/// Find a target by its name ID; may return null
		/// </summary>
		/// <param name="name"> The string name ID </param>
		/// <returns> The target instance, null if not found </returns>
		public UINavigationTarget FindTarget(string name)
		{
			foreach (UINavigationTarget target in Targets)
				if (target.Name == name)
					return target;

			return null;
		}

		/// <summary>
		/// Find the selected target in the current map, returns null if not found
		/// </summary>
		/// <returns> The selected target, null if not found </returns>
		public UINavigationTarget GetSelectedTarget()
		{
			//if (showAnimationActive)
			//    return null;

			foreach (UINavigationTarget target in Targets)
				if (target.Selected)
					return target;

			return null;
		}

		/// <summary> Clears the "selected" state from all the existing targets </summary>
		public void DeselectAllTargets()
		{
			foreach (UINavigationTarget target in Targets)
				target.Selected = false;
		}

		/// <summary> Resets all targets to their initial state </summary>
		public void ResetAllTargets()
		{
			foreach (UINavigationTarget target in Targets)
			{
				target.Selected = false;
				target.ResetAnimation();
			}
		}

		/// <summary>
		/// Show a fade transition animation towards this map's texture, from another's
		/// </summary>
		/// <param name="previous"> The texture from which the current map's transitions from </param>
		public void ShowAnimation(UINavigationMap previous)
		{
			showAnimationActive = true;
			animationPrevTexture = previous.Texture;
			opacity = 0f;
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			CalculatedStyle dimensions = GetInnerDimensions();
			Rectangle rect = dimensions.ToRectangle();

			if (animationPrevTexture is not null && showAnimationActive)
				spriteBatch.Draw(animationPrevTexture, rect, null, Color.White * (1 - opacity));

			spriteBatch.Draw(Texture, rect, null, Color.White * opacity);
		}
	}
}