using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macrocosm.Content.Rockets.Customization
{
	public class PatternColorFunction
	{
		public bool Unlocked { get; set; }
		public bool UnlockedByDefault { get; private set; }

		private readonly Func<Color[], Color> function;

		public Color Invoke(Color[] inputColors) => function(inputColors);

		public PatternColorFunction(Func<Color[], Color> function, bool unlockedByDefault = true)
		{
			Unlocked = unlockedByDefault;
			UnlockedByDefault = unlockedByDefault;
			this.function = function;
		}
	}
}
