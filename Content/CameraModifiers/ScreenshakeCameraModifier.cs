using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Graphics.CameraModifiers;

namespace Macrocosm.Content.CameraModifiers;

public class ScreenshakeCameraModifier : ICameraModifier
{
    public string UniqueIdentity { get; private set; }
    public bool Finished { get; private set; }

    private float intensity = 0f;
    private float multiplier;

    public ScreenshakeCameraModifier(float intensity, string uniqueIdentity, float multiplier = 0.9f)
    {
        this.intensity = Math.Clamp(intensity, 0, 100);
        this.multiplier = Math.Clamp(multiplier, 0f, 0.99f);
        UniqueIdentity = uniqueIdentity;
    }

    public void Update(ref CameraInfo cameraPosition)
    {
        cameraPosition.CameraPosition += new Vector2(Main.rand.NextFloat(intensity), Main.rand.NextFloat(intensity));
        intensity *= multiplier;

        if (intensity < 0.1f)
        {
            intensity = 0f;
            Finished = true;
        }
    }
}
