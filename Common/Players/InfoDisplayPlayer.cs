using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Common.Players
{
    public class InfoDisplayPlayer : ModPlayer
    {
        public bool Barometer { get; set; }
        public bool GeigerMuller { get; set; }
        public bool Thermometer { get; set; }

        public override void ResetInfoAccessories()
        {
            Barometer = false;
            GeigerMuller = false;
            Thermometer = false;
        }

        public override void RefreshInfoAccessoriesFromTeamPlayers(Player otherPlayer)
        {
            if (otherPlayer.GetModPlayer<InfoDisplayPlayer>().Barometer)
                Barometer = true;

            if (otherPlayer.GetModPlayer<InfoDisplayPlayer>().GeigerMuller)
                GeigerMuller = true;

            if (otherPlayer.GetModPlayer<InfoDisplayPlayer>().Thermometer)
                Thermometer = true;
        }
    }
}
