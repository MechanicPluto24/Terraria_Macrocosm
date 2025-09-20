using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Consumables.Unlockables;

public abstract class PatternDesign : ModItem
{
    public abstract string PatternName { get; }

    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 5;
    }

    public override void SetDefaults()
    {
        Item.width = 30;
        Item.height = 20;
    }
}
