using Macrocosm.Content.Liquids;

namespace Macrocosm.Common.DataStructures
{
    public readonly struct LiquidExtractData
    {
        public LiquidType LiquidType { get; init; }

        public float ExtractedAmount { get; init; }

        public LiquidExtractData()
        {
            LiquidType = LiquidType.None;
        }

        public LiquidExtractData(LiquidType liquidType, float extractedAmount)
        {
            LiquidType = liquidType;
            ExtractedAmount = extractedAmount;
        }

        public bool Valid => LiquidType >= 0;
    }
}
