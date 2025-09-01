using SubworldLibrary;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Hooks;

public class ItemDespawnHooks : ILoadable
{
    public void Load(Mod mod)
    {
        On_Item.DespawnIfMeetingConditions += On_Item_DespawnIfMeetingConditions;
    }

    public void Unload()
    {
        On_Item.DespawnIfMeetingConditions -= On_Item_DespawnIfMeetingConditions;
    }

    private void On_Item_DespawnIfMeetingConditions(On_Item.orig_DespawnIfMeetingConditions orig, Item self, int i)
    {
        // Don't despawn fallen stars on our subworlds
        if (self.type == ItemID.FallenStar && SubworldSystem.AnyActive<Macrocosm>())
            return;

        orig(self, i);
    }
}
