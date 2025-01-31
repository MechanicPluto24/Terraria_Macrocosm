using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.LiquidContainers;
using Macrocosm.Content.Items.Tech;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets.Modules.Engine
{
    public class EngineModuleMk1 : BaseEngineModule
    {
        public override SlotType Slot => SlotType.Engine;
        public override int Tier => 2;
        public override ConfigurationType Configuration => ConfigurationType.Any;

        public override int DrawPriority => 0;

        public override int Width => 80;
        public override int Height => 268;

        public override Vector2 GetOffset(RocketModule[] modules)
        {
            int avgW = modules[0..4].Sum(m => m.Width) / 4;
            return new
            (
                x: (avgW - Width / 2) + 4,
                y: modules[0..3].Sum(m => m.Height)
            );
        }

        public override AssemblyRecipe Recipe { get; } = new AssemblyRecipe()
        {
            new(ModContent.ItemType<RocketPlating>(), 45),
            new(ModContent.ItemType<Canister>(), 15),
            new(ModContent.ItemType<EngineComponentMk1>(), 4),
            new(ModContent.ItemType<LandingGear>(), 3)
        };
    }
}
