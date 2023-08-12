﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Macrocosm.Content.Rockets
{
	public partial class Rocket 
	{
		/// <summary> The customization dummy of this rocket </summary>
		public Rocket CustomizationDummy;

		public Rocket Clone(bool asDummy = true) 
		{
			var tag = SerializeData();

			if(asDummy)
				tag["isDummy"] = true;

			return DeserializeData(tag);
		} 

		/// <summary> Draw the actual rocket as a dummy </summary>
		public void DrawDummy(SpriteBatch spriteBatch, Vector2 offset, Color drawColor)
		{
			// Passing Rocket world position as "screenPosition" cancels it out  
			Draw(spriteBatch, Position - offset, drawColor);
		}

		/// <summary> Draw the rocket customization clone </summary>
		public void DrawCustomizationDummy(SpriteBatch spriteBatch, Vector2 offset, Color drawColor)
		{
			CustomizationDummy.DrawDummy(spriteBatch, offset, drawColor);
		}

		public void RefreshCustomizationDummy()
		{
			CustomizationDummy = Clone();
		}

		public void ApplyCustomizationChanges()
		{
			Nameplate.Text = CustomizationDummy.Nameplate.Text;
			Nameplate.TextColor = CustomizationDummy.Nameplate.TextColor;
			Nameplate.HorizontalAlignment = CustomizationDummy.Nameplate.HorizontalAlignment;
			Nameplate.VerticalAlignment = CustomizationDummy.Nameplate.VerticalAlignment;

			RefreshCustomizationDummy();
		}
	}
}