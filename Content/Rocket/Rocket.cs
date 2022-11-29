using Macrocosm.Common.Drawing;
using Macrocosm.Common.Utility;
using Macrocosm.Content.Buffs.GoodBuffs;
using Macrocosm.Content.Buffs.GoodBuffs.MinionBuffs;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Gores;
using Macrocosm.Content.Subworlds;
using Macrocosm.Content.Subworlds.Moon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SubworldLibrary;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace Macrocosm.Content.Rocket
{ 
	public class Rocket : ModNPC
	{
		public override void SetDefaults()
		{
			NPC.width = 84;
			NPC.height = 84;

			NPC.friendly = true;
			NPC.damage = 0;
			NPC.lifeMax = 1;
			NPC.ShowNameOnHover = false;

			NPC.noGravity = true;
			NPC.noTileCollide = true;
		}
		
		/// <summary> The player in the command module </summary>
		public int PlayerID
		{
			get => (int)NPC.ai[0];
			set => NPC.ai[0] = value;
		}

		Player player => Main.player[PlayerID];
		RocketPlayer rocketPlayer => player.GetModPlayer<RocketPlayer>();

		/// <summary> Rocket sequence timer </summary>
		public int FlightTime
		{
			get => (int)NPC.ai[1];
			set => NPC.ai[1] = value;
		}

		// The world Y coordinate for entering the target subworld
		private float worldExitPosY = 20 * 16f;
		
		// Acceleration values 
		private float flightAcceleration = 0.1f;   // mid-flight
		private float liftoffAcceleration = 0.05f; // during liftoff
		private float startAcceleration = 0.02f;   // initial 


		private float maxFlightSpeed = 25f;

		// Number of ticks of the launch countdown (seconds * 60 ticks/sec)
		private int liftoffTime = 180;

		// Get the initial vertical position
		private float startYPosition;
		public override void OnSpawn(IEntitySource source)
		{
			startYPosition = NPC.Center.Y;
		}

		/// <summary> 
		/// Gets a tick count approximation needed to reach the sky 
		/// (tough to get it exact since acceleration is not constant and speed is capped) 
		/// </summary>
		public float TimeToReachSky => (liftoffTime + 60) + MathF.Sqrt(2 * (startYPosition - worldExitPosY) / flightAcceleration);

		public float Progress => Utils.GetLerpValue(180, TimeToReachSky, FlightTime + 1, clamped: true);
		public float InvProgress => 1f - Progress;


		public override void AI()
		{
 			NPC.direction = 1;
			NPC.spriteDirection = -1;

			FlightTime++;

			if (FlightTime >= liftoffTime)
			{
				SetAcceleration();
				SetScreenshake();
				VisualEffects();
 			}

			if(NPC.position.Y < worldExitPosY)
			{
				rocketPlayer.InRocket = false;
				NPC.active = false;
				EnterDestinationSubworld();
			}
		}

		private void EnterDestinationSubworld()
		{
			if (rocketPlayer.TargetSubworldID == "Earth")
				SubworldSystem.Exit();
			else if (rocketPlayer.TargetSubworldID.Equals("") && rocketPlayer.TargetSubworldID is not null)
				SubworldSystem.Enter("Macrocosm:" + rocketPlayer.TargetSubworldID);
		}

		private void SetAcceleration()
		{
			if (Math.Abs(NPC.velocity.Y) > maxFlightSpeed)
				return;

			if (FlightTime >= liftoffTime + 60)
				NPC.velocity.Y -= flightAcceleration;
			else if (FlightTime >= liftoffTime + 40)
				NPC.velocity.Y -= liftoffAcceleration;
			else
				NPC.velocity.Y -= startAcceleration;
		}

		private void SetScreenshake()
		{
			if (FlightTime >= liftoffTime && FlightTime < liftoffTime + 5)
				player.Macrocosm().ScreenShakeIntensity = 80f;
			if (FlightTime >= liftoffTime + 5 && FlightTime < liftoffTime + 40)
				player.Macrocosm().ScreenShakeIntensity = 40f;
			if (FlightTime >= liftoffTime + 20 && FlightTime < liftoffTime + 60)
				player.Macrocosm().ScreenShakeIntensity = 20f;
			else
				player.Macrocosm().ScreenShakeIntensity = 15f * InvProgress;
		}

		private void VisualEffects()
		{
			int dustCnt = FlightTime > liftoffTime + 40 ? 10 : 4;
 			for (int i = 0; i < dustCnt; i++)
			{
				Dust dust = Dust.NewDustDirect(NPC.position + new Vector2(0 + (int)((float)NPC.width / 2f * (Progress * 0.5f)), NPC.height - (NPC.height / 2 * Progress)), (int)((float)NPC.width * (InvProgress * 0.5f + 0.5f)), 1, DustID.Flare, Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(20f, 200f) * Progress, Scale: Main.rand.NextFloat(1.5f, 3f));
				dust.noGravity = false;
			}
 			

			for (int g = 0; g < 3; g++)
			{
				if (Main.rand.NextBool(2))
				{
					int type = Main.rand.NextFromList<int>(GoreID.Smoke1, GoreID.Smoke2, GoreID.Smoke3);
					Gore.NewGore(NPC.GetSource_FromAI(), NPC.position + new Vector2(Main.rand.Next(0, NPC.width / 2), NPC.height - NPC.height / 4), new Vector2(Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(0, 8f)), type);
				}
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			if (FlightTime < liftoffTime)
			{
				string text = (liftoffTime/60 - (int)FlightTime/60).ToString();
				ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.DeathText.Value, text, NPC.Center + new Vector2(-15, -90) - Main.screenPosition, Color.Red, 0f, Vector2.Zero, new Vector2(1.2f));
			}
			
			return true;
		}

		public override void OnKill()
		{
			rocketPlayer.InRocket = false;
		}

		/// <summary> Make the rocket draw over players </summary>
		public override void DrawBehind(int index)
		{
			Main.instance.DrawCacheNPCsOverPlayers.Add(index);
		}

	}
}
