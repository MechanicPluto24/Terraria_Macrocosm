using Terraria.Audio;

namespace Macrocosm.Common
{
	public static class SFX
	{
		public const string SoundAssetPath = "Macrocosm/Assets/Sounds/SFX/";

		public static readonly SoundStyle AssaultRifle = new(SoundAssetPath + "AssaultRifle_", 4);
		public static readonly SoundStyle GrenadeLauncherThunk = new(SoundAssetPath + "GrenadeLauncherThunk");
		public static readonly SoundStyle MinigunWindup = new(SoundAssetPath + "MinigunWindup");
		public static readonly SoundStyle MinigunFire = new(SoundAssetPath + "MinigunFire");
		public static readonly SoundStyle MinigunWinddown = new(SoundAssetPath + "MinigunWinddown");
		public static readonly SoundStyle Ricochet = new(SoundAssetPath + "Ricochet_", 6);
	}
}
