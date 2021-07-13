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
        internal static void INTERNAL_SubworldTileFraming()
        {
            for (int i = 0; i < Main.maxTilesX; i++)
            {
                for (int j = Main.maxTilesY - 180; j < Main.maxTilesY; j++)
                {
                    if (Framing.GetTileSafely(i, j).active())
                        WorldGen.SquareTileFrame(i, j);
                    if (Main.tile[i, j] != null)
                        WorldGen.SquareWallFrame(i, j);
                }
            }
        }
        public static Mod Instance => ModContent.GetInstance<Macrocosm>();
        public override void Load()
        {
            Content.NPCs.GlobalNPCs.LowGravityNPC.DetourNPCGravity();
            Common.Drawing.EarthDrawing.InitializeDetour();
            On.Terraria.UI.ItemSlot.PickItemMovementAction += MoonCoin_AllowCoinSlotPlacement;
            CurrencyManager.LoadCurrencies();
            if (!Main.dedServ)
                LoadMoonSky();

            var ta = ModLoader.GetMod("TerrariaAmbience");
            var taAPI = ModLoader.GetMod("TerrariaAmbienceAPI");
            ta?.Call("AddTilesToList", this, "Stone", new string[] { "Regolith", "RegolithBrick", "Hemostone" }, null); // ech
            taAPI?.Call(this, "Sounds/Ambient/Moon", "MoonAmbience", 1f, 0.0075f, new Func<bool>(Subworld.IsActive<Moon>));
        }

        private bool _anySubworldActive;
        private bool _anySubworldActiveLastTick;
        public override void PostUpdateEverything()
        {
            _anySubworldActive = Subworld.AnyActive(this);
            if (_anySubworldActive && !_anySubworldActiveLastTick)
                INTERNAL_SubworldTileFraming();

            _anySubworldActiveLastTick = _anySubworldActive;
        }
        private int MoonCoin_AllowCoinSlotPlacement(On.Terraria.UI.ItemSlot.orig_PickItemMovementAction orig, Item[] inv, int context, int slot, Item checkItem)
        {
            if (context == 1 && checkItem.type == ModContent.ItemType<MoonCoin>())
            {
                return 0;
            }
            else
            {
                return orig(inv, context, slot, checkItem);
            }
        }
        private void LoadMoonSky()
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