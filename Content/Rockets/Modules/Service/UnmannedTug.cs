using Macrocosm.Common.Sets;
using Macrocosm.Content.Items.Tech;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets.Modules.Service
{
    public class UnmannedTug : RocketModule
    {
        public override SlotType Slot => SlotType.Service;
        public override int Tier => 1;
        public override ConfigurationType Configuration => ConfigurationType.Unmanned;

        public override int DrawPriority => 3;

        public override int Width => 88;
        public override int Height => 114;

        public override Vector2 GetOffset(RocketModule[] modules)
        {
            return new
            (
                x: modules[0..3].Sum(m => m.Width) / 4,
                y: modules[0..0].Sum(m => m.Height)
            );
        }

        public override AssemblyRecipe Recipe { get; } = new AssemblyRecipe()
        {
            new(ModContent.ItemType<RocketPlating>(), 25),
            new(ModContent.ItemType<PowerJunction>(), 3),
            new(ModContent.ItemType<Computer>(), 3)
        };
    }
}
