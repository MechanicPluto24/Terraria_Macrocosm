using Macrocosm.Common.Bases.Items;
using Macrocosm.Common.Bases.Tiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria.ModLoader;

namespace Macrocosm.Common.Loaders;

/// <summary>
/// Automatically loads enemy banners from any "EnemyBanners/" directory.
/// <br/> - Tile texture: "EnemyBanners/EnemyNameBanner.png"
/// <br/> - Item texture: "EnemyBanners/EnemyNameBanner_Item.png"
/// <br/> Banners are automatically assigned to NPCs matching their name in <see cref="Global.NPCs.BannerGlobalNPC"/>.
/// </summary>
public class EnemyBannerLoader : ILoadable
{
    public void Load(Mod mod)
    {
        // Dedicated servers have an empty asset catalogue, but need the same item/tile types.
        // Read packaged filenames instead; no texture assets need to be loaded.
        var textures = (mod.GetFileNames() ?? [])
            .Where(path => Path.GetExtension(path) is ".rawimg" or ".png")
            .Select(path => Path.ChangeExtension(path.Replace('\\', '/'), null))
            .ToHashSet(StringComparer.Ordinal);

        LoadBanners(mod, textures, "EnemyBanners", large: false);
        LoadBanners(mod, textures, "EnemyBannersLarge", large: true);
    }

    private static void LoadBanners(Mod mod, HashSet<string> textures, string directoryName, bool large)
    {
        foreach (string texturePath in textures
            .Where(path => ("/" + path).Contains($"/{directoryName}/", StringComparison.Ordinal))
            .OrderBy(path => path, StringComparer.Ordinal))
        {
            string internalName = Path.GetFileName(texturePath);
            string modTexturePath = $"{mod.Name}/{texturePath}";
            const string itemSuffix = "_Item";
            if (internalName.EndsWith(itemSuffix, StringComparison.Ordinal))
                continue;

            // Load the tile before its matching item; allow tile-only artwork.
            ModTile tile = large
                ? new EnemyBannerLargeTile(modTexturePath, internalName)
                : new EnemyBannerTile(modTexturePath, internalName);
            mod.AddContent(tile);
            if (textures.Contains(texturePath + itemSuffix))
            {
                // Item and tile names have separate registries, the suffix is only for the texture
                var item = new EnemyBannerItem(modTexturePath + itemSuffix, internalName, tile.Type);
                mod.AddContent(item);
                ModTypeLookup<ModItem>.RegisterLegacyNames(item, internalName + itemSuffix, internalName + "Item");
            }
        }
    }

    public void Unload()
    {
    }
}
