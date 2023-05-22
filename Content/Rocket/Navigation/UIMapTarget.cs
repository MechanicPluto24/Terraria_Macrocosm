using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rocket.Navigation.LaunchConds;
using Macrocosm.Content.Subworlds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SubworldLibrary;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Content.Rocket.Navigation
{
    public class UIMapTarget : UIElement
    {
        public UINavigationPanel OwnerPanel { get; set; }
        public UINavigationMap OwnerMap => OwnerPanel.CurrentMap;

        public readonly string TargetID = "default";

        /// <summary> Determine whether the subworld is accesible </summary>
        public LaunchConditions LaunchConditions { get; set; } = null;
        public bool IsReachable => CheckLaunchConditions() || OwnerMap.Next != null;
        public bool AlreadyHere => TargetID == MacrocosmSubworld.SafeCurrentID;

        /// <summary> Target selected </summary>
        public bool Selected;

        // selection outline, has default
        private readonly Texture2D selectionOutline = ModContent.Request<Texture2D>("Macrocosm/Content/Rocket/Navigation/Buttons/SelectionOutlineSmall", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

        // selection outline rotation
        private float rotation = 0f;

        /// <summary> Creates a new UIMapTarget based on a target Macrocosm Subworld </summary>
        /// <param name="owner"> The owner navigation panel </param>
        /// <param name="position"> The map target's position relative to the top corner of the NavigationMap </param>
        /// <param name="width"> Interactible area width in pixels </param>
        /// <param name="height"> Interactible area height in pixels </param>
        /// <param name="targetSubworld"> The subworld (instance) associated with the map target </param>
        public UIMapTarget(UINavigationPanel owner, Vector2 position, float width, float height, MacrocosmSubworld targetSubworld, Texture2D outline = null) : this(owner, position, width, height)
        {
			LaunchConditions = targetSubworld.LaunchConditions;
            TargetID = targetSubworld.Name;

            selectionOutline = outline ?? selectionOutline;
        }

        /// <summary>  Creates a new UIMapTarget with the given custom data. Used for special navigation, like towards Earth  </summary>
        /// <param name="owner"> The owner navigation panel </param>
        /// <param name="position"> The map target's position relative to the top corner of the NavigationMap </param>
        /// <param name="width"> Interactible area width in pixels </param>
        /// <param name="height"> Interactible area height in pixels </param>
        /// <param name="targetId"> The special ID of the target, handled in <see cref="RocketNPC.EnterDestinationSubworld"/> </param>
        /// <param name="canLaunch"> Function that determines whether the target is selectable, defaults to false </param>
        public UIMapTarget(UINavigationPanel owner, Vector2 position, float width, float height, string targetId, LaunchConditions launchConditions = null, Texture2D outline = null) : this(owner, position, width, height)
        {
			this.LaunchConditions = launchConditions;
            TargetID = targetId;

            selectionOutline = outline ?? selectionOutline;
        }

        /// <summary> Creates a new UIMapTarget </summary>
        /// <param name="owner"> The owner navigation panel </param>
        /// <param name="position"> The map target's position relative to the top corner of the NavigationMap </param>
        /// <param name="width"> Interactible area width in pixels </param>
        /// <param name="height"> Interactible area height in pixels </param>
        private UIMapTarget(UINavigationPanel owner, Vector2 position, float width, float height)
        {
            OwnerPanel = owner;
            Width.Set(width + 40, 0);
            Height.Set(height + 40, 0);
            Top.Set(position.Y - 20, 0);
            Left.Set(position.X - 20, 0);
        }

		public override void OnInitialize()
        {
            OnLeftClick += (_, _) =>
            {
                foreach (UIElement element in OwnerMap.Children)
                {
                    if (element is UIMapTarget target && !ReferenceEquals(target, this))
                        target.Selected = false;
                }
                Selected = true;
            };

            OnRightClick += (_, _) => { Selected = false; };

            OnLeftDoubleClick += (_, _) => { OwnerPanel.ZoomIn(useDefault: false); };

            //OnRightDoubleClick += (_, _) => { OwnerPanel.ZoomOut();  };
        }

		public bool CheckLaunchConditions()
		{
			if (LaunchConditions is not null)
				return LaunchConditions.Check();

			return false;
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Rectangle rect = GetDimensions().ToRectangle();
            Vector2 pos = GetDimensions().Center();
            Vector2 origin = new(selectionOutline.Width / 2f, selectionOutline.Height / 2f);

            rotation += 0.006f;
            if (!Selected)
                rotation = 0f;

            var state = spriteBatch.SaveState();
            spriteBatch.End();
            spriteBatch.Begin(BlendState.NonPremultiplied, state);

            if (Selected)
            {
                if (IsReachable)
                    spriteBatch.Draw(selectionOutline, pos, null, new Color(0, 255, 0), rotation, origin, 1f, SpriteEffects.None, 0f);
                else if (AlreadyHere)
                    spriteBatch.Draw(selectionOutline, pos, null, Color.Gray, rotation, origin, 1f, SpriteEffects.None, 0f);
                else
                    spriteBatch.Draw(selectionOutline, pos, null, new Color(255, 0, 0), rotation, origin, 1f, SpriteEffects.None, 0f);
            }
            else if (IsMouseHovering)
                spriteBatch.Draw(selectionOutline, pos, null, Color.Gold, 0f, origin, 1f, SpriteEffects.None, 0f);

            spriteBatch.Restore(state);
        }
    }
}
