using Macrocosm.Common.ItemCreationContexts;
using Macrocosm.Common.Storage;
using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace Macrocosm.Content.Machines.Consumers.Autocrafters;

public abstract class AutocrafterTEBase : ConsumerTE
{
    public abstract int OutputSlots { get; }
    public const int InputSlotsPerOutput = 15;
    public virtual int InputPoolSize => OutputSlots * InputSlotsPerOutput;
    public sealed override int InventorySize => OutputSlots + InputPoolSize;

    protected virtual bool AllowHandCrafting => false;
    protected virtual int[] AvailableCraftingStations => [];
    public Recipe[] SelectedRecipes { get; private set; }

    private float craftTimer;
    private float CraftRate => 60f;
    private bool suppressRecipeSync;

    public override float PowerDemand => IsEnabledByPlayer && HasCraftingWork() ? MaxPower : 0f;

    public override void OnFirstUpdate()
    {
        for (int i = 0; i < OutputSlots; i++)
            Inventory.SetSlotRole(i, InventorySlotRole.Output);

        for (int i = OutputSlots; i < Inventory.Size; i++)
            Inventory.SetSlotRole(i, InventorySlotRole.Input);

        Inventory.CanInsertIntoSlot = CanInsertIntoSlot;
    }

    private bool CanInsertIntoSlot(int slot, Item item)
    {
        if (slot < OutputSlots)
            return false;

        if (SelectedRecipes is null || item is null || item.IsAir)
            return false;

        return SelectedRecipes.Any(recipe => recipe is not null && recipe.requiredItem.Any(requiredItem => requiredItem.type == item.type));
    }

    public virtual bool RecipeAllowed(Recipe recipe)
    {
        int requiredInputSlots = recipe.requiredItem
            .Where(item => item.type > ItemID.None && item.stack > 0)
            .Select(item => item.type)
            .Distinct()
            .Count();

        if (requiredInputSlots > InputPoolSize)
            return false;

        int[] requiredTiles = recipe.requiredTile.Where(tile => tile != -1).ToArray();

        if (requiredTiles.Length == 0)
            return AllowHandCrafting;

        return requiredTiles.All(tile => AvailableCraftingStations.Contains(tile));
    }

    public bool CanOverwriteRecipeAt(int outputSlot)
    {
        if (outputSlot < 0 || outputSlot >= OutputSlots)
            return false;

        if (!Inventory[outputSlot].IsAir)
            return false;

        return true;
    }

    public bool SelectRecipeInFreeSlot(Recipe recipe)
    {
        if (recipe is null)
            return false;

        EnsureSelectedRecipes();

        int outputSlot = -1;
        for (int i = 0; i < OutputSlots; i++)
        {
            if (SelectedRecipes[i] == null)
            {
                outputSlot = i;
                break;
            }
        }

        if (outputSlot == -1)
            return false;

        return SelectRecipeInSlot(outputSlot, recipe);
    }

    public bool SelectRecipeInSlot(int outputSlot, Recipe recipe) => SelectRecipeInSlot(outputSlot, recipe, restoring: false);

    private bool SelectRecipeInSlot(int outputSlot, Recipe recipe, bool restoring)
    {
        if (recipe is null)
            return ClearRecipeSlot(outputSlot);

        EnsureSelectedRecipes();

        if (!restoring && !CanOverwriteRecipeAt(outputSlot))
            return false;

        if (outputSlot < 0 || outputSlot >= OutputSlots)
            return false;

        Inventory.ClearReserved(outputSlot);

        SelectedRecipes[outputSlot] = recipe;

        Inventory.SetReserved(
            outputSlot,
            recipe.createItem.type,
            tooltip: null,
            texture: TextureAssets.Item[recipe.createItem.type],
            color: Color.White,
            stack: recipe.createItem.stack
        );

        SyncRecipeSelection();
        return true;
    }

    public bool ClearRecipeSlot(int outputSlot)
    {
        EnsureSelectedRecipes();

        if (outputSlot < 0 || outputSlot >= OutputSlots)
            return false;

        if (SelectedRecipes[outputSlot] is null)
            return false;

        if (!CanOverwriteRecipeAt(outputSlot))
            return false;

        Inventory.ClearReserved(outputSlot);
        SelectedRecipes[outputSlot] = null;
        SyncRecipeSelection();
        return true;
    }

    private void EnsureSelectedRecipes()
    {
        if (SelectedRecipes == null || SelectedRecipes.Length != OutputSlots)
            SelectedRecipes = new Recipe[OutputSlots];
    }

    private void SyncRecipeSelection()
    {
        if (!suppressRecipeSync && Main.netMode != NetmodeID.SinglePlayer)
            NetSync();
    }

