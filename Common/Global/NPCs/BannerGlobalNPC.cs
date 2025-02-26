using Macrocosm.Content.Tiles.Banners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Common.Global.NPCs
{
    public class BannerGlobalNPC : GlobalNPC
    {
        public override void SetDefaults(NPC npc)
        {
            if (npc.ModNPC is ModNPC modNPC && modNPC.Mod == Mod)
            {
                int customBanner = Mod.TryFind(modNPC.Name + "BannerItem", out ModItem modItem) ? modItem.Type : -1;
                if (customBanner > 0) 
                {
                    modNPC.Banner = modNPC.Type;
                    modNPC.BannerItem = customBanner;
                }
            }
        }
    }
}
