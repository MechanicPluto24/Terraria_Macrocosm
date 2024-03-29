using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.CameraModifiers;

namespace Macrocosm.Content.CameraModifiers
{
    public class ScreenshakeCameraModifier : ICameraModifier
    {
        public string UniqueIdentity { get; private set; }
        public bool Finished { get; private set; }

        private float screenShakeIntensity = 0f;

        public ScreenshakeCameraModifier(float intensity, string uniqueIdentity)
        {
            screenShakeIntensity = MathHelper.Clamp(intensity, 0, 100);
            UniqueIdentity = uniqueIdentity;
        }

        public void Update(ref CameraInfo cameraPosition)
        {
            cameraPosition.CameraPosition += new Vector2(Main.rand.NextFloat(screenShakeIntensity), Main.rand.NextFloat(screenShakeIntensity));
            screenShakeIntensity *= 0.9f;

            if (screenShakeIntensity < 0.1f)
            {
                screenShakeIntensity = 0f;
                Finished = true;
            }
        }
    }
}
