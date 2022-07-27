using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Systems.Music {
    public class ZoneMusicSystem {
        // TODO: Zone classes for behavior organization
        public void UpdateMusic(MacrocosmPlayer player, ref int music, ref SceneEffectPriority priority) {
            if (player.ZoneMoon) {
                var musicPath = "Sounds/Music/MoonNight";
                if (Main.dayTime)
                    musicPath = "Sounds/Music/MoonDay";
                music = MusicLoader.GetMusicSlot(Macrocosm.Instance, musicPath);
                priority = SceneEffectPriority.Environment;
            }
        }
    }
}
