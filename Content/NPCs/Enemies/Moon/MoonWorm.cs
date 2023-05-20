using Macrocosm.Content.Biomes;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.NPCs.Enemies.Moon
{
	// These three class showcase usage of the WormHead, WormBody and WormTail ExampleMod classes from Worm.cs
	internal class MoonWormHead : WormHead
	{
		public override int BodyType => ModContent.NPCType<MoonWormBody>();
		public override int TailType => ModContent.NPCType<MoonWormTail>();

		public override void SetStaticDefaults() 
		{
			var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers(0)    // Influences how the NPC looks in the Bestiary
			{   
				CustomTexturePath = "Macrocosm/Content/NPCs/Enemies/Moon/MoonWorm_Bestiary", // If the NPC is multiple parts like a worm, a custom texture for the Bestiary is encouraged.
				Position = new Vector2(40f, 24f),
				PortraitPositionXOverride = 0f,
				PortraitPositionYOverride = 12f
			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
		}

		public override void SetDefaults() 
		{
			NPC.CloneDefaults(NPCID.DiggerHead);
			NPC.damage = 100;
			NPC.defense = 20;
			NPC.width = 92;
			NPC.height = 92;
			NPC.aiStyle = -1;

			SpawnModBiomes = new int[1] { ModContent.GetInstance<MoonBiome>().Type }; // Associates this NPC with the Moon Biome in Bestiary

		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
		{
			// We can use AddRange instead of calling Add multiple times in order to add multiple items at once
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
			{
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
				//BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("BIG boi")
			});
		}

		public override void Init() 
		{
			// Set the segment variance
			// If you want the segment length to be constant, set these two properties to the same value
			MinSegmentLength = 12;
			MaxSegmentLength = 18;
			FlipSprite = true;

			CommonWormInit(this);
		}

		internal static void CommonWormInit(Worm worm) 
		{
			// These two properties handle the movement of the worm
			worm.MoveSpeed = 15.5f;
			worm.Acceleration = 0.12f;
		}

		private int attackCounter;
		public override void SendExtraAI(BinaryWriter writer) 
		{
			writer.Write(attackCounter);
		}

		public override void ReceiveExtraAI(BinaryReader reader) 
		{
			attackCounter = reader.ReadInt32();
		}

		public override void AI() {
			if (Main.netMode != NetmodeID.MultiplayerClient) 
			{
				// tick down the attack counter.
				if (attackCounter > 0) 
 					attackCounter--; 
 
				Player target = Main.player[NPC.target];
				// If the attack counter is 0, this NPC is less than 12.5 tiles away from its target, and has a path to the target unobstructed by blocks, summon a projectile.
				if (attackCounter <= 0 && Vector2.Distance(NPC.Center, target.Center) < 200 && Collision.CanHit(NPC.Center, 1, 1, target.Center, 1, 1))
				{
					// some projectile attack here?
				}
			}
		}
	}

	internal class MoonWormBody : WormBody
	{
		public override void SetStaticDefaults() 
		{
			Main.npcFrameCount[Type] = 2;
			NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0) { Hide = true }; // Hides this NPC from the Bestiary, useful for multi-part NPCs whom you only want one entry.
			NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
		}

		public override void SetDefaults() 
		{
			NPC.CloneDefaults(NPCID.DiggerBody);
			NPC.damage = 80;
			NPC.defense = 30;
			NPC.width = 58;
			NPC.height = 58;
			NPC.aiStyle = -1;
		}

		public override void Init() 
		{
			FlipSprite = true;
			MoonWormHead.CommonWormInit(this);
		}

		public override void OnSpawn(IEntitySource source)
		{
			NPC.frameCounter = Main.rand.Next(2);
		}

		public override void FindFrame(int frameHeight)
		{
			NPC.frame.Y = (int)NPC.frameCounter * frameHeight;
 		}
	}

	internal class MoonWormTail : WormTail
	{
		public override void SetStaticDefaults() 
		{
			NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) 
			{
				Hide = true // Hides this NPC from the Bestiary, useful for multi-part NPCs whom you only want one entry.
			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
		}

		public override void SetDefaults() 
		{
			NPC.CloneDefaults(NPCID.DiggerTail);
			NPC.damage = 84;
			NPC.defense = 64;
			NPC.width = 54;
			NPC.height = 54;
			NPC.aiStyle = -1;
		}

		public override void Init() 
		{
			FlipSprite = true;
			MoonWormHead.CommonWormInit(this);
		}
	}
}
