using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader.IO;

namespace Macrocosm.Content.Rockets.LaunchPads
{
	public class LaunchPad : TagSerializable
	{
		public Point16 StartTile { get; set; }
		public Point16 EndTile { get; set; }
		public bool HasRocket { get; set; }	
		public bool IsDefault { get; set; }

		public Rectangle Hitbox => new((int)(StartTile.X * 16f), (int)(StartTile.Y * 16f), (EndTile.X + 1 - StartTile.X) * 16, 16);
		public Vector2 Position => new(((StartTile.X + (EndTile.X - StartTile.X) / 2f) * 16f), StartTile.Y);

		public LaunchPad()
		{
			StartTile = new();
			EndTile = new();
		}

		public LaunchPad(int startTileX, int startTileY)
		{
			StartTile = new(startTileX, startTileY);
			EndTile = StartTile;
		}

		public LaunchPad(Point16 startTile)
		{
			StartTile = startTile;
			EndTile = startTile;
		}

		public LaunchPad(Point16 startTile, Point16 endTile)
		{
			StartTile = startTile;
			EndTile = endTile;
		}

		public LaunchPad(int startTileX, int startTileY, int endTileX, int endTileY)
		{
			StartTile = new(startTileX, startTileY);
			EndTile = new(endTileX, endTileY);
		}

		public void Update()
		{
			if (IsDefault)
				return;

			for(int i = 0; i < RocketManager.MaxRockets; i++)
			{
				Rocket rocket = RocketManager.Rockets[i];

				if (rocket.ActiveInCurrentSubworld)
				{
					if (Hitbox.Intersects(rocket.Bounds))
						HasRocket = true;
				}
			}
		}

		public void Draw(SpriteBatch spriteBatch, Vector2 screenPosition)
		{
			Rectangle rect = Hitbox;
			rect.X -= (int)screenPosition.X; 
			rect.Y -= (int)screenPosition.Y;

			if (Hitbox.Contains((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y)) 
				spriteBatch.Draw(TextureAssets.ColorBlip.Value, rect, Color.Gold * 0.25f);
		}
		
		public static readonly Func<TagCompound, LaunchPad> DESERIALIZER = DeserializeData;

		public TagCompound SerializeData()
		{
			TagCompound tag = new()
			{
				[nameof(StartTile)] = StartTile,
				[nameof(EndTile)] = EndTile,
				[nameof(HasRocket)] = HasRocket,
				[nameof(IsDefault)] = IsDefault
			};

			return tag;
		}

		public static LaunchPad DeserializeData(TagCompound tag)
		{
			LaunchPad launchPad = new()
			{
				HasRocket = tag.ContainsKey(nameof(HasRocket)),
				IsDefault = tag.ContainsKey(nameof(IsDefault))
			};

			if (tag.ContainsKey(nameof(StartTile)))
				launchPad.StartTile = tag.Get<Point16>(nameof(StartTile));

			if (tag.ContainsKey(nameof(EndTile)))
				launchPad.EndTile = tag.Get<Point16>(nameof(EndTile));

			return launchPad;
		}
	}
}
