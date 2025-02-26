using System.Runtime.CompilerServices;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;

namespace Macrocosm.Common.Bases.Tiles
{
    public class EnemyBannerTile : ModBannerTile
    {
        private readonly string name;
        private readonly string texture;
        public override string Name => name;
        public override string Texture => texture;

        public EnemyBannerTile(string texture, string name)
        {
            this.texture = texture;
            this.name = name;
        }
    }
}
