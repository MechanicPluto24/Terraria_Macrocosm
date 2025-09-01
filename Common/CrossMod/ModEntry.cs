using Terraria.ModLoader;

namespace Macrocosm.Common.CrossMod
{
    public readonly record struct ModEntry(string Name)
    {
    	public readonly string name = Name;

    	public readonly bool Enabled
    	{
    		get
    		{
    			if (CrossMod.LoadedMods.ContainsKey(name))
    				return true;

    			if (ModLoader.TryGetMod(name, out var mod))
    			{
                    CrossMod.LoadedMods.Add(name, mod);
    				return true;
    			}

    			return false;
    		}
    	}

    	/// <summary> The mod instance associated with this entry.
    	/// <br/>Should not be used unless you know that this mod is enabled (<see cref="Enabled"/>). </summary>
    	public readonly Mod Instance
    	{
    		get
    		{
    			if (CrossMod.LoadedMods.TryGetValue(name, out var mod))
    				return mod;

    			var getMod = ModLoader.GetMod(name);
                CrossMod.LoadedMods.Add(name, getMod);

    			return getMod;
    		}
    	}

    	/// <inheritdoc cref="Mod.TryFind{T}(string, out T)"/>
    	public readonly bool TryFind<T>(string s, out T t) where T : ModType => ((Mod)this).TryFind(s, out t);
    	public static explicit operator Mod(ModEntry e) => e.Instance;
    }
}
