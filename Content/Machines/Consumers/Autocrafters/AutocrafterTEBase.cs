using Macrocosm.Common.ItemCreationContexts;
using Macrocosm.Common.Storage;
using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace Macrocosm.Content.Machines.Consumers.Autocrafters;

public abstract class AutocrafterTEBase : ConsumerTE, IMultiInventoryOwner, IInventoryAutomationOwner
{
    public abstract int OutputSlots { get; }
    public const int InputSlotsPerOutput = 15;
    public sealed override int InventorySize => OutputSlots;
    protected virtual bool AllowHandCrafting => false;
    protected virtual int[] AvailableCraftingStations => [];

    public AutocrafterLane[] Lanes { get; private set; }
    public Recipe[] SelectedRecipes => Lanes?.Select(lane => lane.SelectedRecipe).ToArray();
    public int InventoryLayoutVersion { get; private set; }

    public IReadOnlyList<Inventory> Inventories
    {
        get
        {
            EnsureLanes();
            return [Inventory, .. Lanes.Select(lane => lane.InputInventory)];
        }
    }

    private float craftTimer;
    private const float CraftRate = 60f;
    private bool suppressRecipeSync;
    private int automationLaneCursor;

    public override float PowerDemand => IsEnabledByPlayer && HasCraftingWork() ? MaxPower : 0f;

    public int GetInventoryRevision(int inventoryIndex)
    {
        EnsureLanes();
        int laneIndex = inventoryIndex - 1;
        return laneIndex >= 0 && laneIndex < Lanes.Length ? Lanes[laneIndex].Revision : 0;
    }

    public IEnumerable<Inventory> GetAutomationOutputInventories()
    {
        yield return Inventory;
    }

    public bool TryInsertFromAutomation(ref Item item, bool sound = false)
    {
        EnsureLanes();
        if (item is null || item.IsAir || Lanes.Length == 0)
            return false;

        for (int offset = 0; offset < Lanes.Length; offset++)
        {
            int laneIndex = (automationLaneCursor + offset) % Lanes.Length;
            Inventory input = Lanes[laneIndex].InputInventory;
            if (input.Size == 0 || !input.TryPlacingItem(ref item, InventoryPlacementSource.Automation, sound: sound))
                continue;
            automationLaneCursor = (laneIndex + 1) % Lanes.Length;
            return true;
        }
        return false;
    }

    public override void OnFirstUpdate()
    {
        EnsureLanes();
        for (int i = 0; i < Inventory.Size; i++)
            Inventory.SetSlotRole(i, InventorySlotRole.Output);
    }

    public override void OnKill()
    {
        base.OnKill();
        EnsureLanes();
        foreach (AutocrafterLane lane in Lanes)
            lane.InputInventory.DropAllItems(InventoryPosition);
    }

    public virtual bool RecipeAllowed(Recipe recipe)
    {
        if (AutocrafterLane.GetRequirements(recipe).Count() > InputSlotsPerOutput)
            return false;
        int[] requiredTiles = recipe.requiredTile.Where(tile => tile != -1).ToArray();
        return requiredTiles.Length == 0 ? AllowHandCrafting : requiredTiles.All(tile => AvailableCraftingStations.Contains(tile));
    }

    public bool CanOverwriteRecipeAt(int outputSlot)
    {
        EnsureLanes();
        return outputSlot >= 0 && outputSlot < OutputSlots && Inventory[outputSlot].IsAir && Lanes[outputSlot].InputInventory.IsEmpty;
    }

    public bool SelectRecipeInFreeSlot(Recipe recipe)
    {
        if (recipe is null)
            return false;
        EnsureLanes();
        int outputSlot = Array.FindIndex(Lanes, lane => lane.SelectedRecipe is null);
        return outputSlot >= 0 && SelectRecipeInSlot(outputSlot, recipe);
    }

    public bool SelectRecipeInSlot(int outputSlot, Recipe recipe) => SelectRecipeInSlot(outputSlot, recipe, restoring: false);

