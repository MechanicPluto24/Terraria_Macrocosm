using Macrocosm.Common.Bases;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Materials.Tech;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets.Modules
{
    public class ServiceModule : RocketModule
    {
        public override int DrawPriority => 3;

        public override int Width => 80;
        public override int Height => 110;

        public override Rectangle Hitbox => base.Hitbox with { Y = base.Hitbox.Y + 2 };

        public override AssemblyRecipe Recipe { get; } = new AssemblyRecipe()
        {
            new(ModContent.ItemType<RocketPlating>(), 25),
            new(ModContent.ItemType<PowerJunction>(), 3),
            new(ModContent.ItemType<OxygenSystem>(), 3),
            new((item) => ItemSets.Chests[item.type], Language.GetText("Mods.Macrocosm.UI.LaunchPad.AnyChest"), 1) 
        };
    }
}
