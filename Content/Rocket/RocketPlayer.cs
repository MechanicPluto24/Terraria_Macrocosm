using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rocket
{
	public class RocketPlayer : ModPlayer
	{
		public bool InRocket;
		
		public string TargetSubworldID;

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
					Player.moveSpeed = 0f;
					Player.velocity = rocket.velocity;
					Player.Center = rocket.Center + new Vector2(0,20);
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
				Player.noItems = true;

 		}
	}
}
