using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Systems.Music
{
    public class ZoneMusicSystem
    {
        // TODO: Zone classes for behavior organization
        public void UpdateMusic(MacrocosmPlayer player, ref int music, ref MusicPriority priority)
        {
            if (player.ZoneMoon)
            {
                var musicPath = "Sounds/Music/MoonNight";
                if (Main.dayTime) 
                    musicPath = "Sounds/Music/MoonDay";
                music = Macrocosm.Instance.GetSoundSlot(SoundType.Music, musicPath);
                priority = MusicPriority.Environment;
            }
        }
    }
}
