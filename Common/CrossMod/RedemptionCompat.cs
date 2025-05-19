
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Common.CrossMod;

public static class MoRHelper
{
	private static readonly Mod redemption = CrossModLoader.Redemption.Instance;

	public const short Arcane = 1;
	public const short Fire = 2;
	public const short Water = 3;
	public const short Ice = 4;
	public const short Earth = 5;
	public const short Wind = 6;
	public const short Thunder = 7;
	public const short Holy = 8;
	public const short Shadow = 9;
	public const short Nature = 10;
	public const short Poison = 11;
	public const short Blood = 12;
	public const short Psychic = 13;
	public const short Celestial = 14;
	public const short Explosive = 15;

	public const string NPCType_Skeleton = "Skeleton";
	public const string NPCType_SkeletonHumanoid = "SkeletonHumanoid";
	public const string NPCType_Humanoid = "Humanoid";
	public const string NPCType_Undead = "Undead";
	public const string NPCType_Spirit = "Spirit";
	public const string NPCType_Plantlike = "Plantlike";
	public const string NPCType_Demon = "Demon";
	public const string NPCType_Cold = "Cold";
	public const string NPCType_Hot = "Hot";
	public const string NPCType_Wet = "Wet";
	public const string NPCType_Dragonlike = "Dragonlike";
	public const string NPCType_Inorganic = "Inorganic";
	public const string NPCType_Robotic = "Robotic";
	public const string NPCType_Armed = "Armed";
	public const string NPCType_Hallowed = "Hallowed";
	public const string NPCType_Dark = "Dark";
	public const string NPCType_Blood = "Blood";
	public const string NPCType_Slime = "Slime";

	// ------------------------------------------------------------------------------------------------------
	// These go in SetStaticDefaults()
	public static void AddItemToBluntSwing(int itemType)
	{
		if (!CrossModLoader.Redemption.Enabled)
			return;
		redemption.Call("addItemToBluntSwing", itemType);
	}

	public static void AddElement(Entity entity, int elementID, bool projsInheritElements = false)
	{
		if (!CrossModLoader.Redemption.Enabled)
			return;
		if (entity is Item item)
			redemption.Call("addElementItem", elementID, item.type, projsInheritElements);
		else if (entity is NPC npc)
			redemption.Call("addElementNPC", elementID, npc.type);
		else if (entity is Projectile proj)
			redemption.Call("addElementProj", elementID, proj.type, projsInheritElements);
	}

	public static void AddElementToItem(int type, int elementID, bool projsInheritElements = false)
	{
		if (!CrossModLoader.Redemption.Enabled)
			return;
		redemption.Call("addElementItem", elementID, type, projsInheritElements);
	}

	public static void AddElementToNPC(int type, int elementID)
	{
		if (!CrossModLoader.Redemption.Enabled)
			return;
		redemption.Call("addElementNPC", elementID, type);
	}

	public static void AddElementToProjectile(int type, int elementID, bool projsInheritElements = false)
	{
		if (!CrossModLoader.Redemption.Enabled)
			return;
		redemption.Call("addElementProj", elementID, type, projsInheritElements);
	}

	public static void AddNPCToElementList(int npcType, string typeString)
	{
		if (!CrossModLoader.Redemption.Enabled)
			return;
		redemption.Call("addNPCToElementTypeList", typeString, npcType);
	}

	// ------------------------------------------------------------------------------------------------------
	// These are dynamic, so they can go in SetDefaults or wherever you want to update them
	// Keep in mind they don't get reset, so not required to put in an Update method that happens every frame
	public static void OverrideElement(Entity entity, int elementID, int overrideID = 1)
	{
		if (!CrossModLoader.Redemption.Enabled)
			return;
		if (entity is Item item)
			redemption.Call("elementOverrideItem", item, elementID, overrideID);
		else if (entity is NPC npc)
			redemption.Call("elementOverrideNPC", npc, elementID, overrideID);
		else if (entity is Projectile proj)
			redemption.Call("elementOverrideProj", proj, elementID, overrideID);
	}

	public static void OverrideElementMultiplier(NPC npc, int elementID, float value = 1, bool dontSetMultipliers = false)
	{
		if (!CrossModLoader.Redemption.Enabled)
			return;
		redemption.Call("elementMultiplier", npc, elementID, value, dontSetMultipliers);
	}

	public static void NoBossMultiplierCap(NPC npc, bool uncap = true)
	{
		if (!CrossModLoader.Redemption.Enabled)
			return;
		redemption.Call("uncapBossElementMultiplier", npc, uncap);
	}

	public static void HideElementIcon(Item item, int elementID, bool hidden = true)
	{
		if (!CrossModLoader.Redemption.Enabled)
			return;
		redemption.Call("hideElementIcon", item, elementID, hidden);
	}

	public static bool Decapitation(NPC target, ref int damageDone, ref bool crit, int chance = 200)
	{
		if (!CrossModLoader.Redemption.Enabled)
			return false;
		return (bool)redemption.Call("decapitation", target, damageDone, crit, chance);
	}

	public static bool SetSlashBonus(Item item, bool setBonus = true)
	{
		if (!CrossModLoader.Redemption.Enabled)
			return false;
		return (bool)redemption.Call("setSlashBonus", item, setBonus);
	}

	public static bool SetAxeBonus(Item item, bool setBonus = true)
	{
		if (!CrossModLoader.Redemption.Enabled)
			return false;
		return (bool)redemption.Call("setAxeBonus", item, setBonus);
	}

	public static bool SetAxeBonus(Projectile proj, bool setBonus = true)
	{
		if (!CrossModLoader.Redemption.Enabled)
			return false;
		return (bool)redemption.Call("setAxeProj", proj, setBonus);
	}

	public static bool SetHammerBonus(Item item, bool setBonus = true)
	{
		if (!CrossModLoader.Redemption.Enabled)
			return false;
		return (bool)redemption.Call("setHammerBonus", item, setBonus);
	}

	public static bool SetHammerBonus(Projectile proj, bool setBonus = true)
	{
		if (!CrossModLoader.Redemption.Enabled)
			return false;
		return (bool)redemption.Call("setHammerProj", proj, setBonus);
	}

	// Items already have "ItemID.Sets.Spears[Item.type]" to set them as spears
	public static bool SetSpearBonus(Projectile proj, bool setBonus = true)
	{
		if (!CrossModLoader.Redemption.Enabled)
			return false;
		return (bool)redemption.Call("setSpearProj", proj, setBonus);
	}

	// ------------------------------------------------------------------------------------------------------
	public static bool HasElement(Entity entity, int elementID)
	{
		if (!CrossModLoader.Redemption.Enabled)
			return false;
		if (entity is Item item)
			return (bool)redemption.Call("hasElementItem", item, elementID);
		else if (entity is NPC npc)
			return (bool)redemption.Call("elementOverrideNPC", npc, elementID);
		else if (entity is Projectile proj)
			return (bool)redemption.Call("elementOverrideProj", proj, elementID);
		return false;
	}
	public static int GetFirstElement(Entity entity, bool ignoreExplosive = false)
	{
		if (!CrossModLoader.Redemption.Enabled)
			return 0;
		if (entity is Item item)
			return (int)redemption.Call("getFirstElementItem", item, ignoreExplosive);
		else if (entity is NPC npc)
			return (int)redemption.Call("getFirstElementNPC", npc, ignoreExplosive);
		else if (entity is Projectile proj)
			return (int)redemption.Call("getFirstElementProj", proj, ignoreExplosive);
		return 0;
	}
}