using SubworldLibrary;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Global.Items
{
    public class SeedGlobalItem : GlobalItem
    {
    public override bool CanUseItem(Item item, Player player)
    {   
        if (SubworldSystem.AnyActive<Macrocosm>()){
        if(item.type==62||item.type==60||item.type==70||item.type==59||item.type==199||item.type==369||item.type==5214)//There is no seed item set. Or maybe there is?
            return false;
        if(item.type==27||item.type==4871||item.type==4907||item.type==307||item.type==309||item.type==308||item.type==311||item.type==2357||item.type==310||item.type==312||item.type==1828)
            return false;
        if(item.type==4041||item.type==4042||item.type==4043||item.type==4044||item.type==4045||item.type==4046||item.type==4047||item.type==4048||item.type==4241)
            return false;
        }
        
        return true;
    }

       
    }
}