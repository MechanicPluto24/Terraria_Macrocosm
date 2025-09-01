using Macrocosm.Common.CrossMod;
using Macrocosm.Common.Enums;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Systems;
using Macrocosm.Common.Systems.Flags;
using Macrocosm.Content.Biomes;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Items.Accessories;
using Macrocosm.Content.Items.Armor.Vanity.MoonChampion;
using Macrocosm.Content.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Content.NPCs.TownNPCs;

[AutoloadHead]
public class MoonChampion : ModNPC
{
    public bool HasBeenChatWithForTheFirstTime = false;

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 27;
        NPCID.Sets.ExtraFramesCount[NPC.type] = 10;
        NPCID.Sets.AttackFrameCount[NPC.type] = 6;

        NPCID.Sets.AttackType[NPC.type] = 3;
        NPCID.Sets.DangerDetectRange[NPC.type] = 35;
        NPCID.Sets.AttackTime[NPC.type] = 20;
        NPCID.Sets.AttackAverageChance[NPC.type] = 1;

        NPCID.Sets.HatOffsetY[NPC.type] = 0;

        //NPCID.Sets.ShimmerTownTransform[NPC.type] = true; // TODO

        NPCSets.MoonNPC[NPC.type] = true;

        NPCSets.Material[Type] = NPCMaterial.Supernatural;

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
        SpawnModBiomes = [ModContent.GetInstance<MoonBiome>().Type];
    }

    public override bool CanTownNPCSpawn(int numTownNPCs) => WorldData.DownedCraterDemon;

    public override List<string> SetNPCNameList()
    {
        return [
            "Mann"   , // Hugh Mann (Interstellar)
            "Doyle"  , // (Interstellar)
            "Romilly", // (Interstellar)
            "Miller" , // Dr. Miller (Interstellar)
            "Edmunds", // Wolf Edmunds (Interstellar)
            "Neil"   , // Neil Armstrong
            "Buzz"   , // Buzz Aldrin
            "Chris"  , // Chris Hadfield
            "Cooper"   // Joseph Cooper (Interstellar)
        ];
    }

    private const string chatPath = "Mods.Macrocosm.NPCs.MoonChampion.Chat.";

    public override string GetChat()
    {
        Player player = Main.player[Main.myPlayer];

        if (!HasBeenChatWithForTheFirstTime)
        {
            HasBeenChatWithForTheFirstTime = true;
            NPC.netUpdate = true;
            return Language.GetTextValue(chatPath + $"Rescued{Main.rand.Next(1, 3 + 1)}");
        }

        List<string> chatBag = new();

        if (Main.bloodMoon)
        {
            for (int i = 1; i <= 4; i++)
                chatBag.Add(Language.GetTextValue(chatPath + $"BloodMoon{i}"));
        }
        /*
        else if (MacrocosmWorld.SolarStorm)
        {
            for (int i = 1; i <= 4; i++)
                chatBag.Add(Language.GetTextValue(chatPath + $"SolarStorm{i}"));
        }
        */
        else if (Main.dayTime)
        {
            for (int i = 1; i <= 9; i++)
                chatBag.Add(Language.GetTextValue(chatPath + $"Day{i}"));
        }
        else
        {
            for (int i = 1; i <= 8; i++)
                chatBag.Add(Language.GetTextValue(chatPath + $"Night{i}"));
        }

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
            Main.npcChatText = Language.GetTextValue(chatPath + $"Advice{Main.rand.Next(1, 5 + 1)}");
        }
    }

    public override void PostAI()
    {
    }

    public override void AddShops()
    {
        var shop = new NPCShop(Type);

        void AddNewSlot(int type, int price, params Condition[] conditions)
        {
            shop.Add(new Item(type)
            {
                shopCustomPrice = price,
                shopSpecialCurrency = CurrencySystem.MoonStone,
            }, conditions);
        }

        //AddNewSlot(ModContent.ItemType<RocketFuelCanister>(), 2);

        AddNewSlot(ModContent.ItemType<MoonChampionHelmet>(), 20);
        AddNewSlot(ModContent.ItemType<MoonChampionSuit>(), 20);
        AddNewSlot(ModContent.ItemType<MoonChampionLeggings>(), 20);
        AddNewSlot(ModContent.ItemType<MannedManeuveringUnit>(), 50);

        AddNewSlot(ModContent.ItemType<CrescentScripture>(), 50, new Condition(LocalizedText.Empty, () => WorldData.LuminiteShrineUnlocked));

        shop.Register();
    }

    public override void ModifyActiveShop(string shopName, Item[] items)
    {
        /*
        foreach (Item item in items)
        {
            if (item is null)
                continue;

            if (item.type == ModContent.ItemType<LunarCrystal>())
                item.stack = 20;
        }
        */
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        if (NPC.life > 0)
        {
            for (int i = 0; i < 30; i++)
            {
                int dustType = Utils.SelectRandom<int>(Main.rand, ModContent.DustType<RegolithDust>(), DustID.Blood);

                Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, dustType);
                dust.velocity.X *= (dust.velocity.X + +Main.rand.Next(0, 100) * 0.015f) * hit.HitDirection;
                dust.velocity.Y = 3f + Main.rand.Next(-50, 51) * 0.01f;
                dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
                dust.noGravity = true;
            }
        }

        if (Main.dedServ)
            return; // don't run on the server

        if (NPC.life <= 0)
        {
            var entitySource = NPC.GetSource_Death();

            Gore.NewGore(entitySource, NPC.position, NPC.velocity, Mod.Find<ModGore>("MoonChampionGoreHead").Type);
            Gore.NewGore(entitySource, NPC.position, NPC.velocity, Mod.Find<ModGore>("MoonChampionGoreArm").Type);
            Gore.NewGore(entitySource, NPC.position, NPC.velocity, Mod.Find<ModGore>("MoonChampionGoreArm").Type);
            Gore.NewGore(entitySource, NPC.position, NPC.velocity, Mod.Find<ModGore>("MoonChampionGoreLeg").Type);
            Gore.NewGore(entitySource, NPC.position, NPC.velocity, Mod.Find<ModGore>("MoonChampionGoreLeg").Type);
        }
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
        damage = 60;
        knockback = 5f;
    }

    public override void DrawTownAttackSwing(ref Texture2D item, ref Rectangle itemFrame, ref int itemSize, ref float scale, ref Vector2 offset)
    {
        item = TextureAssets.Item[ItemID.None].Value;
        scale = 1f;
    }

    public override void TownNPCAttackSwing(ref int itemWidth, ref int itemHeight)
    {
        itemWidth = 20;
        itemHeight = NPC.height;
    }

    public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
    {
        cooldown = 20;
        randExtraCooldown = 30;
    }
}
