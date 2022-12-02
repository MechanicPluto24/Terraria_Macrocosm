using Macrocosm.Content.Dusts;
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
				SyncPlayer(-1, 255, false);
			}
		}

		public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
		{
			UpdateStatus(InRocket);
		}

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
				int rocketId = NPC.FindFirstNPC(ModContent.NPCType<RocketEntity>());
				NPC rocket;

				if(rocketId >= 0)
				{
					rocket = Main.npc[rocketId];
					if(Player.whoAmI == Main.myPlayer)
					{
						Player.moveSpeed = 0f;
						Player.velocity = rocket.velocity;
						Player.Center = rocket.Center + new Vector2(0, 20);
						NetMessage.SendData(MessageID.PlayerControls, number: Player.whoAmI);
					}
					
 				}
				else
				{
					InRocket = false;
				}
					
			}
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
