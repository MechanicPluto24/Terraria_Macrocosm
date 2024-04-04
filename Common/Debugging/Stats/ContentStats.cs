using Macrocosm.Common.Utils;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Map;
using Terraria.ModLoader;

namespace Macrocosm.Common.Debugging.Stats
{
    public class ContentStats : ModSystem
    {
        static string Path => Program.SavePathShared + "/ModSources/Macrocosm/Common/Debugging/Stats/";

        public override void PostSetupContent()
        {
        }

        public override void PostSetupRecipes()
        {
            #if DEBUG
            try
            {
                AnalyzeItems(Mod);
                AnalyzeContent(Mod);
            }
            catch
            {
                Utility.LogChatMessage("Failed to generate content stats", Utility.MessageSeverity.Warn);
            }          
            #endif
        }

        public static void AnalyzeContent(Mod mod)
        {
            using StreamWriter npcWriter = new(Path + "NPCStats.txt");
            foreach (ILoadable content in mod.GetContent())
            {
                if (content is ModNPC modNPC)
                {
                    NPC npc = modNPC.NPC;
                    string name = modNPC.Name;
                    npcWriter.WriteLine($"{name}");
                    npcWriter.WriteLine($"\tLife: {npc.lifeMax}");
                    npcWriter.WriteLine($"\tDefense: {npc.defense}");
                    npcWriter.WriteLine($"\tKB resist: {npc.knockBackResist}");
                    npcWriter.WriteLine($"\tDamage: {npc.damage}");

                    bool alreadyFoundDropsForThisNPC = false;
                    foreach (IItemDropRule rule in Main.ItemDropsDB.GetRulesForNPCID(npc.type, includeGlobalDrops: false))
                    {
                        if (!alreadyFoundDropsForThisNPC)
                        {
                            npcWriter.WriteLine("\tDrops:");
                            alreadyFoundDropsForThisNPC = true;
                        }

                        List<DropRateInfo> dropRates = new();
                        DropRateInfoChainFeed ratesInfo = new(1f);
                        rule.ReportDroprates(dropRates, ratesInfo);

                        foreach(DropRateInfo info in dropRates)
                        {
                            info.GetInfoText(false, out string stackRange, out string dropRate);
                            if (!string.IsNullOrEmpty(stackRange))
                                dropRate = stackRange + " " + dropRate;

                            string conditions = "";
                            if(info.conditions is not null)
                            {
                                foreach (IItemDropRuleCondition condition in info.conditions)
                                {
                                    string description = condition.GetConditionDescription();
                                    if (!string.IsNullOrEmpty(description))
                                        conditions += ", " + description;
                                }
                            }
                               
                            npcWriter.WriteLine($"\t\t{Lang.GetItemName(info.itemId)}, {dropRate}{conditions}");
                        }
                    }

                    npcWriter.WriteLine();
                }
            }        
        }

        public static void AnalyzeItems(Mod mod)
        {
            using StreamWriter itemWriter = new(Path + "ItemStats.txt");
            foreach (ILoadable content in mod.GetContent())
            {
                if (content is ModItem modItem)
                {
                    string name = modItem.Name;
                    itemWriter.WriteLine($"{name}");

                    Item item = modItem.Item;

                    if (item.value > 0)
                        itemWriter.WriteLine("\tValue: " + Main.ValueToCoins(item.value));
                    else
                        itemWriter.WriteLine("\tValue: No value");

                    bool alreadyFoundRecipesForThisItem = false;
                    foreach (Recipe recipe in Main.recipe)
                    {
                        if (recipe.createItem.type == item.type)
                        {
                            if (!alreadyFoundRecipesForThisItem)
                            {
                                itemWriter.WriteLine("\tRecipes:");
                                alreadyFoundRecipesForThisItem = true;
                            }

                            foreach (Item component in recipe.requiredItem)
                            {
                                itemWriter.WriteLine($"\t\t{component.Name} x {component.stack}");
                            }

                            if(recipe.requiredTile.Count > 0)
                                itemWriter.WriteLine("\t\t@");

                            foreach (int tileId in recipe.requiredTile)
                            {
                                string tileName = Lang.GetMapObjectName(MapHelper.TileToLookup(tileId, 0));
                                itemWriter.WriteLine($"\t\t{tileName}");
                            }

                            itemWriter.WriteLine();
                        }
                    }

                    itemWriter.WriteLine();
                }

                
            }
        }
    }
}
