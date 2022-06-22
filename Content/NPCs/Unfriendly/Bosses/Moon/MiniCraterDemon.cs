using Macrocosm.Content.Biomes;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.NPCs.Unfriendly.Bosses.Moon {
	public class MiniCraterDemon : ModNPC {
		public ref float AI_Timer => ref NPC.ai[0];
		public ref float AI_Attack => ref NPC.ai[1];
		public ref float AI_AttackProgress => ref NPC.ai[2];
		public int ParentBoss => (int)NPC.ai[3];

		public const int Spawning = -2;
		public const int Wait = -1;
		public const int FloatTowardPlayer = 0;
		public const int ChargeAtPlayer = 1;
		public const int Chomp = 2;

		private int targetFrame;
		private bool spawned;
		private float targetAlpha;

		private int chargeTicks;

		public const int WaitTime = 4 * 60;

		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Crater Demonite");
			Main.npcFrameCount[NPC.type] = 4;
			NPCID.Sets.TrailCacheLength[NPC.type] = 5;
			NPCID.Sets.TrailingMode[NPC.type] = 0;
		}

		public override void SetDefaults() {
			NPC.width = NPC.height = 56;

			NPC.lifeMax = 6000;
			NPC.defense = 60;
			NPC.damage = 80;
			NPC.knockBackResist = 0f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;

			NPC.aiStyle = -1;

			NPC.HitSound = SoundID.NPCHit2;
			NPC.DeathSound = SoundID.NPCDeath2;

			SpawnModBiomes = new int[1] { ModContent.GetInstance<MoonBiome>().Type }; // Associates this NPC with the Moon Biome in Bestiary
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
			{
				new FlavorTextBestiaryInfoElement(
					"Smaller companions of the infamous Crater Demon, these lesser demons aid their master in combat.")
			});
		}

		public override void FindFrame(int frameHeight) {
			NPC.frame.Y = targetFrame * frameHeight;
		}

		public override void OnHitPlayer(Player target, int damage, bool crit) {
			if(Main.expertMode)
				target.GetModPlayer<MacrocosmPlayer>().accMoonArmorDebuff = 80;
		}

		public override Color? GetAlpha(Color drawColor) 
			=> Color.White * (1f - targetAlpha / 255f);

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 vector, Color drawColor) {
			Texture2D texture = (Texture2D)TextureAssets.Npc[NPC.type];

			Color color = GetAlpha(drawColor) ?? Color.White;

			SpriteEffects effect = (NPC.rotation > MathHelper.PiOver2 && NPC.rotation < 3 * MathHelper.PiOver2) || (NPC.rotation < -MathHelper.PiOver2 && NPC.rotation > -3 * MathHelper.PiOver2)
				? SpriteEffects.FlipVertically
				: SpriteEffects.None;

			if (AI_Attack == ChargeAtPlayer && chargeTicks > 0) {
				int length = Math.Min(chargeTicks, NPC.oldPos.Length);
				for (int i = 0; i < length; i++) {
					Vector2 drawPos = NPC.oldPos[i] - Main.screenPosition + NPC.Size / 2f;

					Color draw = color * (((float)NPC.oldPos.Length - i) / NPC.oldPos.Length);

					spriteBatch.Draw(texture, drawPos, NPC.frame, draw * 0.6f, NPC.rotation, NPC.Size / 2f, NPC.scale, effect, 0f);
				}
			}

			spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, NPC.frame, color, NPC.rotation, NPC.Size / 2f, NPC.scale, effect, 0);

			return false;
		}

		public override void AI() {
			if (!spawned) {
				spawned = true;

				AI_Attack = Spawning;
				AI_Timer = 2 * 60;
				AI_AttackProgress = 0;
				targetAlpha = 255f;

				NPC.TargetClosest();

				NPC.spriteDirection = 1;
			}
			
			Player player = NPC.target >= 0 && NPC.target < Main.maxPlayers ? Main.player[NPC.target] : null;

			if (AI_Attack == Wait && !(NPC.target < 0 || NPC.target >= Main.maxPlayers || player.dead || !player.active)) {
				//Chase the new player
				AI_Attack = FloatTowardPlayer;
				AI_Timer = WaitTime;
			}

			switch (AI_Attack) {
				case Spawning:
					targetAlpha -= 255f / (2 * 60);

					SpawnDusts();

					if (targetAlpha <= 0) {
						targetAlpha = 0;

						AI_Attack = Wait;
					}

					targetFrame = 0;
					NPC.frameCounter = 0;
					break;

				case Wait:
					//Player is dead/not connected?  Target a new one
					if (NPC.target < 0 || NPC.target >= Main.maxPlayers || player.dead || !player.active) {
						NPC.velocity *= 1f - 5f / 60f;

						if (Math.Abs(NPC.velocity.X) < 0.02f)
							NPC.velocity.X = 0;
						if (Math.Abs(NPC.velocity.Y) < 0.02f)
							NPC.velocity.Y = 0;

						NPC.TargetClosest();
					}

					CycleAnimation();
					break;

				case FloatTowardPlayer:
					MoveTowardTargetPlayer(player);

					AdjustRotation(player);

					if (AI_Timer <= 0 && targetFrame == 0) {
						AI_Attack = ChargeAtPlayer;
						AI_Timer = (int)(1.25f * 60);
						AI_AttackProgress = 0;
					} else
						CycleAnimation();

					break;

				case ChargeAtPlayer:
					//Wait until mouth is open
					if (AI_AttackProgress == 0) {
						chargeTicks = 0;

						if (targetFrame != 2) {
							AI_Timer++;

							CycleAnimation();

							MoveTowardTargetPlayer(player);

							AdjustRotation(player);
						} else {
							Terraria.Audio.SoundEngine.PlaySound(SoundID.ForceRoar, NPC.Center);

							Vector2 dir = NPC.DirectionTo(player.Center);
							NPC.rotation = dir.ToRotation();

							NPC.velocity = dir * (Main.expertMode ? 25f : 15f);

							AI_AttackProgress++;
						}
					} else
						chargeTicks++;

					if (AI_Timer <= 0) {
						AI_Attack = Chomp;
						AI_Timer = 20;
						AI_AttackProgress = 0;
					}
					break;

				case Chomp:
					NPC.velocity *= 1f - 5f / 60f;

					if (targetFrame != 0)
						CycleAnimation();
					else if (AI_Timer <= 0) {
						AI_Attack = Wait;
						AI_AttackProgress = 0;
					}
					break;
			}

			AI_Timer--;

			NPC.alpha = (int)targetAlpha;

			NPC.spriteDirection = NPC.velocity.X > 0
				? 1
				: (NPC.velocity.X < 0
					? -1
					: NPC.spriteDirection);
		}

		private void MoveTowardTargetPlayer(Player player) {
			const float inertia = 60f;
			float speedX = Main.expertMode ? 14f : 8f;
			float speedY = speedX * 0.5f;

			const float minDistance = 5 * 16;
			if (NPC.DistanceSQ(player.Center) >= minDistance * minDistance) {
				Vector2 direction = NPC.DirectionTo(player.Center) * speedX;

				NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;

				if (NPC.velocity.X < -speedX)
					NPC.velocity.X = -speedX;
				else if (NPC.velocity.X > speedX)
					NPC.velocity.X = speedX;

				if (NPC.velocity.Y < -speedY)
					NPC.velocity.Y = -speedY;
				else if (NPC.velocity.Y > speedY)
					NPC.velocity.Y = speedY;
			}
		}

		private void AdjustRotation(Player player){
			float NPCRotation = NPC.rotation;
			float targetRotation = NPC.DirectionTo(player.Center).ToRotation();
			
			//Prevent spinning
			if (NPCRotation - targetRotation > MathHelper.Pi)
				targetRotation += MathHelper.TwoPi;
			else if (targetRotation - NPCRotation > MathHelper.Pi)
				NPCRotation += MathHelper.TwoPi;

			NPC.rotation = MathHelper.Lerp(NPCRotation, targetRotation, 4.3f / 60f);
		}

		private void CycleAnimation(){
			if (++NPC.frameCounter >= 10) {
				NPC.frameCounter = 0;
				targetFrame = ++targetFrame % Main.npcFrameCount[NPC.type];
			}
		}

		private void SpawnDusts(){
			var type = ModContent.DustType<RegolithDust>();

			for (int i = 0; i < 4; i++)
				CraterDemon.SpawnDustsInner(NPC.position, NPC.width, NPC.height, type);
		}
	}
}
