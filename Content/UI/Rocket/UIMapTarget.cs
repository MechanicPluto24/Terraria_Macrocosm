using Macrocosm.Common.Drawing;
using Macrocosm.Content.Subworlds;
using Macrocosm.Content.Subworlds.Earth;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SubworldLibrary;
using System;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Content.UI.Rocket
{
	public class UIMapTarget : UIElement
	{
		/// <summary> Subworld data displayed in the info panel </summary>
		public WorldInfo TargetWorldInfo = default;

		/// <summary> The subworld ID used for accessing the subworld </summary>
		public string TargetID;
		private string targetName = "A";

		/// <summary> Function to determine whether the subworld is accesible </summary>
		public delegate bool FuncCanLaunch();
		public readonly FuncCanLaunch CanLaunch = () => false;

		public Texture2D SelectionOutline = ModContent.Request<Texture2D>("Macrocosm/Content/UI/Rocket/Buttons/SelectionOutlineSmall", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

		public bool IsSelectable => CanLaunch() || (Parent as UINavigationMap).Next != null;

		public bool AlreadyHere => targetName == (SubworldSystem.AnyActive<Macrocosm>() ? SubworldSystem.Current.Name : "Earth");

		/// <summary> The current selected  </summary>
		public bool Selected;

		/// <summary>
		/// Creates a new UIMapTarget
		/// </summary>
		/// <param name="position"> The map target's position relative to the top corner of the NavigationMap </param>
		/// <param name="width"> Interactible area width in pixels </param>
		/// <param name="height"> Interactible area height in pixels </param>
		public UIMapTarget(Vector2 position, float width, float height)
		{
			Width.Set(width + 40, 0);
			Height.Set(height + 40, 0);
			Top.Set(position.Y - 20, 0);
			Left.Set(position.X - 20, 0);

		}

		/// <summary>
		/// Creates a new UIMapTarget with the given custom data. Used for special navigation, like towards Earth 
		/// </summary>
		/// <param name="position"> The map target's position relative to the top corner of the NavigationMap </param>
		/// <param name="width"> Interactible area width in pixels </param>
		/// <param name="height"> Interactible area height in pixels </param>
		/// <param name="targetWorldInfo"> Information about the world, empty by default </param>
		/// <param name="canLaunch"> Function that determines whether the target is selectable, defaults to false </param>
		/// <param name="targetID"> The special ID of the target, handled in the RocketNPC entity code. If not set it will use the info panel display name </param>
		public UIMapTarget(Vector2 position, float width, float height, WorldInfo targetWorldInfo = default, FuncCanLaunch canLaunch = null, string targetID = "", Texture2D outline = null) : this(position, width, height)
		{
			TargetWorldInfo = targetWorldInfo;
			CanLaunch = canLaunch ?? CanLaunch;
			TargetID = targetID.Equals("") ? targetWorldInfo.DisplayName : targetID;
			targetName = TargetID;
			SelectionOutline = outline ?? SelectionOutline;
		}

		/// <summary>
		/// Creates a new UIMapTarget based on a target Macrocosm Subworld
		/// </summary>
		/// <param name="position"> The map target's position relative to the top corner of the NavigationMap </param>
		/// <param name="width"> Interactible area width in pixels </param>
		/// <param name="height"> Interactible area height in pixels </param>
		/// <param name="targetSubworld"> The subworld (instance) associated with the map target </param>
		public UIMapTarget(Vector2 position, float width, float height, MacrocosmSubworld targetSubworld, Texture2D outline = null) : this(position, width, height)
		{
			TargetWorldInfo = targetSubworld.WorldInfo;
			CanLaunch = () => targetSubworld.CanTravelTo();
			targetName = targetSubworld.Name;
			TargetID = Macrocosm.Instance.Name + "/" + targetName;
			SelectionOutline = outline ?? SelectionOutline;
		}

		public override void OnInitialize()
		{
			OnClick += (_, _) =>
			{
				foreach (UIElement element in Parent.Children)
				{
					if (element is UIMapTarget target && !ReferenceEquals(target, this))
						target.Selected = false;
				}
				Selected = true;
			};

			OnRightClick += (_, _) => { Selected = false; };

			OnDoubleClick += (_, _) => { (Parent.Parent as UINavigationPanel).ZoomIn(useDefault: false); };

			//OnRightDoubleClick += (_, _) => { (Parent.Parent as UINavigationPanel).ZoomOut();  };
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			Rectangle rect = GetDimensions().ToRectangle();
 			Vector2 pos = new Vector2(rect.Center.X, rect.Center.Y);
			Vector2 origin = new Vector2(SelectionOutline.Width / 2, SelectionOutline.Height / 2);

			SpriteBatchState state = spriteBatch.SaveState();
			spriteBatch.End();
			spriteBatch.Begin(BlendState.NonPremultiplied, state);

			if (Selected)
			{
				if (IsSelectable)
					spriteBatch.Draw(SelectionOutline, pos, null, new Color(0, 255, 0), 0f, origin, 1f, SpriteEffects.None, 0f);
				else if (AlreadyHere)
					spriteBatch.Draw(SelectionOutline, pos, null, Color.Gray, 0f, origin, 1f, SpriteEffects.None, 0f);
				else
					spriteBatch.Draw(SelectionOutline, pos, null, new Color(255, 0, 0), 0f, origin, 1f, SpriteEffects.None, 0f);
			}
			else if (IsMouseHovering)
				spriteBatch.Draw(SelectionOutline, pos, null, Color.Gold, 0f, origin, 1f, SpriteEffects.None, 0f);

			spriteBatch.Restore(state);
		}
	}
}
