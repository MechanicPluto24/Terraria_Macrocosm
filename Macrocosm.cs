using Terraria.ModLoader;
using Macrocosm.Items;
using SubworldLibrary;
using Terraria;
using Macrocosm;
using Terraria.GameContent.UI;
using Macrocosm.Backgrounds;
using Terraria.Graphics.Effects;
using Macrocosm.Items.Currency;

namespace Macrocosm
{
    public class Macrocosm : Mod
    {
        public static Macrocosm instance => ModContent.GetInstance<Macrocosm>();
        public Macrocosm()
        {
            Properties = new ModProperties()
            {
                Autoload = true,
                AutoloadGores = true,
                AutoloadSounds = true,
                AutoloadBackgrounds = true
            };
        }
        public override void Load()
        {
            if (!Main.dedServ)
            {
                LoadClient();
            }
            CurrencyManager.LoadCurrencies();
        }
        public void LoadClient()
        {
            MoonSky moonSky = new MoonSky();
            Filters.Scene["Macrocosm:MoonSky"] = new Filter(new MoonSkyData("FilterMiniTower").UseColor(0f, 0f, 0f).UseOpacity(0f), EffectPriority.High);
            SkyManager.Instance["Macrocosm:MoonSky"] = moonSky;
        }
        public override void UpdateMusic(ref int music, ref MusicPriority priority)
        {
            if (Main.gameMenu)
                return;
            if (priority > MusicPriority.Environment)
                return;
            Player player = Main.LocalPlayer;
            if (!player.active)
                return;

            if (Main.myPlayer != -1 && !Main.gameMenu)
            {
                if (Main.player[Main.myPlayer].GetModPlayer<MacrocosmPlayer>().ZoneMoon)
                {
                    music = GetSoundSlot(SoundType.Music, "Sounds/Music/Moon");

                    priority = MusicPriority.Environment;
                }
            }
        }
    }
}