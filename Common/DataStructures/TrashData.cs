using Macrocosm.Common.Sets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Macrocosm.Common.DataStructures
{
    public enum TrashCategory
    {
        None,
        Item,
        Projectile,
        Gore
    }

    public struct TrashData
    {
        private static WeightedRandom<TrashData> _trashEntityPool;
        public static WeightedRandom<TrashData> RandomPool => _trashEntityPool ??= PrepareTrashEntityPool();

        public bool Valid { get; init; } = false;

        public int Type { get; init; } = -1;
        public float Chance { get; init; } = 0f;
        public int DustType { get; init; } = -1;
        public Color Color { get; init; } = Color.White;

        public TrashCategory Category { get; init; } = TrashCategory.None;

        public int Tilt { get; private set; } = 0;
        public int Offset { get; private set; } = 0;

        public readonly Asset<Texture2D> Texture => Category switch
        {
            TrashCategory.Item => TextureAssets.Item[Type],
            TrashCategory.Projectile => TextureAssets.Projectile[Type],
            TrashCategory.Gore => TextureAssets.Gore[Type],
            _ => Macrocosm.EmptyTex,
        };

        public TrashData(int type, int dustType = -1, float chance = 1f, Color color = default)
        {
            Type = type;
            DustType = dustType;
            Chance = chance;
            Color = color == default ? Color.White : color;

            Valid = true;
        }

        public void Randomize()
        {
            Tilt = Main.rand.Next(-16, 16);
            Offset = Main.rand.Next(0, 90);
        }

        private static WeightedRandom<TrashData> PrepareTrashEntityPool()
        {
            _trashEntityPool = new();
            for (int type = 0; type < ItemLoader.ItemCount; type++)
                if (ItemSets.TrashData.IndexInRange(type) && ItemSets.TrashData[type].Valid)
                    _trashEntityPool.Add(ItemSets.TrashData[type] with { Category = TrashCategory.Item });

            for (int type = 0; type < ProjectileLoader.ProjectileCount; type++)
                if (ProjectileSets.TrashData.IndexInRange(type) && ProjectileSets.TrashData[type].Valid)
                    _trashEntityPool.Add(ProjectileSets.TrashData[type] with { Category = TrashCategory.Projectile });

            for (int type = 0; type < GoreLoader.GoreCount; type++)
                if (GoreSets.TrashData.IndexInRange(type) && GoreSets.TrashData[type].Valid)
                    _trashEntityPool.Add(GoreSets.TrashData[type] with { Category = TrashCategory.Gore });

            return _trashEntityPool;
        }

    }
}
