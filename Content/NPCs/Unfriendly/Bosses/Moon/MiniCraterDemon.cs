using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.NPCs.Unfriendly.Bosses.Moon{
	public class MiniCraterDemon : ModNPC{
		public ref float AI_Timer => ref npc.ai[0];
		public ref float AI_Attack => ref npc.ai[1];
		public ref float AI_AttackProgress => ref npc.ai[2];
		public int ParentBoss => (int)npc.ai[3];

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

		public override void SetStaticDefaults(){
			DisplayName.SetDefault("Crater Demonite");
			Main.npcFrameCount[npc.type] = 4;
			NPCID.Sets.TrailCacheLength[npc.type] = 5;
			NPCID.Sets.TrailingMode[npc.type] = 0;
		}

		public override void SetDefaults(){
			npc.width = npc.height = 56;

			npc.lifeMax = 6000;
			npc.defense = 60;
			npc.damage = 80;
			npc.knockBackResist = 0f;
			npc.noGravity = true;
			npc.noTileCollide = true;

			npc.aiStyle = -1;

			npc.HitSound = SoundID.NPCHit2;
			npc.DeathSound = SoundID.NPCDeath2;
		}

		public override void FindFrame(int frameHeight){
			npc.frame.Y = targetFrame * frameHeight;
		}

		public override void OnHitPlayer(Player target, int damage, bool crit){
			if(Main.expertMode)
				target.GetModPlayer<MacrocosmPlayer>().accMoonArmorDebuff = 80;
		}

		public override Color? GetAlpha(Color drawColor)
			=> Color.White * (1f - targetAlpha / 255f);

		public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor){
			Texture2D texture = Main.npcTexture[npc.type];

			Color color = GetAlpha(drawColor) ?? Color.White;

			SpriteEffects effect = (npc.rotation > MathHelper.PiOver2 && npc.rotation < 3 * MathHelper.PiOver2) || (npc.rotation < -MathHelper.PiOver2 && npc.rotation > -3 * MathHelper.PiOver2)
				? SpriteEffects.FlipVertically
				: SpriteEffects.None;

			if(AI_Attack == ChargeAtPlayer && chargeTicks > 0){
				int length = Math.Min(chargeTicks, npc.oldPos.Length);
				for(int i = 0; i < length; i++){
					Vector2 drawPos = npc.oldPos[i] - Main.screenPosition + npc.Size / 2f;

					Color draw = color * (((float)npc.oldPos.Length - i) / npc.oldPos.Length);

					spriteBatch.Draw(texture, drawPos, npc.frame, draw * 0.6f, npc.rotation, npc.Size / 2f, npc.scale, effect, 0f);
				}
			}

			spriteBatch.Draw(texture, npc.Center - Main.screenPosition, npc.frame, color, npc.rotation, npc.Size / 2f, npc.scale, effect, 0);

			return false;
		}

		public override void AI(){
			if(!spawned){
				spawned = true;

				AI_Attack = Spawning;
				AI_Timer = 2 * 60;
				AI_AttackProgress = 0;
				targetAlpha = 255f;

				npc.TargetClosest();

				npc.spriteDirection = 1;
			}
			
			Player player = npc.target >= 0 && npc.target < Main.maxPlayers ? Main.player[npc.target] : null;

			if(AI_Attack == Wait && !(npc.target < 0 || npc.target >= Main.maxPlayers || player.dead || !player.active)){
				//Chase the new player
				AI_Attack = FloatTowardPlayer;
				AI_Timer = WaitTime;
			}

			switch(AI_Attack){
				case Spawning:
					targetAlpha -= 255f / (2 * 60);

					SpawnDusts();

					if(targetAlpha <= 0){
						targetAlpha = 0;

						AI_Attack = Wait;
					}

					targetFrame = 0;
					npc.frameCounter = 0;
					break;
				case Wait:
					//Player is dead/not connected?  Target a new one
					if(npc.target < 0 || npc.target >= Main.maxPlayers || player.dead || !player.active){
						npc.velocity *= 1f - 5f / 60f;

						if(Math.Abs(npc.velocity.X) < 0.02f)
							npc.velocity.X = 0;
						if(Math.Abs(npc.velocity.Y) < 0.02f)
							npc.velocity.Y = 0;

						npc.TargetClosest();
					}

					CycleAnimation();
					break;
				case FloatTowardPlayer:
					MoveTowardTargetPlayer(player);

					AdjustRotation(player);

					if(AI_Timer <= 0 && targetFrame == 0){
						AI_Attack = ChargeAtPlayer;
						AI_Timer = (int)(1.25f * 60);
						AI_AttackProgress = 0;
					}else
						CycleAnimation();

					break;
				case ChargeAtPlayer:
					//Wait until mouth is open
					if(AI_AttackProgress == 0){
						chargeTicks = 0;

						if(targetFrame != 2){
							AI_Timer++;

							CycleAnimation();

							MoveTowardTargetPlayer(player);

							AdjustRotation(player);
						}else{
							Main.PlaySound(SoundID.ForceRoar, (int)npc.Center.X, (int)npc.Center.Y, -1, volumeScale: 0.3f);

							Vector2 dir = npc.DirectionTo(player.Center);
							npc.rotation = dir.ToRotation();

							npc.velocity = dir * (Main.expertMode ? 25f : 15f);

							AI_AttackProgress++;
						}
					}else
						chargeTicks++;

					if(AI_Timer <= 0){
						AI_Attack = Chomp;
						AI_Timer = 20;
						AI_AttackProgress = 0;
					}
					break;
				case Chomp:
					npc.velocity *= 1f - 5f / 60f;

					if(targetFrame != 0)
						CycleAnimation();
					else if(AI_Timer <= 0){
						AI_Attack = Wait;
						AI_AttackProgress = 0;
					}
					break;
			}

			AI_Timer--;

			npc.alpha = (int)targetAlpha;

			npc.spriteDirection = npc.velocity.X > 0
				? 1
				: (npc.velocity.X < 0
					? -1
					: npc.spriteDirection);
		}

		private void MoveTowardTargetPlayer(Player player){
			const float inertia = 60f;
			float speedX = Main.expertMode ? 14f : 8f;
			float speedY = speedX * 0.5f;

			const float minDistance = 5 * 16;
			if(npc.DistanceSQ(player.Center) >= minDistance * minDistance){
				Vector2 direction = npc.DirectionTo(player.Center) * speedX;

				npc.velocity = (npc.velocity * (inertia - 1) + direction) / inertia;

				if(npc.velocity.X < -speedX)
					npc.velocity.X = -speedX;
				else if(npc.velocity.X > speedX)
					npc.velocity.X = speedX;

				if(npc.velocity.Y < -speedY)
					npc.velocity.Y = -speedY;
				else if(npc.velocity.Y > speedY)
					npc.velocity.Y = speedY;
			}
		}

		private void AdjustRotation(Player player){
			float npcRotation = npc.rotation;
			float targetRotation = npc.DirectionTo(player.Center).ToRotation();
			
			//Prevent spinning
			if(npcRotation - targetRotation > MathHelper.Pi)
				targetRotation += MathHelper.TwoPi;
			else if(targetRotation - npcRotation > MathHelper.Pi)
				npcRotation += MathHelper.TwoPi;

			npc.rotation = MathHelper.Lerp(npcRotation, targetRotation, 4.3f / 60f);
		}

		private void CycleAnimation(){
			if(++npc.frameCounter >= 10){
				npc.frameCounter = 0;
				targetFrame = ++targetFrame % Main.npcFrameCount[npc.type];
			}
		}

		private void SpawnDusts(){
			var type = ModContent.DustType<RegolithDust>();

			for(int i = 0; i < 4; i++)
				CraterDemon.SpawnDustsInner(npc.position, npc.width, npc.height, type);
		}
	}
}
