using System;
using System.Collections;
using System.Collections.Generic;
using Terraria;

namespace Macrocosm.Content.Rockets.Modules;

public class AssemblyRecipe : IEnumerable<AssemblyRecipeEntry>
{
    private readonly List<AssemblyRecipeEntry> entries;

    public RocketModule LinkedResult { get; private set; }
    public bool Linked => LinkedResult is not null;

    public AssemblyRecipe()
    {
        entries = new();
    }

    public AssemblyRecipe(params AssemblyRecipeEntry[] entries) : this()
    {
        this.entries = [.. entries];
    }

    public AssemblyRecipeEntry this[int index] => entries[index];

    public void Add(AssemblyRecipeEntry entry)
    {
        entries.Add(entry);
    }

    public void Add(int itemType, int requiredAmount = 1)
    {
        entries.Add(new(itemType, requiredAmount));
    }

    public void AddRange(params AssemblyRecipeEntry[] entries)
    {
        this.entries.AddRange(entries);
    }

    // TODO: add check regardless of order
    public bool Check(bool consume, params Item[] items)
    {
        if (Linked)
            return LinkedResult.Recipe.Check(consume, items);

        int count = Math.Min(entries.Count, items.Length);
        bool met = true;
        for (int i = 0; i < count; i++)
        {
            AssemblyRecipeEntry recipeEntry = entries[i];
            Item item = items[i];

            met &= recipeEntry.Check(item, consume);
        }

        return met;
    }

    public AssemblyRecipe LinkWith<T>() where T : RocketModule
    {
        foreach (var template in RocketModule.Templates)
        {
            if (typeof(T) == template.GetType())
            {
                AddRange(template.Recipe.entries.ToArray());
                LinkedResult = template;
            }
        }

        return this;
    }

    public AssemblyRecipe Clone()
    {
        var newRecipe = new AssemblyRecipe();
        foreach (var entry in entries)
        {
            if (entry.ItemType.HasValue)
                newRecipe.Add(new(entry.ItemType.Value, entry.RequiredAmount));
            else
                newRecipe.Add(new(entry.ItemCheck, entry.Description, entry.RequiredAmount));
        }

        return newRecipe;
    }

    public IEnumerator GetEnumerator()
    {
        return ((IEnumerable)entries).GetEnumerator();
    }

    IEnumerator<AssemblyRecipeEntry> IEnumerable<AssemblyRecipeEntry>.GetEnumerator()
    {
        return ((IEnumerable<AssemblyRecipeEntry>)entries).GetEnumerator();
    }
}
