using Macrocosm.Common.Bases.Projectiles;
using Macrocosm.Common.CrossMod;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Bars;
using Macrocosm.Content.Particles;
using Macrocosm.Content.Projectiles.Friendly.Melee;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Melee;

public class MegaBlade : ModItem
{
    public override void SetStaticDefaults()
    {
    }
    public override void SetDefaults()
    {
        Item.damage = 400;
        Item.DamageType = DamageClass.Melee;
        Item.width = 84;
        Item.height = 84;
        Item.useTime = 30;
        Item.useAnimation = 30;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.noMelee = true;
        Item.shootSpeed = 25f;
        Item.knockBack = 10;
        Item.value = Item.sellPrice(0, 20, 0, 0);
        Item.rare = ModContent.RarityType<MoonRarity2>();
        Item.UseSound = SoundID.Item1;
        Item.autoReuse = true;
        Item.noUseGraphic = true;
        Item.shoot = ModContent.ProjectileType<MegaBladeProjectile>();
        Item.channel = true;

        Redemption.SetSlashBonus(Item);
    }


}