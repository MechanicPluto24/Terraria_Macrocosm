using Macrocosm.Common.Netcode;
using Macrocosm.Common.Utils;
using Macrocosm.Content.CameraModifiers;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Rockets.Navigation;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets
{
    public class RocketPlayer : ModPlayer
	{
		public bool InRocket { get; set; } = false;
		public bool AsCommander { get; set; } = false;
		public int RocketID { get; set; } = -1;
		public string TargetSubworldID { get; set; } = "";

		private PanCameraModifier cameraModifier;

		public override void CopyClientState(ModPlayer targetCopy)
		{
			RocketPlayer cloneRocketPlayer = targetCopy as RocketPlayer;

			cloneRocketPlayer.InRocket = InRocket;
			cloneRocketPlayer.AsCommander = AsCommander;
			cloneRocketPlayer.RocketID = RocketID;
			cloneRocketPlayer.TargetSubworldID = TargetSubworldID;
		}

		public override void SendClientChanges(ModPlayer clientPlayer)
		{
			RocketPlayer clientRocketPlayer = clientPlayer as RocketPlayer;

			if (clientRocketPlayer.InRocket != InRocket ||
				clientRocketPlayer.AsCommander != AsCommander ||
				clientRocketPlayer.RocketID != RocketID ||
				clientRocketPlayer.TargetSubworldID != TargetSubworldID)
			{
				SyncPlayer(-1, -1, false);
			}
		}

		public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
		{
			ModPacket packet = Mod.GetPacket();
			packet.Write((byte)MessageType.SyncPlayerRocketStatus);
			packet.Write((byte)Player.whoAmI);
			packet.Write(new BitsByte(InRocket, AsCommander));
			packet.Write((byte)RocketID);
			packet.Write(TargetSubworldID);
			packet.Send(toWho, fromWho);
		}

		public static void ReceiveSyncPlayer(BinaryReader reader, int whoAmI)
		{
			int rocketPlayerID = reader.ReadByte();
			RocketPlayer rocketPlayer = Main.player[rocketPlayerID].RocketPlayer();
			BitsByte bb = reader.ReadByte();
			rocketPlayer.InRocket = bb[0];
			rocketPlayer.AsCommander = bb[1];
			rocketPlayer.RocketID = reader.ReadByte();
			rocketPlayer.TargetSubworldID = reader.ReadString();

			if (Main.netMode == NetmodeID.Server)
				rocketPlayer.SyncPlayer(-1, whoAmI, false);
		}

		public override void ResetEffects()
		{
			if (RocketID < 0 || RocketID >= RocketManager.ActiveRocketCount)
				InRocket = false;
			else if (!RocketManager.Rockets[RocketID].Active)
				InRocket = false;

			if (!InRocket) 
			{
 				AsCommander = false;
				RocketID = -1;
				Player.mouseInterface = false;
				Player.noItems = false;
			}
  		}

		public void EmbarkPlayerInRocket(int rocketId, bool asCommander = false)
		{
			RocketID = rocketId;
			AsCommander = asCommander;

			if(Player.whoAmI == Main.myPlayer)
			{
				cameraModifier = new(RocketManager.Rockets[RocketID].Center - new Vector2(Main.screenWidth, Main.screenHeight)/2f, Main.screenPosition, 0.015f, "PlayerInRocket", Utility.QuadraticEaseOut);
				Main.instance.CameraModifiers.Add(cameraModifier);
			}

			InRocket = true;

			Player.StopVanityActions();
			Player.RemoveAllGrapplingHooks();

			if (Player.mount.Active)
				Player.mount.Dismount(Player);
		}

		public void DisembarkFromRocket()
		{
			InRocket = false;
			AsCommander = false;

			if (Player.whoAmI == Main.myPlayer)
			{
				if (cameraModifier is not null && !cameraModifier.Finished)
					cameraModifier.ReturnToOriginalPosition = true;
			}
		}

		public override void PreUpdateMovement()
		{
			if (InRocket)
			{
				Rocket rocket = RocketManager.Rockets[RocketID];

				Player.sitting.isSitting = true;

				Player.velocity = rocket.Velocity;
				Player.Center = new Vector2(rocket.Position.X + rocket.Width / 2 - 2f, rocket.Position.Y + 100) - (AsCommander ? new Vector2(0, 50) : Vector2.Zero);
				cameraModifier.TargetPosition = RocketManager.Rockets[RocketID].Center - new Vector2(Main.screenWidth, Main.screenHeight) / 2f;

				if (Player.whoAmI == Main.myPlayer)
				{
					// Escape or 'R' will disembark this player, but not during flight
					if ((Player.controlInv || Player.controlMount) && !(rocket.InFlight))
						DisembarkFromRocket();

					if (!rocket.InFlight && !rocket.Descending)
						RocketUISystem.Show(rocket);
					else
						RocketUISystem.Hide();
				}
			}
			else if (Player.whoAmI == Main.myPlayer)
			{
				RocketUISystem.Hide();
				Player.sitting.isSitting = false;
			}
		}

		public override void PreUpdateBuffs()
		{
			if (InRocket)
			{
				Player.noItems = true;
				Player.releaseMount = true;
			}
 		}
	}
}

