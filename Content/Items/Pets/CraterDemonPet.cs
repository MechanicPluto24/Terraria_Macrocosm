using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Pets;

public class CraterDemonPet : ModItem
{
    public override void SetDefaults()
    {
        Item.damage = 0;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.width = 15;
        Item.height = 15;
        Item.UseSound = SoundID.Item2;
        Item.useAnimation = 20;
        Item.useTime = 20;
        Item.rare = ModContent.RarityType<MoonRarity2>();
        Item.noMelee = true;
        Item.shoot = ModContent.ProjectileType<Projectiles.Friendly.Pets.CraterDemonPet>();
        Item.buffType = ModContent.BuffType<Buffs.Pets.CraterDemonPet>();
    }

    public override void UseStyle(Player player, Rectangle heldItemFrame)
    {
        if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
        {
            player.AddBuff(Item.buffType, 3600);
        }
    }
}
