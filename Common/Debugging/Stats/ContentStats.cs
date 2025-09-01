using Macrocosm.Common.Utils;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.Map;
using Terraria.ModLoader;

namespace Macrocosm.Common.Debugging.Stats
{
    public class ContentStats : ModSystem
    {
        static string Path => Program.SavePathShared;

        public override void PostSetupContent()
        {
        }

        public override void PostSetupRecipes()
        {
            /*
            #if DEBUG
            try
            {
                if (!Main.dedServ)
                    Analyze(Mod);
            }
            catch
            {
                Utility.LogChatMessage("Failed to generate content stats", Utility.MessageSeverity.Warn);
            }
            #endif
            */
        }

        public static void Analyze(Mod mod)
        {
            using StreamWriter itemWriter = new(Path + "ItemStats.txt");
            using StreamWriter npcWriter = new(Path + "NPCStats.txt");
            foreach (ILoadable content in mod.GetContent())
            {
                if (content is ModItem modItem)
                    AnalyzeItem(itemWriter, modItem);

                if (content is ModNPC modNPC)
                    AnalyzeNPC(npcWriter, modNPC);
            }
        }

        private static void AnalyzeItem(StreamWriter writer, ModItem modItem)
        {
            string name = modItem.Name;
            writer.WriteLine($"{name}");

            Item item = modItem.Item;

            if (item.value > 0)
                writer.WriteLine($"\tValue: {Main.ValueToCoins(item.value)}");
            else
                writer.WriteLine("\tValue: No value");

            writer.WriteLine($"\tResearch count: {item.ResearchUnlockCount}");

            bool alreadyFoundRecipesForThisItem = false;
            foreach (Recipe recipe in Main.recipe)
            {
                if (recipe.createItem.type == item.type)
                {
                    if (!alreadyFoundRecipesForThisItem)
                    {
                        writer.WriteLine("\tRecipes:");
                        alreadyFoundRecipesForThisItem = true;
                    }

                    foreach (Item component in recipe.requiredItem)
                    {
                        writer.WriteLine($"\t\t{component.Name} x {component.stack}");
                    }

                    if (recipe.requiredTile.Count > 0)
                        writer.WriteLine("\t\tat");

                    foreach (int tileId in recipe.requiredTile)
                    {
                        string tileName = Lang.GetMapObjectName(MapHelper.TileToLookup(tileId, 0));
                        writer.WriteLine($"\t\t{tileName}");
                    }

                    if (recipe.createItem.stack > 1)
                        writer.WriteLine($"\t\t\t=> x{recipe.createItem.stack}");

                    writer.WriteLine();
                }
            }

            writer.WriteLine();
        }

        private static void AnalyzeNPC(StreamWriter writer, ModNPC modNPC)
        {
            NPC npc = modNPC.NPC;
            string name = modNPC.Name;
            writer.WriteLine($"{name}");
            writer.WriteLine($"\tLife: {npc.lifeMax}");
            writer.WriteLine($"\tDefense: {npc.defense}");
            writer.WriteLine($"\tKB resist: {npc.knockBackResist}");
            writer.WriteLine($"\tDamage: {npc.damage}");

            bool alreadyFoundDropsForThisNPC = false;
            foreach (IItemDropRule rule in Main.ItemDropsDB.GetRulesForNPCID(npc.type, includeGlobalDrops: false))
            {
                if (!alreadyFoundDropsForThisNPC)
                {
                    writer.WriteLine("\tDrops:");
                    alreadyFoundDropsForThisNPC = true;
                }

                List<DropRateInfo> dropRates = new();
                DropRateInfoChainFeed ratesInfo = new(1f);
                rule.ReportDroprates(dropRates, ratesInfo);

                foreach (DropRateInfo info in dropRates)
                {
                    info.GetInfoText(false, out string stackRange, out string dropRate);
                    if (!string.IsNullOrEmpty(stackRange))
                        dropRate = stackRange + " " + dropRate;

                    string conditions = "";
                    if (info.conditions is not null)
                    {
                        foreach (IItemDropRuleCondition condition in info.conditions)
                        {
                            string description = condition.GetConditionDescription();
                            if (!string.IsNullOrEmpty(description))
                                conditions += ", " + description;
                        }
                    }

                    writer.WriteLine($"\t\t{Lang.GetItemName(info.itemId)}, {dropRate}{conditions}");
                }
            }

            writer.WriteLine();
        }

    }
}
