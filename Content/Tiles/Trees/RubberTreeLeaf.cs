using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Tiles.Trees
{
    public class RubberTreeLeaf : ModGore
    {
        public override void SetStaticDefaults()
        {
            ChildSafety.SafeGore[Type] = true;
            GoreID.Sets.SpecialAI[Type] = 3;
            GoreID.Sets.PaintedFallingLeaf[Type] = true;
        }
    }
}
