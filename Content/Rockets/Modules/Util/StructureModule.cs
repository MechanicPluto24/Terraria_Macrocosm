using Macrocosm.Content.Items.Tech;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets.Modules.Util
{
    public class StructureModule : RocketModule
    {
        public override int Slot => 2;
        public override int Tier => 1;
        public override ConfigurationType Configuration => ConfigurationType.Any;

        public override int DrawPriority => 2;

        public override int Width => 76;
        public override int Height => 80;

        public override Vector2 Offset => new(102, 190);

        public override AssemblyRecipe Recipe { get; } = new AssemblyRecipe()
        {
            new(ModContent.ItemType<RocketPlating>(), 10)
        };
    }
}
