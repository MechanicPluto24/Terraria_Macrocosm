using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Items.Weapons      //We need this to basically indicate the folder where it is to be read from, so you the texture will load correctly
{
    public class ReaperEX : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Pluto's mighty reaper scythe.");
        }
        
        public override void SetDefaults()
        {
            item.damage = 15000;
            item.melee = true;
            item.width = 80;
            item.height = 80;
            item.useTime = 7;
            item.useAnimation = 10;
            item.channel = true;
            item.useStyle = 100;
            item.knockBack = 8f;
            item.value = Item.buyPrice(0, 10, 0, 0);
            item.rare = 11;       
            item.shoot = mod.ProjectileType("ReaperEXProjectile");
            item.noUseGraphic = true;
        }

        public override bool UseItemFrame(Player player)
        {
            player.bodyFrame.Y = 3 * player.bodyFrame.Height;
            return true;
        }
    }
}