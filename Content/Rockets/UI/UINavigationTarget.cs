using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets.Navigation.Checklist;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Content.Rockets.UI
{
    public class UINavigationTarget : UIElement
    {
        /// <summary> The panel instance this target belongs to </summary>
        public UINavigationPanel OwnerPanel { get; set; }

        /// <summary> The navigation map instance this target belongs to </summary>
        public UINavigationMap OwnerMap => OwnerPanel.CurrentMap;

        /// <summary> 
        /// The identification of this target, used in conjuction with Subworld IDs.
        /// Must be unique within the same <c>UINavigationMap</c>, but can have same ID-linked targets in other navigation maps
        /// </summary>
        public readonly string Name = "default";

        /// <summary> Collection to determine whether the subworld is accesible </summary>
        public ChecklistConditionCollection LaunchConditions { get; set; } = null;

        /// <summary> Whether the target satisfies the launch conditions </summary>
        public bool IsReachable = false;

        /// <summary> Whether this target's ID is equal to the current subworld </summary>
        public bool AlreadyHere => Name == MacrocosmSubworld.CurrentMacrocosmID;

        /// <summary> Whether the target is currently selected </summary>
        public bool Selected;

        public void ResetAnimation()
        {
            rotation = 0f;
            targetOpacity = 0f;
            drawColor = new Color(0, 0, 0, 0);
            targetColor = new Color(0, 0, 0, 0);
            targetColorLerp = 0f;
        }

        // selection outline texture, has default
        private readonly Texture2D selectionOutline = ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/Buttons/SelectionOutlineSmall", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

        // Selection outline rotation
        private float rotation = 0f;

        // Target opacity of the selection outline
        private float targetOpacity = 0f;

        // The selection outline draw color
        private Color drawColor = new(0, 0, 0, 0);

        // The selection outline target color 
        private Color targetColor = new(0, 0, 0, 0);

        // The draw color and target color blending factor
        private float targetColorLerp = 0f;

        /// <summary> Creates a new UINavigationTarget based on a target Macrocosm Subworld </summary>
        /// <param name="owner"> The owner navigation panel </param>
        /// <param name="position"> The map target's position relative to the top corner of the NavigationMap </param>
        /// <param name="width"> Interactible area width in pixels </param>
        /// <param name="height"> Interactible area height in pixels </param>
        /// <param name="targetSubworld"> The subworld (instance) associated with the map target </param>
        public UINavigationTarget(UINavigationPanel owner, Vector2 position, float width, float height, MacrocosmSubworld targetSubworld, Texture2D outline = null) : this(owner, position, width, height)
        {
            LaunchConditions = targetSubworld.LaunchConditions;
            Name = targetSubworld.Name;

            selectionOutline = outline ?? selectionOutline;
        }

        /// <summary>  Creates a new UINavigationTarget with the given custom data. Used for special navigation, like towards Earth  </summary>
        /// <param name="owner"> The owner navigation panel </param>
        /// <param name="position"> The map target's position relative to the top corner of the NavigationMap </param>
        /// <param name="width"> Interactible area width in pixels </param>
        /// <param name="height"> Interactible area height in pixels </param>
        /// <param name="targetId"> The special ID of the target, handled in <see cref="Rocket.Travel"/> </param>
        /// <param name="canLaunch"> Function that determines whether the target is selectable, defaults to false </param>
        public UINavigationTarget(UINavigationPanel owner, Vector2 position, float width, float height, string targetId, ChecklistConditionCollection launchConditions = null, Texture2D outline = null) : this(owner, position, width, height)
        {
            LaunchConditions = launchConditions;
            Name = targetId;

            selectionOutline = outline ?? selectionOutline;
        }

        /// <summary> Creates a new UINavigationTarget </summary>
        /// <param name="owner"> The owner navigation panel </param>
        /// <param name="position"> The map target's position relative to the top corner of the NavigationMap </param>
        /// <param name="width"> Interactible area width in pixels </param>
        /// <param name="height"> Interactible area height in pixels </param>
        private UINavigationTarget(UINavigationPanel owner, Vector2 position, float width, float height)
        {
            OwnerPanel = owner;
            Width.Set(width, 0);
            Height.Set(height, 0);
            Top.Set(position.Y, 0);
            Left.Set(position.X, 0);
        }

        public override void OnInitialize()
        {
            OnLeftClick += (_, _) =>
            {
                if (!OwnerMap.AnimationActive)
                {
                    OwnerMap.DeselectAllTargets();
                    Selected = true;
                }
            };

            OnRightClick += (_, _) => Selected = false;

            OnLeftDoubleClick += (_, _) => OwnerPanel.ZoomIn(useDefault: false);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            rotation += 0.006f;

            // Compute target opacity of the selection outline
            if (Selected || IsMouseHovering)
                targetOpacity += 0.1f;
            else
                targetOpacity -= 0.1f;

            targetOpacity = MathHelper.Clamp(targetOpacity, 0, 1);

            if (targetOpacity > 0f)
            {
                // Get the target color
                if (Selected)
                {
                    if (AlreadyHere)
                        targetColor = Color.Gray;
                    else if (IsReachable)
                        targetColor = new Color(0, 255, 0);
                    else
                        targetColor = new Color(255, 0, 0);
                }
                else if (IsMouseHovering)
                {
                    targetColor = Color.Gold;
                }

                if (drawColor != targetColor)
                    targetColorLerp = 0f;

                targetColorLerp += 0.1f;
                targetColorLerp = MathHelper.Clamp(targetColorLerp, 0, 1);

                drawColor = Color.Lerp(drawColor, targetColor, targetColorLerp);
            }
            else
            {
                drawColor = new Color(0, 0, 0, 0);
                targetColor = new Color(0, 0, 0, 0);
                targetColorLerp = 0f;
            }
        }

        /// <summary> Check whether all the launch conditions specific to this target have been met </summary>
        public bool CheckLaunchConditions()
        {
            if (LaunchConditions is not null)
                return LaunchConditions.AllMet();

            return true;
        }

        private SpriteBatchState state;
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Rectangle rect = GetDimensions().ToRectangle();

            if (RocketUISystem.DebugModeActive)
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, rect, Color.Green.WithOpacity(0.1f));

            // Should draw the outline if not fully transparent
            if (targetOpacity > 0f)
            {
                Vector2 origin = new(selectionOutline.Width / 2f, selectionOutline.Height / 2f);

                state.SaveState(spriteBatch);
                spriteBatch.End();
                spriteBatch.Begin(BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, state);

                float scale = 0.918f + (0.98f - 0.918f) * Utility.NormalizedUIScale;
                spriteBatch.Draw(selectionOutline, rect.Center(), null, drawColor.WithOpacity(targetOpacity), rotation, origin, scale, SpriteEffects.None, 0f);

                spriteBatch.End();
                spriteBatch.Begin(state);
            }
        }
    }
}
