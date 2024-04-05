using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Loot;
using Macrocosm.Common.Loot.DropConditions;
using Macrocosm.Common.Loot.DropRules;
using Macrocosm.Common.Storage;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Materials.Ores;
using Macrocosm.Content.Items.Placeable.Blocks;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Content.Machines
{
    public class SolarPanelLargeTE : MachineTE
    {
        public override MachineTile MachineTile => ModContent.GetInstance<SolarPanelLarge>();
        public override bool Operating => Main.dayTime;

        public override void OnFirstUpdate()
        {
        }

        public override void MachineUpdate()
        {
            if(Operating)
                Power = 1f;
        }
    }
}
