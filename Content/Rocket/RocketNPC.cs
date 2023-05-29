using Macrocosm.Common.Drawing;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Buffs.GoodBuffs;
using Macrocosm.Content.Buffs.GoodBuffs.MinionBuffs;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Gores;
using Macrocosm.Content.Items.Dev;
using Macrocosm.Content.Subworlds;
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
using System.IO;
using Macrocosm.Common.Netcode;
using Macrocosm.Common.Subworlds;
using Macrocosm.Content.Rocket.Navigation;

namespace Macrocosm.Content.Rocket
{
    public class RocketNPC : ModNPC
	{
		public override string Texture => "Macrocosm/Content/Rocket/RocketCommandPod"; 

		public static NPC First => Main.npc[NPC.FindFirstNPC(ModContent.NPCType<RocketNPC>())];
		public override void SetDefaults()
		{
			NPC.width = 84;
			NPC.height = 564;

			NPC.friendly = true;
			NPC.damage = 0;
			NPC.dontTakeDamage = true;
			NPC.lifeMax = 1;
			NPC.ShowNameOnHover = false;

			NPC.noGravity = true;
			NPC.noTileCollide = true;
		}

		public override bool CheckActive() => false;

		public RocketPlayer GetRocketPlayer(int playerID) => Main.player[playerID].RocketPlayer();
		public bool CheckPlayerInRocket(int playerID) => GetRocketPlayer(playerID).InRocket && GetRocketPlayer(playerID).RocketID == NPC.whoAmI;
		public bool CheckPlayerCommander(int playerID) => GetRocketPlayer(playerID).AsCommander && GetRocketPlayer(playerID).RocketID == NPC.whoAmI;

		/// <summary> Rocket sequence timer </summary>
		public int FlightTime
		{
			get => (int)NPC.ai[1];
			set => NPC.ai[1] = value;
		}

		public bool Launching
		{
			get => NPC.ai[2] != 0;
			set => NPC.ai[2] = value ? 1f : 0f;
		}

		public ref float Fuel => ref NPC.ai[3];

		#region Private vars
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
		#endregion

		public override void OnSpawn(IEntitySource source)
		{
			startYPosition = NPC.Center.Y;
		}

		/// <summary> 
		/// Gets a tick count approximation needed to reach the sky 
		/// (tough to get it exact since acceleration is not constant and speed is capped) 
		/// </summary>
		public float TimeToReachSky => (liftoffTime + 60) + MathF.Sqrt(2 * (startYPosition - worldExitPosY) / flightAcceleration);

		public float Progress => Utils.GetLerpValue(liftoffTime, TimeToReachSky, FlightTime + 1, clamped: true);
		public float InvProgress => 1f - Progress;

		public override void AI()
		{
 			NPC.direction = 1;
			NPC.spriteDirection = -1;

			Fuel = 1000f;

			if (!Launching)
			{
				Interact();
				LookForCommander();
				return;
			}

			FlightTime++;

			if (FlightTime >= liftoffTime)
			{
				SetAcceleration();
				SetScreenshake();
				VisualEffects();
 			}

			if(NPC.position.Y < worldExitPosY)
			{
				EnterDestinationSubworld();
				Despawn();
			}
		}

		public void Interact()
		{
			if (Main.netMode == NetmodeID.Server)
				return;

			if (NPC.Hitbox.Contains(Main.MouseWorld.ToPoint()) && !Launching)
			{
				if (Main.mouseRight)
				{
					bool noCommanderInRocket = (Main.netMode == NetmodeID.SinglePlayer) || !TryFindingCommander(out _);
					GetRocketPlayer(Main.myPlayer).InRocket = true;
					GetRocketPlayer(Main.myPlayer).AsCommander = noCommanderInRocket;
					GetRocketPlayer(Main.myPlayer).RocketID = NPC.whoAmI;

					// Interaction is done locally. Send embark status to the server.
					if (Main.netMode == NetmodeID.MultiplayerClient)
						SendEmbarkedPlayer(Main.myPlayer, noCommanderInRocket);
				}
				else
				{
					if(!UIRocket.Active)
					{
						Main.LocalPlayer.cursorItemIconEnabled = true;
						Main.LocalPlayer.cursorItemIconID = ModContent.ItemType<RocketPlacer>();
					}
				}
			}
		}

