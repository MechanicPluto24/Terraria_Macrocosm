using Macrocosm.Content.Dusts;
using Macrocosm.Content.Rocket.UI;
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
		public string TargetSubworldID { get; set; } = "";

		public override void clientClone(ModPlayer clientClone)
		{
			RocketPlayer cloneRocketPlayer = clientClone as RocketPlayer;

			cloneRocketPlayer.InRocket = InRocket;
			cloneRocketPlayer.AsCommander = InRocket;
			cloneRocketPlayer.TargetSubworldID = TargetSubworldID;
		}

		public override void SendClientChanges(ModPlayer clientPlayer)
		{
			RocketPlayer clientRocketPlayer = clientPlayer as RocketPlayer;

			if (clientRocketPlayer.InRocket != InRocket ||
				clientRocketPlayer.AsCommander != InRocket ||
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
				int rocketId = NPC.FindFirstNPC(ModContent.NPCType<Rocket>());
				NPC rocket;

				if(rocketId >= 0)
				{
					rocket = Main.npc[rocketId];

					if(Player.whoAmI == Main.myPlayer)
					{
						if ((Player.controlInv || Player.controlMount) && !(rocket.ModNPC as Rocket).Launching)
							InRocket = false;

						if (!(rocket.ModNPC as Rocket).Launching)
							UIRocket.Show(rocketId);
						else
							UIRocket.Hide();
					}

					Player.moveSpeed = 0f;
					Player.velocity = rocket.velocity;
					Player.Center = new Vector2(rocket.position.X + rocket.width / 2 - 2f, rocket.position.Y + 50) + (AsCommander ? new Vector2(0, 50) : Vector2.Zero);
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
			}
 		}
	}
}