    public override void MachineUpdate()
    {
        if (!IsRunning || SelectedRecipes is null)
            return;

        craftTimer += 1f * RatedPowerProgress;
        if (craftTimer < CraftRate)
            return;

        craftTimer -= CraftRate;

        for (int outputSlot = 0; outputSlot < SelectedRecipes.Length; outputSlot++)
        {
            Recipe recipe = SelectedRecipes[outputSlot];
            if (recipe is null)
                continue;

            if (!CanCraftRecipe(recipe))
                continue;

            if (!CanStoreRecipeOutput(outputSlot, recipe))
                continue;

            ConsumeRecipeIngredients(recipe);

            Item result = recipe.createItem.Clone();
            result.OnCreated(new MachineItemCreationContext(result, this));
            if (!Inventory.TryPlacingItemInSlot(ref result, outputSlot, InventoryPlacementSource.Internal, sound: false, serverSync: true) && result.stack > 0)
                Item.NewItem(new EntitySource_TileEntity(this), InventoryPosition, result);
        }
    }

    private bool CanCraftRecipe(Recipe recipe)
    {
        foreach (var requiredItem in GetRequiredItems(recipe))
        {
            if (Inventory.CountItems(requiredItem.Type, startIndex: OutputSlots) < requiredItem.Stack)
                return false;
        }

        return true;
    }

    private bool HasCraftingWork()
    {
        if (SelectedRecipes is null)
            return false;

        for (int outputSlot = 0; outputSlot < SelectedRecipes.Length; outputSlot++)
        {
            Recipe recipe = SelectedRecipes[outputSlot];
            if (recipe is not null && CanCraftRecipe(recipe) && CanStoreRecipeOutput(outputSlot, recipe))
                return true;
        }

        return false;
    }

    private bool CanStoreRecipeOutput(int outputSlot, Recipe recipe)
    {
        Item result = recipe.createItem.Clone();
        return Inventory.TryPlacingItemInSlot(ref result, outputSlot, InventoryPlacementSource.Internal, justCheck: true, sound: false, serverSync: false);
    }

    private void ConsumeRecipeIngredients(Recipe recipe)
    {
        foreach (var requiredItem in GetRequiredItems(recipe))
        {
            int toConsume = requiredItem.Stack;
            for (int slot = OutputSlots; slot < Inventory.Size; slot++)
            {
                if (Inventory[slot].type == requiredItem.Type)
                {
                    int consume = Math.Min(toConsume, Inventory[slot].stack);
                    Inventory[slot].DecreaseStack(consume);
                    toConsume -= consume;
                    if (toConsume <= 0)
                        break;
                }
            }
        }
    }

    private static IEnumerable<(int Type, int Stack)> GetRequiredItems(Recipe recipe)
        => recipe.requiredItem
            .Where(item => item.type > ItemID.None && item.stack > 0)
            .GroupBy(item => item.type)
            .Select(group => (Type: group.Key, Stack: group.Sum(item => item.stack)));

    protected override void ConsumerSaveData(TagCompound tag)
    {
        base.ConsumerSaveData(tag);
        if (SelectedRecipes is not null)
        {
            TagCompound[] recipeTags = new TagCompound[SelectedRecipes.Length];
            for (int i = 0; i < SelectedRecipes.Length; i++)
            {
                var recipe = SelectedRecipes[i];
                if (recipe is not null)
                {
                    recipeTags[i] = new TagCompound
                    {
                        [nameof(Recipe.createItem)] = ItemIO.Save(recipe.createItem),
                        [nameof(Recipe.requiredItem)] = recipe.requiredItem
                            .Where(item => item.type > ItemID.None && item.stack > 0)
                            .Select(ItemIO.Save)
                            .ToList()
                    };
                }
                else
                {
                    recipeTags[i] = new TagCompound();
                }
            }
            tag[nameof(SelectedRecipes)] = recipeTags;
        }
    }

    protected override void ConsumerLoadData(TagCompound tag)
    {
        base.ConsumerLoadData(tag);
        if (tag.TryGet(nameof(SelectedRecipes), out TagCompound[] recipeTags))
        {
            SelectedRecipes = new Recipe[OutputSlots];
            for (int i = 0; i < Math.Min(recipeTags.Length, OutputSlots); i++)
            {
                var recipeTag = recipeTags[i];
                if (recipeTag.Count == 0)
                    continue;

                Item result = ItemIO.Load(recipeTag.Get<TagCompound>(nameof(Recipe.createItem)));
                IEnumerable<Item> ingredients = recipeTag.GetList<TagCompound>(nameof(Recipe.requiredItem)).Select(ItemIO.Load);
                Recipe matchingRecipe = Main.recipe.FirstOrDefault(r =>
                {
                    if (r.createItem.type != result.type)
                        return false;
                    IEnumerable<int> recipeIngredients = r.requiredItem.Where(item => item.type > ItemID.None && item.stack > 0).Select(item => item.type);
                    IEnumerable<int> savedIngredients = ingredients.Where(item => item.type > ItemID.None).Select(item => item.type);
                    return recipeIngredients.OrderBy(x => x).SequenceEqual(savedIngredients.OrderBy(x => x));
                });

                if (matchingRecipe != null && RecipeAllowed(matchingRecipe))
                    SelectRecipeInSlotWithoutSync(i, matchingRecipe, restoring: true);
            }
        }
    }

