using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Macrocosm;

namespace Macrocosm {
    public interface GoreInfo {
        //------------------------------------------------------//
        //-----------------GORE INFO CLASS----------------------//
        //------------------------------------------------------//
        // Used to make some method shorthands for gore work.   //
        //------------------------------------------------------//
        //  Author(s): Grox the Great                           //
        //------------------------------------------------------//

        IDictionary<string, int> GetGoreArray();
    }
	
    public interface ZoneInfo {
        //------------------------------------------------------//
        //-----------------ZONE INFO CLASS----------------------//
        //------------------------------------------------------//
        // Used to make cross-compatibility with zones easier.  //
        //------------------------------------------------------//
        //  Author(s): Grox the Great                           //
        //------------------------------------------------------//

        bool InZone(Player p, string zoneName);
    }

    public static class BaseExtensions {
        //------------------------------------------------------//
        //--------------BASE EXTENSIONS CLASS-------------------//
        //------------------------------------------------------//
        // Contains various methods added to existing classes   //
        // to make modding easier, or to create shorthands.     //  
        //                                                      //
        // if you decide to use any of these, be warned: they   //
        // will ONLY work if you put Macrocosm as a dependency.   //
        //------------------------------------------------------//
        //  Author(s): Grox the Great                           //
        //------------------------------------------------------//

        public static bool InZone(this Player p, string zoneName, ZoneInfo info = null) {
            if (info != null) {
				bool inZ = info.InZone(p, zoneName);
				if(inZ) return true;
			}
            switch (zoneName) {
                //TODO: ADD IN BIOMES
                case "Space": return p.position.Y / 16 < Main.worldSurface * 0.1f;
                case "Sky": return p.position.Y / 16 > Main.worldSurface * 0.1f && p.position.Y / 16 < Main.worldSurface * 0.4f;
                case "Surface": return p.position.Y / 16 > Main.worldSurface * 0.4f && p.position.Y / 16 < Main.worldSurface;
                case "DirtLayer":
                case "Underground": return p.position.Y / 16 > Main.worldSurface && p.position.Y / 16 < Main.rockLayer;
                case "RockLayer":
                case "Cavern": return p.position.Y / 16 > Main.rockLayer && p.position.Y / 16 < Main.maxTilesY - 200;
                case "Hell": return p.position.Y / 16 > Main.maxTilesY - 200;

                case "BelowSurface": return p.position.Y / 16 > Main.worldSurface;
                case "BelowDirtLayer":
                case "BelowUnderground": return p.position.Y / 16 > Main.rockLayer;

                case "Rain": return p.ZoneRain;
                case "Desert": return p.ZoneDesert;
                case "UndergroundDesert":
                case "UGDesert": return p.ZoneUndergroundDesert;
                case "Sandstorm": return p.ZoneSandstorm;
                case "Ocean": return p.ZoneBeach;
                case "Jungle": return p.ZoneJungle;
                case "Snow": return p.ZoneSnow;

				case "Purity": return !p.ZoneTowerSolar && !p.ZoneTowerVortex && !p.ZoneTowerNebula && !p.ZoneTowerStardust && !p.ZoneBeach && !p.ZoneDesert && !p.ZoneUndergroundDesert && !p.ZoneSnow && !p.ZoneDungeon && !p.ZoneJungle && !p.ZoneCorrupt && !p.ZoneCrimson && !p.ZoneHallow && !p.ZoneMeteor && !p.ZoneGlowshroom;

				case "Meteor":
                case "Meteorite": return p.ZoneMeteor;
                //case "Granite": return p.ZoneGranite;
                //case "Marble": return p.ZoneMarble;
                case "GlowingMushroom":
                case "Glowshroom": return p.ZoneGlowshroom;

				case "Corrupt":
                case "Corruption": return p.ZoneCorrupt;
				case "Crim":
                case "Crimson": return p.ZoneCrimson;
                case "Hallow": return p.ZoneHallow;
                case "Dungeon": return p.ZoneDungeon;
				
				case "TowerAny": return (p.ZoneTowerSolar || p.ZoneTowerVortex || p.ZoneTowerNebula || p.ZoneTowerStardust);
				case "TowerSolar": return p.ZoneTowerSolar;
				case "TowerVortex": return p.ZoneTowerVortex;
				case "TowerNebula": return p.ZoneTowerNebula;
				case "TowerStardust": return p.ZoneTowerStardust;
				
                //case "Lihzahrd": return p.ZoneLihzahrd;

                /*
                    ZoneCorrupt
                    ZoneHoly
                    ZoneMeteor
                    ZoneJungle
                    ZoneSnow
                    ZoneCrimson
                    ZoneWaterCandle
                    ZonePeaceCandle
                    ZoneTowerSolar
                    ZoneTowerVortex
                    ZoneTowerNebula
                    ZoneTowerStardust
                    ZoneDesert
                    ZoneGlowshroom
                    ZoneUndergroundDesert
                    ZoneSkyHeight
                    ZoneOverworldHeight
                    ZoneDirtLayerHeight
                    ZoneRockLayerHeight
                    ZoneUnderworldHeight
                    ZoneBeach
                    ZoneRain
                    ZoneSandstorm
                 */
            }
            return false;
        }

