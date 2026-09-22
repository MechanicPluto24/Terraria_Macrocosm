using Terraria.ID;

namespace Macrocosm.Content.Items.Connectors;

public class PipeGrandDesign : MulticoloredPipeWrench
{
    public override bool IsTablet => true;
    public override void AddRecipes()
        => CreateRecipe().AddIngredient<MulticoloredPipeWrench>().AddIngredient<ConveyorAttachmentTool>()
            .AddTile(TileID.TinkerersWorkbench).Register();
}
