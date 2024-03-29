﻿using System;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace Macrocosm.Content.Rockets.LaunchPads
{
	public partial class LaunchPad : TagSerializable
	{
		public LaunchPad Clone() => DeserializeData(SerializeData());

		public static readonly Func<TagCompound, LaunchPad> DESERIALIZER = DeserializeData;

		public TagCompound SerializeData()
		{
			TagCompound tag = new()
			{
				[nameof(Active)] = Active,
				[nameof(StartTile)] = StartTile,
				[nameof(EndTile)] = EndTile,
				[nameof(RocketID)] = RocketID,
				[nameof(CompassCoordinates)] = CompassCoordinates,
			};

			return tag;
		}

		public static LaunchPad DeserializeData(TagCompound tag)
		{
			LaunchPad launchPad = new();

			launchPad.Active = tag.ContainsKey(nameof(Active));

			if (tag.ContainsKey(nameof(RocketID)))
				launchPad.RocketID = tag.GetInt(nameof(RocketID));

			if (tag.ContainsKey(nameof(StartTile)))
				launchPad.StartTile = tag.Get<Point16>(nameof(StartTile));

			if (tag.ContainsKey(nameof(EndTile)))
				launchPad.EndTile = tag.Get<Point16>(nameof(EndTile));

			if (tag.ContainsKey(nameof(CompassCoordinates)))
				launchPad.CompassCoordinates = tag.GetString(nameof(CompassCoordinates));

			return launchPad;
		}
	}
}
