using Macrocosm.Content.Liquids;

namespace Macrocosm.Common.Sets.Items
{
    public interface ILiquidExtractable
    {
        // TODO: replace with ModLiquid.Type when it gets implemented in tML
        public LiquidType LiquidType { get; }

        public float ExtractedAmount { get; }
    }
}