		private void LookForCommander()
		{
			if (Main.netMode == NetmodeID.SinglePlayer)
				return;

			if (!TryFindingCommander(out _) && AnyEmbarkedPlayers(out int id))
				GetRocketPlayer(id).AsCommander = true;
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
			if (Main.netMode == NetmodeID.SinglePlayer)
				SetScreenshakeInternal(Main.LocalPlayer);
			else
				for (int i = 0; i < Main.maxPlayers; i++)
					SetScreenshakeInternal(Main.player[i]);
		}

		private void SetScreenshakeInternal(Player player)
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

		public override void OnKill()
		{
			Despawn();
		}

		private void Despawn()
		{
			if(Main.netMode == NetmodeID.SinglePlayer)
			{
				if (CheckPlayerInRocket(Main.myPlayer))
				{
					GetRocketPlayer(Main.myPlayer).InRocket = false;
					GetRocketPlayer(Main.myPlayer).AsCommander = false;
				}
			}
 			else
			{
				for (int i = 0; i < Main.maxPlayers; i++)
				{
					if (CheckPlayerInRocket(i))
					{
						GetRocketPlayer(i).InRocket = false;
						GetRocketPlayer(i).AsCommander = false;
					}
				}
			}

			NPC.active = false;
		}

		private void EnterDestinationSubworld()
		{
			if (Main.netMode == NetmodeID.Server)
				return;

			RocketPlayer commander;
			if (Main.netMode == NetmodeID.SinglePlayer)
 				commander = GetRocketPlayer(Main.myPlayer);
 			else  
			{
				if (TryFindingCommander(out int id))
					commander = GetRocketPlayer(id);
				else if (AnyEmbarkedPlayers(out id))
					commander = GetRocketPlayer(id);
				else
					return;
 			}

			if (CheckPlayerInRocket(Main.myPlayer))
			{
				if (commander.TargetSubworldID == "Earth")
					SubworldSystem.Exit();
				else if (commander.TargetSubworldID != null && commander.TargetSubworldID != "")
					SubworldSystem.Enter(Macrocosm.Instance.Name + "/" + commander.TargetSubworldID);
			}
		}

		private bool TryFindingCommander(out int playerID)
		{
			for(int i = 0; i < Main.maxPlayers; i++)
			{
				if (!Main.player[i].active)
					continue;

				RocketPlayer rocketPlayer = GetRocketPlayer(i);
				if (CheckPlayerInRocket(i) && rocketPlayer.AsCommander)
				{
					playerID = i;
					return true;
				}
			}

			playerID = -1;
			return false;
		}

		public bool AnyEmbarkedPlayers(out int first)
		{
			if(Main.netMode == NetmodeID.SinglePlayer)
			{
				first = Main.myPlayer;
				return CheckPlayerInRocket(first);
			}

			for(int i = 0; i < Main.maxPlayers; i++)
			{
				if (!Main.player[i].active)
					continue;

				if (CheckPlayerInRocket(i))
				{
					first = i;
					return true;
				}
			}
			first = -1;
			return false;
		}

		public void SendEmbarkedPlayer(int playerID, bool asCommander)
		{
			ModPacket packet = Macrocosm.Instance.GetPacket();
			packet.Write((byte)MessageType.SyncEmbarkInRocket);
			packet.Write((byte)playerID);
			packet.Write(asCommander);
			packet.Write((byte)NPC.whoAmI);
			packet.Send();
		}

		public static void ReceiveEmbarkedPlayer(BinaryReader reader, int whoAmI)
		{
			int playerId = reader.ReadByte();
			bool asCommander = reader.ReadBoolean();
			int rocketId = reader.ReadByte();
			RocketNPC rocket = Main.npc[rocketId].ModNPC as RocketNPC;

			// Server receives embark status from client interaction 
			if (Main.netMode == NetmodeID.Server)
			{
				rocket.GetRocketPlayer(playerId).InRocket = true;
				rocket.GetRocketPlayer(playerId).AsCommander = asCommander;
				rocket.GetRocketPlayer(playerId).RocketID = rocketId;
			}
 		}

		public void Launch()
		{
			Launching = true;
			NPC.netUpdate = true;
 		}

		public void SendLaunchMessage(int toClient = -1, int ignoreClient = -1)
		{
			if (Main.netMode == NetmodeID.SinglePlayer)
				return;

			ModPacket packet = Macrocosm.Instance.GetPacket();
			packet.Write((byte)MessageType.SyncLaunchRocket);
			packet.Write((byte)NPC.whoAmI);
			packet.Send(toClient, ignoreClient);
		}

