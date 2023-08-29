using Microsoft.Xna.Framework;
using System;
using System.Linq;

namespace Macrocosm.Content.Rockets.Customization
{
	public class ColorFunction : IUnlockable
	{
		public string Name { get; } = "";

		public bool Unlocked { get; set; }
		public bool UnlockedByDefault { get; private set; }

		private readonly Func<Color[], Color> function;

		/// <summary> Creates a new pattern dynamic color. Use this for unnamed color functions, not unlockable. </summary>
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

		/// <summary> Invokes the color function on an input array. </summary>
		public Color Invoke(Color[] inputColors) => function(inputColors);

		/// <summary> The storage key of this unlockable dynamic color </summary>
		public string GetKey() => Name;

		public static ColorFunction CreateFunctionByName(string name, params object[] parameters) 
		{
			try
			{
				return name switch
				{
					"Lerp" or "lerp" => CreateLerpFunction(Convert.ToInt32(parameters[0]), Convert.ToInt32(parameters[1]), Convert.ToSingle(parameters[2])),
					"Map" or "map" => CreateMapFunction(Convert.ToInt32(parameters[0])),
					_ => throw new ArgumentException($"Unknown function name: {name}")
				}; 
			}
			catch (ArgumentException ex)
			{
				Macrocosm.Instance.Logger.Error(ex.Message);
				return CreateMapFunction(0);
			}
			catch (Exception ex) when (ex is NullReferenceException)
			{
				string errorMessage = $"Error creating function '{name}', parameters not provided.";
				Macrocosm.Instance.Logger.Error(errorMessage + "\n" + ex.Message);
				return CreateMapFunction(0);
			}
			catch (Exception ex) when (ex is IndexOutOfRangeException or InvalidCastException)
			{
				string parameterString = string.Join(", ", parameters.Select(p => p.ToString()));
				string errorMessage = $"Error creating function '{name}' with parameters: {parameterString}";
				Macrocosm.Instance.Logger.Error(errorMessage + "\n" + ex.Message);
				return CreateMapFunction(0);
			}
			
		}

		public static ColorFunction CreateMapFunction(int index)
		{
			Color map(Color[] colors) => colors[index];
			return new(map, "Map");
		}

		public static ColorFunction CreateLerpFunction(int index1, int index2, float amount)
		{
			Color lerp(Color[] colors) => Color.Lerp(colors[index1], colors[index2], amount);
			return new(lerp, "Lerp");
		}
	}

}
