using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.LiquidContainers;
using Macrocosm.Content.Items.Tech;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets.Modules.Engine
{
    [LegacyName("EngineModule")]
    public class EngineModuleMk2 : BaseEngineModule
    {
        public override int Slot => 3;
        public override int Tier => 2;
        public override ConfigurationType Configuration => ConfigurationType.Any;

        public override int DrawPriority => 0;

        public override int Width => 120;
        public override int Height => 302 + (RearLandingLegRaised ? 18 : 26);
        public override Vector2 Offset => new(78, 268);

        public override AssemblyRecipe Recipe { get; } = new AssemblyRecipe()
        {
            new(ModContent.ItemType<RocketPlating>(), 45),
            new(ModContent.ItemType<Canister>(), 15),
            new(ModContent.ItemType<EngineComponentMk1>(), 4),
            new(ModContent.ItemType<LandingGear>(), 3)
        };
    }
}