    protected override void ConsumerNetSend(BinaryWriter writer)
    {
        base.ConsumerNetSend(writer);

        writer.Write((byte)OutputSlots);
        for (int i = 0; i < OutputSlots; i++)
            WriteRecipe(writer, SelectedRecipes is not null && i < SelectedRecipes.Length ? SelectedRecipes[i] : null);
    }

    protected override void ConsumerNetReceive(BinaryReader reader)
    {
        base.ConsumerNetReceive(reader);

        int recipeCount = reader.ReadByte();
        Recipe[] receivedRecipes = new Recipe[OutputSlots];
        for (int i = 0; i < recipeCount; i++)
        {
            Recipe recipe = ReadRecipe(reader);
            if (i < OutputSlots)
                receivedRecipes[i] = recipe;
        }

        RebuildSelectedRecipes(receivedRecipes);
    }

    private static void WriteRecipe(BinaryWriter writer, Recipe recipe)
    {
        writer.Write(recipe is not null);
        if (recipe is null)
            return;

        writer.Write(recipe.createItem.type);
        writer.Write(recipe.createItem.stack);

        Item[] requiredItems = recipe.requiredItem.Where(item => item.type > ItemID.None && item.stack > 0).ToArray();
        writer.Write(requiredItems.Length);
        foreach (Item item in requiredItems)
        {
            writer.Write(item.type);
            writer.Write(item.stack);
        }

        int[] requiredTiles = recipe.requiredTile.Where(tile => tile != -1).ToArray();
        writer.Write(requiredTiles.Length);
        foreach (int tile in requiredTiles)
            writer.Write(tile);
    }

    private Recipe ReadRecipe(BinaryReader reader)
    {
        bool hasRecipe = reader.ReadBoolean();
        if (!hasRecipe)
            return null;

        int createItemType = reader.ReadInt32();
        int createItemStack = reader.ReadInt32();

        int requiredItemCount = reader.ReadInt32();
        (int type, int stack)[] requiredItems = new (int, int)[requiredItemCount];
        for (int i = 0; i < requiredItemCount; i++)
            requiredItems[i] = (reader.ReadInt32(), reader.ReadInt32());

        int requiredTileCount = reader.ReadInt32();
        int[] requiredTiles = new int[requiredTileCount];
        for (int i = 0; i < requiredTileCount; i++)
            requiredTiles[i] = reader.ReadInt32();

        return Main.recipe.FirstOrDefault(recipe =>
            recipe is not null &&
            RecipeAllowed(recipe) &&
            recipe.createItem.type == createItemType &&
            recipe.createItem.stack == createItemStack &&
            RecipeRequirementsMatch(recipe, requiredItems, requiredTiles)
        );
    }

    private static bool RecipeRequirementsMatch(Recipe recipe, (int type, int stack)[] requiredItems, int[] requiredTiles)
    {
        var recipeItems = recipe.requiredItem
            .Where(item => item.type > ItemID.None && item.stack > 0)
            .Select(item => (item.type, item.stack))
            .OrderBy(item => item.type)
            .ThenBy(item => item.stack);

        var receivedItems = requiredItems
            .OrderBy(item => item.type)
            .ThenBy(item => item.stack);

        if (!recipeItems.SequenceEqual(receivedItems))
            return false;

        var recipeTiles = recipe.requiredTile
            .Where(tile => tile != -1)
            .OrderBy(tile => tile);

        return recipeTiles.SequenceEqual(requiredTiles.OrderBy(tile => tile));
    }

    private void RebuildSelectedRecipes(Recipe[] recipes)
    {
        SelectedRecipes = new Recipe[OutputSlots];
        for (int i = 0; i < Inventory.Size; i++)
            Inventory.ClearReserved(i);

        suppressRecipeSync = true;
        try
        {
            for (int i = 0; i < Math.Min(recipes.Length, OutputSlots); i++)
            {
                if (recipes[i] is not null)
                    SelectRecipeInSlot(i, recipes[i], restoring: true);
            }
        }
        finally
        {
            suppressRecipeSync = false;
        }
    }

    private bool SelectRecipeInSlotWithoutSync(int outputSlot, Recipe recipe, bool restoring = false)
    {
        suppressRecipeSync = true;
        try
        {
            return SelectRecipeInSlot(outputSlot, recipe, restoring);
        }
        finally
        {
            suppressRecipeSync = false;
        }
    }
}
