using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Liquids.WaterStyles
{
    public class PollutionWaterStyle : ModWaterStyle
    {
        private Asset<Texture2D> rainTexture;
        public override void Load()
        {
            rainTexture = ModContent.Request<Texture2D>(Texture.Replace(nameof(PollutionWaterStyle), "PollutionRain"));
        }

        public override int ChooseWaterfallStyle() => ModContent.GetInstance<PollutionWaterfallStyle>().Slot;

        public override int GetSplashDust() => DustID.Water;

        public override int GetDropletGore() => GoreID.WaterDrip;

        public override void LightColorMultiplier(ref float r, ref float g, ref float b)
        {
            r = 1f;
            g = 1f;
            b = 1f;
        }

        public override Color BiomeHairColor() => Color.DarkGray;

        public override byte GetRainVariant() => (byte)Main.rand.Next(3);

        public override Asset<Texture2D> GetRainTexture() => rainTexture;
    }
}