        public static void AddRecipeGroup(this Recipe recipe, Mod mod, string groupName, int count)  {
            Mod m = (mod == null ? recipe.Mod : mod);
            recipe.AddRecipeGroup(m.Name + ":" + groupName, count);
        }
        public static void AddItem(this Recipe recipe, int itemID, int count) { recipe.AddIngredient(itemID, count); }
        public static void AddItem(this Recipe recipe, Mod mod, string itemName, int count) { recipe.AddIngredient((mod == null ? recipe.Mod : mod), itemName, count); }

        public static void ClearBuff(this Player player, Mod mod, string name) { player.ClearBuff(mod.BuffType(name)); }
        public static void AddBuff(this Player player, Mod mod, string name, int time, bool sync = true) { player.AddBuff(mod.BuffType(name), time, sync); }
        public static int FindBuffIndex(this Player player, Mod mod, string name){ return player.FindBuffIndex(mod.BuffType(name)); }

        public static int GoreType(this Mod mod, string name, IDictionary<string, int> gores = null) { return BaseUtility.CheckForGore(mod, name, gores); }
        public static int MusicType(this Mod mod, string name, string prefix = "Sounds/Music/") { return mod.GetSoundSlot(Terraria.ModLoader.SoundType.Music, prefix + name); }    
        
        public static LegacySoundStyle SoundCustom(this Mod mod, string name, string prefix = "Sounds/Custom/") { return mod.GetLegacySoundSlot(Terraria.ModLoader.SoundType.Custom, prefix + name); }
        public static LegacySoundStyle SoundItem(this Mod mod, string name, string prefix = "Sounds/Item/") { return mod.GetLegacySoundSlot(Terraria.ModLoader.SoundType.Item, prefix + name); }
        public static LegacySoundStyle SoundNPCHit(this Mod mod, string name, string prefix = "Sounds/NPCHit/") { return mod.GetLegacySoundSlot(Terraria.ModLoader.SoundType.NPCHit, prefix + name); }
        public static LegacySoundStyle SoundNPCKilled(this Mod mod, string name, string prefix = "Sounds/NPCKilled/") { return mod.GetLegacySoundSlot(Terraria.ModLoader.SoundType.NPCKilled, prefix + name); }  
     
        public static int ProjType(this Mod mod, string name) { return mod.ProjectileType(name); }

        public static bool ReadBool(this BinaryReader w) { return w.ReadBoolean(); }
        public static int ReadInt(this BinaryReader w) { return w.ReadInt32(); }
        public static short ReadShort(this BinaryReader w) { return w.ReadInt16(); }
	    public static ushort ReadUShort(this BinaryReader w) { return w.ReadUInt16(); }	
        public static float ReadFloat(this BinaryReader w) { return w.ReadSingle(); }

        public static bool IsBlank(this Item item) {
            if (Item.type <= 0 || Item.stack <= 0) return true;
            return string.IsNullOrEmpty(Item.Name);
        }

