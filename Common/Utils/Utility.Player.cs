using Macrocosm.Content.CameraModifiers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Utils
{
    public static partial class Utility
    {
        public static (int x, int y) SpawnTile => (Main.spawnTileX, Main.spawnTileY);
        public static Point SpawnTilePoint => new(Main.spawnTileX, Main.spawnTileY);
        public static Point16 SpawnTilePoint16 => new(Main.spawnTileX, Main.spawnTileY);
        public static Vector2 SpawnWorldPosition => new(Main.spawnTileX * 16f, Main.spawnTileY * 16f);

        public static bool InPlayerInteractionRange(this Rectangle rectangle, TileReachCheckSettings settings)
        {
            Point location = rectangle.ClosestPointInRect(Main.LocalPlayer.Center).ToTileCoordinates();
            return Main.LocalPlayer.IsInTileInteractionRange(location.X, location.Y, settings);
        }

        // TODO: some sort of netsync where the server or other clients can shake a player's screen
        public static void AddScreenshake(this Player player, float intensity, string context)
        {
            Main.instance.CameraModifiers.Add(new ScreenshakeCameraModifier(intensity, context));
        }

        public static bool AltFunction(this Player player) => player.altFunctionUse == 2;

        public static Rectangle GetSwungItemHitbox(this Player player)
        {
            //Found in Player.cs
            Item item = player.HeldItem;
            bool flag21 = false;

            Rectangle hitbox = new Rectangle((int)player.itemLocation.X, (int)player.itemLocation.Y, 32, 32);
            if (!Main.dedServ)
                hitbox = new Rectangle((int)player.itemLocation.X, (int)player.itemLocation.Y, TextureAssets.Item[item.type].Width(), TextureAssets.Item[item.type].Height());

            hitbox.Width = (int)(hitbox.Width * item.scale);
            hitbox.Height = (int)(hitbox.Height * item.scale);
            if (player.direction == -1)
                hitbox.X -= hitbox.Width;

            if (player.gravDir == 1f)
                hitbox.Y -= hitbox.Height;

            if (item.useStyle == ItemUseStyleID.Swing)
            {
                if (player.itemAnimation < player.itemAnimationMax * 0.333)
                {
                    if (player.direction == -1)
                        hitbox.X -= (int)(hitbox.Width * 1.4 - hitbox.Width);

                    hitbox.Width = (int)(hitbox.Width * 1.4);
                    hitbox.Y += (int)(hitbox.Height * 0.5 * player.gravDir);
                    hitbox.Height = (int)(hitbox.Height * 1.1);
                }
                else if (player.itemAnimation >= player.itemAnimationMax * 0.666)
                {
                    if (player.direction == 1)
                        hitbox.X -= (int)(hitbox.Width * 1.2);

                    hitbox.Width *= 2;
                    hitbox.Y -= (int)((hitbox.Height * 1.4 - hitbox.Height) * player.gravDir);
                    hitbox.Height = (int)(hitbox.Height * 1.4);
                }
            }
            else if (item.useStyle == ItemUseStyleID.Thrust)
            {
                if (player.itemAnimation > player.itemAnimationMax * 0.666)
                {
                    flag21 = true;
                }
                else
                {
                    if (player.direction == -1)
                        hitbox.X -= (int)(hitbox.Width * 1.4 - hitbox.Width);

                    hitbox.Width = (int)(hitbox.Width * 1.4);
                    hitbox.Y += (int)(hitbox.Height * 0.6);
                    hitbox.Height = (int)(hitbox.Height * 0.6);
                }
            }

            ItemLoader.UseItemHitbox(item, player, ref hitbox, ref flag21);

            return hitbox;
        }

        public static Player PrepareDummy(this Player player, Vector2 targetPosition)
        {
            Player clonePlayer = new();
            clonePlayer.CopyVisuals(player);
            clonePlayer.ResetEffects();
            clonePlayer.ResetVisibleAccessories();
            clonePlayer.UpdateDyes();
            clonePlayer.DisplayDollUpdate();
            clonePlayer.UpdateSocialShadow();
            clonePlayer.velocity.Y = 0f;
            clonePlayer.wingFrame = 3;
            clonePlayer.PlayerFrame();
            clonePlayer.socialIgnoreLight = true;
            clonePlayer.position.X = targetPosition.X + Main.screenPosition.X;
            clonePlayer.position.Y = targetPosition.Y + Main.screenPosition.Y;

            return clonePlayer;
        }

        public static void DrawPetDummy(this Player player, Vector2 targetPosition, bool animated)
        {
            if (!player.hideMisc[0])
            {
                Item item = player.miscEquips[0];
                if (!item.IsAir && item.buffType > 0 && Main.vanityPet[item.buffType] && !Main.lightPet[item.buffType])
                {
                    Projectile projectile = new();
                    projectile.SetDefaults(item.shoot);
                    projectile.isAPreviewDummy = true;
                    Vector2 vector = targetPosition + new Vector2(0f, player.height) + new Vector2(20f, 0f) + new Vector2(0f, -projectile.height);
                    projectile.position = vector + Main.screenPosition;
                    projectile.velocity = new Vector2(0.1f, 0f);
                    projectile.direction = 1;
                    projectile.owner = player.whoAmI;
                    ProjectileID.Sets.CharacterPreviewAnimations[projectile.type].ApplyTo(projectile, animated);
                    Main.instance.DrawProjDirect(projectile);
                }
            }
        }


        #region BaseMod BasePlayer methods

        //------------------------------------------------------//
        //--------------------- BASE PLAYER  -------------------//
        //------------------------------------------------------//
        // Contains methods relating to players.                //
        //------------------------------------------------------//
        //  Author(s): Grox the Great                           //
        //------------------------------------------------------//

        //NOTE: DO NOT CALL IN ANY ConsumeAmmo HOOK!!! Will cause an infinite loop!
        public static bool ConsumeAmmo(Player player, Item item, Item ammo)
        {
            bool consume = true;
            if (player.magicQuiver && ammo.ammo == AmmoID.Arrow && Main.rand.NextBool(5)) consume = false;
            if (player.ammoBox && Main.rand.NextBool(5)) consume = false;
            if (player.ammoPotion && Main.rand.NextBool(5)) consume = false;
            if (player.ammoCost80 && Main.rand.NextBool(5)) consume = false;
            if (player.ammoCost75 && Main.rand.NextBool(4)) consume = false;
            if (!PlayerLoader.CanConsumeAmmo(player, item, ammo)) consume = false;
            if (!ItemLoader.CanConsumeAmmo(item, ammo, player)) consume = false;
            return consume;
        }

        public static void ReduceSlot(Player player, int slot, int amount)
        {
            player.inventory[slot].stack -= amount;
            if (player.inventory[slot].stack <= 0)
            {
                player.inventory[slot] = new Item();
            }
        }
        public static bool HasEmptySlots(Player player, int slotCount, bool includeInventory = true, bool includeCoins = false, bool includeAmmo = false)
        {
            int count = 0;
            for (int m = includeInventory ? 0 : includeCoins ? 50 : 54; m < (includeAmmo ? 58 : includeCoins ? 54 : 50); m++)
            {
                Item item = player.inventory[m]; if (item == null || item.IsAir) { count++; if (count >= slotCount) { return true; } }
            }
            return false;
        }

        public static int GetEmptySlot(Player player, bool includeInventory = true, bool includeCoins = false, bool includeAmmo = false)
        {
            for (int m = includeInventory ? 0 : includeCoins ? 50 : 54; m < (includeAmmo ? 58 : includeCoins ? 54 : 50); m++)
            {
                Item item = player.inventory[m]; if (item == null || item.IsAir) { return m; }
            }
            return -1;
        }

        public static bool DowngradeMoney(Player player, int moneySlot, ref int splitSlot)
        {
            Item item = player.inventory[moneySlot];
            if (item is not { type: > ItemID.CopperCoin and < ItemID.FallenStar }) { return false; } //can't downgrade copper coins or non-coin items
            int typeToBecome = item.type - 1;
            splitSlot = GetEmptySlot(player, false, true);
            if (splitSlot == -1) { splitSlot = GetEmptySlot(player); }
            if (splitSlot == -1) { return false; } //no space
            player.inventory[splitSlot].SetDefaults(typeToBecome); player.inventory[splitSlot].stack = 100;
            player.inventory[moneySlot].stack--; if (player.inventory[moneySlot].stack <= 0) { player.inventory[moneySlot] = new Item(); }
            return true;
        }

        /*
         * Returns true if it successfully reduces mana. If there is not enough mana to reduce it by the amount given it returns false.
         * autoRefill : If true, checks if the player has a mana flower and automatically boosts mana if it's needed.
         */
        public static bool ReduceMana(Player player, int amount, bool autoRefill = true)
        {
            if (autoRefill && player.manaFlower && player.statMana < (int)(amount * player.manaCost))
            {
                player.QuickMana();
            }
            if (player.statMana >= (int)(amount * player.manaCost))
            {
                //player.manaRegenDelay = (int)player.maxRegenDelay;
                player.statMana -= (int)(amount * player.manaCost);
                if (player.statMana < 0) { player.statMana = 0; }
                return true;
            }
            return false;
        }

        public static bool HasHelmet(Player player, int itemType, bool vanity = true) { return HasArmor(player, itemType, 0, vanity); }
        public static bool HasChestplate(Player player, int itemType, bool vanity = true) { return HasArmor(player, itemType, 1, vanity); }
        public static bool HasLeggings(Player player, int itemType, bool vanity = true) { return HasArmor(player, itemType, 2, vanity); }

        /*
         * Returns true if the player is wearing the given armor
         * armorType : 0 == helmet, 1 == chestplate, 2 == leggings.
         * vanity : If true, include vanity slots.
         */
        public static bool HasArmor(Player player, int itemType, int armorType, bool vanity = true)
        {
            if (vanity)
            {
                if (armorType == 0)
                    return player.armor[10] != null && player.armor[10].type == itemType || player.armor[0] != null && player.armor[0].type == itemType;
                if (armorType == 1)
                    return player.armor[11] != null && player.armor[11].type == itemType || player.armor[1] != null && player.armor[1].type == itemType;
                if (armorType == 2)
                    return player.armor[12] != null && player.armor[12].type == itemType || player.armor[2] != null && player.armor[2].type == itemType;
            }
            else
            {
                if (armorType == 0)
                    return player.armor[0] != null && player.armor[0].type == itemType;
                if (armorType == 1)
                    return player.armor[1] != null && player.armor[1].type == itemType;
                if (armorType == 2)
                    return player.armor[2] != null && player.armor[2].type == itemType;
            }
            return false;
        }


        /**
         * Returns the total monetary value (in copper coins) the player has.
         * includeInventory : True to include inventory slots, false to only include coin slots.
         */
        public static int GetMoneySum(Player player, bool includeInventory = false)
        {
            int totalSum = 0;
            for (int m = includeInventory ? 0 : 50; m < 54; m++)
            {
                Item item = player.inventory[m];
                if (item != null)
                {
                    if (item.type == ItemID.CopperCoin) { totalSum += item.stack; }
                    else
                        if (item.type == ItemID.SilverCoin) { totalSum += item.stack * 100; }
                    else
                            if (item.type == ItemID.GoldCoin) { totalSum += item.stack * 10000; }
                    else
                                if (item.type == ItemID.PlatinumCoin) { totalSum += item.stack * 1000000; }
                }
            }
            return totalSum;
        }


        public static int GetItemstackSum(Player player, int type, bool typeIsAmmo = false, bool includeAmmo = false, bool includeCoins = false)
        {
            return GetItemstackSum(player, new[] { type }, typeIsAmmo, includeAmmo, includeCoins);
        }

        /*
         * Returns the total itemstack sum of a specific item in the player's inventory.
         * 
         * typeIsAmmo : If true, types are considered ammo types. Else, types are considered item types.
         */
        public static int GetItemstackSum(Player player, int[] types, bool typeIsAmmo = false, bool includeAmmo = false, bool includeCoins = false)
        {
            int stackCount = 0;
            if (includeCoins)
            {
                for (int m = 50; m < 54; m++)
                {
                    Item item = player.inventory[m];
                    if (item != null && (typeIsAmmo ? Utility.InArray(types, item.ammo) : Utility.InArray(types, item.type))) { stackCount += item.stack; }
                }
            }
            if (includeAmmo)
            {
                for (int m = 54; m < 58; m++)
                {
                    Item item = player.inventory[m];
                    if (item != null && (typeIsAmmo ? Utility.InArray(types, item.ammo) : Utility.InArray(types, item.type))) { stackCount += item.stack; }
                }
            }
            for (int m = 0; m < 50; m++)
            {
                Item item = player.inventory[m];
                if (item != null && (typeIsAmmo ? Utility.InArray(types, item.ammo) : Utility.InArray(types, item.type))) { stackCount += item.stack; }
            }
            return stackCount;
        }

        public static bool HasItem(Player player, int[] types, int[] counts = default, bool includeAmmo = false, bool includeCoins = false)
        {
            int dummyIndex = 0;
            bool hasItem = HasItem(player, types, ref dummyIndex, counts, includeAmmo, includeCoins);
            return hasItem;
        }

        /**
         * Returns true if the given player has any of the given item types in thier inventory.
         * index : Is set to the index of the item found. If it isn't found, it is set to -1.
         * counts : the minimum stack per item needed for HasItem to return true.
         * includeAmmo : true if you wish to include the ammo slots.
         * includeCoins : true if you wish to include the coin slots.
         */
        public static bool HasItem(Player player, int[] types, ref int index, int[] counts = default, bool includeAmmo = false, bool includeCoins = false)
        {
            if (types == null || types.Length == 0) return false; //no types to check!			
            if (counts == null || counts.Length == 0) { counts = Utility.FillArray(new int[types.Length], 1); }
            int countIndex = -1;
            if (includeCoins)
            {
                for (int m = 50; m < 54; m++)
                {
                    Item item = player.inventory[m];
                    if (item != null && Utility.InArray(types, item.type, ref countIndex) && item.stack >= counts[countIndex]) { index = m; return true; }
                }
            }
            if (includeAmmo)
            {
                for (int m = 54; m < 58; m++)
                {
                    Item item = player.inventory[m];
                    if (item != null && Utility.InArray(types, item.type, ref countIndex) && item.stack >= counts[countIndex]) { index = m; return true; }
                }
            }
            for (int m = 0; m < 50; m++)
            {
                Item item = player.inventory[m];
                if (item != null && Utility.InArray(types, item.type, ref countIndex) && item.stack >= counts[countIndex]) { index = m; return true; }
            }
            return false;
        }

        public static bool HasAllItems(Player player, int[] types, ref int[] indicies, int[] counts = default, bool includeAmmo = false, bool includeCoins = false)
        {
            if (types == null || types.Length == 0) return false; //no types to check!
            if (counts == null || counts.Length == 0) { counts = Utility.FillArray(new int[types.Length], 1); }
            int[] indexArray = new int[types.Length];
            bool[] foundItem = new bool[types.Length];
            if (includeCoins)
            {
                for (int m = 50; m < 54; m++)
                {
                    for (int m2 = 0; m2 < types.Length; m2++)
                    {
                        if (foundItem[m2]) continue;
                        Item item = player.inventory[m];
                        if (item != null && item.type == types[m2] && item.stack >= counts[m2]) { foundItem[m2] = true; indexArray[m2] = m; }
                    }
                }
            }
            if (includeAmmo)
            {
                for (int m = 54; m < 58; m++)
                {
                    for (int m2 = 0; m2 < types.Length; m2++)
                    {
                        if (foundItem[m2]) continue;
                        Item item = player.inventory[m];
                        if (item != null && item.type == types[m2] && item.stack >= counts[m2]) { foundItem[m2] = true; indexArray[m2] = m; }
                    }
                }
            }
            for (int m = 0; m < 50; m++)
            {
                for (int m2 = 0; m2 < types.Length; m2++)
                {
                    if (foundItem[m2]) continue;
                    Item item = player.inventory[m];
                    if (item != null && item.type == types[m2] && item.stack >= counts[m2]) { foundItem[m2] = true; indexArray[m2] = m; }
                }
            }
            foreach (bool f in foundItem) if (!f) return false;
            return true;
        }

        public static bool HasItem(Player player, int type, int count = 1, bool includeAmmo = false, bool includeCoins = false)
        {
            int dummyIndex = 0;
            bool hasItem = HasItem(player, type, ref dummyIndex, count, includeAmmo, includeCoins);
            return hasItem;
        }

        /**
         * Returns true if the given player has the given item type in thier inventory.
         * 
         * index : Is set to the index of the item found. If it isn't found, it is set to -1.
         * count : the minimum stack needed for HasItem to return true.
         * includeAmmo : true if you wish to include the ammo slots.
         * includeCoins : true if you wish to include the coin slots.
         */
        public static bool HasItem(Player player, int type, ref int index, int count = 1, bool includeAmmo = false, bool includeCoins = false)
        {
            if (includeCoins)
            {
                for (int m = 50; m < 54; m++)
                {
                    Item item = player.inventory[m];
                    if (item != null && item.type == type && item.stack >= count) { index = m; return true; }
                }
            }
            if (includeAmmo)
            {
                for (int m = 54; m < 58; m++)
                {
                    Item item = player.inventory[m];
                    if (item != null && item.type == type && item.stack >= count) { index = m; return true; }
                }
            }
            for (int m = 0; m < 50; m++)
            {
                Item item = player.inventory[m];
                if (item != null && item.type == type && item.stack >= count) { index = m; return true; }
            }
            index = -1;
            return false;
        }


        /**
         * Returns true if the given player has the given item ammo type in thier inventory.
         * 
         * index : Is set to the index of the item found. If it isn't found, it is set to -1.
         * count : the minimum stack needed for HasItem to return true.
         * includeAmmo : true if you wish to include the ammo slots.
         * includeCoins : true if you wish to include the coin slots.
		 * ingoreConsumable : true if you wish to ignore consumable checks (this is used for infinite ammo items like musket pouch)
         */
        public static bool HasAmmo(Player player, int ammoType, ref int index, int count = 1, bool includeAmmo = false, bool includeCoins = false, bool ignoreConsumable = false)
        {
            if (includeCoins)
            {
                for (int m = 50; m < 54; m++)
                {
                    Item item = player.inventory[m];
                    if (item != null && item.ammo == ammoType && (!ignoreConsumable && !item.consumable || item.stack >= count)) { index = m; return true; }
                }
            }
            if (includeAmmo)
            {
                for (int m = 54; m < 58; m++)
                {
                    Item item = player.inventory[m];
                    if (item != null && item.ammo == ammoType && (!ignoreConsumable && !item.consumable || item.stack >= count)) { index = m; return true; }
                }
            }
            for (int m = 0; m < 50; m++)
            {
                Item item = player.inventory[m];
                if (item != null && item.ammo == ammoType && (!ignoreConsumable && !item.consumable || item.stack >= count)) { index = m; return true; }
            }
            index = -1;
            return false;
        }

        /**
         * Returns true if the given items all contain a phrase within thier names.
         * setName : The phrase in the set's name (ie "Copper", "Iron", "Hallowed" etc.)
		 * items : Array of items to check.
         */
        public static bool IsInSet(string setName, params Item[] items)
        {
            return IsInSet(null, setName, items);
        }

        /**
         * Returns true if the given items all contain a phrase within thier names.
		 * mod : The mod the items in question are from. Use as a filter to ensure the items you wish to check are applied.
         * setName : The phrase in the set's name (ie "Copper", "Iron", "Hallowed" etc.)
		 * items : Array of items to check.
         */
        public static bool IsInSet(Mod mod, string setName, params Item[] items)
        {
            foreach (Item item in items)
            {
                if (item == null || item.IsAir) return false; //items in the list cannot be null!
                if (!item.Name.StartsWith(setName)) return false;
                if (mod != null && item.ModItem != null && !item.ModItem.Mod.Name.ToLower().Equals(mod.Name.ToLower())) return false;
            }
            return true;
        }

        /**
         * Returns true if the given player has the given armor set equipped.	 
         * armorName : First word in the armor's name (ie "Copper", "Iron", "Hallowed" etc.)
         * vanity : true if your checking vanity slots, else normal armor slots.
         */
        public static bool HasArmorSet(Player player, string armorName, bool vanity = false)
        {
            return HasArmorSet(null, player, armorName, vanity);
        }

        /**
         * Returns true if the given player has the given armor set equipped.
		 * mod : The mod the items in question are from. Use as a filter to ensure the items you wish to check are applied.		 
         * armorName : First word in the armor's name (ie "Copper", "Iron", "Hallowed" etc.)
         * vanity : true if your checking vanity slots, else normal armor slots.
         */
        public static bool HasArmorSet(Mod mod, Player player, string armorName, bool vanity = false)
        {
            Item itemHelm = player.armor[vanity ? 10 : 0], itemBody = player.armor[vanity ? 11 : 1], itemLegs = player.armor[vanity ? 12 : 2];
            return IsInSet(mod, armorName, itemHelm, itemBody, itemLegs);
        }


        public static bool IsVanitySlot(int slot, bool acc = true) { return acc ? slot is >= 13 and <= 18 : slot is >= 10 and <= 12; }


        public static bool HasAccessories(Player player, int[] types, bool normal, bool vanity, bool oneOf)
        {
            int dummy = 0; bool dummeh = false;
            return HasAccessories(player, types, normal, vanity, oneOf, ref dummeh, ref dummy);
        }

        /**
         * Returns true if the given player has the given accessories equipped.
         * oneOf : If true, checks if you have any of the types instead of all of them.
         */
        public static bool HasAccessories(Player player, int[] types, bool normal, bool vanity, bool oneOf, ref bool social, ref int index)
        {
            int trueCount = 0;

            if (vanity)
            {
                for (int m = 13; m < 18 + player.GetAmountOfExtraAccessorySlotsToShow(); m++)
                {
                    Item item = player.armor[m];
                    if (item is { IsAir: false })
                    {
                        foreach (int type in types)
                        {
                            if (item.type == type)
                            {
                                index = m; social = true; if (oneOf) { return true; }

                                trueCount++;
                            }
                        }
                    }
                }
            }
            if (normal)
            {
                for (int m = 3; m < 8 + player.GetAmountOfExtraAccessorySlotsToShow(); m++)
                {
                    Item item = player.armor[m];
                    if (item is { IsAir: false })
                    {
                        foreach (int type in types)
                        {
                            if (item.type == type)
                            {
                                index = m; social = false; if (oneOf) { return true; }

                                trueCount++;
                            }
                        }
                    }
                }
            }
            return trueCount >= types.Length;
        }

        public static bool HasAccessory(Player player, int type, bool normal, bool vanity)
        {
            int dummy = 0; bool dummeh = false;
            return HasAccessory(player, type, normal, vanity, ref dummeh, ref dummy);
        }

        /**
         * Returns true if the given player has the given accessory equipped.
         */
        public static bool HasAccessory(Player player, int type, bool normal, bool vanity, ref bool social, ref int index)
        {
            if (vanity)
            {
                for (int m = 13; m < 18 + player.GetAmountOfExtraAccessorySlotsToShow(); m++)
                {
                    Item item = player.armor[m];
                    if (item is { IsAir: false } && item.type == type) { index = m; social = true; return true; }
                }
            }
            if (normal)
            {
                for (int m = 3; m < 8 + player.GetAmountOfExtraAccessorySlotsToShow(); m++)
                {
                    Item item = player.armor[m];
                    if (item is { IsAir: false } && item.type == type) { index = m; social = false; return true; }
                }
            }
            return false;
        }

        #endregion
    }
}
