using TerrariaAmbienceAPI.Common;
using Terraria;
using Terraria.ModLoader;
using SubworldLibrary;
using Macrocosm.Content.Subworlds;

namespace Macrocosm.Content.AmbienceTracks
{
    public class MoonAmbienceTrack : ModAmbience
    {
        public override string Name => "MoonAmbience";

        public override float MaxVolume => 1f;
        public override float VolumeStep => base.VolumeStep;
        public override bool WhenToPlay => Subworld.IsActive<Moon>();
        public override void Initialize()
        {
            Sound = mod.GetSound("Sounds/Ambient/Moon");
            SoundInstance = Sound.CreateInstance();
        }
        public override void UpdateActive()
        {
            int worldBottom = Main.maxTilesY;
            var player = Main.LocalPlayer;
            var distance_worldBottom_playerCenter = worldBottom - (int)player.Center.Y;

            // volume = distance_worldBottom_playerCenter / worldBottom;
            Main.NewText(distance_worldBottom_playerCenter / worldBottom);
        }
    }
}