    private bool SelectRecipeInSlot(int outputSlot, Recipe recipe, bool restoring)
    {
        EnsureLanes();
        if (outputSlot < 0 || outputSlot >= OutputSlots)
            return false;
        if (recipe is null)
            return ClearRecipeSlot(outputSlot);
        if (!restoring && !CanOverwriteRecipeAt(outputSlot))
            return false;

        Inventory.ClearReserved(outputSlot);
        Inventory.SetReserved(outputSlot, recipe.createItem.type, texture: Terraria.GameContent.TextureAssets.Item[recipe.createItem.type], stack: recipe.createItem.stack);
        Lanes[outputSlot].Configure(recipe, this);
        InventoryLayoutVersion++;
        SyncRecipeSelection();
        return true;
    }

    public bool ClearRecipeSlot(int outputSlot)
    {
        EnsureLanes();
        if (outputSlot < 0 || outputSlot >= OutputSlots || Lanes[outputSlot].SelectedRecipe is null || !CanOverwriteRecipeAt(outputSlot))
            return false;
        Inventory.ClearReserved(outputSlot);
        Lanes[outputSlot].Configure(null, this);
        InventoryLayoutVersion++;
        SyncRecipeSelection();
        return true;
    }

    private void EnsureLanes()
    {
        if (Lanes is not null && Lanes.Length == OutputSlots)
            return;
        Lanes = Enumerable.Range(0, OutputSlots).Select(index => new AutocrafterLane(index, this)).ToArray();
        InventoryLayoutVersion++;
    }

    private void SyncRecipeSelection()
    {
        if (!suppressRecipeSync && Main.netMode != NetmodeID.SinglePlayer)
            NetSync();
    }

    public override void MachineUpdate()
    {
        if (!IsRunning)
            return;
        EnsureLanes();
        craftTimer += RatedPowerProgress;
        if (craftTimer < CraftRate)
            return;
        craftTimer -= CraftRate;

        foreach (AutocrafterLane lane in Lanes)
        {
            Recipe recipe = lane.SelectedRecipe;
            if (recipe is null || !CanCraftRecipe(lane) || !CanStoreRecipeOutput(lane.Index, recipe))
                continue;
            ConsumeRecipeIngredients(lane);
            Item result = recipe.createItem.Clone();
            result.OnCreated(new MachineItemCreationContext(result, this));
            if (!Inventory.TryPlacingItemInSlot(ref result, lane.Index, InventoryPlacementSource.Internal, sound: false, serverSync: true) && result.stack > 0)
                Item.NewItem(new EntitySource_TileEntity(this), InventoryPosition, result);
        }
    }

    private static bool CanCraftRecipe(AutocrafterLane lane)
        => AutocrafterLane.GetRequirements(lane.SelectedRecipe).All(requirement => lane.InputInventory.CountItems(requirement.Type) >= requirement.Stack);

    private bool HasCraftingWork()
    {
        EnsureLanes();
        return Lanes.Any(lane => lane.SelectedRecipe is not null && CanCraftRecipe(lane) && CanStoreRecipeOutput(lane.Index, lane.SelectedRecipe));
    }

    private bool CanStoreRecipeOutput(int outputSlot, Recipe recipe)
    {
        Item result = recipe.createItem.Clone();
        return Inventory.TryPlacingItemInSlot(ref result, outputSlot, InventoryPlacementSource.Internal, justCheck: true, sound: false, serverSync: false);
    }

    private static void ConsumeRecipeIngredients(AutocrafterLane lane)
    {
        foreach (var requirement in AutocrafterLane.GetRequirements(lane.SelectedRecipe))
        {
            int toConsume = requirement.Stack;
            for (int slot = 0; slot < lane.InputInventory.Size && toConsume > 0; slot++)
            {
                Item item = lane.InputInventory[slot];
                if (item.type != requirement.Type)
                    continue;
                int consume = Math.Min(toConsume, item.stack);
                item.DecreaseStack(consume);
                toConsume -= consume;
                lane.InputInventory.SyncItem(slot);
            }
        }
    }

