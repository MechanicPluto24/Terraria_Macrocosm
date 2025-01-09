using Macrocosm.Common.Sets;
using Macrocosm.Content.Items.Machines;
using Macrocosm.Content.Items.Tech;
using Microsoft.Xna.Framework;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets.Modules
{
    public class ServiceModule : RocketModule
    {
        public override int DrawPriority => 3;

        public override int Width => 80;
        public override int Height => 110;

        public override Vector2 Offset => new(98, 80);

        public override AssemblyRecipe Recipe { get; } = new AssemblyRecipe()
        {
            new(ModContent.ItemType<RocketPlating>(), 25),
            new(ModContent.ItemType<PowerJunction>(), 3),
            new(ModContent.ItemType<OxygenSystem>(), 3),
            new((item) => ItemSets.Chest[item.type], Language.GetText("Mods.Macrocosm.UI.LaunchPad.AnyChest"), 1)
        };
    }
}
