using Macrocosm.Common.Systems;
using Macrocosm.Content.Biomes;
using Macrocosm.Content.Items.Armor.Astronaut;
using Macrocosm.Content.Items.Currency;
using Macrocosm.Content.Subworlds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SubworldLibrary;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Content.NPCs.Friendly.TownNPCs
{
    [AutoloadHead]
    public class MoonChampion : ModNPC
    {
        public bool HasBeenChatWithForTheFirstTime = false;

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

            //NPCID.Sets.ShimmerTownTransform[NPC.type] = true; // TODO

            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new()
            {
                Velocity = 1f // Draws the NPC in the bestiary as if its walking +1 tiles in the x direction
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
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
            SpawnModBiomes = new int[1] { ModContent.GetInstance<MoonBiome>().Type };
        }

        public override bool CanTownNPCSpawn(int numTownNPCs)
            => WorldDataSystem.Instance.DownedCraterDemon;

        public override List<string> SetNPCNameList()
        {
            // TODO: should we localize these?
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

        private const string chatPath = "Mods.Macrocosm.NPCs.MoonChampion.Chat.";

        // NOTE: I cannot guarentee that all of this code is in tip-top shape, so if there is anything off, just let ryan know :poggers:
        // "Ok I think I made it in tip-top shape now probably" - 4mbr0s3 2
        // Localized by Feldy.. see the chat lines and comments for references in the localization files
        public override string GetChat()
        {
            Player player = Main.player[Main.myPlayer];

            if (!HasBeenChatWithForTheFirstTime)
            {
                HasBeenChatWithForTheFirstTime = true;
                NPC.netUpdate = true;
                return Language.GetTextValue(chatPath + "FirstTime");
            }

            List<string> chatBag = new();
            if (Main.dayTime)
            {
                chatBag.Add(Language.GetTextValue(chatPath + "Day1"));
                chatBag.Add(Language.GetTextValue(chatPath + "Day2"));
                chatBag.Add(Language.GetTextValue(chatPath + "Day3"));
                if (Main.rand.NextFloat() < 0.2f)
                    chatBag.Add(Language.GetTextValue(chatPath + "DayRare"));
            }
            else
            {
                chatBag.Add(Language.GetTextValue(chatPath + "Night1"));
                chatBag.Add(Language.GetTextValue(chatPath + "Night2"));
                chatBag.Add(Language.GetTextValue(chatPath + "Night3"));
            }

            int coinCount = player.CountItem(ModContent.ItemType<Moonstone>());
            if (coinCount > 0)
                chatBag.Add(Language.GetText(chatPath + "Moonstones").Format(coinCount));
            else
                chatBag.Add(Language.GetTextValue(chatPath + "NoMoonstones"));

            chatBag.Add(Language.GetTextValue(chatPath + "Standard1"));
            chatBag.Add(Language.GetTextValue(chatPath + "Standard2"));
            chatBag.Add(Language.GetTextValue(chatPath + "Standard3"));
            chatBag.Add(Language.GetTextValue(chatPath + "Standard4"));
            chatBag.Add(Language.GetTextValue(chatPath + "Standard5"));
            chatBag.Add(Language.GetTextValue(chatPath + "Standard6"));
            chatBag.Add(Language.GetTextValue(chatPath + "Standard7"));
            chatBag.Add(Language.GetTextValue(chatPath + "Standard8"));
            chatBag.Add(Language.GetTextValue(chatPath + "Standard9"));
            chatBag.Add(Language.GetTextValue(chatPath + "Standard10"));

            return chatBag[Main.rand.Next(chatBag.Count)];
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue(chatPath + "Shop");
            button2 = Language.GetTextValue(chatPath + "Advice");
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            if (firstButton)
            {
                shopName = Language.GetTextValue(chatPath + "ShopName");
            }
            else
            {
                Main.npcChatText = Main.rand.Next(3) switch
                {
                    1 => Language.GetTextValue(chatPath + "Advice1"),
                    2 => Language.GetTextValue(chatPath + "Advice2"),
                    _ => Language.GetTextValue(chatPath + "Advice3")
                };
            }
        }

        public override void PostAI()
        {
            if (!SubworldSystem.IsActive<Moon>())
                NPC.active = false;
        }

        public override void AddShops()
        {
            var shop = new NPCShop(Type);

            void AddNewSlot(int type, int price)
            {
                shop.Add(new Item(type)
                {
                    shopCustomPrice = price,
                    shopSpecialCurrency = CurrencyManager.MoonStone
                });
            }

            // TODO: for testing, subject to change - Feldy
            AddNewSlot(ItemID.SuperHealingPotion, 3);
            AddNewSlot(ModContent.ItemType<AstronautHelmet>(), 20);
            AddNewSlot(ModContent.ItemType<AstronautSuit>(), 20);
            AddNewSlot(ModContent.ItemType<AstronautLeggings>(), 20);

            shop.Register();
        }

        public override bool CanGoToStatue(bool toKingStatue) => toKingStatue;

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(HasBeenChatWithForTheFirstTime);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            HasBeenChatWithForTheFirstTime = reader.ReadBoolean();
        }

        public override void SaveData(TagCompound tag)
        {
            if (HasBeenChatWithForTheFirstTime)
                tag[nameof(HasBeenChatWithForTheFirstTime)] = true;
        }

        public override void LoadData(TagCompound tag)
        {
            HasBeenChatWithForTheFirstTime = tag.ContainsKey(nameof(HasBeenChatWithForTheFirstTime));
        }

        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 125;
            knockback = 5f;
        }
        public override void DrawTownAttackSwing(ref Texture2D item, ref Rectangle itemFrame, ref int itemSize, ref float scale, ref Vector2 offset)
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
