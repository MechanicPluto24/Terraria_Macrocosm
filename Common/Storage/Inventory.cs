using Macrocosm.Common.UI;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Common.Storage;

public partial class Inventory : IEnumerable<Item>
{
    /// <summary>
    /// <br> The inventories currently displayed by custom UI, reset every UI update. </br>
    /// <br> Set this in the UI update hook when displaying one or more inventory UIs. </br>
    /// <br> When accessing, check <see cref="CustomInventoriesActive"/> first. </br>
    /// </summary>
    public static List<Inventory> ActiveInventories { get; } = new();

    /// <summary> Whether one or more custom inventories are currently being displayed. </summary>
    public static bool CustomInventoriesActive => ActiveInventories.Count > 0;

    private Item[] items;
    private UIInventorySlot[] uiItemSlots;

    private InventorySlotRole[] slotRoles;

    private bool[] reservedSlots;
    private int[] reservedTypes;
    private int[] reservedStacks;
    private Func<Item, bool>[] reservedChecks;
    private LocalizedText[] reservedTooltips;
    private Asset<Texture2D>[] reservedTextures;
    private Color?[] reservedColors;

    public Func<int, Item, bool> CanInsertIntoSlot { get; set; }

    public Item this[int index]
    {
        get => items[index];
        set => items[index] = value;
    }

    public Item[] Items => items;

    public IInventoryOwner Owner { get; set; }

    public const int MaxInventorySize = ushort.MaxValue;
    public int Size
    {
        get => items.Length;
        set
        {
            value = (int)MathHelper.Clamp(value, 0, MaxInventorySize);

            if (items.Length != value)
            {
                OnResize(items.Length, value);

                if (Main.netMode != NetmodeID.SinglePlayer)
                    SyncEverything();
            }
        }
    }

    public bool CanInteract => interactingPlayer == Main.myPlayer;

    private int interactingPlayer = 255;
    public int InteractingPlayer
    {
        get => interactingPlayer;
        set => SetInteractingPlayer(value, sync: true);
    }

    internal void SetInteractingPlayer(int value, bool sync)
    {
        if (value < 0 || value > Main.maxPlayers)
            return;

        int last = interactingPlayer;
        interactingPlayer = value;
        if (sync && last != value && Main.netMode != NetmodeID.SinglePlayer)
            SyncInteraction();
    }

    public bool IsEmpty => items.All(item => item.type == ItemID.None);

    public Inventory(int size, IInventoryOwner owner = null)
    {
        int clampedSize = (int)MathHelper.Clamp(size, 0, MaxInventorySize);

        items = new Item[clampedSize];
        uiItemSlots = new UIInventorySlot[clampedSize];

        slotRoles = new InventorySlotRole[clampedSize];

        reservedSlots = new bool[clampedSize];
        reservedTypes = new int[clampedSize];
        reservedStacks = new int[clampedSize];
        reservedChecks = new Func<Item, bool>[clampedSize];
        reservedTooltips = new LocalizedText[clampedSize];
        reservedTextures = new Asset<Texture2D>[clampedSize];
        reservedColors = new Color?[clampedSize];

        for (int i = 0; i < items.Length; i++)
        {
            items[i] = new Item();
            uiItemSlots[i] = new(this, i);
            slotRoles[i] = InventorySlotRole.General;
            ClearReserved(i);
        }

        Owner = owner;

        if (Main.netMode == NetmodeID.SinglePlayer)
            interactingPlayer = Main.myPlayer;
        else
            interactingPlayer = Main.maxPlayers;
    }

    public void SetGlow(int index, float time, float hue) => uiItemSlots[index].SetGlow(time, hue);

    public void DropAllItems(Vector2 worldPosition, bool sync = true, bool fromClient = false)
    {
        for (int i = 0; i < Size; i++)
        {
            DropItem(i, worldPosition, sync, fromClient);
        }
    }

