using Terraria.Audio;

namespace Macrocosm.Content.Sounds
{
    public static class SFX
    {
        public const string SFXPath = Macrocosm.SFXAssetsPath;

        public static readonly SoundStyle AssaultRifle = new(SFXPath + "AssaultRifle_", 4);
        public static readonly SoundStyle GrenadeLauncherThunk = new(SFXPath + "GrenadeLauncherThunk");
        public static readonly SoundStyle MinigunWindup = new(SFXPath + "MinigunWindup");
        public static readonly SoundStyle MinigunFire = new(SFXPath + "MinigunFire");
        public static readonly SoundStyle MinigunWinddown = new(SFXPath + "MinigunWinddown");
        public static readonly SoundStyle Ricochet = new(SFXPath + "Ricochet_", 6);
        public static readonly SoundStyle HandheldThrusterFlame = new(SFXPath + "HandheldThrusterFlame");
        public static readonly SoundStyle HandheldThrusterOverheat = new(SFXPath + "HandheldThrusterOverheat");
        public static readonly SoundStyle DesertEagleShoot = new(SFXPath + "DesertEagleShoot");

    }
}
