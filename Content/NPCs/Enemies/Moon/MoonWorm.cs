using Macrocosm.Common.Global.NPCs;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Macrocosm.Content.Biomes;
using Macrocosm.Content.Projectiles.Environment.Debris;
using Macrocosm.Content.Projectiles.Hostile;
using Macrocosm.Content.Dusts;
namespace Macrocosm.Content.NPCs.Enemies.Moon
{
    // These three class showcase usage of the WormHead, WormBody and WormTail ExampleMod classes from Worm.cs
    public class MoonWormHead : WormHead, IMoonEnemy
    {
        public override int BodyType => ModContent.NPCType<MoonWormBody>();
        public override int TailType => ModContent.NPCType<MoonWormTail>();

        public override void SetStaticDefaults()
        {
            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers()    // Influences how the NPC looks in the Bestiary
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
            NPC.damage = 175;
            NPC.lifeMax = 3250;
            NPC.defense = 63;
            NPC.width = 86;
            NPC.height = 86;
            NPC.aiStyle = -1;
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, -NPC.velocity, Mod.Find<ModGore>("MoonWormHead1").Type);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, -NPC.velocity, Mod.Find<ModGore>("MoonWormHead2").Type);
            }
        }
        public override void ModifyNPCLoot(NPCLoot loot)
        {
            loot.Add(ItemDropRule.Common(ModContent.ItemType<AlienResidue>(), 1,2,8));//Make the worms drop quite a bit of residue, because they're rare.
        }
       

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
				//BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
			});
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.Player.InModBiome<MoonBiome>() && !Main.dayTime && spawnInfo.SpawnTileY <= Main.worldSurface + 200 ? .01f : 0f;
        }
        public override void Init()//Made the worm a little bit longer
        {
            // Set the segment variance
            // If you want the segment length to be constant, set these two properties to the same value
            MinSegmentLength = 14;
            MaxSegmentLength = 22;
            FlipSprite = true;
            NPC.position.Y+=1200;
            SoundEngine.PlaySound(SoundID.NPCDeath10);
            CommonWormInit(this);
        }

        public static void CommonWormInit(Worm worm)
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

        public bool CheckForSoildTile(int x, int y){
            Tile tile = Main.tile[x, y];
            if (tile.HasUnactuatedTile==true)
                return true;
            else{
            return false;
            }
        }
        public bool burst= false;//Has the worm bursted out of the ground.
        public override void AI()
        {
        
                
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
               


                



                NPC.despawnEncouraged = false;
                // tick down the attack counter.
                if (attackCounter > 0)
                    attackCounter--;

                Player target = Main.player[NPC.target];
                // If the attack counter is 0, this NPC is less than 12.5 tiles away from its target, and has a path to the target unobstructed by blocks, summon a projectile.
                if (attackCounter <= 0 && Vector2.Distance(NPC.Center, target.Center) < 2000 && Collision.CanHit(NPC.Center, 1, 1, target.Center, 1, 1)&&burst==false)
                {
                    SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode);
                    burst=true;
                    for(int i =0; i<50; i++){ //Spawn a lot of debris
                        Projectile.NewProjectile(NPC.GetSource_FromAI(),NPC.Center,NPC.velocity.SafeNormalize(Vector2.UnitX).RotatedByRandom(MathHelper.Pi)*(float)Main.rand.NextFloat(1.0f,30.0f), ModContent.ProjectileType<RegolithDebris>(),0,0f,-1);
                    }
                    for(int i =0; i<90; i++){ //Spawn a lot of dust
                                Projectile.NewProjectile(NPC.GetSource_FromAI(),NPC.Center,NPC.velocity.SafeNormalize(Vector2.UnitX).RotatedByRandom(MathHelper.Pi)*(float)Main.rand.NextFloat(1.0f,15.0f), ModContent.ProjectileType<MoonWormDust>(),0,0f,-1);
                    }
                    for(int i =0; i<10; i++)//Spawn damaging projectiles
                    {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(),NPC.Center,NPC.velocity.SafeNormalize(Vector2.UnitX).RotatedByRandom(MathHelper.Pi)*(float)Main.rand.NextFloat(10.0f,20.0f), ModContent.ProjectileType<MoonRubble>(),50,0f,-1);

                    }



                }

            }
        }
        
    }

    public class MoonWormBody : WormBody
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 2;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true }; // Hides this NPC from the Bestiary, useful for multi-part NPCs whom you only want one entry.
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
        }

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.DiggerBody);
            NPC.damage = 120;
            NPC.defense = 69;
            NPC.width = 54;
            NPC.height = 54;
            NPC.aiStyle = -1;
        }

        public override void Init()
        {
            FlipSprite = true;
            NPC.position.Y+=1200;
            MoonWormHead.CommonWormInit(this);
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                if ((int)NPC.frameCounter==1){
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, -NPC.velocity, Mod.Find<ModGore>("MoonWormSegment1").Type);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, -NPC.velocity, Mod.Find<ModGore>("MoonWormSegment2").Type);
                }
                else{
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, -NPC.velocity, Mod.Find<ModGore>("MoonWormAlternate1").Type);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, -NPC.velocity, Mod.Find<ModGore>("MoonWormAlternate2").Type);
                }
            }
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

    public class MoonWormTail : WormTail
    {
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new()
            {
                Hide = true // Hides this NPC from the Bestiary, useful for multi-part NPCs whom you only want one entry.
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
        }

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.DiggerTail);
            NPC.damage = 100;
            NPC.defense = 75;
            NPC.width = 50;
            NPC.height = 50;
            NPC.aiStyle = -1;
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
              
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, -NPC.velocity, Mod.Find<ModGore>("MoonWormTail").Type);
           
                
                
            }
        }

        public override void Init()
        {
            FlipSprite = true;
            
            MoonWormHead.CommonWormInit(this);
        }
    }
}
