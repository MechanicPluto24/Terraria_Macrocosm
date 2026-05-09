using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Gores;

public class PollutionWaterDropletGore : ModGore
{
    public override void SetStaticDefaults()
    {
        ChildSafety.SafeGore[Type] = true;
        GoreID.Sets.LiquidDroplet[Type] = true;
        UpdateType = GoreID.WaterDrip;
    }
}

public class PollutionWaterDropletGore2 : PollutionWaterDropletGore
{
}

public class PollutionWaterDropletGore3 : PollutionWaterDropletGore
{
}

public class PollutionWaterDropletGore4 : PollutionWaterDropletGore
{
}

public class PollutionWaterDropletGore5 : PollutionWaterDropletGore
{
}
