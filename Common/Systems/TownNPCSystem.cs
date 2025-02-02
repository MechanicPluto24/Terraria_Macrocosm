using Macrocosm.Common.Sets;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Subworlds;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using SubworldLibrary;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Common.Systems
{
    public class TownNPCSystem : ModSystem
    {
        private static readonly List<int> townNPCTypes = [];
        private static readonly Dictionary<string, string> homeSubworldByNPCPersistentID = [];

        public override void Load()
        {
            IL_Main.UpdateTime_SpawnTownNPCs += IL_Main_UpdateTime_SpawnTownNPCs;
            On_NPC.AnyNPCs += On_NPC_AnyNPCs;
        }

        public override void Unload()
        {
            IL_Main.UpdateTime_SpawnTownNPCs -= IL_Main_UpdateTime_SpawnTownNPCs;
            On_NPC.AnyNPCs -= On_NPC_AnyNPCs;
        }

        public override void PostSetupContent()
        {
            PrepareDefaultHomeSubworlds();
        }

        public override void ClearWorld()
        {
            PrepareDefaultHomeSubworlds();
        }

        public static bool IsTownNPCAllowedToSpawnHere(int type)
            => IsTownNPCAllowedToSpawnOn(MacrocosmSubworld.CurrentID, ContentSamples.NpcPersistentIdsByNetIds[type]);

        public static bool IsTownNPCAllowedToSpawnHere(string npcPersistentID)
                => IsTownNPCAllowedToSpawnOn(MacrocosmSubworld.CurrentID, npcPersistentID);

        public static bool IsTownNPCAllowedToSpawnOn(string subworldID, int type)
            => IsTownNPCAllowedToSpawnOn(ContentSamples.NpcPersistentIdsByNetIds[type], subworldID);

        public static bool IsTownNPCAllowedToSpawnOn(string subworldID, string npcPersistentID)
            => homeSubworldByNPCPersistentID.TryGetValue(npcPersistentID, out string homeSubworldID) && subworldID == homeSubworldID;

        public static void ChangeTownNPCHomeSubworld(int type, string subworldID)
            => ChangeTownNPCHomeSubworld(ContentSamples.NpcPersistentIdsByNetIds[type], subworldID);

        public static void ChangeTownNPCHomeSubworld(string npcPersistentID, string subworldID)
        {
            if (homeSubworldByNPCPersistentID.ContainsKey(npcPersistentID))
                homeSubworldByNPCPersistentID[npcPersistentID] = subworldID;
        }

        private static void PrepareDefaultHomeSubworlds()
        {
            townNPCTypes.Clear();
            homeSubworldByNPCPersistentID.Clear();

            foreach (var kvp in ContentSamples.NpcsByNetId)
            {
                int type = kvp.Key;
                NPC npc = kvp.Value;
                string persistentID = ContentSamples.NpcPersistentIdsByNetIds[type];

                if (npc.townNPC)
                {
                    townNPCTypes.Add(npc.type);

                    if (NPCSets.MoonNPC[type])
                        homeSubworldByNPCPersistentID.Add(persistentID, Moon.Instance.ID);
                    //else if(NPCSets.MarsNPC[type])
                    //  homeSubworldByNPCTypeName.Add(persistentID, ModContent.GetInstance<Mars>().ID);
                    else
                        homeSubworldByNPCPersistentID.Add(persistentID, Earth.ID);
                }
            }
        }

        public override void SaveWorldData(TagCompound tag) => SaveData(tag);
        public override void LoadWorldData(TagCompound tag) => LoadData(tag);

        public static void SaveData(TagCompound tag)
        {
            TagCompound dict = new();

            foreach (var kvp in homeSubworldByNPCPersistentID)
                dict[kvp.Key] = kvp.Value;

            tag[nameof(homeSubworldByNPCPersistentID)] = dict;
        }

        public static void LoadData(TagCompound tag)
        {
            if (tag.ContainsKey(nameof(homeSubworldByNPCPersistentID)))
            {
                TagCompound dict = tag.GetCompound(nameof(homeSubworldByNPCPersistentID));

                foreach (var kvp in dict)
                    homeSubworldByNPCPersistentID[kvp.Key] = dict.GetString(kvp.Key);
            }
        }

        // TODO: sync state across all servers
        public static void NetSync()
        {
        }

        public override void PostUpdateTime()
        {
            if (SubworldSystem.AnyActive<Macrocosm>() && !MacrocosmSubworld.Current.NormalUpdates)
                UpdateTime_SpawnTownNPCs();

            Main.checkForSpawns += 100000;
        }

        public static void UpdateTownNPCSpawns()
        {
            WorldGen.spawnDelay++;
            if (Main.invasionType > 0 || Main.eclipse)
                WorldGen.spawnDelay = 0;

            if (WorldGen.spawnDelay >= 20)
            {
                WorldGen.spawnDelay = 0;
                PrioritizeHomelessNPCs();
            }
        }

        private static void PrioritizeHomelessNPCs()
        {
            if (WorldGen.prioritizedTownNPCType != NPCID.OldMan)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].active && Main.npc[i].homeless && Main.npc[i].townNPC && Main.npc[i].type != NPCID.TravellingMerchant)
                    {
                        if (Main.npc[i].ModNPC?.TownNPCStayingHomeless is true)
                            continue;

                        WorldGen.prioritizedTownNPCType = Main.npc[i].type;
                        break;
                    }
                }
            }
        }

        // Get the vanilla Update method to spawn NPCs in !NormalUpdates subworlds
        // Does not do anything on multiplayer clients
        private static void UpdateTime_SpawnTownNPCs() => typeof(Main).InvokeMethod("UpdateTime_SpawnTownNPCs");

        // Get the method that spawns NPCs
        public static void TrySpawningTownNPC(int i, int j) => typeof(WorldGen).InvokeMethod("TrySpawningTownNPC", parameters: [i, j]);

        // Prevents NPCs from spawning if not IsTownNPCAllowedToSpawnHere
        private void IL_Main_UpdateTime_SpawnTownNPCs(ILContext il)
        {
            var c = new ILCursor(il);

            //Match:
            //  for (int k = 0; k < 200; k++)
            //      if (npc[k].active && npc[k].townNPC)
            if (!c.TryGotoNext(

                i => i.MatchLdcI4(out _),
                i => i.MatchStloc(out _),
                i => i.MatchBr(out _),
                i => i.MatchLdsfld<Main>("npc"),
                i => i.MatchLdloc(out _),
                i => i.MatchLdelemRef(),
                i => i.MatchLdfld<Entity>("active"),
                i => i.MatchBrfalse(out _),
                i => i.MatchLdsfld<Main>("npc"),
                i => i.MatchLdloc(out _),
                i => i.MatchLdelemRef(),
                i => i.MatchLdfld<NPC>("townNPC"),
                i => i.MatchBrfalse(out _)
            ))
            {
                Macrocosm.Instance.Logger.Error("Failed to inject ILHook: Terraria. Main. UpdateTime_SpawnTownNPCs");
                return;
            }
            else
            {
                // Store position before the code of the for loop, to be able to come back here 
                ILLabel preLoopLabel = c.DefineLabel();
                c.MarkLabel(preLoopLabel);

                int npcID = -1; // Literal integer representing the NPCID being checked 
                int indexOfNpcCountVariable = -1; // Local variable index of the corresponding NPC count (num2 to num39)

                List<int> npcIDs = [];
                List<int> npcCountIndexes = [];

                // This loop is only used to populate the lists with all hardcoded NPC types and their respective NPC count variable
                while (c.TryGotoNext(
                    // Match if(Main.npc[k] == type)
                    i => i.MatchLdsfld<Main>("npc"),
                    i => i.MatchLdloc(out _),
                    i => i.MatchLdelemRef(),
                    i => i.MatchLdfld<NPC>("type"),
                    i => i.MatchLdcI4(out npcID),
                    i => i.MatchBneUn(out _),

                    // Match npcCount++
                    i => i.MatchLdloc(out _),
                    i => i.MatchLdcI4(out _),
                    i => i.MatchAdd(),
                    i => i.MatchStloc(out indexOfNpcCountVariable)
                ))
                {
                    npcIDs.Add(npcID);
                    npcCountIndexes.Add(indexOfNpcCountVariable);
                }

                // Move back before the loop
                c.GotoLabel(preLoopLabel);

                // Inject checks for every NPC type.
                for (int i = 0; i < npcCountIndexes.Count; i++)
                {
                    int type = npcIDs[i];
                    int index = npcCountIndexes[i];

                    // Check if IsTownNPCAllowedToSpawnHere
                    c.Emit(OpCodes.Ldc_I4, type);
                    c.EmitDelegate((int type) => IsTownNPCAllowedToSpawnHere(type));

                    // If allowed, don't change the variable
                    ILLabel skip = c.DefineLabel();
                    c.Emit(OpCodes.Brtrue, skip);

                    // If not allowed, set the NPC count to some non-zero value
                    c.Emit(OpCodes.Ldc_I4, Main.maxNPCs - 1);
                    c.Emit(OpCodes.Stloc, index);

                    c.MarkLabel(skip);
                }
            }

            //foreach (var instruction in il.Body.Instructions)
            //     Macrocosm.Instance.Logger.Info($"{instruction.Offset:X4}: {instruction.OpCode} {instruction.Operand}");
        }

        // If not IsTownNPCAllowedToSpawnHere, make AnyNPCs() consider ModNPCs already here for the purpose of town spawning. 
        // TODO: find a better solution; Hoping to not break more than 100 mods with this -- Feldy
        private static bool On_NPC_AnyNPCs(On_NPC.orig_AnyNPCs orig, int Type)
        {
            bool result = orig(Type);

            if (!result)
            {
                if (ContentSamples.NpcsByNetId.TryGetValue(Type, out NPC npc) && npc.ModNPC != null && npc.townNPC)
                    return !IsTownNPCAllowedToSpawnHere(Type); // If not allowed to spawn, the method returns true
            }

            return result;
        }
    }
}
