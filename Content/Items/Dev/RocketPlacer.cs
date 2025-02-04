using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets;
using Macrocosm.Content.Rockets.Modules;
using Macrocosm.Content.Rockets.Modules.Service;
using Macrocosm.Content.Rockets.Modules.Top;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Dev
{
    class RocketPlacer : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemSets.DeveloperItem[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 38;
            Item.maxStack = 1;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item6;
        }

        public override bool AltFunctionUse(Player player) => true;

        private int moduleConfig;
        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.AltFunction())
                {
                    bool despawned = false;
                    foreach (var rocket in RocketManager.Rockets.Where(r => r.Active))
                    {
                        if (rocket.Bounds.Contains(Main.MouseWorld.ToPoint()))
                        {
                            rocket.Despawn();
                            despawned = true;
                            break;
                        }
                    }

                    if (!despawned)
                    { 
                        moduleConfig++;
                        if (moduleConfig > 2)
                            moduleConfig = 0;

                        switch (moduleConfig)
                        {
                            case 0:
                                Main.NewText("Selected tier 2 configuration");
                                break;
                            case 1:
                                Main.NewText("Selected tier 1 configuration");
                                break;
                            case 2:
                                Main.NewText("Selected tier 1 unmanned configuration");
                                break;
                            default:
                                break;
                        }
                    }
                }
                else
                {
                    RocketModule[] modules = null;
                    switch (moduleConfig)
                    {
                        case 0:
                            modules = RocketModule.DefaultLegacyModules.ToArray();
                            break;
                        case 1:
                            modules = RocketModule.DefaultModules.ToArray();
                            break;
                        case 2:
                            modules = RocketModule.DefaultModules.ToArray();
                            modules[0] = RocketModule.Templates[ModContent.GetInstance<PayloadPod>().Type];
                            modules[1] = RocketModule.Templates[ModContent.GetInstance<UnmannedTug>().Type];
                            break;
                        default:
                            break;
                    }

                    var rocket = Rocket.Create(Main.MouseWorld, modules, sync: true, action: (r) =>
                    {
                        r.Fuel = r.FuelCapacity;
                    }
                    );
                    rocket.Position -= rocket.Size / 2f;
                }
            }

            return true;
        }
    }
}