    /// <summary> Drops an item in the world </summary>
    /// <param name="index"> The item slot index </param>
    /// <param name="worldPosition"> The world position to drop this item </param>
    public void DropItem(int index, Vector2 worldPosition, bool sync = true, bool fromClient = false)
    {
        if (index < 0 || index >= Size)
            return;

        if (fromClient)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                Main.LocalPlayer.QuickSpawnItem(items[index].GetSource_Misc("Macrocosm:Inventory"), items[index], items[index].stack);
                items[index].TurnToAir();

                if (sync)
                    SyncItem(index);
            }
        }
        else
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Item.NewItem(items[index].GetSource_Misc("Macrocosm:Inventory"), worldPosition, items[index]);
                items[index].TurnToAir();

                if (sync)
                    SyncItem(index);
            }
        }
    }

    private void OnResize(int oldSize, int newSize)
    {
        if (oldSize > newSize)
        {
            Vector2 dropLocation = Owner is not null ? Owner.InventoryPosition : Main.LocalPlayer.Center;
            for (int i = oldSize - 1; i >= newSize; i--)
                DropItem(i, dropLocation);
        }

        if (oldSize != newSize)
        {
            Array.Resize(ref items, newSize);
            Array.Resize(ref uiItemSlots, newSize);

            Array.Resize(ref slotRoles, newSize);

            Array.Resize(ref reservedSlots, newSize);
            Array.Resize(ref reservedTypes, newSize);
            Array.Resize(ref reservedStacks, newSize);
            Array.Resize(ref reservedChecks, newSize);
            Array.Resize(ref reservedTooltips, newSize);
            Array.Resize(ref reservedTextures, newSize);
            Array.Resize(ref reservedColors, newSize);
        }

        if (oldSize < newSize)
        {
            Array.Fill(items, new Item(), oldSize, newSize - oldSize);
            for (int i = oldSize; i < newSize; i++)
            {
                uiItemSlots[i] = new(this, i);
                slotRoles[i] = InventorySlotRole.General;
                ClearReserved(i);
            }
        }
    }
    public void SetSlotRole(int index, InventorySlotRole role)
    {
        if (index >= 0 && index < Size)
            slotRoles[index] = role;
    }

    public InventorySlotRole GetSlotRole(int index)
    {
        if (index >= 0 && index < Size)
            return slotRoles[index];
        return InventorySlotRole.General;
    }


    public void SetReserved(int index, Func<Item, bool> checkReserved, LocalizedText tooltip = null, Asset<Texture2D> texture = null, Color? color = null)
    {
        if (index < 0 || index >= Size)
            return;

        reservedSlots[index] = true;
        reservedTypes[index] = ItemID.None;
        reservedStacks[index] = 1;
        reservedChecks[index] = checkReserved;
        reservedTooltips[index] = tooltip;
        reservedTextures[index] = texture;
        reservedColors[index] = color;
    }

    public void SetReserved(int index, int itemType, LocalizedText tooltip = null, Asset<Texture2D> texture = null, Color? color = null, int stack = 1)
    {
        if (index < 0 || index >= Size)
            return;

        reservedSlots[index] = true;
        reservedTypes[index] = itemType;
        reservedStacks[index] = Math.Max(1, stack);
        reservedChecks[index] = (item) => item.type == itemType;
        reservedTooltips[index] = tooltip;
        reservedTextures[index] = texture;
        reservedColors[index] = color;
    }

    public void SetReserved(Func<Item, bool> checkReserved, LocalizedText tooltip = null, Asset<Texture2D> texture = null, Color? color = null)
    {
        for (int i = 0; i < Size; i++)
        {
            reservedSlots[i] = true;
            reservedTypes[i] = ItemID.None;
            reservedStacks[i] = 1;
            reservedChecks[i] = checkReserved;
            reservedTooltips[i] = tooltip;
            reservedTextures[i] = texture;
            reservedColors[i] = color;
        }
    }

    public void SetReserved(int itemType, LocalizedText tooltip = null, Asset<Texture2D> texture = null, Color? color = null, int stack = 1)
    {
        for (int i = 0; i < Size; i++)
        {
            reservedSlots[i] = true;
            reservedTypes[i] = itemType;
            reservedStacks[i] = Math.Max(1, stack);
            reservedChecks[i] = (item) => item.type == itemType;
            reservedTooltips[i] = tooltip;
            reservedTextures[i] = texture;
            reservedColors[i] = color;
        }
    }

    public void ClearReserved(int index)
    {
        if (index < 0 || index >= Size)
            return;

        reservedSlots[index] = false;
        reservedTypes[index] = ItemID.None;
        reservedStacks[index] = 1;
        reservedChecks[index] = null;
        reservedTooltips[index] = null;
        reservedTextures[index] = null;
        reservedColors[index] = null;
    }

    public bool ReservedCheck(int index, Item item)
    {
        if (index < 0 || index >= Size || item.type == ItemID.None || !reservedSlots[index])
            return true;

        return reservedChecks[index]?.Invoke(item) ?? true;
    }

    public bool TryGetReservedItemClone(int index, out Item item)
    {
        if (index >= 0 && index < reservedTooltips.Length && reservedTypes[index] > 0)
        {
            item = new Item(reservedTypes[index], reservedStacks[index]);
            return true;
        }

        item = new(0);
        return false;
    }


    public int CountItems(int type, int startIndex = 0, int? endIndex = null, bool countStack = true) => CountItems((i) => i.type == type, startIndex, endIndex, countStack);
    public int CountItems(Func<Item, bool> predicate, int startIndex = 0, int? endIndex = null, bool countStack = true)
    {
        int count = 0;
        for (int i = startIndex; i <= Math.Min(Size - 1, endIndex ?? Size - 1); i++)
        {
            Item item = Items[i];
            if (predicate(item))
                count += countStack ? item.stack : 1;
        }
        return count;
    }


    public LocalizedText GetReservedTooltip(int index) => index >= 0 && index < reservedTooltips.Length ? reservedTooltips[index] : null;
    public Asset<Texture2D> GetReservedTexture(int index) => index >= 0 && index < reservedTextures.Length ? reservedTextures[index] : null;
    public Color? GetReservedColor(int index) => index >= 0 && index < reservedColors.Length ? reservedColors[index] : null;
    public int GetReservedStack(int index) => index >= 0 && index < reservedStacks.Length ? reservedStacks[index] : 1;

    public bool CanAcceptItem(int slot, Item item, InventoryPlacementSource insertionSource, bool ignoreReserved = false)
    {
        if (slot < 0 || slot >= Size || item is null || item.IsAir)
            return false;

        if (insertionSource == InventoryPlacementSource.Player && !uiItemSlots[slot].CanInteractWithItem)
            return false;

        InventorySlotRole role = GetSlotRole(slot);
        bool roleAllowsInsertion = insertionSource switch
        {
            InventoryPlacementSource.Internal => true,
            InventoryPlacementSource.Player or InventoryPlacementSource.Automation => role is InventorySlotRole.General or InventorySlotRole.Input,
            _ => false
        };

        if (!roleAllowsInsertion)
            return false;

        if (insertionSource != InventoryPlacementSource.Internal && !(CanInsertIntoSlot?.Invoke(slot, item) ?? true))
            return false;

        return ignoreReserved || ReservedCheck(slot, item);
    }

    public bool CanExtractItem(int slot, InventoryExtractionSource extractionSource)
    {
        if (slot < 0 || slot >= Size)
            return false;

        if (extractionSource == InventoryExtractionSource.Player && !uiItemSlots[slot].CanInteractWithItem)
            return false;

        InventorySlotRole role = GetSlotRole(slot);
        return extractionSource switch
        {
            InventoryExtractionSource.Internal or InventoryExtractionSource.Player => true,
            InventoryExtractionSource.Automation => role is InventorySlotRole.General or InventorySlotRole.Output,
            _ => false
        };
    }

    public bool TryPlacingItem(ref Item item, InventoryPlacementSource insertionSource, bool justCheck = false, bool sound = true, bool serverSync = true, int startIndex = 0, int? endIndex = null, bool ignoreReserved = false)
    {
        if (ChestUI.IsBlockedFromTransferIntoChest(item, items))
            return false;

        bool shouldSync = Main.netMode == NetmodeID.MultiplayerClient || (serverSync && Main.netMode == NetmodeID.Server);

        Player player = Main.LocalPlayer;
        bool result = false;
        if (item.maxStack > 1)
        {
            for (int i = startIndex; i <= Math.Min(Size - 1, endIndex ?? Size - 1); i++)
            {
                if (!CanAcceptItem(i, item, insertionSource, ignoreReserved))
                    continue;

                if (items[i].stack >= items[i].maxStack || item.type != items[i].type)
                    continue;

                if (!ItemLoader.CanStack(items[i], item))
                    continue;

                int stackDifference = item.stack;
                if (item.stack + items[i].stack > items[i].maxStack)
                    stackDifference = items[i].maxStack - items[i].stack;

                if (justCheck)
                {
                    result = result || stackDifference > 0;
                    break;
                }

                result = true;

                ItemLoader.StackItems(items[i], item, out _);

                if (sound)
                    SoundEngine.PlaySound(SoundID.Grab);

                if (item.stack <= 0)
                {
                    item.SetDefaults();
                    uiItemSlots[i].ClearGlow();

                    if (Main.netMode == NetmodeID.MultiplayerClient)
                        SyncItem(i);

                    break;
                }

                if (items[i].type == ItemID.None)
                {
                    items[i] = item.Clone();
                    item.SetDefaults();
                    uiItemSlots[i].ClearGlow();
                }

                if (shouldSync)
                    SyncItem(i);
            }
        }

        if (item.stack > 0)
        {
            for (int j = startIndex; j <= Math.Min(Size - 1, endIndex ?? Size - 1); j++)
            {
                if (!CanAcceptItem(j, item, insertionSource, ignoreReserved))
                    continue;

                if (items[j].stack != 0)
                    continue;

                if (justCheck)
                {
                    result = true;
                    break;
                }

                if (sound)
                    SoundEngine.PlaySound(SoundID.Grab);

                items[j] = item.Clone();
                item.SetDefaults();
                uiItemSlots[j].ClearGlow();
                ItemSlot.AnnounceTransfer(new ItemSlot.ItemTransferInfo(items[j], 0, 3));

                if (shouldSync)
                    SyncItem(j);

                result = true;
            }
        }

        return result;
    }

    public bool TryPlacingItemInSlot(ref Item item, int slot, InventoryPlacementSource insertionSource, bool justCheck = false, bool sound = true, bool serverSync = true, bool ignoreReserved = false)
        => TryPlacingItem(ref item, insertionSource, justCheck, sound, serverSync, slot, slot, ignoreReserved);

    /// <summary> Loot all items from this inventory, to the player's </summary>
    public void LootAll()
    {
        Player player = Main.LocalPlayer;
        for (int i = 0; i < Size; i++)
        {
            if (items[i].type > ItemID.None)
            {
                if (!CanExtractItem(i, InventoryExtractionSource.Player))
                    continue;

                items[i].position = player.Center;
                items[i] = player.GetItem(Main.myPlayer, items[i], GetItemSettings.LootAllSettingsRegularChest);
                uiItemSlots[i].ClearGlow();

                if (Main.netMode == NetmodeID.MultiplayerClient)
                    SyncItem(i);
            }
        }
    }

    /// <summary> Deposit all items from the player's inventory to this inventory </summary>
    /// <param name="context"> The transfer context, used for visual transfers </param>
    public void DepositAll(ContainerTransferContext context)
    {
        Player player = Main.LocalPlayer;
        bool transferredAny = false;

        for (int slot = 49; slot >= 10; slot--)
        {
            Item item = player.inventory[slot];
            if (item.IsAir || item.favorited)
                continue;

            int stackBefore = item.stack;
            if (TryPlacingItem(ref player.inventory[slot], InventoryPlacementSource.Player, sound: false) && player.inventory[slot].stack < stackBefore)
                transferredAny = true;
        }

        if (transferredAny)
            SoundEngine.PlaySound(SoundID.Grab);
    }

    public void QuickStack(ContainerTransferContext context)
    {
        Player player = Main.LocalPlayer;
        HashSet<int> eligibleTypes = new();

        for (int i = 0; i < Size; i++)
        {
            Item existingItem = items[i];
            if (!existingItem.IsAir && !Utility.IsCoin(i) && CanAcceptItem(i, existingItem, InventoryPlacementSource.Player))
                eligibleTypes.Add(existingItem.netID);
        }

        bool transferredAny = false;
        for (int slot = 10; slot < 50; slot++)
        {
            Item playerItem = player.inventory[slot];
            if (playerItem.IsAir || playerItem.favorited || !eligibleTypes.Contains(playerItem.netID))
                continue;

            Item transferVisual = playerItem.Clone();
            int stackBefore = playerItem.stack;
            if (!TryPlacingItem(ref player.inventory[slot], InventoryPlacementSource.Player, sound: false))
                continue;

            int transferred = stackBefore - player.inventory[slot].stack;
            if (transferred <= 0)
                continue;

            transferredAny = true;
            if (context.CanVisualizeTransfers)
                Chest.VisualizeChestTransfer(player.Center, context.GetContainerWorldPosition(), transferVisual, transferred);
        }

        if (transferredAny)
            SoundEngine.PlaySound(SoundID.Grab);
    }

    /// <summary> Restock items from the inventory to the player's inventory </summary>
    public void Restock()
    {
        Player player = Main.LocalPlayer;
        Item[] playerInventory = player.inventory;

        HashSet<int> foundItemIds = new();
        List<int> playerSlotsToRestock = new();
        List<int> emptyItemSlots = new();
        for (int slot = 57; slot >= 0; slot--)
        {
            if ((slot < 50 || slot >= 54) && (playerInventory[slot].type < ItemID.CopperCoin || playerInventory[slot].type > ItemID.PlatinumCoin))
            {
                if (playerInventory[slot].stack > 0 && playerInventory[slot].maxStack > 1)
                {
                    foundItemIds.Add(playerInventory[slot].netID);
                    if (playerInventory[slot].stack < playerInventory[slot].maxStack)
                        playerSlotsToRestock.Add(slot);
                }
                else if (playerInventory[slot].stack == 0 || playerInventory[slot].netID == 0 || playerInventory[slot].type == ItemID.None)
                {
                    emptyItemSlots.Add(slot);
                }
            }
        }

        bool successfulTransfer = false;
        for (int i = 0; i < Size; i++)
        {
            if (!CanExtractItem(i, InventoryExtractionSource.Player))
                continue;

            if (items[i].stack < 1 || !foundItemIds.Contains(items[i].netID))
                continue;

            bool startNewStack = false;
            for (int j = 0; j < playerSlotsToRestock.Count; j++)
            {
                int slot = playerSlotsToRestock[j];
                int context = ItemSlot.Context.InventoryItem;

                if (slot >= 50)
                    context = ItemSlot.Context.InventoryAmmo;

                if (playerInventory[slot].netID != items[i].netID)
                    continue;

                if (ItemSlot.PickItemMovementAction(playerInventory, context, slot, items[i]) == -1)
                    continue;

                if (!ItemLoader.TryStackItems(playerInventory[slot], items[i], out _))
                    continue;

                successfulTransfer = true;
                if (playerInventory[slot].stack == playerInventory[slot].maxStack)
                {
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                        SyncItem(i);

                    playerSlotsToRestock.RemoveAt(j);
                    j--;
                }

                if (items[i].stack == 0)
                {
                    items[i] = new Item();
                    startNewStack = true;
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                        SyncItem(i);

                    break;
                }
            }

            if (startNewStack || emptyItemSlots.Count <= 0 || items[i].ammo == 0)
                continue;

            for (int k = 0; k < emptyItemSlots.Count; k++)
            {
                int context = ItemSlot.Context.InventoryItem;

                if (emptyItemSlots[k] >= 50)
                    context = ItemSlot.Context.InventoryAmmo;

                if (ItemSlot.PickItemMovementAction(playerInventory, context, emptyItemSlots[k], items[i]) != -1)
                {
                    Terraria.Utils.Swap(ref playerInventory[emptyItemSlots[k]], ref items[i]);

                    if (Main.netMode == NetmodeID.MultiplayerClient)
                        SyncItem(i);

                    playerSlotsToRestock.Add(emptyItemSlots[k]);
                    emptyItemSlots.RemoveAt(k);
                    successfulTransfer = true;
                    break;
                }
            }
        }

        if (successfulTransfer)
            SoundEngine.PlaySound(SoundID.Grab);
    }

    /// <summary> Sort the inventory's items </summary>
    public void Sort()
    {
        (int type, int stack, int prefix)[] preSort = new (int, int, int)[Size];
        (int type, int stack, int prefix)[] postSort = new (int, int, int)[Size];

        for (int i = 0; i < Size; i++)
        {
            preSort[i] = (items[i].netID, items[i].stack, items[i].prefix);
        }

        SortItems(items);

        for (int i = 0; i < Size; i++)
        {
            postSort[i] = (items[i].netID, items[i].stack, items[i].prefix);
        }

        if (Main.netMode != NetmodeID.MultiplayerClient)
            return;

        for (int k = 0; k < Size; k++)
        {
            if (postSort[k] != preSort[k])
                SyncItem(k);
        }
    }

    // This uses reflection to avoid copying a large chunk of code that does exactly what we need already.
    // However, this needed a detour on Inventory.Hooks.On_ItemSlot_SetGlow in order to set the inventory slot colors accordingly
    /// <summary> Sort items in an inventory using the vanilla logic </summary>
    public static void SortItems(Item[] items, params int[] ignoreSlots)
        => Utility.InvokeMethod(typeof(ItemSorting), "Sort", items, parameters: [ignoreSlots]);

    // Adapted from Terraria.UI.ChestUI.MoveCoins, but to support dynamic inventory size,
    // and the option of moving coins even though there are no coins already in the container.
    // Didn't really understand the purpose of some parts of the code, hence the non-descriptive variable names -- Feldy
    /// <summary> Transfers coins from an inventory to another </summary>
    /// <param name="source"> The source inventory </param>
    /// <param name="destination"> The destination inventory </param>
    /// <param name="moveEvenIfNoCoinsAreThereAlready">
    /// Run the logic even if there are no coins in the destination already, 
    /// in contrast to vanilla "Deposit all" behaviour 
    /// </param>
    /// <returns> The coin count in copper value </returns>
    public long MoveCoins(Item[] source, Item[] destination, bool moveEvenIfNoCoinsAreThereAlready = false)
    {
        bool success = false;
        bool foundCoinsInDestination = false;

        int[] coinsByType = new int[4];

        int[] tempArray = new int[destination.Length];
        List<int> tempList = new();
        List<int> tempList2 = new();

        long coinCount = Terraria.Utils.CoinsCount(out bool overflowing, source);

        int coinId;

        for (int i = 0; i < destination.Length; i++)
        {
            tempArray[i] = -1;
            if (destination[i].stack < 1 || destination[i].type <= ItemID.None)
            {
                tempList2.Add(i);
                destination[i] = new Item();
            }

            if (destination[i] != null && destination[i].stack > 0)
            {
                coinId = 0;
                if (destination[i].type == ItemID.CopperCoin)
                    coinId = 1;

                if (destination[i].type == ItemID.SilverCoin)
                    coinId = 2;

                if (destination[i].type == ItemID.GoldCoin)
                    coinId = 3;

                if (destination[i].type == ItemID.PlatinumCoin)
                    coinId = 4;

                tempArray[i] = coinId - 1;
                if (coinId > 0)
                {
                    coinsByType[coinId - 1] += destination[i].stack;
                    tempList2.Add(i);
                    destination[i] = new Item();
                    foundCoinsInDestination = true;
                }
            }
        }

        if (!foundCoinsInDestination && !moveEvenIfNoCoinsAreThereAlready)
            return 0L;

        for (int j = 0; j < source.Length; j++)
        {
            if (j != 58 && source[j] != null && source[j].stack > 0 && !source[j].favorited)
            {
                coinId = 0;
                if (source[j].type == ItemID.CopperCoin)
                    coinId = 1;

                if (source[j].type == ItemID.SilverCoin)
                    coinId = 2;

                if (source[j].type == ItemID.GoldCoin)
                    coinId = 3;

                if (source[j].type == ItemID.PlatinumCoin)
                    coinId = 4;

                if (coinId > 0)
                {
                    success = true;
                    coinsByType[coinId - 1] += source[j].stack;
                    tempList.Add(j);
                    source[j] = new Item();
                }
            }
        }

        for (int k = 0; k < 3; k++)
        {
            while (coinsByType[k] >= 100)
            {
                coinsByType[k] -= 100;
                coinsByType[k + 1]++;
            }
        }

        for (int l = 0; l < destination.Length; l++)
        {
            if (tempArray[l] < 0 || destination[l].type != ItemID.None)
                continue;

            int idx = l;
            coinId = tempArray[l];
            if (coinsByType[coinId] > 0)
            {
                destination[idx].SetDefaults(ItemID.CopperCoin + coinId);
                destination[idx].stack = coinsByType[coinId];
                if (destination[idx].stack > destination[idx].maxStack)
                    destination[idx].stack = destination[idx].maxStack;

                coinsByType[coinId] -= destination[idx].stack;
                tempArray[l] = -1;
            }

            if (Main.netMode == NetmodeID.MultiplayerClient)
                SyncItem(idx);

            tempList2.Remove(idx);
        }

        for (int m = 0; m < destination.Length; m++)
        {
            if (tempArray[m] < 0 || destination[m].type != ItemID.None)
                continue;

            int idx = m;
            coinId = 3;
            while (coinId >= 0)
            {
                if (coinsByType[coinId] > 0)
                {
                    destination[idx].SetDefaults(ItemID.CopperCoin + coinId);
                    destination[idx].stack = coinsByType[coinId];
                    if (destination[idx].stack > destination[idx].maxStack)
                        destination[idx].stack = destination[idx].maxStack;

                    coinsByType[coinId] -= destination[idx].stack;
                    tempArray[m] = -1;
                    break;
                }

                if (coinsByType[coinId] == 0)
                    coinId--;
            }

            if (Main.netMode == NetmodeID.MultiplayerClient)
                SyncItem(idx);

            tempList2.Remove(idx);
        }

        while (tempList2.Count > 0)
        {
            int idx = tempList2[0];
            coinId = 3;
            while (coinId >= 0)
            {
                if (coinsByType[coinId] > 0)
                {
                    destination[idx].SetDefaults(ItemID.CopperCoin + coinId);
                    destination[idx].stack = coinsByType[coinId];
                    if (destination[idx].stack > destination[idx].maxStack)
                        destination[idx].stack = destination[idx].maxStack;

                    coinsByType[coinId] -= destination[idx].stack;
                    break;
                }

                if (coinsByType[coinId] == 0)
                    coinId--;
            }

            if (Main.netMode == NetmodeID.MultiplayerClient)
                SyncItem(tempList2[0]);

            tempList2.RemoveAt(0);
        }

        coinId = 3;
        while (coinId >= 0 && tempList.Count > 0)
        {
            int idx = tempList[0];
            if (coinsByType[coinId] > 0)
            {
                source[idx].SetDefaults(ItemID.CopperCoin + coinId);
                source[idx].stack = coinsByType[coinId];
                if (source[idx].stack > source[idx].maxStack)
                    source[idx].stack = source[idx].maxStack;

                coinsByType[coinId] -= source[idx].stack;
                success = false;
                tempList.RemoveAt(0);
            }

            if (coinsByType[coinId] == 0)
                coinId--;
        }

        if (success)
            SoundEngine.PlaySound(SoundID.Grab);

        long coinsCountPostMove = Terraria.Utils.CoinsCount(out bool overflowPostMove, source);
        if (overflowing || overflowPostMove)
            return 0L;

        return coinCount - coinsCountPostMove;
    }

    public IEnumerator<Item> GetEnumerator()
    {
        return ((IEnumerable<Item>)items).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return items.GetEnumerator();
    }
}
