using Macrocosm.Content.Dusts;
using Macrocosm.Content.UI.Rocket;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rocket
{
	public class RocketPlayer : ModPlayer
	{
		public bool InRocket { get; set; } = false;
		public bool AsCommander { get; set; } = false;
		public int RocketID { get; set; } 
		public string TargetSubworldID { get; set; } = "";


		public override void clientClone(ModPlayer clientClone)
		{
			RocketPlayer cloneRocketPlayer = clientClone as RocketPlayer;

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
			packet.Send();
		}

		public override void ResetEffects()
		{
			if (!InRocket)
 				AsCommander = false;
  		}


		public override void PreUpdateMovement()
		{
			if (InRocket)
			{
 				NPC rocket;

				if(RocketID >= 0 && RocketID < Main.maxNPCs)
				{
					rocket = Main.npc[RocketID];

					if(Player.whoAmI == Main.myPlayer)
					{
						if ((Player.controlInv || Player.controlMount) && !(rocket.ModNPC as RocketNPC).Launching)
							InRocket = false;

						if (!(rocket.ModNPC as RocketNPC).Launching)
							UIRocket.Show(RocketID);
						else
							UIRocket.Hide();
					}

					Player.moveSpeed = 0f;
					Player.velocity = rocket.velocity;
					Player.Center = new Vector2(rocket.position.X + rocket.width / 2 - 2f, rocket.position.Y + 100) - (AsCommander ? new Vector2(0, 50) : Vector2.Zero);
 				}
				else
				{
					InRocket = false;
				}	
			}
			else if (Player.whoAmI == Main.myPlayer)
				UIRocket.Hide();
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
