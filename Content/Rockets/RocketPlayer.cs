using Macrocosm.Common.Netcode;
using Macrocosm.Common.Utils;
using Macrocosm.Content.CameraModifiers;
using Macrocosm.Content.Rockets.UI;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets
{
    public class RocketPlayer : ModPlayer
	{
		public bool InRocket { get; set; } = false;
		public bool IsCommander { get; set; } = false;
		public int RocketID { get; set; } = -1;
		public string TargetSubworldID { get; set; } = "";

		private PanCameraModifier cameraModifier;

		public override void CopyClientState(ModPlayer targetCopy)
		{
			RocketPlayer cloneRocketPlayer = targetCopy as RocketPlayer;

			cloneRocketPlayer.InRocket = InRocket;
			cloneRocketPlayer.IsCommander = IsCommander;
			cloneRocketPlayer.RocketID = RocketID;
			cloneRocketPlayer.TargetSubworldID = TargetSubworldID;
		}

		public override void SendClientChanges(ModPlayer clientPlayer)
		{
			RocketPlayer clientRocketPlayer = clientPlayer as RocketPlayer;

			if (clientRocketPlayer.InRocket != InRocket ||
				clientRocketPlayer.IsCommander != IsCommander ||
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
			packet.Write(new BitsByte(InRocket, IsCommander));
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
			rocketPlayer.IsCommander = bb[1];
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
 				IsCommander = false;
				RocketID = -1;
				Player.mouseInterface = false;
				Player.noItems = false;
			}
  		}

		public void EmbarkPlayerInRocket(int rocketId, bool asCommander = false)
		{
			RocketID = rocketId;
			IsCommander = asCommander;

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

			Utility.UICloseOthers();

			if (Player.whoAmI == Main.myPlayer)
				RocketUISystem.Show(RocketManager.Rockets[RocketID]);
		}

		public void DisembarkFromRocket()
		{
			InRocket = false;
			IsCommander = false;

			if (Player.whoAmI == Main.myPlayer)
			{
				if (cameraModifier is not null && !cameraModifier.Finished)
					cameraModifier.ReturnToNormalPosition = true;
			}
		}

		public override void PreUpdateMovement()
		{
			if (InRocket)
			{
				Rocket rocket = RocketManager.Rockets[RocketID];

				Player.sitting.isSitting = true;

				if(rocket.InFlight || rocket.Landing)
					Player.velocity = rocket.Velocity;
				else 
					Player.velocity = Vector2.Zero;

				Player.direction = 1;
				Player.Center = new Vector2(rocket.Center.X + Player.direction * 5, rocket.Position.Y + 110) - (IsCommander ? new Vector2(0, 50) : Vector2.Zero);

				if (Player.whoAmI == Main.myPlayer)
				{
					cameraModifier.TargetPosition = RocketManager.Rockets[RocketID].Center - new Vector2(Main.screenWidth, Main.screenHeight) / 2f;

					bool escapePressed = Player.controlInv && RocketUISystem.Active;

					// Escape or 'R' will disembark this player, but not during flight
					if ((escapePressed || Player.controlMount) && !(rocket.Launched))
						DisembarkFromRocket();

					if (rocket.Launched || rocket.Landing)
						RocketUISystem.Hide();
					else if (!RocketUISystem.Active)
						RocketUISystem.Show(rocket);
				}
			}
			else if (Player.whoAmI == Main.myPlayer)
			{
				RocketUISystem.Hide();
				Player.sitting.SitUp(Player);

				if(cameraModifier is not null)
					cameraModifier.ReturnToNormalPosition = true;
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

