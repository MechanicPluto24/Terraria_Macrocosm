using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Common.Players;

public class InfoDisplayPlayer : ModPlayer
{
    public bool Barometer { get; set; }
    public bool GeigerMuller { get; set; }
    public bool Thermometer { get; set; }
    public bool PollutionMeter { get; set; }

    public override void ResetInfoAccessories()
    {
        Barometer = false;
        GeigerMuller = false;
        Thermometer = false;
        PollutionMeter = false;
    }

    public override void RefreshInfoAccessoriesFromTeamPlayers(Player otherPlayer)
    {
        if (otherPlayer.GetModPlayer<InfoDisplayPlayer>().Barometer)
            Barometer = true;

        if (otherPlayer.GetModPlayer<InfoDisplayPlayer>().GeigerMuller)
            GeigerMuller = true;

        if (otherPlayer.GetModPlayer<InfoDisplayPlayer>().Thermometer)
            Thermometer = true;

        if (otherPlayer.GetModPlayer<InfoDisplayPlayer>().PollutionMeter)
            PollutionMeter = true;
    }
}
