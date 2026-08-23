namespace Macrocosm.Common.DataStructures;

public readonly struct LiquidExtractData
{
    public int LiquidType { get; init; }

    public float ExtractedAmount { get; init; }

    public LiquidExtractData()
    {
        LiquidType = -1;
        ExtractedAmount = 0f;
    }

    public LiquidExtractData(int liquidType, float extractedAmount)
    {
        LiquidType = liquidType;
        ExtractedAmount = extractedAmount;
    }

    public bool Valid => LiquidType >= 0 && ExtractedAmount > 0f;
}
