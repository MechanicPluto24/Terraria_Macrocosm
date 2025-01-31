using Macrocosm.Common.Sets;
using Macrocosm.Content.Items.Machines;
using Macrocosm.Content.Items.Tech;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets.Modules.Service
{
    public class ServiceModule : RocketModule
    {
        public override SlotType Slot => SlotType.Service;
        public override int Tier => 1;
        public override ConfigurationType Configuration => ConfigurationType.Manned;

        public override int DrawPriority => 3;

        public override int Width => 88;
        public override int Height => 110;

        public override Vector2 GetOffset(RocketModule[] modules)
        {
            int avgW = modules[0..4].Sum(m => m.Width) / 4;
            return new
            (
                x: (avgW - Width / 2) + 4,
                y: modules[0].Height
            );
        }

        public override AssemblyRecipe Recipe { get; } = new AssemblyRecipe()
        {
            new(ModContent.ItemType<RocketPlating>(), 25),
            new(ModContent.ItemType<PowerJunction>(), 3),
            new(ModContent.ItemType<OxygenSystem>(), 3),
            new((item) => ItemSets.Chest[item.type], Language.GetText("Mods.Macrocosm.UI.LaunchPad.AnyChest"), 1)
        };
    }
}
