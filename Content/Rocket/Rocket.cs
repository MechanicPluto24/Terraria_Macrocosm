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
		
		int playerId
		{
			get => (int)NPC.ai[0];
			set => NPC.ai[0] = value;
		}

		Player player => Main.player[playerId];
		RocketPlayer rocketPlayer => player.GetModPlayer<RocketPlayer>();

		public override void AI()
		{
 			NPC.direction = 1;
			NPC.spriteDirection = -1;

			NPC.ai[1]++;

			if (NPC.ai[1] > 180)
			{
				float progress = Utils.GetLerpValue(180, 600, NPC.ai[1] + 1);
 				float invProgress = 1f - progress;

				if (NPC.ai[1] == 181)
					player.Macrocosm().ScreenShakeIntensity += 60f;		
				if (NPC.ai[1] == 220)
					player.Macrocosm().ScreenShakeIntensity += 20f;
				if (NPC.ai[1] == 240)
					player.Macrocosm().ScreenShakeIntensity += 10f;
				else
					player.Macrocosm().ScreenShakeIntensity = 15f * invProgress;

				if (NPC.ai[1] >= 260)
					NPC.velocity.Y -= 0.1f;
				else if (NPC.ai[1] >= 220)
					NPC.velocity.Y -= 0.05f;
				else 
					NPC.velocity.Y -= 0.02f;

				for(int i = 0; i < 10; i++)
				{
					for(int g = 0; g < 3; g++)
					{
						if (Main.rand.NextBool((int)(10 + (10 * invProgress))))
						{
							int type = Main.rand.NextFromList<int>(GoreID.Smoke1, GoreID.Smoke2, GoreID.Smoke3);
							Gore.NewGore(NPC.GetSource_FromAI(), NPC.position + new Vector2(Main.rand.Next(0, NPC.width / 2), NPC.height - NPC.height / 4), new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-1f, 6f)), type);

						}

					}
 					 
					Dust dust = Dust.NewDustDirect(NPC.position + new Vector2(0 + (int)((float)NPC.width/2f * progress) , NPC.height), (int)((float)NPC.width * invProgress), (int)((float)30 * progress), DustID.Flare, Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(5f, 20f) * progress, Scale: Main.rand.NextFloat(1.2f, 2.4f));
					dust.noGravity = false;
				}

 			}

			if(NPC.position.Y < 1 * 16)
			{
				rocketPlayer.InRocket = false;
				NPC.active = false;

				if (SubworldSystem.AnyActive<Macrocosm>())
				{
					if (MacrocosmSubworld.Current() is Moon)
						SubworldSystem.Exit(); // here could be the planet selection logic 
				}
				else 
					SubworldSystem.Enter<Moon>();
			}
		}

		public override void OnKill()
		{
 			rocketPlayer.InRocket = false;
 		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			if (NPC.ai[1] < 180f)
			{
				string text = (3 - (int)NPC.ai[1]/60).ToString();
				ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.DeathText.Value, text, NPC.Center + new Vector2(-15, -90) - Main.screenPosition, Color.Red, 0f, Vector2.Zero, new Vector2(1.2f));
			}
			
			return true;
		}

		public override void DrawBehind(int index)
		{
			Main.instance.DrawCacheNPCsOverPlayers.Add(index);
		}



	}
}
