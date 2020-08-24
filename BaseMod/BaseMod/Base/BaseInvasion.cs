using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Utilities;
using Terraria.ModLoader;
using Macrocosm;

namespace Macrocosm
{
	/*public class BaseInvasion
	{
		public static List<int> net = new List<int>();
		public static List<int> special = new List<int>();
		public static List<int> netAll = new List<int>();

		public int dc = 0;
		public int dcMax = 0;
		public int hardMobs = 0;
		public int hardMobsMax = 0;
		public int hardMobsCooldown = 0;
		public int hardMobsCooldownMax = 300;
		public int spawnHardMobCount = 0;
		public int killInterval = 50;		

		public int undeadipedeCount = 0;

		public int[] zombieNet;
		public int[] skeletonNet;
		public int[] zombieHalloweenNet;
		public int[] zombieHMNet;
		public int[] skeletonHMNet;
		public int[] zombieXmasHMNet;
		public int[] zombieExpertNet;		
		public int[] zombieMiscNet;

		public int[] zombieAllNet;

		public List<ZombieInfo> zombieInfo = new List<ZombieInfo>();
		
		public int displayCount = 0;
		public float displayAlpha = 0f;
		public Texture2D icon = null, iconHM = null;
		public bool lastFightComplete = false;

		public BaseInvasion(int ki)
		{
			killInterval = ki;
		}

		public void ReportInvasionProgress()
		{
			if(MWorld.zombieInvasion) displayCount = 160;
		}

		public void OnWorldUpdate()
		{
			if(Main.netMode == 1) return;
			int oldCount = undeadipedeCount;
			undeadipedeCount = Macrocosm.BaseAI.GetNPCs(default(Vector2), GRealm.UNDEADIPEDE_TYPES[0], -1, null).Length + Macrocosm.BaseAI.GetNPCs(default(Vector2), GRealm.UNDEADIPEDE_TYPES[1], -1, null).Length;	
			if(oldCount == 0 && undeadipedeCount > 0)
			{
				HandleHardMobSpawn(GRealm.UNDEADIPEDE_TYPES[0]);
			}else
			if(oldCount > 0 && undeadipedeCount == 0)
			{
				bool old = MWorld.downedUndeadipede; MWorld.downedUndeadipede = true; if(!old && Main.netMode == 2) MNet.SendNetMessage(MNet.DownedGRealmBoss, (byte)2, MWorld.downedUndeadipede);
				HandleHardMobDeath(GRealm.UNDEADIPEDE_TYPES[0]);
			}
		}

		public void Setup()
		{
            Mod mod = GRealm.self;
			if(Main.netMode != 2 && !Main.dedServ)
			{
				icon = GRealm.GetTexture("HordeIcon");
				iconHM =  GRealm.GetTexture("HordeIcon2");
			}
			zombieNet = new int[] { -45, -44, -37, -36, -35, -34, -33, -32, -31, -30, -29, -28, -27, -26, 3, 132, 186, 187, 188, 189, 200, mod.NPCType("TorchZombie"), mod.NPCType("FitnessZombie"), mod.NPCType("MaggotZombie") };
			skeletonNet = new int[] { -46, -47, -48, -49, -50, -51, -52, -53, 21 };
			zombieHalloweenNet = new int[] { 319, 320, 321, 316 };
			zombieHMNet = new int[] { mod.NPCType("Undead1a"), mod.NPCType("Undead2a"), mod.NPCType("Undead3a"), mod.NPCType("Undead4a"), mod.NPCType("Undead1b"), mod.NPCType("Undead2b"), mod.NPCType("Undead3b"), mod.NPCType("Undead4b"), mod.NPCType("Undead1c"), mod.NPCType("Undead2c"), mod.NPCType("Undead3c"), mod.NPCType("Undead4c") }; //undead
			skeletonHMNet = new int[] { 77, 110, 82 };
			zombieXmasHMNet = new int[] { 338, 339, 340 };
			zombieExpertNet = new int[] { NPCID.ArmedZombie, NPCID.ArmedZombiePincussion, NPCID.ArmedZombieSlimed, NPCID.ArmedZombieSwamp, NPCID.ArmedZombieTwiggy, NPCID.ArmedZombieCenx };
			zombieMiscNet = new int[]{ NPCID.ArmedZombieEskimo, NPCID.BloodZombie, NPCID.BigRainZombie, NPCID.SmallRainZombie, NPCID.ZombieRaincoat, NPCID.ZombieEskimo, NPCID.ZombieMushroom, NPCID.ZombieMushroomHat, NPCID.ZombieDoctor, NPCID.ZombieSuperman, NPCID.ZombiePixie, NPCID.ZombieXmas, NPCID.ZombieSweater };	

			netAll.AddRange(zombieNet);
			netAll.AddRange(skeletonNet);		
			netAll.AddRange(zombieHalloweenNet);
			netAll.AddRange(zombieHMNet);
			netAll.AddRange(skeletonHMNet);	
			netAll.AddRange(zombieXmasHMNet);	
			netAll.AddRange(zombieExpertNet);		
			netAll.AddRange(zombieMiscNet);	
			
			IDictionary<string, ModNPC> allNPCs = (IDictionary<string, ModNPC>)typeof(Mod).GetField("npcs", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(GRealm.self);
			string[] keys = new string[allNPCs.Count];
			allNPCs.Keys.CopyTo(keys, 0);
			for(int m = 0; m < keys.Length; m++)
			{
				ModNPC modNPC = allNPCs[keys[m]];
				if(modNPC is Zombies && !netAll.Contains(modNPC.npc.type) && ((Zombies)modNPC).countsForKill)
				{
					netAll.Add(modNPC.npc.type);
				}
			}
			zombieAllNet = netAll.ToArray();
		}

		public void HandleHardMobSpawn(int type)
		{
            if (Main.netMode != 1 && IsHardMob(type))
			{
				hardMobs++;
				spawnHardMobCount = Math.Max(0, spawnHardMobCount - 1);
				hardMobsCooldown = (NPC.downedMoonlord ? (int)(hardMobsCooldownMax * 0.5f) : NPC.downedFishron ? (int)(hardMobsCooldownMax * 0.75f) : hardMobsCooldownMax); //all bets are off post moon lord
				string chat = GetHardMobSpawnText(type);
				if(chat != null) Macrocosm.BaseUtility.Chat(chat, Macrocosm.BaseConstants.CHATCOLOR_PURPLE);				
			}	
		}

		public void HandleHardMobDeath(int type)
		{
			if (Main.netMode != 1 && IsHardMob(type))
			{
				hardMobs = Math.Max(0, hardMobs - 1); 
				string chat = GetHardMobDeathText(type);				
				if(chat != null) Macrocosm.BaseUtility.Chat(chat, Macrocosm.BaseConstants.CHATCOLOR_PURPLE);			
			}
		}

		public bool IsHardMob(int type)
		{
			Mod mod = GRealm.self;
			if(type == mod.NPCType("Barbarian") || type == mod.NPCType("UndeadipedeHead") || type == mod.NPCType("BumblebirbRider")) return true;
			if(zombieInfo.Count > 0)
			{
				for(int m = 0; m < zombieInfo.Count; m++)
				{
					if(zombieInfo[m].npcType == type && zombieInfo[m].hardMob) return true;
				}
			}
			return false;
		}

		public string GetHardMobSpawnText(int type)
		{
			Mod mod = GRealm.self;			
			if(type == mod.NPCType("Barbarian")){ return "A Barbarian roars into battle!"; }
			if(type == mod.NPCType("UndeadipedeHead")){ return "An Undeadipede hungers for flesh..."; }
			if(type == mod.NPCType("BumblebirbRider")){ return "A Bumblebirb Rider buzzes along..."; }
			return null; //"You spawned an impossible zombie!";
		}

		public string GetHardMobDeathText(int type)
		{
			Mod mod = GRealm.self;			
			if(type == mod.NPCType("Barbarian")){ return "A Barbarian has perished!"; }		
			if(type == mod.NPCType("UndeadipedeHead")){ return "An Undeadipede withers away..."; }
			if(type == mod.NPCType("BumblebirbRider")){ return "A Bumblebirb Rider has been fried!"; }		
			return null; //"You killed an impossible zombie!";
		}

		public virtual int GetMusic()
		{
			return -1;
		}

		public virtual void BeginInvasion(bool dev = false){}

		public virtual void EndInvasion(bool defeated, string chat, Color chatColor){}

		public virtual int GetDCValue(NPC npc)
		{				
			return 1;
		}

		public virtual void SetDCMax(){}

		public virtual void SpawnRate(ref int spawnRate, ref int maxSpawns)
		{
			int activePlayers = 0;
			for (int m = 0; m < Main.player.Length; m++)
			{
				Player p = Main.player[m]; if (p != null && p.active && !p.dead){ activePlayers++; }
			}
			spawnRate = 15;
			maxSpawns = (int)((double)5 * (2.2 + 0.3 * (double)activePlayers));
		}

		public int lastPercentage = 0;
		public void KillInterval(int prevDC, int currentDC)
		{
			int percentage = currentDC % killInterval;
			if(lastPercentage >= percentage)
			{
				for (int m = 0; m < Main.player.Length; m++)
				{
					Player p = Main.player[m];
					if (p != null && p.active && !p.dead && MWorld.InInvasionArea(p)) 
					{
						spawnHardMobCount += (MPlayer.DoubleSpawnrate(p) ? 2 : 1);
					}
				}				
			}
			lastPercentage = percentage;
		}
		
		public static IDictionary<int, float> SpawnPool(IDictionary<int, float> startingPool, int x, int y, Player player)
		{
            Mod mod = GRealm.self;
            List<int> pool = new List<int>(); pool.AddRange(startingPool.Keys);
			//SpawnPoolAll(x, y, player);	
			net.Clear();
			hardMobsCooldown = Math.Max(0, hardMobsCooldown - 1);
			int rarity = (DEV || NPC.downedBoss3 || NPC.downedQueenBee ? 1 : (NPC.downedBoss2 ? 2 : 3));			
			int rarityHM = (DEV || NPC.downedGolemBoss ? 1 : (NPC.downedPlantBoss ? 2 : 3));		
			if(zombieInfo.Count > 0)
			{
				foreach (ZombieInfo info in zombieInfo)
				{
					if(info.spawnConditions != null && info.CanSpawn(x, y, player, info.hardmode ? rarityHM : rarity))
					{
						net.Add(info.npcType);								
					}
				}
			}
			if (Main.hardMode) //hardmode zombies
			{
				if(spawnHardMobCount > 0 && hardMobs < hardMobsMax && hardMobsCooldown == 0)
				{
					bool addedHardMob = false;
					if (ModSupport.calamity != null && ModSupport.downedBoss_calamity != null && (DEV || ModSupport.downedBoss_calamity("Bumblebirb")) && Main.rand.Next(3) != 0) //bumblebirb rider
					{
						addedHardMob = true;
						net.Add(mod.NPCType("BumblebirbRider"));				
					}else
					if ((DEV || NPC.downedFishron) && Main.rand.Next(3) != 0) //undeadipede
					{
						addedHardMob = true;
						net.Add(GRealm.UNDEADIPEDE_TYPES[0]);				
					}else
					if (DEV || NPC.downedGolemBoss) //barbarian
					{
						addedHardMob = true;
						net.Add(mod.NPCType("Barbarian"));
					}
					if(addedHardMob) goto SpawnEnd;
				}
				int[] range = GetBasicHorde(player, true);
				if(range.Length > 0) net.AddRange(range);				
				if (Main.xMas && NPC.downedPlantBoss) //christmas zombies (zombie elves)
				{
					net.AddRange(zombieXmasHMNet);
				}
				if (Main.rand.Next(4) == 0) //crawler
				{
					net.Add(mod.NPCType("Crawler"));
				}
				if (ZONEDEV || player.InZone("Desert")) //hollow husk
				{
					net.Add(mod.NPCType("HollowHusk"));
				}
				
				#region boss specials
				if ((DEV || NPC.downedBoss1) && Main.rand.Next(rarityHM) == 0) //eyemaster, gundead, dynadead
				{
					if(ModSupport.calamity != null && Main.rand.Next(2) == 0)
					{
						net.Add(mod.NPCType("BlightedEyemaster"));
					}else
					{
						net.Add(mod.NPCType("Eyemaster"));
					}
					if(!NPC.downedMoonlord || Main.rand.Next(3) == 0) net.Add(mod.NPCType("Gundead"));
				}
				if ((DEV || NPC.downedBoss2) && Main.rand.Next(rarityHM) == 0) //cursed mage, crystal mage, frozen mage, ichor mage
				{
					if (ZONEDEV || player.InZone("Hallow"))
					{
						net.Add(mod.NPCType("CrystalMage"));
					}
					if (ZONEDEV || player.InZone("Corruption"))
					{
						net.Add(mod.NPCType("CursedMage"));
					}
					if (ZONEDEV || player.InZone("Crimson"))
					{
						net.Add(mod.NPCType("IchorMage"));
					}
					if (ZONEDEV || player.InZone("Snow"))
					{
						net.Add(mod.NPCType("FrozenMage"));
					}
				}
				if ((DEV || NPC.downedBoss3) && Main.rand.Next(rarityHM) == 0)//libraramaster, bone knight
				{
					net.Add(mod.NPCType("BoneKnight"));
				}
				if ((DEV || NPC.downedQueenBee) && (ZONEDEV || player.InZone("Jungle")) && Main.rand.Next(rarityHM) == 0) //wasp whisperer
				{
					net.Add(mod.NPCType("WaspWhisperer"));
				}			
				bool downedMechBossAny = NPC.downedMechBoss1 || NPC.downedMechBoss2 || NPC.downedMechBoss3;
				if ((DEV || downedMechBossAny) && Main.rand.Next(rarityHM + 1 + (NPC.downedMoonlord ? 2 : 0)) == 0) //zombinator
				{
					net.Add(mod.NPCType("Zombinator"));
				}
				if ((DEV || NPC.downedPlantBoss) && (ZONEDEV || player.InZone("Jungle")) && Main.rand.Next(rarityHM + 1) == 0) //gardener
				{
					net.Add(mod.NPCType("Gardener"));
				}
				if ((DEV || NPC.downedMoonlord) && Main.rand.Next(rarityHM + 1) == 0) //bone paladin
				{
					net.Add(mod.NPCType("BonePaladin"));
					net.Add(mod.NPCType("BoomBlaster"));
				}
				#endregion
				
				
				if ((ZONEDEV || player.InZone("Snow")) && Main.rand.Next(rarityHM) == 0) //armored viking
				{
					net.Add(197);
				}
				if ((ZONEDEV || player.InZone("Desert"))) //mummies
				{
					int type = player.InZone("Hallow") ? 80 : player.InZone("Corruption") || player.InZone("Crimson") ? 79 : 78;
					net.Add(type);
				}
				if ((ZONEDEV || player.InZone("Glowshroom"))) //mushroom zombies
				{
					net.Add(254);
					net.Add(255);
				}
			}else //normal zombies
			{
				int[] range = GetBasicHorde(player, false);
				if(range.Length > 0) net.AddRange(range);	
				if(Main.halloween) net.AddRange(zombieHalloweenNet);
				if(Main.xMas) { net.Add(331); net.Add(332); } //christmas zombies
				if(Main.raining && !Main.slimeRain){ net.Add(NPCID.BigRainZombie); net.Add(NPCID.SmallRainZombie); net.Add(NPCID.ZombieRaincoat); } //raincoat zombie
				if (ZONEDEV || player.InZone("Desert")) //sandswept zombie
				{
					net.Add(mod.NPCType("SandsweptZombie"));
				}

				#region boss specials
				if ((DEV || NPC.downedBoss1) && Main.rand.Next(rarity) == 0) //eyerider, bowman, grenader zombie
				{
					net.Add(mod.NPCType("Eyerider"));
					net.Add(mod.NPCType("BowmanZombie"));
					net.Add(mod.NPCType("GrenaderZombie"));
				}
				if ((DEV || NPC.downedBoss2) && Main.rand.Next(rarity + 1) == 0) //crude caster, corrupt caster, crim caster, cold caster
				{
					net.Add(mod.NPCType("CrudeCaster"));
					if (ZONEDEV || player.InZone("Corruption"))
					{
						net.Add(mod.NPCType("CorruptCaster"));
					}
					if (ZONEDEV || player.InZone("Crimson"))
					{
						net.Add(mod.NPCType("CrimCaster"));
					}
					if (ZONEDEV || player.InZone("Snow"))
					{
						net.Add(mod.NPCType("ColdCaster"));
					}
				}
				if ((DEV || NPC.downedBoss3) && Main.rand.Next(rarity) == 0) //librarapprentice, bone warrior
				{
					net.Add(mod.NPCType("BoneWarrior"));
				}
				if ((DEV || NPC.downedQueenBee) && (ZONEDEV || player.InZone("Jungle")) && Main.rand.Next(rarity) == 0) //beekeeper (2/5)
				{
					net.Add(mod.NPCType("Beekeeper"));
				}
				#endregion
				if (ZONEDEV || player.InZone("Snow")) //undead viking, zombie eskimo
				{
					if(Main.rand.Next(Math.Max(1, rarity - 1)) == 0) net.Add(167);
					net.Add(NPCID.ZombieEskimo);
					if(Main.expertMode) net.Add(NPCID.ArmedZombieEskimo);				
				}
				if (ZONEDEV || Macrocosm.BaseTile.LiquidCount(x, y, 25, 0) > 120) //floaty zombie
				{
					net.Add(mod.NPCType("FloatyZombie"));
				}
				if (ZONEDEV || player.ZoneBeach) //scuba zombie
				{
					net.Add(mod.NPCType("ScubaZombie"));
				}				
			}
			if((Main.invasionType > 0 || Main.pumpkinMoon || Main.snowMoon || Main.eclipse) && Main.rand.Next(8) < 5) return startingPool;
			SpawnEnd:
			pool.AddRange(net);
            for (int m = 0; m < pool.Count; m++)
            {
				if(startingPool.ContainsKey(pool[m])) startingPool.Remove(pool[m]);
                startingPool.Add(pool[m], 1f);
            }
            return startingPool;
		}
	}*/
}