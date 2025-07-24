using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Fishes;

public class SmogWispfish : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 2;
        ItemID.Sets.CanBePlacedOnWeaponRacks[Type] = true;
    }

    public override void SetDefaults()
    {
        Item.DefaultToQuestFish();
    }

    public override bool IsQuestFish() => true;
    public override bool IsAnglerQuestAvailable() => true;
    public override void AnglerQuestChat(ref string description, ref string catchLocation)
    {
        description = this.GetLocalization("AnglerDescription").Value;
        catchLocation = this.GetLocalization("AnglerLocation").Value;
    }
}
