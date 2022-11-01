using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;
using SubworldLibrary;
using Macrocosm.Content.Buffs.Debuffs;
using Macrocosm.Content.Subworlds;
using Macrocosm.Content.Subworlds.Moon;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Systems;

namespace Macrocosm.Content
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

		#region Stamina mechanic

		private float staminaRegenCooldown = 90f;
		private float staminaRegenPeriod = 60f;
		public void ResetStaminaCooldown(float value) => staminaRegenCooldown = value;

		private float meleeStamina = 1f;
		public float MeleeStamina
		{
			get => meleeStamina;
			set => meleeStamina = MathHelper.Clamp(value, 0.01f, 1f);
		}
		#endregion

		#region Dash mechanic 

 		public bool AccDashHorizontal = false;
		public bool AccDashVertical = false;

		public float AccDashDamage = 0f;    // Set this to > 0 in ModItem.UpdateAccessory for damaging collision 

		// EoC defaults, overridable in ModItem.UpdateAccessory
		public float AccDashVelocity = 10f; // EoC default velocity 
		public float AccDashKnockback = 9f; // EoC default knockback (if damage > 0)
		public int AccDashImmuneTime = 4;   // EoC default immune time  (if damage > 0)

		public int AccDashCooldown = 50;  
		public int AccDashDuration = 35;  

		private bool celestialBulwarkVisible = false;
		
		public enum DashDir { Down, Up, Right, Left, None = -1 }

		private DashDir dashDir = DashDir.None;  
		private int dashDelay = 0;  
		private int dashTimer = 0;  
		#endregion

		public override void ResetEffects()
		{
 			AccMoonArmor = false;
			ResetDashEffects();

			RadNoiseIntensity = 0f;
		}

		public override void PreUpdateMovement()
		{
			UpdateDashMovement();
		}

		public override void PostUpdateBuffs()
		{
			if (SubworldSystem.IsActive<Moon>())
			{
				if (!AccMoonArmor)
 					Player.AddBuff(ModContent.BuffType<SuitBreach>(), 2);
 			}

			celestialBulwarkVisible = (Player.shield == EquipLoader.GetEquipSlot(Macrocosm.Instance, "CelestialBulwark", EquipType.Shield));

			if (celestialBulwarkVisible)
				Lighting.AddLight(Player.Center, MacrocosmWorld.CelestialColor.ToVector3() * 0.4f);
 		}

		public override void PostUpdateMiscEffects()
		{
			if(SubworldSystem.AnyActive<Macrocosm>())
				Player.gravity = Player.defaultGravity * MacrocosmSubworld.Current().GravityMultiplier;

			if (AccMoonArmorDebuff > 0)
				Player.buffImmune[ModContent.BuffType<SuitBreach>()] = false;

			UpdateBuffs();
			UpdateStamina();

			UpdateFilterEffects();
		}

		public override void ModifyScreenPosition()
		{
			if (ScreenShakeIntensity > 0.1f)
			{
				Main.screenPosition += new Vector2(Main.rand.NextFloat(ScreenShakeIntensity), Main.rand.NextFloat(ScreenShakeIntensity));
				ScreenShakeIntensity *= 0.9f;
			}
		}

		private void UpdateBuffs()
		{
			if (AccMoonArmorDebuff > 0)
				AccMoonArmorDebuff--;
		}

		private void UpdateStamina()
		{
			if (MeleeStamina < 1f)
			{
				staminaRegenCooldown--;
				if (staminaRegenCooldown <= 0f)
				{
					staminaRegenCooldown = 0f;
					MeleeStamina += 0.2f / staminaRegenPeriod;
				}
			}
			else
			{
				ResetStaminaCooldown(90f);
			}
		}

		private void UpdateFilterEffects()
		{
			if (ZoneIrradiation)
			{
				if (!Filters.Scene["Macrocosm:RadiationNoiseEffect"].IsActive())
					Filters.Scene.Activate("Macrocosm:RadiationNoiseEffect");

				RadNoiseIntensity += 0.45f * Utils.GetLerpValue(400, 10000, TileCountSystem.TileCounts.IrradiatedRockCount, clamped: true);

 				Filters.Scene["Macrocosm:RadiationNoiseEffect"].GetShader().UseIntensity(RadNoiseIntensity);
			}
			else
			{
				if (Filters.Scene["Macrocosm:RadiationNoiseEffect"].IsActive())
					Filters.Scene.Deactivate("Macrocosm:RadiationNoiseEffect");
			}
		}

		private void ResetDashEffects()
		{
			AccDashHorizontal = false;
			AccDashVertical = false;

			AccDashDamage = 0f;
			AccDashKnockback = 9f;

			AccDashImmuneTime = 4;
			AccDashVelocity = 10f;

			AccDashCooldown = 50;
			AccDashDuration = 35;

			// ResetEffects is called not long after player.doubleTapCardinalTimer's values have been set
			// When a directional key is pressed and released, vanilla starts a 15 tick (1/4 second) timer during which a second press activates a dash
			// If the timers are set to 15, then this is the first press just processed by the vanilla logic.  Otherwise, it's a double-tap
			if (Player.controlDown && Player.releaseDown && Player.doubleTapCardinalTimer[(int)DashDir.Down] < 15)
				dashDir = DashDir.Down;
			else if (Player.controlUp && Player.releaseUp && Player.doubleTapCardinalTimer[(int)DashDir.Up] < 15)
				dashDir = DashDir.Up;
			else if (Player.controlRight && Player.releaseRight && Player.doubleTapCardinalTimer[(int)DashDir.Right] < 15)
				dashDir = DashDir.Right;
			else if (Player.controlLeft && Player.releaseLeft && Player.doubleTapCardinalTimer[(int)DashDir.Left] < 15)
				dashDir = DashDir.Left;
			else
				dashDir = DashDir.None;
		}

		private void UpdateDashMovement()
		{
			bool canDash = Player.dashType == 0 && // player doesn't have Tabi or EoCShield equipped (give priority to those dashes)
						  !Player.setSolar && // player isn't wearing solar armor
						  !Player.mount.Active;    // player isn't mounted, since dashes on a mount look weird

			if (canDash && dashDir != DashDir.None && dashDelay == 0)
			{
				Vector2 newVelocity = Player.velocity;

				switch (dashDir)
				{
					// Only apply the dash velocity if our current speed in the wanted direction is less than DashVelocity
					case DashDir.Up when Player.velocity.Y > -AccDashVelocity && AccDashVertical:
					case DashDir.Down when Player.velocity.Y < AccDashVelocity && AccDashVertical:
						{
							// Y-velocity is set here
							// If the direction requested was DashUp, then we adjust the velocity to make the dash appear "faster" due to gravity being immediately in effect
							// This adjustment is roughly 1.3x the intended dash velocity
							float dashDirection = dashDir == DashDir.Down ? 1 : -1.3f;
							newVelocity.Y = dashDirection * AccDashVelocity;
							break;
						}
					case DashDir.Left when Player.velocity.X > -AccDashVelocity && AccDashHorizontal:
					case DashDir.Right when Player.velocity.X < AccDashVelocity && AccDashHorizontal:
						{
							// X-velocity is set here
							float dashDirection = dashDir == DashDir.Right ? 1 : -1;
							newVelocity.X = dashDirection * AccDashVelocity;
							break;
						}
					default:
						return; // not moving fast enough, so don't start the dash
				}

				// start the dash
				dashDelay = AccDashCooldown;
				dashTimer = AccDashDuration;
				Player.velocity = newVelocity;

				#region Start of dash visual effects 

				if (celestialBulwarkVisible)
				{
					for (int i = 0; i < 30; i++)
					{
						int dustIdx = Dust.NewDust(new Vector2(Player.position.X - 20, Player.position.Y - 10), Player.width + 20, Player.height + 10, ModContent.DustType<CelestialDust>(), Player.direction * -1f, 0f, 0, default, 2f);
						Main.dust[dustIdx].position.X += Main.rand.Next(-5, 6);
						Main.dust[dustIdx].position.Y += Main.rand.Next(-5, 6);
						Main.dust[dustIdx].velocity.X *= 0.6f;
 						Main.dust[dustIdx].scale *= 1.4f + (float)Main.rand.Next(20) * 0.01f;
 					}
				}

				#endregion
			}

			if (dashDelay > 0)
				dashDelay--;

			if (dashDelay == 0)
				Player.eocHit = -1;

			if (dashTimer > 0)
			{
				Player.eocDash = dashTimer;
				Player.armorEffectDrawShadowEOCShield = true;
				dashTimer--;

				#region Dash visual effects 

				if (celestialBulwarkVisible)
				{
					for (int k = 0; k < 3; k++)
					{
						int dustType = ModContent.DustType<CelestialDust>();
						//int dustIdx = ((Player.velocity.Y != 0f) ? 
						//	Dust.NewDust(new Vector2(Player.position.X, Player.position.Y + Player.height / 2 - 8f), Player.width, 16, dustType, 0f, 0f, 100, default, 1.4f) : 
						//	Dust.NewDust(new Vector2(Player.position.X, Player.position.Y + Player.height - 4f), Player.width, 8, dustType, 0f, 0f, 100, default, 1.4f));

						int dustIdx = Dust.NewDust(new Vector2(Player.position.X, Player.position.Y), Player.width, Player.height, dustType, 0f, 0f, 100, default, 1.4f);				
						Main.dust[dustIdx].velocity *= 0.1f;
						Main.dust[dustIdx].scale *= 1f + (float)Main.rand.Next(20) * 0.01f;
					}
				}

				#endregion

				#region Dash damage

				if (Player.eocHit < 0)
				{
					if (AccDashDamage > 0f)
					{
						Rectangle rectangle = new((int)(Player.position.X + Player.velocity.X * 0.5f - 4.0f), (int)(Player.position.Y + Player.velocity.Y * 0.5f - 4.0f), Player.width + 8, Player.height + 8);

						for (int i = 0; i < Main.maxNPCs; i++)
						{
							NPC npc = Main.npc[i];

							if (!npc.active || npc.dontTakeDamage || npc.friendly || (npc.aiStyle == Terraria.ID.NPCAIStyleID.Fairy && !(npc.ai[2] <= 1f)) || !Player.CanNPCBeHitByPlayerOrPlayerProjectile(npc))
								continue;

							Rectangle rect = npc.getRect();
							if (rectangle.Intersects(rect) && (npc.noTileCollide || Player.CanHit(npc)))
							{
								float damage = AccDashDamage * Player.GetDamage(DamageClass.Melee).Multiplicative;
								float knockback = AccDashKnockback;
								int direction = Player.direction;
								bool crit = false;

								if (Player.kbGlove)
									knockback *= 2f;

								if (Player.kbBuff)
									knockback *= 1.5f;

								if (Main.rand.Next(100) < Player.GetTotalCritChance(DamageClass.Melee))
									crit = true;

								if (Player.velocity.X < 0f)
									direction = -1;

								if (Player.velocity.X > 0f)
									direction = 1;

								if (Player.whoAmI == Main.myPlayer)
									Player.ApplyDamageToNPC(npc, (int)damage, knockback, direction, crit);

								Player.eocDash = 10;
								Player.dashDelay = AccDashCooldown;
								Player.velocity.X = -direction * AccDashVelocity * 0.75f;
								Player.velocity.Y = -1f * AccDashVelocity * 0.25f;
								Player.GiveImmuneTimeForCollisionAttack(AccDashImmuneTime);
								Player.eocHit = i;
							}
						}
					}
				}
				else if ((!Player.controlLeft || !(Player.velocity.X < 0f)) && (!Player.controlRight || !(Player.velocity.X > 0f)))
				{
					Player.velocity.X *= 0.95f;
				}

				#endregion
			}
		}

		
	}
}
