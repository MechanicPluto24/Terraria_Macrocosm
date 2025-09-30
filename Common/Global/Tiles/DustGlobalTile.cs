using Macrocosm.Common.Hooks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Macrocosm.Common.Subworlds;

namespace Macrocosm.Common.Global.Tiles;

public class DustGlobalTile : ModSystem
{
    public override void Load()
    {
        On_Player.MakeFloorDust += DustAway;
    }

    public override void Unload()
    {
        On_Player.MakeFloorDust -= DustAway;
    }
    //Though the explosion global tile works well for tiles, it doesn't work for walls :(
    private void DustAway(On_Player.orig_MakeFloorDust orig,Player player, bool Falling, int type, int paintColor)
    {
        if (MacrocosmSubworld.GetGravityMultiplier() == 0f)
            return;
        orig(player,Falling, type, paintColor);
    }
}
