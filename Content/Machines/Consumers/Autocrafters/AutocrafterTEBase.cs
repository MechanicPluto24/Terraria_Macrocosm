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

namespace Macrocosm.Content.Machines.Consumers.Autocrafters
{
    public abstract class AutocrafterTEBase : ConsumerTE
    {
        public abstract int OutputSlots { get; }
        public virtual int InputSlots => OutputSlots * 30; // Should be enough
        public sealed override int InventorySize => InputSlots + OutputSlots;

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

        protected virtual bool RecipeAllowed(Recipe recipe)
        {
            // "By Hand" recipes allowed by default
            if (recipe.requiredTile.All(tile => tile == -1))
                return true;

            // Whether AvailableCraftingStations contains all recipe.requiredTile
            return recipe.requiredTile.Where(tile => tile != -1).All(tile => AvailableCraftingStations.Contains(tile));
        }

        public void SelectRecipe(int outputSlot, Recipe recipe)
        {
            if (SelectedRecipes == null || SelectedRecipes.Length != OutputSlots)
                SelectedRecipes = new Recipe[OutputSlots];

            if (outputSlot < 0 || outputSlot >= OutputSlots)
                return;

            SelectedRecipes[outputSlot] = recipe;

            int inputsPerOutput = InputSlots / OutputSlots;
            int inputStart = OutputSlots + outputSlot * inputsPerOutput;
            int inputEnd = inputStart + inputsPerOutput;

            for (int i = inputStart; i < inputEnd; i++)
                Inventory.ClearReserved(i);

            if (recipe is null)
                return;

            int index = inputStart;
            foreach (var item in recipe.requiredItem)
            {
                if (item.type <= ItemID.None)
                    continue;

                Inventory.SetReserved(
                    index,
                    item.type,
                    tooltip: null,
                    texture: TextureAssets.Item[item.type],
                    color: Color.White * 0.5f
                );

                index++;
                if (index >= inputEnd)
                    break;
            }
        }


        public override void MachineUpdate()
        {
            if (!PoweredOn || SelectedRecipes.Length == 0)
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

                int inputStart = OutputSlots + outputSlot * (InputSlots / OutputSlots);
                int inputEnd = inputStart + (InputSlots / OutputSlots);

                if (!CanCraftRecipe(recipe, inputStart, inputEnd))
                    continue;

                foreach (var requiredItem in recipe.requiredItem)
                {
                    if (requiredItem.type <= ItemID.None || requiredItem.stack <= 0)
                        continue;

                    int amountToConsume = requiredItem.stack;

                    for (int inputSlot = inputStart; inputSlot < inputEnd; inputSlot++)
                    {
                        if (Inventory[inputSlot].type == requiredItem.type)
                        {
                            int consume = Math.Min(Inventory[inputSlot].stack, amountToConsume);
                            Inventory[inputSlot].DecreaseStack(consume);
                            amountToConsume -= consume;
                            if (amountToConsume <= 0)
                                break;
                        }
                    }
                }

                Item result = recipe.createItem.Clone();
                result.OnCreated(new MachineItemCreationContext(result, this));
                if (!Inventory.TryPlacingItemInSlot(ref result, outputSlot, sound: false, serverSync: true) && result.stack > 0)
                    Item.NewItem(new EntitySource_TileEntity(this), InventoryPosition, result);
            }
        }

        private bool CanCraftRecipe(Recipe recipe, int inputStart, int inputEnd)
        {
            foreach (var requiredItem in recipe.requiredItem)
            {
                if (requiredItem.type <= ItemID.None || requiredItem.stack <= 0)
                    continue;

                int amountNeeded = requiredItem.stack;
                int amountFound = 0;

                for (int i = inputStart; i < inputEnd; i++)
                {
                    if (Inventory[i].type == requiredItem.type)
                        amountFound += Inventory[i].stack;

                    if (amountFound >= amountNeeded)
                        break;
                }

                if (amountFound < amountNeeded)
                    return false;
            }
            return true;
        }

        public override void DrawMachinePowerInfo(SpriteBatch spriteBatch, Vector2 basePosition, Color lightColor)
        {
            //basePosition.X -= 12;
            base.DrawMachinePowerInfo(spriteBatch, basePosition, lightColor);
        }

        public override void NetSend(BinaryWriter writer)
        {
            base.NetSend(writer);
        }

        public override void NetReceive(BinaryReader reader)
        {
            base.NetReceive(reader);
        }

        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);

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
                        recipeTags[i] = new TagCompound(); // empty if no recipe
                    }
                }

                tag[nameof(SelectedRecipes)] = recipeTags;
            }
        }

        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);

            if (tag.TryGet(nameof(SelectedRecipes), out TagCompound[] recipeTags))
            {
                SelectedRecipes = new Recipe[OutputSlots];
                for (int i = 0; i < Math.Min(recipeTags.Length, OutputSlots); i++)
                {
                    var recipeTag = recipeTags[i];
                    if (recipeTag.Count == 0)
                        continue;

                    Item result = ItemIO.Load(recipeTag.Get<TagCompound>(nameof(Recipe.createItem)));
                    List<Item> ingredients = recipeTag.GetList<TagCompound>(nameof(Recipe.requiredItem)).Select(ItemIO.Load).ToList();

                    Recipe matchingRecipe = Main.recipe.FirstOrDefault(r =>
                    {
                        if (r.createItem.type != result.type) return false;
                        IEnumerable<int> recipeIngredients = r.requiredItem.Where(item => item.type > ItemID.None && item.stack > 0).Select(item => item.type);
                        IEnumerable<int> savedIngredients = ingredients.Where(item => item.type > ItemID.None).Select(item => item.type);
                        return recipeIngredients.OrderBy(x => x).SequenceEqual(savedIngredients.OrderBy(x => x));
                    });

                    if (matchingRecipe != null && RecipeAllowed(matchingRecipe))
                        SelectRecipe(i, matchingRecipe);
                }
            }
        }
    }
}
