using Terraria.ModLoader;
using Terraria;
using Macrocosm.Backgrounds;
using Terraria.Graphics.Effects;
using Macrocosm.Content.Items.Currency;
using Microsoft.Xna.Framework;
using Macrocosm.Content;
using SubworldLibrary;
using Macrocosm.Common;
using Macrocosm.Content.Subworlds;
using System;

namespace Macrocosm
{
    public class Macrocosm : Mod
    {
        public static Mod Instance => ModContent.GetInstance<Macrocosm>();
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
			
			var ta = ModLoader.GetMod("TerrariaAmbience");
            var taAPI = ModLoader.GetMod("TerrariaAmbienceAPI");
            ta?.Call("AddTilesToList", this, "Stone", new string[] { }, null);
            // mod, path, name, maxVol, volStep, playWhen, actionInit, actionUpdate, SNQAction
            taAPI?.Call(this, "Sounds/Ambient/Moon", "MoonAmbience", 1f, 0.0075f, new Func<bool>(Subworld.IsActive<Moon>), null, null, null);
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
                if (!Main.dayTime && Main.player[Main.myPlayer].GetModPlayer<MacrocosmPlayer>().ZoneMoon)
                {
                    music = GetSoundSlot(SoundType.Music, "Sounds/Music/MoonNight");

                    priority = MusicPriority.Environment;
                }
                if (Main.player[Main.myPlayer].GetModPlayer<MacrocosmPlayer>().ZoneMoon && Main.dayTime)
                {
                    music = GetSoundSlot(SoundType.Music, "Sounds/Music/MoonDay");

                    priority = MusicPriority.Environment;
                }
            }
        }
    }
}