    protected override void ConsumerSaveData(TagCompound tag)
    {
        base.ConsumerSaveData(tag);
        EnsureLanes();
        tag[nameof(Lanes)] = Lanes.Select(lane => new TagCompound
        {
            [nameof(AutocrafterLane.SelectedRecipe)] = SaveRecipe(lane.SelectedRecipe),
            [nameof(AutocrafterLane.InputInventory)] = lane.InputInventory.SerializeData()
        }).ToArray();
    }

    protected override void ConsumerLoadData(TagCompound tag)
    {
        base.ConsumerLoadData(tag);
        EnsureLanes();
        TagCompound[] laneTags = tag.TryGet(nameof(Lanes), out TagCompound[] savedLanes) ? savedLanes : [];
        bool hasSelfContainedLanes = laneTags.Any(laneTag => laneTag.ContainsKey(nameof(AutocrafterLane.InputInventory)));
        TagCompound[] recipeTags = hasSelfContainedLanes
            ? laneTags.Select(laneTag => laneTag.GetCompound(nameof(AutocrafterLane.SelectedRecipe))).ToArray()
            : tag.TryGet(nameof(SelectedRecipes), out TagCompound[] savedRecipes) ? savedRecipes : [];
        suppressRecipeSync = true;
        try
        {
            RebuildRecipes(recipeTags.Select(FindSavedRecipe).ToArray());
            for (int i = 0; i < Math.Min(laneTags.Length, Lanes.Length); i++)
            {
                TagCompound inventoryTag = hasSelfContainedLanes
                    ? laneTags[i].GetCompound(nameof(AutocrafterLane.InputInventory))
                    : laneTags[i];
                Lanes[i].RestoreInventory(Inventory.DeserializeData(inventoryTag), this);
            }
        }
        finally { suppressRecipeSync = false; }
    }

    protected override void ConsumerNetSend(BinaryWriter writer)
    {
        base.ConsumerNetSend(writer);
        EnsureLanes();
        writer.Write((byte)OutputSlots);
        foreach (AutocrafterLane lane in Lanes)
        {
            writer.Write(lane.Revision);
            writer.Write((byte)lane.InputInventory.InteractingPlayer);
            WriteRecipe(writer, lane.SelectedRecipe);
        }
        foreach (AutocrafterLane lane in Lanes) TagIO.ToStream(lane.InputInventory.SerializeData(), writer.BaseStream, compress: true);
    }

    protected override void ConsumerNetReceive(BinaryReader reader)
    {
        base.ConsumerNetReceive(reader);
        int recipeCount = reader.ReadByte();
        Recipe[] recipes = new Recipe[OutputSlots];
        int[] revisions = new int[OutputSlots];
        int[] interactingPlayers = new int[OutputSlots];
        for (int i = 0; i < recipeCount; i++)
        {
            int revision = reader.ReadInt32();
            int interactingPlayer = reader.ReadByte();
            Recipe recipe = ReadRecipe(reader);
            if (i < recipes.Length)
            {
                recipes[i] = recipe;
                revisions[i] = revision;
                interactingPlayers[i] = interactingPlayer;
            }
        }
        suppressRecipeSync = true;
        try
        {
            RebuildRecipes(recipes, revisions, interactingPlayers);
            for (int i = 0; i < recipeCount; i++)
            {
                Inventory received = Inventory.DeserializeData(TagIO.FromStream(reader.BaseStream, compressed: true));
                if (i < Lanes.Length) Lanes[i].RestoreInventory(received, this);
            }
        }
        finally { suppressRecipeSync = false; }
    }

