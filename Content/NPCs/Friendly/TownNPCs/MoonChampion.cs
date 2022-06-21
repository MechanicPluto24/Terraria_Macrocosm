using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Macrocosm.Content.Items.Currency;
using Macrocosm.Content.Items.Materials;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using SubworldLibrary;
using Macrocosm.Content.Subworlds.Moon;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Macrocosm.Content.Biomes;

namespace Macrocosm.Content.NPCs.Friendly.TownNPCs
{
    [AutoloadHead]
    public class MoonChampion : ModNPC
    {

        // These are done automatically now - Feldy

            //public override string Texture => "Macrocosm/Content/NPCs/Friendly/TownNPCs/MoonChampion";

            // public override bool Autoload(ref string name)
            // {
            //     name = "MoonChampion";
            //     return mod.Properties.Autoload;
            // }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 26;
            NPCID.Sets.ExtraFramesCount[NPC.type] = 9;
            NPCID.Sets.AttackFrameCount[NPC.type] = 5;
            NPCID.Sets.DangerDetectRange[NPC.type] = 50;
            NPCID.Sets.AttackType[NPC.type] = 0;
            NPCID.Sets.AttackTime[NPC.type] = 60;
            NPCID.Sets.AttackAverageChance[NPC.type] = 30;
            NPCID.Sets.HatOffsetY[NPC.type] = 0;
        }
        public override void SetDefaults()
        {
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.width = 18;
            NPC.height = 40;
            NPC.aiStyle = 7;
            NPC.damage = 1000;
            NPC.defense = 100;
            NPC.lifeMax = 5000;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath3;
            NPC.knockBackResist = 0.2f;
            AnimationType = NPCID.Guide;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<MoonBiome>().Type }; // Associates this NPC with the Moon Biome in Bestiary
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement(
                    "A mysterious, superhuman lunatic who has dedicated his life to protecting the Moon, and challenging all who threaten it.")
            });
        }
        
        public override bool CanTownNPCSpawn(int numTownNPCs, int money)
        {
            Player player = Main.player[Main.myPlayer];
            if (player.HasItem(ModContent.ItemType<MoonCoin>()))
            {
                return true;
            }
            return false;
        }

        public override List<string> SetNPCNameList()
        {
            return new List<string>() {
                "Mann"   , // Hugh Mann (Interstellar)
                "Doyle"  , // (Interstellar)
                "Romilly", // (Interstellar)
                "Miller" , // Dr. Miller (Interstellar)
                "Edmunds", // Wolf Edmunds (Interstellar)
                "Neil"   , // Neil Armstrong
                "Buzz"   , // Buzz Aldrin
                "Chris"  , // Chris Hadfield
                "Cooper"   // Joseph Cooper (Interstellar)
            };
        }


        // NOTE: I cannot guarentee that all of this code is in tip-top shape, so if there is anything off, just let ryan know :poggers:
        // "Ok I think I made it in tip-top shape now probably" - 4mbr0s3 2
        public override string GetChat()
        {
            Player player = Main.player[Main.myPlayer];
            /* 
            if (eventual_rescue_condition)
            {
                return "Woah... Thank you, human! You have saved me from that scary Moon monster! Perhaps you are also a champion of the Moon?";
            }
            */
            List<string> chatBag = new List<string>();
            if (Main.dayTime)
			{
                chatBag.Add("The Moon is not so bad once you get used to it! I personally find it quite beautiful! Just stay indoors during the night, I shall defend you from those evil Moon monsters!");
                if (Main.rand.NextFloat() < 0.2f)
				{
                    // Note to all: Yes, this is a TF2 reference.
                    chatBag.Add("Am I a good Moon Champion? If I wasn't a good Moon Champion, I wouldn't be sitting here discussing it with you now would I?");
				}
                chatBag.Add("I found an old space lander once! It looked abandoned, and there was this weird pole with cloth attached to it. Do you know anything about this?");
                chatBag.Add("Earth looks very pretty! I want to visit it someday, but I do not know how to leave the Moon...");
			}
            else
			{
                chatBag.Add("Human, I am curious, is the Earth made of cheese?");
                chatBag.Add("Stand alert, human! Night-time lasts far longer than it does on Earth, and lots of scary monsters emerge from the craters and shadows looking for food!");
                chatBag.Add("You look troubled, are you afraid of those Moon monsters? Do not be, for I will defend you! Those Moon monsters will never defeat me!");
			}
            int unuCount = player.CountItem(ModContent.ItemType<MoonCoin>());
            if (unuCount > 0)
            {
                chatBag.Add($"I see you have {player.CountItem(ModContent.ItemType<MoonCoin>())} Moon coin{(unuCount == 1 ? "" : "s")}! Why don't you try spending them here?");
            }
            else
            {
                chatBag.Add("Hmm, you appear to have no Moon coins, try killing some Moon monsters to get some!");
            }
            chatBag.Add("What was I doing on the Moon before I got eaten? I was fighting Moon monsters, of course!");
            // Reference to Houston Space Center, Texas
            chatBag.Add("A while back I discovered I could tune in on human conversations if they were happening close enough to the Moon! I kept hearing the word 'Houston' come up in their conversations. I would like to meet this Houston some day!");
            // Reference to "SPACE IS COOL" Songify Remix by SCHMOYOHO
            chatBag.Add("Space is so cool! There are thousands of billions of billions of stars and galaxies! There is nothing I like more than space!");
            // Reference to "Space Oddity" by David Bowie
            chatBag.Add("Planet Earth is blue, and there's nothing I can do...");
            
            // References to Touhou lore ;)
            chatBag.Add("So, what made you join the Lunar War? What do you mean you don't know any war? ");
            chatBag.Add("If you humans keep trying to establish bases here, you'll soon meet the wrath of the Lunarians.");
            chatBag.Add("How does it feel to be free from the confinements of Earth? You're lucky to have made it here. I know a fugitive princess who met an opposite fate.");
            chatBag.Add("There are many here who fled to Earth when war was declared with the Invasion of 1969. I stayed to watch the fireworks.");
            chatBag.Add("Are you looking for the Lunar Civilization? A barrier hides its existence, and, because you are human, I'm afraid you can't even come close. Well, there was an exception. One mysterious... shrine maiden.");
            // LoLK reference
            chatBag.Add("A few years ago, there was chaos in these lunar outskirts. You should've seen it yourself.");

            return chatBag[Main.rand.Next(chatBag.Count)];
        }
        
        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = "Shop";
            button2 = "Advice";
        }

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            if (firstButton)
            {
                shop = true;
            }
            else
            {
                switch (Main.rand.Next(3))
                {
                    case 1:
                        Main.npcChatText = "Monsters on the Moon drop these strange coins! If you can find enough of them, I'll trade them for some supplies!";
                        break;
                    case 2:
                        Main.npcChatText = "Be prepared for anything, and stay alert! The Moon is a very dangerous place crawling with all kinds of monsters, always be ready for anything!";
                        break;
                    default:
                        Main.npcChatText = "If you wish to explore the Moon, do not forget your spacesuit!";
                        break;
                }
            }
        }
		public override void PostAI()
		{
			base.PostAI();
            if (!SubworldSystem.IsActive<Moon>())
            {
                return;
            }
            {
				NPC.active = false;
			}
		}
        // TODO: Bad shop, sprite fast, die hard (ambrose plesea ima die)
        // No - 4mbr0s3 2
        // Æ: Sigma
        public override void SetupShop(Chest shop, ref int nextSlot)
        {
            Item AddNewSlot(ref int nextSlotRef, int type, int price)
			{
                shop.item[nextSlotRef].SetDefaults(type);
                shop.item[nextSlotRef].shopCustomPrice = price;
                shop.item[nextSlotRef].shopSpecialCurrency = CurrencyManager.UnuCredit;
                return shop.item[nextSlotRef];
            }
            AddNewSlot(ref nextSlot, ModContent.ItemType<ChandriumBar>(), 20);
        }
        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 125;
            knockback = 5f;
        }
        public override void DrawTownAttackSwing(ref Texture2D item, ref int itemSize, ref float scale, ref Vector2 offset)
        {
            Main.instance.LoadItem(ItemID.None);
            item = TextureAssets.Item[ItemID.None].Value;
            scale = 1f;
        }
        public override void TownNPCAttackSwing(ref int itemWidth, ref int itemHeight)
        {
            itemWidth = 20;
            itemHeight = 5;
        }
        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 1;
            randExtraCooldown = 1;
        }
    }
}
