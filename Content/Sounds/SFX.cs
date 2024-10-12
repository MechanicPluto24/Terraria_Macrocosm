using Terraria.Audio;

namespace Macrocosm.Content.Sounds
{
    public static class SFX
    {
        public const string SFXPath = Macrocosm.SFXPath;

        public static readonly SoundStyle AssaultRifle = new(SFXPath + "AssaultRifle_", 4);
        public static readonly SoundStyle GrenadeLauncherThunk = new(SFXPath + "GrenadeLauncherThunk");
        public static readonly SoundStyle MinigunWindup = new(SFXPath + "MinigunWindup");
        public static readonly SoundStyle MinigunFire = new(SFXPath + "MinigunFire");
        public static readonly SoundStyle MinigunWinddown = new(SFXPath + "MinigunWinddown");
        public static readonly SoundStyle Ricochet = new(SFXPath + "Ricochet_", 6);
        public static readonly SoundStyle HandheldThrusterFlame = new(SFXPath + "HandheldThrusterFlame");
        public static readonly SoundStyle HandheldThrusterOverheat = new(SFXPath + "HandheldThrusterOverheat");
        public static readonly SoundStyle DesertEagleShoot = new(SFXPath + "DesertEagleShoot");
        public static readonly SoundStyle RocketLoop = new(SFXPath + "RocketLoop");
        public static readonly SoundStyle RocketLaunch = new(SFXPath + "RocketLaunch");
        public static readonly SoundStyle RocketLandingLeg = new(SFXPath + "RocketLandingLeg");
        public static readonly SoundStyle WaveGunJoin = new(SFXPath + "WaveGunJoin");
        public static readonly SoundStyle WaveGunSplit = new(SFXPath + "WaveGunSplit");
        public static readonly SoundStyle WaveGunShot = new(SFXPath + "WaveGunShot");
        public static readonly SoundStyle WaveGunShotRifle = new(SFXPath + "WaveGunShotRifle");
        public static readonly SoundStyle ZombieDeath = new(SFXPath + "ZombieDeath");
        public static readonly SoundStyle Zombie = new(SFXPath + "ZombieDeath_", 5);
        public static readonly SoundStyle ZombieEngineerSprint = new(SFXPath + "ZombieEngineerSprint");
        public static readonly SoundStyle ZombieEngineerGasLeak = new(SFXPath + "ZombieEngineerGasLeak");
        public static readonly SoundStyle ZombieEngineerDeath = new(SFXPath + "ZombieEngineerDeath");
    }
}
