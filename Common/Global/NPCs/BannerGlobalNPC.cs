using Macrocosm.Common.Bases.NPCs;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Common.Global.NPCs;

public class BannerGlobalNPC : GlobalNPC
{
    public override void SetDefaults(NPC npc)
    {
        if (npc.ModNPC is ModNPC modNPC && modNPC.Mod == Mod)
        {
            ModNPC bannerNPC = modNPC;

            if (modNPC is Worm worm)
                bannerNPC = ModContent.GetModNPC(worm.HeadType);


            int customBanner = Mod.TryFind(bannerNPC.Name + "Banner", out ModItem modItem) ? modItem.Type : -1;
            if (customBanner > 0)
            {
                modNPC.Banner = bannerNPC.Type;
                modNPC.BannerItem = customBanner;
            }
        }
    }
}
