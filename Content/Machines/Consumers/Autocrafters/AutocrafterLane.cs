using Macrocosm.Common.Storage;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace Macrocosm.Content.Machines.Consumers.Autocrafters;

public sealed class AutocrafterLane
{
    public int Index { get; }
    public int Revision { get; private set; }
    public Recipe SelectedRecipe { get; private set; }
    public Inventory InputInventory { get; private set; }

    public AutocrafterLane(int index, IInventoryOwner owner)
    {
        Index = index;
        InputInventory = new Inventory(0, owner);
    }

    public void Configure(Recipe recipe, IInventoryOwner owner)
    {
        int interactingPlayer = InputInventory?.InteractingPlayer ?? Main.maxPlayers;
        Revision++;
        SelectedRecipe = recipe;
        var requirements = GetRequirements(recipe).ToArray();
        InputInventory = new Inventory(requirements.Length, owner);

        for (int slot = 0; slot < requirements.Length; slot++)
        {
            var requirement = requirements[slot];
            InputInventory.SetSlotRole(slot, InventorySlotRole.Input);
            InputInventory.SetReserved(slot, requirement.Type, texture: TextureAssets.Item[requirement.Type], color: Color.White, stack: requirement.Stack);
        }

        InputInventory.SetInteractingPlayer(interactingPlayer, sync: false);
    }

    public void RestoreInventory(Inventory saved, IInventoryOwner owner)
    {
        if (saved is null)
            return;

        int count = System.Math.Min(saved.Size, InputInventory.Size);
        for (int i = 0; i < count; i++)
            InputInventory[i] = saved[i].Clone();

        InputInventory.Owner = owner;
    }

    public void SetRevision(int revision) => Revision = revision;

    public static IEnumerable<(int Type, int Stack)> GetRequirements(Recipe recipe)
        => recipe?.requiredItem
            .Where(item => item.type > ItemID.None && item.stack > 0)
            .GroupBy(item => item.type)
            .Select(group => (Type: group.Key, Stack: group.Sum(item => item.stack)))
            ?? Enumerable.Empty<(int Type, int Stack)>();
}
