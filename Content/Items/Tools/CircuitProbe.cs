﻿using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Refined;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Tools
{
    [LegacyName("WiringKit")]
    public class CircuitProbe : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 1;
            Item.value = Item.buyPrice(gold: 1);
            Item.useStyle = ItemUseStyleID.RaiseLamp;
            Item.useTurn = true;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.mech = true;
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer && Utility.TryGetTileEntityAs(Player.tileTargetX, Player.tileTargetY, out MachineTE machine))
            {
                if (machine.MachineType is MachineType.Generator or MachineType.Battery)
                    Main.NewText(machine.GetMachineNameAndStatusInfo(), Color.LightGreen);
                else
                    Main.NewText(machine.GetMachineNameAndStatusInfo(), Color.Orange);

                return true;
            }

            return null;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Wire, 50)
                .AddIngredient<Plastic>(5)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}
