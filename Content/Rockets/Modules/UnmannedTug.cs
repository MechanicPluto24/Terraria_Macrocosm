using Macrocosm.Common.Sets;
using Macrocosm.Content.Items.Tech;
using Microsoft.Xna.Framework;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets.Modules
{
    public class UnmannedTug : RocketModule
    {
        public override int Slot => 0;
        public override int Tier => 1;
        public override ConfigurationType Configuration => ConfigurationType.Unmanned;

        public override int DrawPriority => 3;

        public override int Width => 88;
        public override int Height => 114;

        public override Vector2 Offset => new(94, 76);

        public override AssemblyRecipe Recipe { get; } = new AssemblyRecipe()
        {
            new(ModContent.ItemType<RocketPlating>(), 25),
            new(ModContent.ItemType<PowerJunction>(), 3),
            new(ModContent.ItemType<Computer>(), 3)
        };
    }
}
