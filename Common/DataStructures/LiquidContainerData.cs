using Macrocosm.Content.Liquids;

namespace Macrocosm.Common.DataStructures;

public readonly struct LiquidContainerData
{
    public int LiquidType { get; init; }
    public float Capacity { get; init; }
    public int EmptyContainerType { get; init; }
    public bool Empty { get; init; }

    public LiquidContainerData()
    {
        LiquidType = 0;
        Capacity = 0;
        EmptyContainerType = -1;
        Empty = false;
    }

    public LiquidContainerData(int liquidType, float capacity, int emptyContainerType)
    {
        LiquidType = liquidType;
        Capacity = capacity;
        EmptyContainerType = emptyContainerType;
        Empty = false;
    }

    public static LiquidContainerData CreateEmpty(float capacity)
    {
        return new()
        {
            Empty = true,
            Capacity = capacity
        };
    }

    public static LiquidContainerData CreateInfinite(int liquidType)
    {
        return new()
        {
            LiquidType = liquidType,
            Capacity = int.MaxValue
        };
    }

    public bool Infinite => Capacity == int.MaxValue;
    public bool Valid => Capacity > 0;

    public static int GetEmptyType(LiquidContainerData[] data, int filledType)
    {
        LiquidContainerData filledTypeData = data[filledType];
        if (!filledTypeData.Valid || filledTypeData.Empty)
            return 0;

        return filledTypeData.EmptyContainerType;
    }

    public static int GetFillType(LiquidContainerData[] data, int liquidType, int emptyType)
    {
        LiquidContainerData emptyTypeData = data[emptyType];
        if (!emptyTypeData.Valid || !emptyTypeData.Empty)
            return 0;

        for (int type = 0; type < data.Length; type++)
        {
            LiquidContainerData container = data[type];
            if (container.EmptyContainerType == emptyType && container.LiquidType == liquidType)
            {
                return type;
            }
        }

        return 0;
    }
}