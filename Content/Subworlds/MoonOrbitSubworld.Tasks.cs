using Macrocosm.Common.WorldGeneration;
using Macrocosm.Content.Tiles.Blocks.Terrain;
using Macrocosm.Content.WorldGeneration.Structures;
using Macrocosm.Content.WorldGeneration.Structures.Orbit.Moon;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.WorldBuilding;
using static Macrocosm.Common.Utils.Utility;
using static Terraria.ModLoader.ModContent;
using Macrocosm.Content.Tiles.Furniture.Industrial;
using Macrocosm.Content.Tiles.Furniture.Luminite;
using Terraria.ModLoader;
using Macrocosm.Content.Items.Accessories;
using Macrocosm.Content.Items.Armor.Vanity.Employee;
using Macrocosm.Content.Items.Bars;
using Macrocosm.Content.Items.Consumables.BossSummons;
using Macrocosm.Content.Items.Consumables.Potions;
using Macrocosm.Content.Items.Consumables.Throwable;
using Macrocosm.Content.Items.Currency;
using Macrocosm.Content.Items.Drops;
using Macrocosm.Content.Items.LiquidContainers;
using Macrocosm.Content.Items.Ores;
using Macrocosm.Content.Items.Refined;
using Macrocosm.Content.Items.Tools.Hammers;
using Macrocosm.Content.Items.Torches;
using Macrocosm.Content.Items.Weapons.Magic;
using Macrocosm.Content.Items.Weapons.Melee;
using Macrocosm.Content.Items.Weapons.Ranged;
using Macrocosm.Content.Items.Weapons.Summon;
using Macrocosm.Common.Utils;
using Macrocosm.Common.Enums;

namespace Macrocosm.Content.Subworlds;

public partial class MoonOrbitSubworld
{
    private StructureMap gen_StructureMap;
    private int gen_spawnExclusionRadius;

    [Task]
    private void PrepareTask(GenerationProgress progress)
    {
        gen_StructureMap = new();
        gen_spawnExclusionRadius = 200;
    }

    [Task]
    private void SpawnTask(GenerationProgress progress)
    {
        Main.spawnTileX = Main.maxTilesX / 2;
        Main.spawnTileY = Main.maxTilesY / 2;
    }

    [Task]
    private void MainTask(GenerationProgress progress)
    {
        BaseOrbitSubworld.CommonGen(progress,gen_StructureMap,new List<ushort>{(ushort)TileType<Tiles.Ores.ArtemiteOre>(), (ushort)TileType<Tiles.Ores.SeleniteOre>(),(ushort)TileType<Tiles.Ores.DianiteOre>(),(ushort)TileType<Tiles.Ores.ChandriumOre>()},FleshMeteors:true);
    }

    [Task]
    private void StructureTask(GenerationProgress progress)
    {
        //The cool stuff
        for (int x = 50; x < Main.maxTilesX - 50; x++)
        {
            for (int y = 50; y < Main.maxTilesY - 50; y++)
            {
                if (Math.Abs(x - Main.spawnTileX) < gen_spawnExclusionRadius)
                    continue;
                if (WorldGen.genRand.NextBool(50000))
                {
                    int random = WorldGen.genRand.Next(10);
                    Structure structure = random switch
                    {
                        0 => Structure.Get<LunarianCameoPod>(),
                        1 => Structure.Get<LunarRemnant1>(),
                        2 => Structure.Get<LuminiteOrbitVein1>(),
                        3 => Structure.Get<LuminiteOrbitVein3>(),
                        4 => Structure.Get<LuminiteOrbitVein2>(),
                        5 => Structure.Get<LuminiteOrbitVein4>(),
                        6 => Structure.Get<LunarSatellite1>(),
                        7 => Structure.Get<LCShip1>(),
                        8 => Structure.Get<LCShip2>(),
                        _ => Structure.Get<ManmadePod1>()
                        
                    };
                    
                    if (gen_StructureMap.CanPlace(new Rectangle(x - 10, y - 10, structure.Size.X + 10, structure.Size.Y + 10)))
                    {
                        structure.Place(new(x, y), gen_StructureMap);
                    }
                }
            }

        }
    }
    [Task]
    private void LootTask(GenerationProgress progress)
    {
        for (int i = 1; i < Main.maxTilesX; i++)
        {
            for (int j = 1; j < Main.maxTilesY; j++)
            {
                Tile tile = Main.tile[i, j];
                if(tile.TileType==ModContent.TileType<IndustrialChest>())
                    Utility.SetTileStyle(i, j, 0, 0);
                if(tile.TileType==ModContent.TileType<LuminiteChest>())
                    Utility.SetTileStyle(i, j, 0, 0);
            }
        }
        for (int i = 0; i < Main.maxChests; i++)
        {
            Chest chest = Main.chest[i];
            if (chest != null)
            {
                if (Main.tile[chest.x, chest.y].TileType == TileType<IndustrialChest>())
                {
                    ManageIndustrialChest(chest, i);
                }
                if (Main.tile[chest.x, chest.y].TileType == TileType<LuminiteChest>())
                {
                    ManageLuminiteChest(chest, i);
                }
            }
        }
    }

