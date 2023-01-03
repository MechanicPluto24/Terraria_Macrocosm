using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;
using SubworldLibrary;
using Macrocosm.Content.Buffs.Debuffs;
using Macrocosm.Content.Subworlds;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Systems;
using Macrocosm.Content.Particles;
using Macrocosm.Content.Buffs.GoodBuffs;
using Macrocosm.Common.Drawing.Particles;
using Terraria.GameContent;

namespace Macrocosm.Content.Players
{
    public class MacrocosmPlayer : ModPlayer
	{
		public bool AccMoonArmor = false;
		public int AccMoonArmorDebuff = 0;

		public bool ZoneMoon = false;
		public bool ZoneBasalt = false;
		public bool ZoneIrradiation = false;

		public float RadNoiseIntensity = 0f;

		public int ChandriumEmpowermentStacks = 0;

		#region Screenshake mechanic 
		private float screenShakeIntensity = 0f;
		public float ScreenShakeIntensity
		{
			get => screenShakeIntensity;
			set => screenShakeIntensity = MathHelper.Clamp(value, 0, 100);
		}
		#endregion

		public override void ResetEffects()
		{
			AccMoonArmor = false;
			RadNoiseIntensity = 0f;
		}

		public override void PostUpdateBuffs()
		{
			if (SubworldSystem.IsActive<Moon>())
			{
				if (!AccMoonArmor)
					Player.AddBuff(ModContent.BuffType<SuitBreach>(), 2);
			}
		}

		public override void PostUpdateMiscEffects()
		{
			UpdateGravity();
			UpdateFilterEffects();

			if (AccMoonArmorDebuff > 0)
			{
				Player.buffImmune[ModContent.BuffType<SuitBreach>()] = false;
				AccMoonArmorDebuff--;
			}
		}

		public override void ModifyScreenPosition()
		{
			if (ScreenShakeIntensity > 0.1f)
			{
				Main.screenPosition += new Vector2(Main.rand.NextFloat(ScreenShakeIntensity), Main.rand.NextFloat(ScreenShakeIntensity));
				ScreenShakeIntensity *= 0.9f;
			}
		}

		private void UpdateGravity()
		{
			if (SubworldSystem.AnyActive<Macrocosm>())
				Player.gravity = Player.defaultGravity * MacrocosmSubworld.Current().GravityMultiplier;
		}

		private void UpdateFilterEffects()
		{
			if (ZoneIrradiation)
			{
				if (!Filters.Scene["Macrocosm:RadiationNoiseEffect"].IsActive())
					Filters.Scene.Activate("Macrocosm:RadiationNoiseEffect");

				RadNoiseIntensity += 0.189f * Utils.GetLerpValue(400, 10000, TileCounts.Instance.IrradiatedRockCount, clamped: true);

				Filters.Scene["Macrocosm:RadiationNoiseEffect"].GetShader().UseIntensity(RadNoiseIntensity);
			}
			else
			{
				if (Filters.Scene["Macrocosm:RadiationNoiseEffect"].IsActive())
					Filters.Scene.Deactivate("Macrocosm:RadiationNoiseEffect");
			}
		}
	}
}
