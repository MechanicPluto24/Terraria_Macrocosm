using Terraria.ModLoader;
using Terraria;
using Terraria.Graphics.Effects;
using Macrocosm.Content.Items.Currency;
using System;
using SubworldLibrary;
using Macrocosm.Content.Subworlds.Moon;
using Terraria.Graphics.Shaders;
using Macrocosm.Content.Systems.Music;
using Macrocosm.Backgrounds.Moon;
using Macrocosm.Content.Tiles;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace Macrocosm
{
    public class Macrocosm : Mod {
        public static Mod Instance => ModContent.GetInstance<Macrocosm>();

        public const string emptyTexPath = "Macrocosm/Assets/Empty";
        public static Asset<Texture2D> EmptyTexAsset { get; set; }
        public static Texture2D EmptyTex => EmptyTexAsset.Value;

        public override void Load() {

            EmptyTexAsset = ModContent.Request<Texture2D>(emptyTexPath);
            RyanModCalls();
        }

        private void RyanModCalls()
        {
            try
            {
                var ta = ModLoader.GetMod("TerrariaAmbience");
                ta?.Call("AddTilesToList", null, "Stone", Array.Empty<string>(), new int[] { ModContent.TileType<Regolith>() });
            }
            catch (Exception e)
            {
                Logger.Warn(e.Message + " failed to load TerrariaAmbience. ");
            }

            try
            {
                var taAPI = ModLoader.GetMod("TerrariaAmbienceAPI");
                taAPI?.Call("Ambience", this, "MoonAmbience", "Sounds/Ambient/Moon", 1f, 0.0075f, new Func<bool>(SubworldSystem.IsActive<Moon>));
            }
            catch (Exception e)
            {
                Logger.Warn(e.Message + " failed to load TerrariaAmbienceAPI. ");
            }
        }
    }
}