		public static void ReceiveLaunchMessage(BinaryReader reader, int whoAmI)
		{
			int rocketId = reader.ReadByte();
			RocketNPC rocket = Main.npc[rocketId].ModNPC as RocketNPC;

			// launch on the server
			rocket.Launch();

			// send the launch message to the other clients
			if (Main.netMode == NetmodeID.Server)
				rocket.SendLaunchMessage(ignoreClient: whoAmI);
		}

		public bool CheckFlightPathObstruction()
		{
			int startY = (int)((NPC.Center.Y - NPC.height/2) / 16);
			
			for(int offsetX = 0; offsetX < (NPC.width/16); offsetX++)
			{
				if (Utility.GetFirstTileCeiling((int)(NPC.position.X / 16f) + offsetX, startY, solid: true) != 10)
					return false;
			}

			return true;
		}

		private void VisualEffects()
		{
			int dustCnt = FlightTime > liftoffTime + 40 ? 10 : 4;
 			for (int i = 0; i < dustCnt; i++)
			{
				Dust dust = Dust.NewDustDirect(NPC.position + new Vector2(-10, NPC.height - 15 - 50 * Progress), (int)((float)NPC.width + 20), 1, DustID.Flare, Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(20f, 200f) * Progress, Scale: Main.rand.NextFloat(1.5f, 3f));
				dust.noGravity = false;
			}
 			
			for (int g = 0; g < 3; g++)
			{
				if (Main.rand.NextBool(2))
				{
					int type = Main.rand.NextFromList<int>(GoreID.Smoke1, GoreID.Smoke2, GoreID.Smoke3);
					Gore.NewGore(NPC.GetSource_FromAI(), NPC.position + new Vector2(Main.rand.Next(-25, NPC.width), NPC.height - 15 - 30 * Progress), new Vector2(Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(0, 8f)), type);
				}
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			if (Launching && FlightTime < liftoffTime)
			{
				string text = (liftoffTime/60 - (int)FlightTime/60).ToString();
				ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.DeathText.Value, text, NPC.position + new Vector2(26, -90) - Main.screenPosition, Color.Red, 0f, Vector2.Zero, new Vector2(1.2f));
			}

			Texture2D commandPod = TextureAssets.Npc[Type].Value;
			Texture2D serviceModule = ModContent.Request<Texture2D>("Macrocosm/Content/Rocket/RocketServiceModule", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
			Texture2D reactorModule = ModContent.Request<Texture2D>("Macrocosm/Content/Rocket/RocketReactorModule", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
			Texture2D engineModule = ModContent.Request<Texture2D>("Macrocosm/Content/Rocket/RocketEngineModule", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

			spriteBatch.Draw(commandPod, NPC.position - Main.screenPosition, null, drawColor, NPC.rotation, Vector2.Zero, 1f, SpriteEffects.None, 0);

			Vector2 servicePos = NPC.position + commandPod.Size()/2 + new Vector2(-2, commandPod.Height + 15) - Main.screenPosition;
			Vector2 reactorPos = servicePos + new Vector2(0, serviceModule.Height / 2 + reactorModule.Height / 2);
			Vector2 enginePos = reactorPos + new Vector2(0, reactorModule.Height / 2 + engineModule.Height / 2);
			spriteBatch.Draw(serviceModule, servicePos, null, drawColor, NPC.rotation, new Vector2(serviceModule.Width / 2, serviceModule.Height / 2), 1f, SpriteEffects.None, 0);
			spriteBatch.Draw(reactorModule, reactorPos, null, drawColor, NPC.rotation, new Vector2(reactorModule.Width / 2, reactorModule.Height / 2), 1f, SpriteEffects.None, 0);
			spriteBatch.Draw(engineModule, enginePos, null, drawColor, NPC.rotation, new Vector2(engineModule.Width / 2, engineModule.Height / 2), 1f, SpriteEffects.None, 0);

			return false;
		}

		/// <summary> Make the rocket draw over players while players are embarked </summary>
		public override void DrawBehind(int index)
		{
			if(AnyEmbarkedPlayers(out _))
				Main.instance.DrawCacheNPCsOverPlayers.Add(index);
		}

	}
}
