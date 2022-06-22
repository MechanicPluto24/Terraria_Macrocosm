using Terraria.ModLoader;
using Terraria;
using Macrocosm.Backgrounds;
using Terraria.Graphics.Effects;
using Macrocosm.Content.Items.Currency;
using System;
using SubworldLibrary;
using Macrocosm.Content.Subworlds.Moon;
using Terraria.Graphics.Shaders;
using Macrocosm.Content.Systems.Music;

namespace Macrocosm {
    public class Macrocosm : Mod {
        public static Mod Instance => ModContent.GetInstance<Macrocosm>();
        public override void Load() {
            Content.NPCs.GlobalNPCs.LowGravityNPC.DetourNPCGravity();
            Common.Drawing.EarthDrawing.InitializeDetour();
            Common.Drawing.RemoveBackgroundAmbient.InitializeDetour();
            On.Terraria.UI.ItemSlot.PickItemMovementAction += MoonCoin_AllowCoinSlotPlacement;
            CurrencyManager.LoadCurrencies();
            if (!Main.dedServ)
                LoadMoonSky();
            
            try
            {
                var ta = ModLoader.GetMod("TerrariaAmbience");
                var taAPI = ModLoader.GetMod("TerrariaAmbienceAPI");
                ta?.Call("AddTilesToList", this, "Stone", new string[] { "Regolith", "RegolithBrick", "Hemostone" }, null); // ech
                taAPI?.Call(this, "Sounds/Ambient/Moon", "MoonAmbience", 1f, 0.0075f, new Func<bool>(SubworldSystem.IsActive<Moon>));
            }
            catch (Exception e)
            {
                Logger.Warn(e.Message + " Failed to load TerrariaAmbience. ");
            }

        }

        private int MoonCoin_AllowCoinSlotPlacement(On.Terraria.UI.ItemSlot.orig_PickItemMovementAction orig, Item[] inv, int context, int slot, Item checkItem) {
            if (context == 1 && checkItem.type == ModContent.ItemType<MoonCoin>()) {
                return 0;
            } else {
                return orig(inv, context, slot, checkItem);
            }
        }

        private void LoadMoonSky() {
            MoonSky moonSky = new MoonSky();
            Filters.Scene["Macrocosm:MoonSky"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0f, 0f, 0f).UseOpacity(0f), EffectPriority.High);
            SkyManager.Instance["Macrocosm:MoonSky"] = moonSky;
        }

        /*public override void UpdateMusic(ref int music, ref MusicPriority priority) {
            if (Main.myPlayer != -1 && !Main.gameMenu && Main.LocalPlayer.active) {
                MacrocosmPlayer modPlayer = Main.LocalPlayer.GetModPlayer<MacrocosmPlayer>();

                if (modPlayer.ZoneMoon && Main.dayTime) {
                    music = GetSoundSlot(SoundType.Music, "Sounds/Music/MoonDay");
                    priority = MusicPriority.Environment;
                }

                if (modPlayer.ZoneMoon && !Main.dayTime) {
                    music = GetSoundSlot(SoundType.Music, "Sounds/Music/MoonDay");
                    priority = MusicPriority.Environment;
                }
            }
        }*/
    }
}