using Terraria.Audio;

namespace Macrocosm.Content.Sounds
{
    public static class SFX
    {
        public const string SFXPath = Macrocosm.SFXPath;

        // Common
        public static SoundStyle BigExplosion { get; } = new(SFXPath + "BigExplosion_", 3);

        // Rocket
        public static SoundStyle RocketLandingLeg { get; } = new(SFXPath + "RocketLandingLeg");
        public static SoundStyle RocketLaunch { get; } = new(SFXPath + "RocketLaunch");
        public static SoundStyle RocketLoop { get; } = new(SFXPath + "RocketLoop");

        // NPCs
        public static SoundStyle Zombie { get; } = new(SFXPath + "ZombieDeath");
        public static SoundStyle ZombieDeath { get; } = new(SFXPath + "ZombieDeath");
        public static SoundStyle ZombieEngineerDeath { get; } = new(SFXPath + "ZombieEngineerDeath");
        public static SoundStyle ZombieEngineerGasLeak { get; } = new(SFXPath + "ZombieEngineerGasLeak");
        public static SoundStyle ZombieEngineerSprint { get; } = new(SFXPath + "ZombieEngineerSprint");

        // Ranged Weapons
        public static SoundStyle LaserShoot { get; } = new(SFXPath + "LaserShoot");
        public static SoundStyle LaserHit { get; } = new(SFXPath + "LaserHit");
        public static SoundStyle AssaultRifle { get; } = new(SFXPath + "AssaultRifle_", 4);
        public static SoundStyle DesertEagleShot { get; } = new(SFXPath + "DesertEagleShot");
        public static SoundStyle Ricochet { get; } = new(SFXPath + "Ricochet_", 6);
        public static SoundStyle GrenadeLauncherThunk { get; } = new(SFXPath + "GrenadeLauncherThunk");
        public static SoundStyle MinigunFire { get; } = new(SFXPath + "MinigunFire");
        public static SoundStyle MinigunWinddown { get; } = new(SFXPath + "MinigunWinddown");
        public static SoundStyle MinigunWindup { get; } = new(SFXPath + "MinigunWindup");

        // Magic weapons
        public static SoundStyle HandheldThrusterFlame { get; } = new(SFXPath + "HandheldThrusterFlame");
        public static SoundStyle HandheldThrusterOverheat { get; } = new(SFXPath + "HandheldThrusterOverheat");
        public static SoundStyle WaveGunJoin { get; } = new(SFXPath + "WaveGunJoin");
        public static SoundStyle WaveGunShot { get; } = new(SFXPath + "WaveGunShot");
        public static SoundStyle WaveGunShotRifle { get; } = new(SFXPath + "WaveGunShotRifle");
        public static SoundStyle WaveGunSplit { get; } = new(SFXPath + "WaveGunSplit");

        // Summon weapons
        public static SoundStyle RobotSummon { get; } = new(SFXPath + "RobotSummon");
    }
}
