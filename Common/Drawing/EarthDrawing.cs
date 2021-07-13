using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using SubworldLibrary;
using Macrocosm.Content.Subworlds;
using Terraria.ModLoader;

namespace Macrocosm.Common.Drawing
{
    public sealed class EarthDrawing
    {
        private static Mod MacrocosmMod => ModContent.GetInstance<Macrocosm>();
        public static void InitializeDetour()
        {
            On.Terraria.Main.DrawBG += Main_DrawBG;
        }

        private static void Main_DrawBG(On.Terraria.Main.orig_DrawBG orig, Main self)
        {
            orig(self);
            if (Subworld.IsActive<Moon>())
            {
                var earthTexture = MacrocosmMod.GetTexture("Assets/Earth");
                var sb = Main.spriteBatch;
                sb.Draw(earthTexture, new Vector2(Main.screenWidth / 2, 200), null, Color.White, 0.4101524f, earthTexture.Size() / 2, 1f, default, 0f); // 0.4101524 is earth's axial tilt to radians
            }
        }
    }
}