using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;

namespace Macrocosm.Common.Utility
{
	public static class NPCUtils
	{
		/// <summary>
		/// Scales this <paramref name="npc"/>'s health by the scale <paramref name="factor"/> provided.
		/// </summary>
		/// <param name="npc">The NPC instance.</param>
		/// <param name="factor">The scale factor that this <paramref name="npc"/>'s health is scaled by.</param>
		public static void ScaleHealthBy(this NPC npc, float factor)
		{
			float bossScale = CalculateBossHealthScale(out _);

			npc.lifeMax = (int)Math.Ceiling(npc.lifeMax * Main.GameModeInfo.EnemyMaxLifeMultiplier);
			npc.lifeMax = (int)Math.Ceiling(npc.lifeMax * factor * bossScale);
		}

		public static float CalculateBossHealthScale(out int playerCount)
		{
			//This is what vanila does
			playerCount = 0;
			float healthFactor = 1f;
			float component = 0.35f;

			if (Main.netMode == NetmodeID.SinglePlayer)
			{
				playerCount = 1;
				return 1f;
			}

			for (int i = 0; i < Main.maxPlayers; i++)
				if (Main.player[i].active)
					playerCount++;

			for (int i = 0; i < playerCount; i++)
			{
				healthFactor += component;
				component += (1f - component) / 3f;
			}

			if (healthFactor > 8f)
				healthFactor = (healthFactor * 2f + 8f) / 3f;
			if (healthFactor > 1000f)
				healthFactor = 1000f;

			return healthFactor;
		}

		public static bool SummonBossDirectlyWithMessage(Vector2 targetPosition, int type)
		{
			//Try to spawn the new NPC.  If that failed, then "npc" will be 200
			int npc = NPC.NewNPC(Entity.GetSource_NaturalSpawn(), (int)targetPosition.X, (int)targetPosition.Y, type);

			//Only display the text if we could spawn the NPC
			if (npc < Main.npc.Length)
			{
				string name = Main.npc[npc].TypeName;

				//Display the "X has awoken!" text since we aren't using NPC.SpawnOnPlayer(int, int)
				Main.NewText(Language.GetTextValue("Announcement.HasAwoken", name), 175, 75, 255);
			}

			return npc != Main.npc.Length;  //Return false if we couldn't generate an NPC
		}

		public static void DrawGlowmask(this NPC npc, SpriteBatch spriteBatch, Texture2D glowmask, Vector2 screenPos, Vector2 drawOffset = default,  SpriteEffects effect = SpriteEffects.None)
		{
			int numFrames = Main.npcFrameCount[npc.type];
			int frameHeight = TextureAssets.Npc[npc.type].Height() / numFrames;
			int frame = npc.frame.Y / frameHeight;
			Rectangle sourceRect = glowmask.Frame(1, numFrames, frameY: frame);
			Vector2 origin = new Vector2(TextureAssets.Npc[npc.type].Width() / 2, frameHeight / 2) - drawOffset;
			spriteBatch.Draw(glowmask, npc.Center - screenPos, sourceRect, Color.White, npc.rotation, origin, npc.scale, effect, 0f);
		}

		public static void Move(this NPC npc, Vector2 moveTo, Vector2 offset, float speed = 3f, float turnResistance = 0.5f)
		{
			moveTo += offset; // Gets the point that the NPC will be moving to.
			Vector2 move = moveTo - npc.Center;
			float magnitude = Magnitude(move);
			if (magnitude > speed)
			{
				move *= speed / magnitude;
			}
			move = (npc.velocity * turnResistance + move) / (turnResistance + 1f);
			magnitude = Magnitude(move);
			if (magnitude > speed)
			{
				move *= speed / magnitude;
			}
			npc.velocity = move;
		}

		private static float Magnitude(Vector2 mag) => (float)Math.Sqrt(mag.X * mag.X + mag.Y * mag.Y);

		public static void UpdateScaleAndHitbox(this NPC npc, int baseWidth, int baseHeight, float newScale)
		{
			Vector2 center = npc.Center;
			npc.width = (int)Math.Max(1f, baseWidth * newScale);
			npc.height = (int)Math.Max(1f, baseHeight * newScale);
			npc.scale = newScale;
			npc.Center = center;
		}
	}
}
