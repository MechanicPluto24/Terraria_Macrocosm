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
            On.Terraria.UI.ItemSlot.PickItemMovementAction += ItemSlot_PickItemMovementAction;
            if (!Main.dedServ)
            {
                LoadClient();
            }
            CurrencyManager.LoadCurrencies();
        }

        private int ItemSlot_PickItemMovementAction(On.Terraria.UI.ItemSlot.orig_PickItemMovementAction orig, Item[] inv, int context, int slot, Item checkItem)
        {
            if (context == 1 && checkItem.type == ModContent.ItemType<UnuCredit>())
            {
                return 0;
            }
            else
            {
                return orig(inv, context, slot, checkItem);
            }
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