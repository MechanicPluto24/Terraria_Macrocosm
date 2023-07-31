using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macrocosm.Content.Rockets.Customization
{
	public class PatternColorFunction : IUnlockable
	{
		public string Name { get; } = "";

		public bool Unlocked { get; set; }
		public bool UnlockedByDefault { get; private set; }


		private readonly Func<Color[], Color> function;

		/// <summary> Creates a new pattern dynamic color. Use this for color functions specific to a pattern, not unlockable on its own. </summary>
		/// <param name="function"> The function </param>
		public PatternColorFunction(Func<Color[], Color> function)
		{
			this.function = function;
		}

		/// <summary>
		/// Creates a new pattern dynamic color. Use this for unlockable dynamic colors.
		/// </summary>
		/// <param name="function"> The function </param>
		/// <param name="name"> The dynamic color name </param>
		/// <param name="unlockedByDefault"> Whether this dynamic color is unlocked by default </param>
		public PatternColorFunction(Func<Color[], Color> function, string name, bool unlockedByDefault = true)
		{
			Unlocked = unlockedByDefault;
			UnlockedByDefault = unlockedByDefault;
			this.function = function;
			Name = name;
		}

		/// <summary> Invokes the color function </summary>
		public Color Invoke(Color[] inputColors) => function(inputColors);

		/// <summary> The storage key of this dynamic color function </summary>
		public string GetKey() => Name;
	}

}
