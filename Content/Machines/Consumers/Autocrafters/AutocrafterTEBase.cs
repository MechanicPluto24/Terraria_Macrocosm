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
    public virtual int InputPoolSize => 100;
    public sealed override int InventorySize => OutputSlots + InputPoolSize;
    public Dictionary<int, List<int>> InputSlotAllocation { get; private set; } = new();

    protected virtual bool AllowHandCrafting => false;
    protected virtual int[] AvailableCraftingStations => [];
    public Recipe[] SelectedRecipes { get; private set; }

    private float craftTimer;
    private float CraftRate => 60f;

    public override void OnFirstUpdate()
    {
        for (int i = 0; i < OutputSlots; i++)
            Inventory.SetSlotRole(i, InventorySlotRole.Output);

        for (int i = OutputSlots; i < Inventory.Size; i++)
            Inventory.SetSlotRole(i, InventorySlotRole.Input);
    }

    public virtual bool RecipeAllowed(Recipe recipe)
    {
        if (AllowHandCrafting && recipe.requiredTile.All(tile => tile == -1))
            return true;

        return recipe.requiredTile.Where(tile => tile != -1).All(tile => AvailableCraftingStations.Contains(tile));
    }

    public bool CanOverwriteRecipeAt(int outputSlot)
    {
        if (!InputSlotAllocation.TryGetValue(outputSlot, out var slots))
            return true;

        return slots.All(i => Inventory[i].IsAir);
    }

    public bool IsInputSlotReserved(int slot, int outputSlot) => InputSlotAllocation.Where(kv => kv.Key != outputSlot).SelectMany(kv => kv.Value).Contains(slot);

    public bool SelectRecipeInFreeSlot(Recipe recipe)
    {
        if (recipe is null)
            return false;

        if (SelectedRecipes == null || SelectedRecipes.Length != OutputSlots)
            SelectedRecipes = new Recipe[OutputSlots];

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

    public bool SelectRecipeInSlot(int outputSlot, Recipe recipe)
    {
        if (SelectedRecipes == null || SelectedRecipes.Length != OutputSlots)
            SelectedRecipes = new Recipe[OutputSlots];

        if (!CanOverwriteRecipeAt(outputSlot))
            return false;

        if (outputSlot < 0 || outputSlot >= OutputSlots)
            return false;

        Inventory.ClearReserved(outputSlot);
        if (InputSlotAllocation.TryGetValue(outputSlot, out var oldInputSlots))
            foreach (var slot in oldInputSlots)
                Inventory.ClearReserved(slot);

        SelectedRecipes[outputSlot] = recipe;
        if (recipe is null)
        {
            InputSlotAllocation.Remove(outputSlot);
            return false;
        }

        Inventory.SetReserved(
            outputSlot,
            recipe.createItem.type,
            tooltip: null,
            texture: TextureAssets.Item[recipe.createItem.type],
            color: Color.White * 0.8f
        );

        List<int> allocatedInputs = new();
        int inputIndex = OutputSlots; // start after output slots
        foreach (var requiredItem in recipe.requiredItem)
        {
            if (requiredItem.type <= ItemID.None)
                continue;

            while (inputIndex < Inventory.Size && (!Inventory[inputIndex].IsAir || IsInputSlotReserved(inputIndex, outputSlot)))
                inputIndex++;

            if (inputIndex >= Inventory.Size)
                break;

            allocatedInputs.Add(inputIndex);
            Inventory.SetReserved(
                inputIndex,
                requiredItem.type,
                tooltip: null,
                texture: TextureAssets.Item[requiredItem.type],
                color: Color.White * 0.5f
            );

            inputIndex++;
        }

        InputSlotAllocation[outputSlot] = allocatedInputs;
        return true;
    }

    public override void MachineUpdate()
    {
        if (!PoweredOn || SelectedRecipes is null)
            return;

        craftTimer += 1f * PowerProgress;
        if (craftTimer < CraftRate)
            return;

        craftTimer -= CraftRate;

        for (int outputSlot = 0; outputSlot < SelectedRecipes.Length; outputSlot++)
        {
            Recipe recipe = SelectedRecipes[outputSlot];
            if (recipe is null)
                continue;

            if (!CanCraftRecipe(outputSlot, recipe))
                continue;

            ConsumeRecipeIngredients(outputSlot, recipe);

            Item result = recipe.createItem.Clone();
            result.OnCreated(new MachineItemCreationContext(result, this));
            if (!Inventory.TryPlacingItemInSlot(ref result, outputSlot, sound: false, serverSync: true) && result.stack > 0)
                Item.NewItem(new EntitySource_TileEntity(this), InventoryPosition, result);
        }
    }

    private bool CanCraftRecipe(int outputSlot, Recipe recipe)
    {
        if (!InputSlotAllocation.TryGetValue(outputSlot, out var inputSlots))
            return false;

        foreach (var requiredItem in recipe.requiredItem)
        {
            if (requiredItem.type <= ItemID.None || requiredItem.stack <= 0)
                continue;

            int found = 0;
            foreach (var slot in inputSlots)
            {
                if (Inventory[slot].type == requiredItem.type)
                    found += Inventory[slot].stack;
            }
            if (found < requiredItem.stack)
                return false;
        }
        return true;
    }

    private void ConsumeRecipeIngredients(int outputSlot, Recipe recipe)
    {
        if (!InputSlotAllocation.TryGetValue(outputSlot, out var inputSlots))
            return;

        foreach (var requiredItem in recipe.requiredItem)
        {
            if (requiredItem.type <= ItemID.None || requiredItem.stack <= 0)
                continue;

            int toConsume = requiredItem.stack;
            foreach (var slot in inputSlots)
            {
                if (Inventory[slot].type == requiredItem.type)
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
                    SelectRecipeInSlot(i, matchingRecipe);
            }
        }
    }

    protected override void ConsumerNetSend(BinaryWriter writer) => base.ConsumerNetSend(writer);
    protected override void ConsumerNetReceive(BinaryReader reader) => base.ConsumerNetReceive(reader);
}
