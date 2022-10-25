using Terraria;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common
{
	public static class EffectLoader
	{
		public const string EffectAssetPath = "Macrocosm/Assets/Effects/";

		public static void LoadEffects()
		{
			Filters.Scene["Macrocosm:RadiationNoiseEffect"] = new Filter(new ScreenShaderData(new Ref<Effect>(ModContent.Request<Effect>("Macrocosm/Assets/Effects/RadiationNoiseEffect").Value), "RadiationNoiseEffect"));
			Filters.Scene["Macrocosm:RadiationNoiseEffect"].Load();
		}
	}
}
