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

        public override int Width => 72;
        public override int Height => 268 + 20;

        public override Vector2 GetOffset(RocketModule[] modules)
        {
            int maxW = modules[0..4].Max(m => m.Width);
            return new
            (
                x: ((maxW - Width) / 2),
                y: modules[0..3].Sum(m => m.Height)
            );
        }

        public override AssemblyRecipe Recipe { get; } = new AssemblyRecipe()
        {
            new(ModContent.ItemType<RocketPlating>(), 35),
            new(ModContent.ItemType<Canister>(), 10),
            new(ModContent.ItemType<EngineComponentMk1>(), 1)
        };
    }
}
