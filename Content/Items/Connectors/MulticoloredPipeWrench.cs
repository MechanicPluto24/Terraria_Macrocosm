using Macrocosm.Common.Players;
using Macrocosm.Common.Systems.Connectors;
using Macrocosm.Common.Systems.UI;
using Macrocosm.Content.Projectiles.Tools;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Connectors;

public class MulticoloredPipeWrench : ModItem
{
    public virtual bool IsTablet => false;
    public override void SetDefaults()
    {
        Item.width = IsTablet ? 46 : 44;
        Item.height = IsTablet ? 26 : 46;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.useTime = Item.useAnimation = 10;
        Item.autoReuse = !IsTablet;
        Item.channel = IsTablet;
        Item.mech = true;
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.buyPrice(gold: IsTablet ? 2 : 1);
    }

    public override bool CanUseItem(Player player)
        => !player.mouseInterface && !PipeToolUI.BlockUse && !PipeToolUI.IsBlocked(player)
        && (!IsTablet || player.ownedProjectileCounts[ModContent.ProjectileType<PipeToolDrag>()] == 0)
        && (IsTablet || PipeSystem.InToolReach(player, new Point(Player.tileTargetX, Player.tileTargetY)));

    public override bool? UseItem(Player player)
    {
        if (player.whoAmI != Main.myPlayer) return null;
        // An animation already in progress can reach UseItem after a menu opens.
        if (PipeToolUI.BlockUse || PipeToolUI.IsBlocked(player) || player.mouseInterface) return false;
        var settings = player.GetModPlayer<PipeToolPlayer>();
        var state = IsTablet ? settings.Tablet : settings.Wrench;
        Point target = new(Player.tileTargetX, Player.tileTargetY);
        if (IsTablet && state.Component == PipeToolComponent.Pipes)
        {
            if (state.Colors == 0) return false;
            int index = Projectile.NewProjectile(player.GetSource_ItemUse(Item), target.ToVector2() * 16 + new Vector2(8), Vector2.Zero, ModContent.ProjectileType<PipeToolDrag>(), 0, 0, player.whoAmI);
            ((PipeToolDrag)Main.projectile[index].ModProjectile).Initialize(target, state);
        }
        else PipeSystem.RequestToolOperation(player, target, target, state, player.direction == 1);
        return true;
    }

    public override void AddRecipes()
    {
        CreateRecipe().AddIngredient<PipeWrenchRed>().AddIngredient<PipeWrenchGreen>()
            .AddIngredient<PipeWrenchBlue>().AddIngredient<PipeWrenchYellow>().AddIngredient<PipeCutter>()
            .AddTile(TileID.TinkerersWorkbench).Register();
    }
}
