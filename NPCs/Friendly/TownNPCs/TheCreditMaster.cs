using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.UI;
using Terraria.Localization;
using Terraria.ModLoader;
using Macrocosm.Items.Currency;
using Macrocosm.Items.Weapons;
using Macrocosm.Items.Materials;

namespace Macrocosm.NPCs.Friendly.TownNPCs
{
    [AutoloadHead]
    public class TheCreditMaster : ModNPC
    {
        public override string Texture
        {
            get { return "Macrocosm/NPCs/Friendly/TownNPCs/TheCreditMaster"; }
        }

        public override bool Autoload(ref string name)
        {
            name = "TheCreditMaster";
            return mod.Properties.Autoload;
        }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 26;
            NPCID.Sets.ExtraFramesCount[npc.type] = 9;
            NPCID.Sets.AttackFrameCount[npc.type] = 5;
            NPCID.Sets.DangerDetectRange[npc.type] = 700;
            NPCID.Sets.AttackType[npc.type] = 0;
            NPCID.Sets.AttackTime[npc.type] = 60;
            NPCID.Sets.AttackAverageChance[npc.type] = 30;
            NPCID.Sets.HatOffsetY[npc.type] = 4;
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
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.knockBackResist = 1f;
            animationType = NPCID.Guide;
        }

        public override bool CanTownNPCSpawn(int numTownNPCs, int money)
        {
            for (int k = 0; k < 255; k++)
            {
                Player player = Main.player[k];
                if (!player.active)
                {
                    continue;
                }

                foreach (Item item in player.inventory)
                {
                    if (item.type == ModContent.ItemType<UnuCredit>())
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override string TownNPCName()
        {
            switch(WorldGen.genRand.Next(4))
            {
                case 0:
                    return "Baron";
                case 1:
                    return "Lord";
                case 2:
                    return "Old Guy";
                default:
                    return "Mr Money";
            }
        }

        public override string GetChat()
        {
            int otherNPC = NPC.FindFirstNPC(NPCID.Merchant);
            if(otherNPC >= 0 && Main.rand.NextBool())
            {
                return "Did you know that " + Main.npc[otherNPC].GivenName + " is the merchant?";
            }
            switch(Main.rand.Next(4))
            {
                case 0:
                    return "dialogue 1";
                case 1:
                    return "dialogue 2";
                case 2:
                    return "dialogue 3";
                default:
                    return "dialogue 4";
            }
        }
        
        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = "Shop";
            button2 = "Advice";
        }

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            if(firstButton)
            {
                shop = true;
            }
            else
            {
                Main.npcChatText = "Enemies on planets drop credits. You must have a lot, so give them to me for some nice  items.";
            }
        }

        public override void SetupShop(Chest shop, ref int nextSlot)
        {
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<ReaperEX>());
            shop.item[nextSlot].shopCustomPrice = new int?(20);
            shop.item[nextSlot].shopSpecialCurrency = CurrencyManager.UnuCredit;
            nextSlot++;
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<BanHammer>());
            shop.item[nextSlot].shopCustomPrice = new int?(20);
            shop.item[nextSlot].shopSpecialCurrency = CurrencyManager.UnuCredit;
            nextSlot++;
        }

        public override void NPCLoot()
        {
            Item.NewItem(npc.getRect(), ModContent.ItemType<CosmicDust>());
        }

        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 1000;
            knockback = 5f;
        }

        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 1;
            randExtraCooldown = 1;
        }

        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        {
            projType = ProjectileID.DemonScythe;
            attackDelay = 1;
        }

        public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
        {
            multiplier = 5f;
            randomOffset = 2f;
        }

        public override bool CheckConditions(int left, int right, int top, int bottom)
        {
            int score = 0;
            for(int x = left; x <= right; x++)
            {
                for (int y = top; y <= bottom; y++)
                {
                    int type = Main.tile[x, y].type;
                    if(type == mod.TileType("RegolithBlock"))
                    {
                        score++;
                    }
                    if(Main.tile[x, y].wall == mod.WallType("RegolithWall"))
                    {
                        score++;
                    }
                }
            }
            return score >= (right - left) * (bottom - top) / 2;
        }
    }
}