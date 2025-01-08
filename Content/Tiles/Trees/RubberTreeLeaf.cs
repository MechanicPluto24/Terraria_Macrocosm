using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Tiles.Trees
{
    public class RubberTreeLeaf : ModGore
    {
        public override void SetStaticDefaults()
        {
            ChildSafety.SafeGore[Type] = true; // Leaf gore should appear regardless of the "Blood and Gore" setting
            GoreID.Sets.SpecialAI[Type] = 3; // Falling leaf behavior
            GoreID.Sets.PaintedFallingLeaf[Type] = true; // This is used for all vanilla tree leaves, related to the bigger spritesheet for tile paints
        }
    }
}
