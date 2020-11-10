using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Macrocosm.Items.Currency;
using Macrocosm.Items.Weapons;
using Macrocosm.Items.Materials;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using SubworldLibrary;
using Macrocosm.Subworlds;

namespace Macrocosm.NPCs.Friendly.TownNPCs
{
    [AutoloadHead]
    public class Astronaut : ModNPC
    {
        public override string Texture => "Macrocosm/NPCs/Friendly/TownNPCs/Astronaut";

        public override bool Autoload(ref string name)
        {
            name = "MoonChampion";
            return mod.Properties.Autoload;
        }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 26;
            NPCID.Sets.ExtraFramesCount[npc.type] = 9;
            NPCID.Sets.AttackFrameCount[npc.type] = 5;
            NPCID.Sets.DangerDetectRange[npc.type] = 50;
            NPCID.Sets.AttackType[npc.type] = 0;
            NPCID.Sets.AttackTime[npc.type] = 60;
            NPCID.Sets.AttackAverageChance[npc.type] = 30;
            NPCID.Sets.HatOffsetY[npc.type] = 0;
        }
        public override void SetDefaults()
        {
            npc.townNPC = true;
            npc.friendly = true;
            npc.width = 18;
            npc.height = 40;
            npc.aiStyle = 7;
            npc.damage = 1000;
            npc.defense = 100;
            npc.lifeMax = 5000;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath3;
            npc.knockBackResist = 0.2f;
            animationType = NPCID.Guide;
        }
        public override bool CanTownNPCSpawn(int numTownNPCs, int money)
        {
            Player player = Main.player[Main.myPlayer];
            if (player.HasItem(ModContent.ItemType<UnuCredit>()))
            {
                return true;
            }
            return false;
        }

        public override string TownNPCName()
        {
            switch(WorldGen.genRand.Next(9))
            {
                case 0:
                    return "Mann"; // Hugh Mann (Interstellar)
                case 1:
                    return "Doyle"; // (Interstellar)
                case 2:
                    return "Romilly"; // (Interstellar)
                case 3:
                    return "Miller"; // Dr. Miller (Interstellar)?
                case 4:
                    return "Edmunds"; // Wolf Edmunds (Interstellar)
                case 5:
                    return "Neil"; // Neil Armstrong
                case 6:
                    return "Buzz"; // Buzz Aldrin
                case 7:
                    return "Chris"; // Chris Hadfield
                default:
                    return "Cooper"; // Joseph Cooper (Interstellar)
            }
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
            int unuCount = player.CountItem(ModContent.ItemType<UnuCredit>());
            if (unuCount > 0)
            {
                chatBag.Add($"I see you have {player.CountItem(ModContent.ItemType<UnuCredit>())} Moon coin{(unuCount == 1 ? "" : "s")}! Why don't you try spending them here?");
            }
            else
            {
                chatBag.Add("Hmm, you appear to have no Moon coins, try killing some Moon monsters to get some!");
            }
            chatBag.Add("What was I doing on the Moon before I got eaten? I was fighting Moon monsters, of course!");
            // Reference to Apollo 13
            chatBag.Add("A while back I discovered I could tune in on human conversations if they were happening close enough to the Moon! I kept hearing the word 'Houston' come up in their conversations. I would like to meet this Houston some day!");
            // There are 10^3 x 10^9 x 10^9 = 10^21, or a sextillion, stars, according to my Google search
            chatBag.Add("Space is so cool! There are thousands of billions of billions of stars and galaxies! There is nothing I like more than space!");
            // Reference to "Space Oddity" by David Bowie
            chatBag.Add("Planet Earth is blue, and there's nothing I can do...");
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
			if (!Subworld.IsActive<Moon>())
			{
				npc.active = false;
			}
		}
        // TODO: Bad shop, sprite fast, die hard (ambrose plesea ima die)
        // No - 4mbr0s3 2
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
            item = ModContent.GetTexture("Terraria/Item_" + ItemID.None);
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
