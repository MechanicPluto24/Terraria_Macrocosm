using Macrocosm.Content.Items.Tech;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Linq;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets.Modules.Utilitary
{
    public class StructureModule : RocketModule
    {
        public override SlotType Slot => SlotType.Utilitary;
        public override int Tier => 1;
        public override ConfigurationType Configuration => ConfigurationType.Any;

        public override int DrawPriority => 2;

        public override int Width => 72;
        public override int Height => 78;

        public override Vector2 GetOffset(RocketModule[] modules)
        {
            int maxW = modules[0..4].Max(m => m.Width);
            return new
            (
                x: ((maxW - Width) / 2),
                y: modules[0..2].Sum(m => m.Height)
            );
        }

        public override AssemblyRecipe Recipe { get; } = new AssemblyRecipe()
        {
            new(ModContent.ItemType<RocketPlating>(), 10)
        };
    }
}
