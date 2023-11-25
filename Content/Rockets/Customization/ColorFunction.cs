using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Linq;

namespace Macrocosm.Content.Rockets.Customization
{
    public class ColorFunction
    {
        public string Name { get; } = "";

		public bool HasParameters => Parameters is not null && Parameters.Length > 0;
		public object[] Parameters { get; private set; }

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
		public ColorFunction(Func<Color[], Color> function, string name)
		{
			this.function = function;
			Name = name;
		}

        /// <summary> Invokes the color function on an input array. </summary>
        public Color Invoke(Color[] inputColors) => function(inputColors);

		public static ColorFunction CreateByName(string name, params object[] parameters) 
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
				LogImportError(ex.Message);
				return CreateMapFunction(0);
			}
			catch (Exception ex) when (ex is NullReferenceException)
			{
				LogImportError($"Error creating function '{name}', expected parameters not provided.");
				return CreateMapFunction(0);
			}
			catch (Exception ex) when (ex is IndexOutOfRangeException or InvalidCastException)
			{
				string parameterString = string.Join(", ", parameters.Select(p => p.ToString()));
				LogImportError($"Error creating function '{name}' with parameters: {parameterString}");
				return CreateMapFunction(0);
			}
		}

        private static void LogImportError(string message)
        {
            Utility.Chat(message, Color.Orange);
            Macrocosm.Instance.Logger.Error(message);
        }

		private static ColorFunction CreateMapFunction(int index)
		{
			if (index < 0 || index >= 8) throw new ArgumentException($"Index out of bounds: {index}");
			return new((colors) => colors[index], "Map") 
			{
				Parameters = new object[] { index } 
			};
		}

		private static ColorFunction CreateLerpFunction(int index1, int index2, float amount)
		{
			if (index1 < 0 || index1 >= 8) throw new ArgumentException($"Index out of bounds: {index1}");
			if (index2 < 0 || index2 >= 8) throw new ArgumentException($"Index out of bounds: {index2}");
			var colorFunc = new ColorFunction((colors) => Color.Lerp(colors[index1], colors[index2], amount), "Lerp") 
			{
				Parameters = new object[] { index1, index2, amount } 
			};
			return colorFunc;
        }
	}
}
