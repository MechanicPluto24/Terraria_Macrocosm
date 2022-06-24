using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Macrocosm.Content.Items.Materials;
using Macrocosm.Content.Items.Currency;
using Macrocosm.Content.Biomes;
using Microsoft.Xna.Framework;
using System;

namespace Macrocosm.Content.NPCs.Unfriendly.Enemies
{
	public class BloodMoonJuggernaut : ModNPC
	{
		public enum ActionState
        {
			Walk,
			Roar,
			Punch,
			Sprint,
			Kick
        };

		public ref float AI_State => ref NPC.localAI[0];
		public ref float AI_Timer => ref NPC.localAI[1];

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 30;
		}

		public override void SetDefaults()
		{
			NPC.width = 52;
			NPC.height = 70;
			NPC.damage = 150;
			NPC.defense = 80;
			NPC.lifeMax = 10000;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath2;
			NPC.knockBackResist = 0.5f;
			NPC.aiStyle = NPCID.Zombie;
			AIType = NPCID.Skeleton;
			Banner = Item.NPCtoBanner(NPCID.Zombie);
			BannerItem = Item.BannerToItem(Banner);
			SpawnModBiomes = new int[1] { ModContent.GetInstance<MoonBiome>().Type }; // Associates this NPC with the Moon Biome in Bestiary
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
			{
				new FlavorTextBestiaryInfoElement(
					"Big mofo")
			});
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			return (spawnInfo.Player.GetModPlayer<MacrocosmPlayer>().ZoneMoon && !Main.dayTime) ? .1f : 0f;
		}

		public override void AI()
		{
			base.AI();
		}

        public override void PostAI()
        {

			if(AI_Timer <= 0)
            {
				AI_Timer = 0;
            }

			Main.NewText(AI_Timer);

			switch (AI_State)
			{
				case (float)ActionState.Walk:

					NPC.width = 52;

					if (NPC.velocity.Y < 0f)
						NPC.velocity.Y += 0.1f;

					if (NPC.life <= NPC.lifeMax / 2)
					{
						AI_State = (float)ActionState.Sprint;
					}

					if((Math.Abs(NPC.velocity.Y) < 1f && Vector2.Distance(NPC.Center, Main.player[NPC.target].Center) <= 3 * 16f) && AI_Timer == 0) // on player close and after cooldown 
					{
						AI_State = (float)ActionState.Punch;
						AI_Timer = 150;
					}

					AI_Timer--;
					break;


				case (float)ActionState.Punch:

					NPC.velocity = Vector2.Zero; 
					NPC.width = 82; // increase hitbox 
					AI_Timer--;

					if(AI_Timer < 150 - 34)
                    {
						AI_State = (float)ActionState.Walk; // punch animation done 
					}

					break;

				case (float)ActionState.Sprint:

					NPC.knockBackResist = 0.25f;
					NPC.defense = 120;
					NPC.damage = 300;

					AIType = NPCID.LihzahrdCrawler;

					if (Math.Abs(NPC.velocity.Y) > 0.5f) // if falling 
                    {
						NPC.velocity.Y += 0.1f;				 // accelerate 	
						NPC.velocity.X = NPC.direction * 6f; // cap horiz velocity?? 
					}
                    else
                    {
						NPC.velocity.X += NPC.direction * 0.4f; // accelerate
						if (Math.Abs(NPC.velocity.X) >= 8f)
						{
							NPC.velocity.X = NPC.direction * 8f; // up to 8f
						}
					}
					
					// on block collision 
					if (NPC.HasValidTarget && NPC.collideX && Math.Abs(NPC.velocity.Y) < 1f)
					{
                        if (NPC.collideX && NPC.collideY)
                        {
							NPC.velocity.Y += 1f;	// fall if it gets stuck (still happens)
						}
                        else 
                        {
							//NPC.velocity.Y += -3f;
							Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);
                        }
					}

					break;
			}
		}

        // Frame 0		  : idle
        // Frames 1 - 8   : walking   
        // Frame 9		  : jump w/ arms
        // Frames 10 - 15 : punch 
        // Frames 15 - 17 : roar
        // Frames 18 - 26 : sprint 
        // Frame 27	      : jump w/o arms
        // Frames 28 - 30 : kick
        public override void FindFrame(int frameHeight)
		{

			bool threeTilesAboveGround = !Main.tile[(int)(NPC.Center.X / 16), (int)((NPC.Center.Y + (NPC.height / 2)) / 16)	+ 1].HasTile &&
									     !Main.tile[(int)(NPC.Center.X / 16), (int)((NPC.Center.Y + (NPC.height / 2)) / 16) + 2].HasTile &&
									     !Main.tile[(int)(NPC.Center.X / 16), (int)((NPC.Center.Y + (NPC.height / 2)) / 16) + 3].HasTile;

			NPC.spriteDirection = NPC.direction;

			if(NPC.velocity == Vector2.Zero && AI_State != (float)ActionState.Punch)
			{
				NPC.frame.Y = 0;	  // idle frame 
				NPC.frameCounter = 0;

			}
			else
            {
				switch (AI_State)
                {
					case (float)ActionState.Walk:

						NPC.frameCounter++;
                       
						if (threeTilesAboveGround || NPC.velocity.Y > 1f) 
						{
							NPC.frame.Y = 9 * frameHeight; // frame while above ground 
						}
						else
						{
							NPC.frame.Y = (int)(NPC.frameCounter / 10 + 1) * frameHeight; // 8 waling frames @ 10 ticks per frame 
							if (NPC.frameCounter >= 79)
							{
								NPC.frameCounter = 0;
							}
						}
						break;

					case (float)ActionState.Punch:

						NPC.frameCounter++;

						NPC.frame.Y = (int)(NPC.frameCounter / 7 + 10) * frameHeight; // 5 punch frames @ 7 ticks per frame 

						if (NPC.frameCounter >= 34)
						{
							NPC.frameCounter = 0;
						}
						break;
						

					case (float)ActionState.Sprint:
						if(NPC.velocity.Y < 1f)
                        {
							NPC.frameCounter++;
						}

						if (threeTilesAboveGround || NPC.velocity.Y > 1.5f)
						{
							NPC.frame.Y = 26 * frameHeight; // frame while above ground  (armless)
						}
                        else
                        {
							NPC.frame.Y = (int)(NPC.frameCounter / 5 + 18) * frameHeight; // 8 sprint frames @ 5 ticks per frame 
							if (NPC.frameCounter >= 39)
							{
								NPC.frameCounter = 0;
							}
						}
						break;
				}
			}
		}

		public override void ModifyNPCLoot(NPCLoot loot)
        {
			loot.Add(ItemDropRule.Common(ModContent.ItemType<CosmicDust>()));             // Always drop 1 cosmic dust
			loot.Add(ItemDropRule.Common(ModContent.ItemType<ArtemiteOre>(), 16, 1, 6));  // 1/16 chance to drop 1-6 Artemite Ore
			loot.Add(ItemDropRule.Common(ModContent.ItemType<ChandriumOre>(), 16, 1, 6)); // 1/16 chance to drop 1-6 Chandrium Ore
			loot.Add(ItemDropRule.Common(ModContent.ItemType<SeleniteOre>(), 16, 1, 6));  // 1/16 chance to drop 1-6 Selenite Ore
			loot.Add(ItemDropRule.Common(ModContent.ItemType<DianiteOre>(), 16, 1, 6));   // 1/16 chance to drop 1-6 DianiteOre Ore
		}

		public override void HitEffect(int hitDirection, double damage)
		{
            
            if(NPC.life > 0)
            {
				for (int i = 0; i < 10; i++)
				{
					int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood);
					Dust dust = Main.dust[dustIndex];
					dust.velocity.X *= dust.velocity.X * 1.25f * hitDirection + Main.rand.Next(0, 100) * 0.015f;
					dust.velocity.Y *= dust.velocity.Y * 0.25f + Main.rand.Next(-50, 51) * 0.01f;
					dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
				}
			}

			if (Main.netMode == NetmodeID.Server)
			{
                return; // don't run on the server
            }

			var entitySource = NPC.GetSource_Death();

			if (NPC.life <= NPC.lifeMax / 2 && AI_State != (float)ActionState.Sprint)
            {
				// TODO: Replace with it's own gore 
				Gore.NewGore(entitySource, NPC.position, NPC.velocity, Mod.Find<ModGore>("JuggernautGoreArm1").Type); 
				Gore.NewGore(entitySource, NPC.position, NPC.velocity, Mod.Find<ModGore>("JuggernautGoreArm2").Type);

				for (int i = 0; i < 50; i++)
				{
					int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood);
					Dust dust = Main.dust[dustIndex];
					dust.velocity.X *= dust.velocity.X * 1.25f * hitDirection + Main.rand.Next(0, 100) * 0.015f;
					dust.velocity.Y *= dust.velocity.Y * 0.25f + Main.rand.Next(-50, 51) * 0.01f;
					dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
				}
			}

			if (NPC.life <= 0)
			{

				for (int i = 0; i < 50; i++)
				{
					int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood);
					Dust dust = Main.dust[dustIndex];
					dust.velocity.X *= dust.velocity.X * 1.25f * hitDirection + Main.rand.Next(0, 100) * 0.015f;
					dust.velocity.Y *= dust.velocity.Y * 0.25f + Main.rand.Next(-50, 51) * 0.01f;
					dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
				}
			}
		}
	}
}
