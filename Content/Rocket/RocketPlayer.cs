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
		public bool InRocket { get; set; }
		public string TargetSubworldID { get; set; }

		public override void clientClone(ModPlayer clientClone)
		{
			(clientClone as RocketPlayer).InRocket = InRocket;
		}

		public override void SendClientChanges(ModPlayer clientPlayer)
		{
			if ((clientPlayer as RocketPlayer).InRocket != InRocket)
			{
				SyncPlayer(-1, -1, false);
			}
		}

		public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
		{
			UpdateStatus(InRocket);
		}

		/// <summary> This should be called on the server, when it updates a client's values </summary>
		public void UpdateStatus(bool inRocket)
		{
			ModPacket packet = Mod.GetPacket();
			packet.Write((byte)MessageType.SyncPlayerRocketStatus);
			packet.Write((byte)Player.whoAmI);
			packet.Write(inRocket);
			packet.Send();
		}

		public override void ResetEffects()
		{
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
					Player.Center = new Vector2(rocket.position.X + rocket.width / 2 - 2f, rocket.position.Y + 50);
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
