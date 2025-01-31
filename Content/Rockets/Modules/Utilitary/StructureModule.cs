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

        public override int Width => 80;
        public override int Height => 78;

        public override Vector2 GetOffset(RocketModule[] modules)
        {
            int avgW = modules[0..4].Sum(m => m.Width) / 4;
            return new
            (
                x: (avgW - Width / 2) + 4,
                y: modules[0..2].Sum(m => m.Height)
            );
        }

        public override AssemblyRecipe Recipe { get; } = new AssemblyRecipe()
        {
            new(ModContent.ItemType<RocketPlating>(), 10)
        };
    }
}
