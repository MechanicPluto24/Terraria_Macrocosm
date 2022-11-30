using Macrocosm.Common.Drawing;
using Macrocosm.Content.Subworlds;
using Macrocosm.Content.Subworlds.Earth;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SubworldLibrary;
using System;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Content.Rocket.UI
{
	public class UIMapTarget : UIElement
	{
		public SubworldData TargetWorldData = default;
		public string TargetID;

		public delegate bool FuncCanLaunch();
		public readonly FuncCanLaunch CanLaunch = () => false;

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
		/// <param name="targetWorldData"> Information about the world, empty by default </param>
		/// <param name="canLaunch"> Function that determines whether the target is selectable, defaults to false </param>
		/// <param name="targetID"> The special ID of the target, handled in the RocketNPC entity code. If not set it will use the info panel display name </param>
		public UIMapTarget(Vector2 position, float width, float height, SubworldData targetWorldData = default, FuncCanLaunch canLaunch = null, string targetID = "") : this(position, width, height)
		{
			TargetWorldData = targetWorldData;
			this.CanLaunch = canLaunch ?? this.CanLaunch;	
			TargetID = targetID.Equals("") ? targetWorldData.DisplayName : targetID;
		}

		/// <summary>
		/// Creates a new UIMapTarget based on a target Macrocosm Subworld
		/// </summary>
		/// <param name="position"> The map target's position relative to the top corner of the NavigationMap </param>
		/// <param name="width"> Interactible area width in pixels </param>
		/// <param name="height"> Interactible area height in pixels </param>
		/// <param name="targetSubworld"> The subworld (instance) associated with the map target </param>
		public UIMapTarget(Vector2 position, float width, float height, MacrocosmSubworld targetSubworld) : this(position, width, height)
		{
			TargetWorldData = targetSubworld.SubworldData; 
			CanLaunch = () => targetSubworld.CanTravelTo();
			TargetID = Macrocosm.Instance.Name + "/" + targetSubworld.Name; 
		}

		public override void OnInitialize()
		{
			OnClick += (_,_) =>
			{
				foreach (UIElement element in Parent.Children)
				{
					if (element is UIMapTarget target && !ReferenceEquals(target, this))
						target.Selected = false;
				}
				Selected = true;
			};

			OnRightClick += (_, _) => { Selected = false; };

			OnDoubleClick += (_, _) => { (Parent.Parent as UINavigationPanel).ZoomIn(); };

			//OnRightDoubleClick += (_, _) => { (Parent.Parent as UINavigationPanel).ZoomOut();  };
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			Rectangle rect = GetDimensions().ToRectangle();
			//rect.Inflate(20, 20);

			Texture2D outline = ModContent.Request<Texture2D>("Macrocosm/Content/Rocket/UI/UISelectionOutline").Value;

			SpriteBatchState state = spriteBatch.SaveState();

			spriteBatch.End();
			spriteBatch.Begin(BlendState.NonPremultiplied, state);

			if (Selected)
			{
				if (CanLaunch())
					spriteBatch.Draw(outline, rect, Color.Green);
				else
					spriteBatch.Draw(outline, rect, Color.Red);
			}
			else if(IsMouseHovering)
				spriteBatch.Draw(outline, rect, Color.Gold);

			spriteBatch.Restore(state);

		}

		public string ParseSubworldData()
		{
			string text = "";
			SubworldData data = TargetWorldData;

			if(data.Gravity > 0) text += $"Gravity: {data.Gravity} G\n";
			if(data.Radius > 0) text += $"Radius: {data.Radius} km\n";
			if(data.DayPeriod > 0) text += $"Day Period: {data.DayPeriod} days\n";
			if(data.ThreatLevel > 0) text += $"Threat Level: {data.ThreatLevel}\n";

			if (data.Hazards.Count <= 0)
				return text;

			string hazards = "";
			foreach (string hazard in data.Hazards)
				hazards += $"- {hazard} \n";

			text += $"Hazards:\n{hazards}";

			return text;
		}
	}
}