    public void ManageLuminiteChest(Chest chest, int index)
    {
        int slot = 0;
        int random;

            switch ((index % 9) + 1)
            {
                case 1:
                    chest.item[slot++].SetDefaults(ItemType<RyuguStaff>());
                    break;
                case 2:
                    chest.item[slot++].SetDefaults(ItemType<CrescentMoon>());
                    break;
                case 3:
                    chest.item[slot++].SetDefaults(ItemType<ArmstrongGauntlets>());
                    break;
                case 4:
                    chest.item[slot++].SetDefaults(ItemType<WornLunarianDagger>());
                    break;
                case 5:
                    chest.item[slot].SetDefaults(ItemType<RocheChakram>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(50, 251);
                    break;
                case 6:
                    chest.item[slot++].SetDefaults(ItemType<ArcaneBarnacle>());
                    break;
                case 7:
                    chest.item[slot++].SetDefaults(ItemType<MomentumLash>());
                    break;
                case 8:
                    chest.item[slot++].SetDefaults(ItemType<TempestuousBand>());
                    break;
                case 9:
                    chest.item[slot++].SetDefaults(ItemType<ThaumaturgicWard>());
                    break;
            }

            random = WorldGen.genRand.Next(1, 5);
            switch (random)
            {
                case 1:
                    chest.item[slot].SetDefaults(ItemType<Items.Ores.SeleniteOre>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(12, 20);
                    break;
                case 2:
                    chest.item[slot].SetDefaults(ItemType<Items.Ores.ChandriumOre>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(12, 20);
                    break;
                case 3:
                    chest.item[slot].SetDefaults(ItemType<Items.Ores.DianiteOre>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(12, 20);
                    break;
                case 4:
                    chest.item[slot].SetDefaults(ItemType<Items.Ores.ArtemiteOre>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(12, 20);
                    break;
            }
        
        
        if (WorldGen.genRand.NextBool())
        {
            chest.item[slot].SetDefaults(ItemID.LunarOre);
            chest.item[slot++].stack = WorldGen.genRand.Next(36, 105);
        }

        if (WorldGen.genRand.NextBool(15))
        {
            chest.item[slot++].SetDefaults(ItemType<CraterDemonSummon>());
        }

        random = WorldGen.genRand.Next(1, 3);
        switch (random)
        {
            case 1:
                chest.item[slot].SetDefaults(ItemType<SpaceDust>());
                chest.item[slot++].stack = WorldGen.genRand.Next(1, 20);
                break;
            case 2:
                chest.item[slot].SetDefaults(ItemType<AlienResidue>());
                chest.item[slot++].stack = WorldGen.genRand.Next(1, 20);
                break;
        }

        random = WorldGen.genRand.Next(1, 3);
        switch (random)
        {
            case 1:
                chest.item[slot].SetDefaults(ItemType<LunarCrystal>());
                chest.item[slot++].stack = WorldGen.genRand.Next(1, 45);
                break;

            case 2:
                chest.item[slot].SetDefaults(ItemType<LuminiteTorch>());
                chest.item[slot++].stack = WorldGen.genRand.Next(1, 125);
                break;
        }

        chest.item[slot].SetDefaults(ItemType<Moonstone>());
        chest.item[slot++].stack = WorldGen.genRand.Next(1, 20);
    }

    public void ManageIndustrialChest(Chest chest, int index)
    {
        int slot = 0;
        int random;

        switch ((index % 9) + 1)
        {
            case 1:
                chest.item[slot++].SetDefaults(ItemType<ClawWrench>());
                break;
            case 2:
                chest.item[slot++].SetDefaults(ItemType<StopSign>());
                chest.item[slot++].SetDefaults(ItemType<EmployeeVisor>());
                chest.item[slot++].SetDefaults(ItemType<EmployeeSuit>());
                chest.item[slot++].SetDefaults(ItemType<EmployeeBoots>());
                break;
            case 3:
                chest.item[slot++].SetDefaults(ItemType<WaveGunRed>());
                break;
            case 4:
                chest.item[slot++].SetDefaults(ItemType<Copernicus>());
                break;
            case 5:
                chest.item[slot++].SetDefaults(ItemType<HummingbirdDroneRemote>());
                break;
            case 6:
                chest.item[slot++].SetDefaults(ItemType<OsmiumBoots>());
                break;
            case 7:
                chest.item[slot++].SetDefaults(ItemType<StalwartTowerShield>());
                break;
            case 8:
                chest.item[slot++].SetDefaults(ItemType<Sledgehammer>());
                break;
            case 9:
                chest.item[slot++].SetDefaults(ItemType<LaserSight>());
                break;
        }

        random = WorldGen.genRand.Next(1, 3);
        switch (random)
        {
            case 1:
                chest.item[slot].SetDefaults(ItemType<Medkit>());
                chest.item[slot++].stack = WorldGen.genRand.Next(5, 16);
                break;

            case 2:
                chest.item[slot].SetDefaults(ItemType<AntiRadiationPills>());
                chest.item[slot++].stack = WorldGen.genRand.Next(5, 16);
                break;
        }

        for (int i = 0; i < 3; i++)
        {
            random = WorldGen.genRand.Next(1, 8);
            switch (random)
            {
                case 1:
                    chest.item[slot].SetDefaults(ItemType<Plastic>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(1, 30);
                    break;
                case 2:
                    chest.item[slot].SetDefaults(ItemType<NickelOre>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(1, 30);
                    break;
                case 3:
                    chest.item[slot].SetDefaults(ItemID.LunarOre);
                    chest.item[slot++].stack = WorldGen.genRand.Next(1, 30);
                    break;
                case 4:
                    chest.item[slot].SetDefaults(ItemType<Items.Ores.LithiumOre>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(1, 30);
                    break;
                case 5:
                    chest.item[slot].SetDefaults(ItemType<Items.Ores.AluminumOre>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(1, 30);
                    break;
                case 6:
                    chest.item[slot].SetDefaults(ItemType<SteelBar>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(1, 30);
                    break;
                case 7:
                    chest.item[slot].SetDefaults(ItemType<RocketFuelCanister>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(1, 30);
                    break;
            }
        }

        random = WorldGen.genRand.Next(1, 5);
        switch (random)
        {
            case 1:
                chest.item[slot].SetDefaults(ItemType<Items.Ores.SeleniteOre>());
                chest.item[slot++].stack = WorldGen.genRand.Next(12, 45);
                break;
            case 2:
                chest.item[slot].SetDefaults(ItemType<Items.Ores.ChandriumOre>());
                chest.item[slot++].stack = WorldGen.genRand.Next(12, 45);
                break;
            case 3:
                chest.item[slot].SetDefaults(ItemType<Items.Ores.DianiteOre>());
                chest.item[slot++].stack = WorldGen.genRand.Next(12, 45);
                break;
            case 4:
                chest.item[slot].SetDefaults(ItemType<Items.Ores.ArtemiteOre>());
                chest.item[slot++].stack = WorldGen.genRand.Next(12, 45);
                break;

        }

        for (int i = 0; i < 2; i++)
        {
            random = WorldGen.genRand.Next(1, 5);
            switch (random)
            {
                case 1:
                    chest.item[slot].SetDefaults(ItemType<Items.Blocks.IndustrialPlating>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(30, 90);
                    break;
                case 2:
                    chest.item[slot].SetDefaults(ItemType<Items.Blocks.Terrain.Protolith>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(30, 90);
                    break;
                case 3:
                    chest.item[slot].SetDefaults(ItemType<Items.Blocks.Terrain.Regolith>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(30, 90);
                    break;
                case 4:
                    chest.item[slot].SetDefaults(ItemType<Items.Blocks.Terrain.Cynthalith>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(30, 90);
                    break;

            }
        }

        chest.item[slot].SetDefaults(ItemType<LunarCrystal>());
        chest.item[slot++].stack = WorldGen.genRand.Next(1, 20);

        chest.item[slot].SetDefaults(ItemType<Moonstone>());
        chest.item[slot++].stack = WorldGen.genRand.Next(1, 20);
    }

}