        public static bool water(this Tile tile) { return tile.LiquidType == LiquidID.Water; }
        public static bool lava(this Tile tile) { return tile.LiquidType == LiquidID.Lava; }
        public static bool honey(this Tile tile) { return tile.LiquidType == LiquidID.Honey; }
    }

    public class BaseConstants {
        //------------------------------------------------------//
        //---------------BASE CONSTANTS CLASS-------------------//
        //------------------------------------------------------//
        // Contains various constants that could be useful in   //
        // modding.                                             //
        //------------------------------------------------------//
        //  Author(s): Grox the Great                           //
        //------------------------------------------------------//

        //---Alternating Variables---//

        //returns the name of the client's player. If the player is null or it's called on the server, it returns null.
		public static string NAME_MAINPLAYER { get { return Main.netMode == 2 || Main.player[Main.myPlayer] == null ? null : Main.player[Main.myPlayer].name; } }
        
        //returns the client player. If the player is null or it's called on the server, it returns null.
        public static Player MAINPLAYER { get { return Main.netMode == 2 ? null : Main.player[Main.myPlayer]; } }

        //returns the name of the given npc. If the npc isn't in the world, it returns null.
        public static string NAME_GUIDE { get { return NPC.AnyNPCs(22) ? NPC.GetFirstNPCNameOrNull(22) : null; } }
        public static string NAME_MERCHANT{ get{ return NPC.AnyNPCs(17) ? NPC.GetFirstNPCNameOrNull(17) : null; } }
        public static string NAME_NURSE { get { return NPC.AnyNPCs(18) ? NPC.GetFirstNPCNameOrNull(18) : null; } }
        public static string NAME_ARMSDEALER { get { return NPC.AnyNPCs(19) ? NPC.GetFirstNPCNameOrNull(19) : null; } }
        public static string NAME_DRYAD { get { return NPC.AnyNPCs(20) ? NPC.GetFirstNPCNameOrNull(20) : null; } }
        public static string NAME_DEMOLITIONIST { get { return NPC.AnyNPCs(38) ? NPC.GetFirstNPCNameOrNull(38) : null; } }
        public static string NAME_CLOTHIER { get { return NPC.AnyNPCs(54) ? NPC.GetFirstNPCNameOrNull(54) : null; } }
        public static string NAME_TINKERER { get { return NPC.AnyNPCs(107) ? NPC.GetFirstNPCNameOrNull(107) : null; } }
        public static string NAME_WIZARD { get { return NPC.AnyNPCs(108) ? NPC.GetFirstNPCNameOrNull(108) : null; } }
        public static string NAME_MECHANIC { get { return NPC.AnyNPCs(124) ? NPC.GetFirstNPCNameOrNull(124) : null; } }
        public static string NAME_TRUFFLE { get { return NPC.AnyNPCs(160) ? NPC.GetFirstNPCNameOrNull(160) : null; } }
        public static string NAME_STEAMPUNKER { get { return NPC.AnyNPCs(178) ? NPC.GetFirstNPCNameOrNull(178) : null; } }
        public static string NAME_DYETRADER { get { return NPC.AnyNPCs(207) ? NPC.GetFirstNPCNameOrNull(207) : null; } }
        public static string NAME_PARTYGIRL { get { return NPC.AnyNPCs(208) ? NPC.GetFirstNPCNameOrNull(208) : null; } }
        public static string NAME_CYBORG { get { return NPC.AnyNPCs(209) ? NPC.GetFirstNPCNameOrNull(209) : null; } }
        public static string NAME_PAINTER { get { return NPC.AnyNPCs(227) ? NPC.GetFirstNPCNameOrNull(227) : null; } }
        public static string NAME_WITCHDOCTOR { get { return NPC.AnyNPCs(228) ? NPC.GetFirstNPCNameOrNull(228) : null; } }
        public static string NAME_PIRATE { get { return NPC.AnyNPCs(229) ? NPC.GetFirstNPCNameOrNull(229) : null; } }
		public static string NAME_STYLIST { get { return NPC.AnyNPCs(353) ? NPC.GetFirstNPCNameOrNull(353) : null; } }
		public static string NAME_ANGLER { get { return NPC.AnyNPCs(369) ? NPC.GetFirstNPCNameOrNull(369) : null; } }