    private void RebuildRecipes(Recipe[] recipes, int[] revisions = null, int[] receivedInteractingPlayers = null)
    {
        int[] interactingPlayers = receivedInteractingPlayers ?? Lanes?.Select(lane => lane.InputInventory.InteractingPlayer).ToArray() ?? [];
        Lanes = Enumerable.Range(0, OutputSlots).Select(index => new AutocrafterLane(index, this)).ToArray();
        for (int i = 0; i < Math.Min(interactingPlayers.Length, Lanes.Length); i++)
            Lanes[i].InputInventory.SetInteractingPlayer(interactingPlayers[i], sync: false);
        for (int i = 0; i < Inventory.Size; i++)
        {
            Inventory.ClearReserved(i);
            Inventory.SetSlotRole(i, InventorySlotRole.Output);
        }
        for (int i = 0; i < Math.Min(recipes.Length, OutputSlots); i++)
            if (recipes[i] is not null) SelectRecipeInSlot(i, recipes[i], restoring: true);
        if (revisions is not null)
            for (int i = 0; i < Math.Min(revisions.Length, Lanes.Length); i++)
                Lanes[i].SetRevision(revisions[i]);
        InventoryLayoutVersion++;
    }

    private static TagCompound SaveRecipe(Recipe recipe) => recipe is null ? new TagCompound() : new TagCompound
    {
        [nameof(Recipe.createItem)] = ItemIO.Save(recipe.createItem),
        [nameof(Recipe.requiredItem)] = recipe.requiredItem.Where(item => item.type > ItemID.None && item.stack > 0).Select(ItemIO.Save).ToList()
    };

    private Recipe FindSavedRecipe(TagCompound tag)
    {
        if (tag is null || tag.Count == 0) return null;
        Item result = ItemIO.Load(tag.GetCompound(nameof(Recipe.createItem)));
        var ingredients = tag.GetList<TagCompound>(nameof(Recipe.requiredItem)).Select(ItemIO.Load).Select(item => (item.type, item.stack));
        return Main.recipe.FirstOrDefault(recipe => recipe.createItem.type == result.type && RecipeAllowed(recipe)
            && NormalizedRequirements(recipe).SequenceEqual(NormalizedRequirements(ingredients)));
    }

    private static void WriteRecipe(BinaryWriter writer, Recipe recipe)
    {
        writer.Write(recipe is not null);
        if (recipe is null) return;
        writer.Write(recipe.createItem.type);
        writer.Write(recipe.createItem.stack);
        var requirements = NormalizedRequirements(recipe).ToArray();
        writer.Write(requirements.Length);
        foreach (var item in requirements) { writer.Write(item.Type); writer.Write(item.Stack); }
        int[] tiles = recipe.requiredTile.Where(tile => tile != -1).OrderBy(tile => tile).ToArray();
        writer.Write(tiles.Length);
        foreach (int tile in tiles) writer.Write(tile);
    }

    private Recipe ReadRecipe(BinaryReader reader)
    {
        if (!reader.ReadBoolean()) return null;
        int resultType = reader.ReadInt32();
        int resultStack = reader.ReadInt32();
        var requirements = new (int Type, int Stack)[reader.ReadInt32()];
        for (int i = 0; i < requirements.Length; i++) requirements[i] = (reader.ReadInt32(), reader.ReadInt32());
        int[] tiles = new int[reader.ReadInt32()];
        for (int i = 0; i < tiles.Length; i++) tiles[i] = reader.ReadInt32();
        return Main.recipe.FirstOrDefault(recipe => RecipeAllowed(recipe) && recipe.createItem.type == resultType && recipe.createItem.stack == resultStack
            && NormalizedRequirements(recipe).SequenceEqual(requirements.OrderBy(item => item.Type))
            && recipe.requiredTile.Where(tile => tile != -1).OrderBy(tile => tile).SequenceEqual(tiles));
    }

    private static IEnumerable<(int Type, int Stack)> NormalizedRequirements(Recipe recipe)
        => AutocrafterLane.GetRequirements(recipe).OrderBy(item => item.Type);

    private static IEnumerable<(int Type, int Stack)> NormalizedRequirements(IEnumerable<(int type, int stack)> items)
        => items.Where(item => item.type > ItemID.None && item.stack > 0).GroupBy(item => item.type)
            .Select(group => (Type: group.Key, Stack: group.Sum(item => item.stack))).OrderBy(item => item.Type);
}
