using Macrocosm.Common.Enums;
using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Machines
{
    public class SolarPanelTileTE : GeneratorTE
    {
        public override MachineTile MachineTile => ModContent.GetInstance<SolarPanelTile>();

        public override bool PoweredOn => Main.dayTime;
        public override bool CanCluster => true;

        public override void MachineUpdate()
        {
            MaxGeneratedPower = 0.1f * ClusterSize;
            GeneratedPower = PoweredOn ? MaxGeneratedPower : 0;
        }
    }
}