        //----Drawing Constants----//


        //The 'bounding box' of a single player frame.
        public static readonly Rectangle FRAME_PLAYER = new Rectangle(0, 0, 40, 54);


        //----NetMessage.SendData Constants----//


        //these ids are more commonly used
        public const int NET_NPC_UPDATE = 23;
        public const int NET_NPC_HIT = 28;
        public const int NET_PROJ_UPDATE = 27;
        public const int NET_PLAYER_UPDATE = 13;
        public const int NET_TILE_UPDATE = 17;
        public const int NET_ITEM_UPDATE = 21;

        public const int NET_PLAYER_LIFE = 16;
        public const int NET_PLAYER_MANA = 42;
        public const int NET_PLAYER_ITEMROT_ITEMANIM = 41;
        public const int NET_PROJ_MANUALKILL = 29;


        //----Dust Constants----//


        public const int DUSTID_FIRE = 6;
        public const int DUSTID_WATERCANDLE = 29;
        public const int DUSTID_GLITTER = 43;
        public const int DUSTID_BLOOD = 5;
        public const int DUSTID_BONE = 26;
        public const int DUSTID_METAL = 30;
        public const int DUSTID_METALDUST = 31;
		public const int DUSTID_CURSEDFIRE = 75;
		public const int DUSTID_ICHOR = 170;
		public const int DUSTID_FROST = 185;
		
		public const int DUSTID_SOLAR = 6; //same as fire
		public const int DUSTID_NEBULA = 242;
		public const int DUSTID_STARDUST = 229;
		public const int DUSTID_VORTEX = 229;
		public const int DUSTID_LUNAR = 249; //???


        //----Item Constants----//


        public const int ITEMID_HEART = 58;
        public const int ITEMID_MANASTAR = 184;

        public const int ITEMID_HEALTHPOTION_LESSER = 28;
        public const int ITEMID_HEALTHPOTION = 188;
        public const int ITEMID_HEALTHPOTION_GREATER = 499;

        public const int ITEMID_MANAPOTION_LESSER = 110;
        public const int ITEMID_MANAPOTION = 189;
        public const int ITEMID_MANAPOTION_GREATER = 500;

        //an array of the vanilla gem types, in order: Amethyst, Topaz, Sapphire, Ruby, Emerald, Diamond, Amber.
        public static readonly int[] ITEMIDS_GEMS = new int[]{ 181, 180, 177, 178, 179, 182, 999 };

        public static readonly int AMMOTYPE_ARROW = AmmoID.Arrow;
        public static readonly int AMMOTYPE_BULLET = AmmoID.Bullet;

        //----Tile Constants----//


        //these 4 arrays are made with conversion to corruption/hallow/normal biomes in mind. (stone, grass, sand, ice)
        public static readonly int[] TILEIDS_CONVERTCORRUPTION = new int[] { 25, 23, 112, 163, 398, 400 }; //Corruption versions of tiles that can be converted.
        public static readonly int[] TILEIDS_CONVERTHALLOW = new int[] { 117, 109, 116, 164, 402, 403 }; //Hallow versions of tiles that can be converted.
		public static readonly int[] TILEIDS_CONVERTCRIMSON = new int[] { 203, 199, 234, 200, 399, 401 }; //Crimson versions of tiles that can be converted.
		public static readonly int[] TILEIDS_CONVERTOVERWORLD = new int[] { 1, 2, 53, 161, 397, 396 }; //Normal versions of tiles that can be converted.
        public static readonly int[] TILEIDS_CONVERTALL = BaseUtility.CombineArrays(BaseUtility.CombineArrays(TILEIDS_CONVERTOVERWORLD, TILEIDS_CONVERTHALLOW), BaseUtility.CombineArrays(TILEIDS_CONVERTCORRUPTION, TILEIDS_CONVERTCRIMSON)); //combined array of all 4 of the above.

