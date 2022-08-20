using Macrocosm.Common.Utility;
using Macrocosm.Content.Buffs.Debuffs;
using Macrocosm.Content.Subworlds;
using Macrocosm.Content.Subworlds.Moon;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content
{
	public class MacrocosmPlayer : ModPlayer
	{

		public bool AccMoonArmor = false;
		public int AccMoonArmorDebuff = 0;

		public bool ZoneMoon = false;
		public bool ZoneBasalt = false;

		private float screenShakeIntensity = 0f;
		public float ScreenShakeIntensity
		{
			get => screenShakeIntensity;
			set => screenShakeIntensity = MathHelper.Clamp(value, 0, 100);
		}

		public override void ResetEffects()
		{
			AccMoonArmor = false;
		}

		public override void PostUpdateBuffs()
		{
			if (SubworldSystem.IsActive<Moon>())
			{
				if (!Player.Macrocosm().AccMoonArmor)
				{
					Player.AddBuff(ModContent.BuffType<SuitBreach>(), 2);
				}
			}
		}

		public override void PostUpdateMiscEffects()
		{
			if(SubworldSystem.AnyActive<Macrocosm>())
				Player.gravity = Player.defaultGravity * MacrocosmSubworld.Current().GravityMultiplier;

			if (AccMoonArmorDebuff > 0)
				Player.buffImmune[ModContent.BuffType<SuitBreach>()] = false;
		}

		public override void PostUpdate()
		{
			if (AccMoonArmorDebuff > 0)
				AccMoonArmorDebuff--;
		}

		public override void ModifyScreenPosition()
		{
			if (ScreenShakeIntensity > 0.1f)
			{
				Main.screenPosition += new Vector2(Main.rand.NextFloat(ScreenShakeIntensity), Main.rand.NextFloat(ScreenShakeIntensity));
				ScreenShakeIntensity *= 0.9f;
			}
		}
	}
}
