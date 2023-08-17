using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets.Customization
{
	public class ColorFunction : IUnlockable
	{
		public string Name { get; } = "";

		public bool Unlocked { get; set; }
		public bool UnlockedByDefault { get; private set; }

		private readonly Func<Color[], Color> function;

		/// <summary> Creates a new pattern dynamic color. Use this for color functions specific to a pattern, not unlockable on its own. </summary>
		/// <param name="function"> The function </param>
		public ColorFunction(Func<Color[], Color> function)
		{
			this.function = function;
		}

		/// <summary>
		/// Creates a new pattern dynamic color. Use this for unlockable dynamic colors.
		/// </summary>
		/// <param name="function"> The function </param>
		/// <param name="name"> The dynamic color name </param>
		/// <param name="unlockedByDefault"> Whether this dynamic color is unlocked by default </param>
		public ColorFunction(Func<Color[], Color> function, string name, bool unlockedByDefault = true)
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

		public static ColorFunction CreateFunctionByName(string name, params object[] parameters) 
		{
			try
			{
				return name switch
				{
					"Lerp" => CreateLerpFunction((int[])parameters[0], (float)parameters[1]),
					"Map" => CreateMapFunction((int)parameters[0]),
					_ => throw new ArgumentException($"Unknown function name: {name}")
				}; ;
			}
			catch (IndexOutOfRangeException ex)
			{
				string parameterString = string.Join(", ", parameters.Select(p => p.ToString()));
				string errorMessage = $"Error creating function '{name}' with parameters: {parameterString}";

				Utility.Chat(errorMessage);
				Macrocosm.Instance.Logger.Error(errorMessage + "\n" + ex.Message);

				return CreateMapFunction(0);
			}
			catch (ArgumentException ex)
			{
				Utility.Chat(ex.Message);
				Macrocosm.Instance.Logger.Error(ex.Message);

				return CreateMapFunction(0);
			}
		}

		private static ColorFunction CreateMapFunction(int index)
		{
			Color map(Color[] colors) => colors[index];
			return new(map);
		}

		public static ColorFunction CreateLerpFunction(int[] indexes, float amount)
		{
			Color lerp(Color[] colors)
			{
				Color[] selectedColors = indexes.Select(index => colors[index]).ToArray();
				return Color.Lerp(selectedColors[0], selectedColors[1], amount);
			}

			return new(lerp);
		}
	}

}