        //tiles found in the dungeon and dungeon-only tiles. (pre-1.2)
        public static readonly int[] TILEIDS_DUNGEON = new int[]{ 10, 11, 12, 13, 19, 21, 28, 41, 42, 43, 44, 48, 49, 50 }; //all tiles found in the Dungeon
        public static readonly int[] TILEIDS_DUNGEONSTRICT = new int[]{ 41, 42, 43, 44, 48, 49, 50 }; //Dungeon-only tiles

        //individual tile ids (type).
        public const int TILEID_DOORCLOSED = 10;
        public const int TILEID_CHESTS = 21;
        public const int TILEID_SKYISLANDBRICK = 202;

        //the 'style' of a chest.
        public const int CHESTSTYLE_WOOD = 0;
        public const int CHESTSTYLE_GOLD = 1;
        public const int CHESTSTYLE_GOLDLOCKED = 2;
        public const int CHESTSTYLE_SHADOW = 3;
        public const int CHESTSTYLE_SHADOWLOCKED = 4;
        public const int CHESTSTYLE_BARREL = 5;
        public const int CHESTSTYLE_TRASHCAN = 6;
        public const int CHESTSTYLE_EBONWOOD = 7;
        public const int CHESTSTYLE_MOHAGONY = 8;
        public const int CHESTSTYLE_HALLOWWOOD = 9;
        public const int CHESTSTYLE_JUNGLE = 10;
        public const int CHESTSTYLE_ICE = 11;
        public const int CHESTSTYLE_VINED = 12;
        public const int CHESTSTYLE_SKY = 13;
        public const int CHESTSTYLE_SHADEWOOD = 14;
        public const int CHESTSTYLE_WEBBED = 15;
        public const int CHESTSTYLE_LIHZAHRD = 16;
        public const int CHESTSTYLE_SEA = 17;
        public const int CHESTSTYLE_DUNGJUNGLE = 18;
        public const int CHESTSTYLE_DUNGCORRUPT = 19;
        public const int CHESTSTYLE_DUNGCRIMSON = 20;
        public const int CHESTSTYLE_DUNGHALLOWED = 21;
        public const int CHESTSTYLE_DUNGICE = 22;
        public const int CHESTSTYLE_DUNGJUNGLELOCKED = 23;
        public const int CHESTSTYLE_DUNGCORRUPTLOCKED = 24;
        public const int CHESTSTYLE_DUNGCRIMSONLOCKED = 25;
        public const int CHESTSTYLE_DUNGHALLOWEDLOCKED = 26;
        public const int CHESTSTYLE_DUNGICELOCKED = 27;

        //----Misc Constants----//

		//various chat colors used throughout the game.
		public static readonly Color CHATCOLOR_PURPLE = new Color(175, 75, 255);
		public static readonly Color CHATCOLOR_GREEN = new Color(50, 255, 130);
		public static readonly Color CHATCOLOR_RED = new Color(255, 25, 25);
		public static readonly Color CHATCOLOR_YELLOW = new Color(255, 240, 20);

		public static readonly Color NPCTEXTCOLOR_BUFF = new Color(255, 140, 40);

		public const int ARMORID_HEAD = 0;
		public const int ARMORID_BODY = 1;
		public const int ARMORID_LEGS = 2;
		public const int ARMORID_HEADVANITY = 10;
		public const int ARMORID_BODYVANITY = 11;
		public const int ARMORID_LEGSVANITY = 12;

        public const int TIME_DAWNDUSK = 0; //if Main.dayTime is true, this is dawn. Else, this is dusk.
        public const int TIME_MIDDAY = 27000;
        public const int TIME_MIDNIGHT = 16200;
		public const int TIME_FULLDAY = 54000; 
		public const int TIME_FULLNIGHT = 32400;
		
		//various invasionTypes
		public const int INVASION_GOBLIN = 1;	
		public const int INVASION_FROSTLEGION = 2;	
		public const int INVASION_PIRATE = 3;		
		public const int INVASION_MARTIAN = 4;

        //----------------------//
    }

    public class DuoObj {
        public object obj1, obj2;

        public DuoObj(object o1, object o2)  { 
            obj1 = o1; obj2 = o2; 
        }
    }
}

