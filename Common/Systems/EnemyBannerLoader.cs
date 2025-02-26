using Macrocosm.Common.Bases.Items;
using Macrocosm.Common.Bases.Tiles;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Linq;
using Terraria.ModLoader;

namespace Macrocosm.Common.Systems
{
    /// <summary>
    /// Automatically loads enemy banners from any "EnemyBanners/" directory.
    /// <br/> - Tile texture: "EnemyBanners/EnemyNameBanner.png"
    /// <br/> - Item texture: "EnemyBanners/EnemyNameBannerItem.png"
    /// <br/> Banners are automatically assigned to NPCs matching their name in <see cref="Global.NPCs.BannerGlobalNPC"/>.
    /// </summary>
    public class EnemyBannerLoader : ILoadable
    {
        public void Load(Mod mod)
        {
            foreach (string fullTexturePath in mod.RootContentSource.EnumerateAssets().Where(t => t.Contains("EnemyBanners/")))
            {
                string texturePath = Path.ChangeExtension(fullTexturePath, null);
                string internalName = Path.GetFileName(texturePath);
                string modTexturePath = $"{mod.Name}/{texturePath}";

                // Load in pairs, to ensure the tile is loaded first
                string itemSuffix = "Item";
                if (!internalName.EndsWith(itemSuffix))
                {
                    var tile = new EnemyBannerTile(modTexturePath, internalName);
                    mod.AddContent(tile); // tile.Type is assigned here
                    if (mod.HasAsset(texturePath + itemSuffix))
                        mod.AddContent(new EnemyBannerItem(modTexturePath + itemSuffix, internalName + itemSuffix, tile.Type));
                }
            }
        }

        public void Unload()
        {
        }
    }
}
