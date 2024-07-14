using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Common.Drawing
{
    public class CursorIcon : ModSystem
    {
        public static Icon Rocket { get; } = new(Macrocosm.SymbolsPath + "Rocket");
        public static Icon LaunchPad { get; } = new(Macrocosm.SymbolsPath + "LaunchPad");
        public static Icon Wrench { get; } = new(Macrocosm.SymbolsPath + "Wrench");
        public static Icon MachineTurnOn { get; } = new(Macrocosm.SymbolsPath + "MachineTurnOn", scale: 1f);
        public static Icon MachineTurnOff { get; } = new(Macrocosm.SymbolsPath + "MachineTurnOff", scale: 1f);
        public static Icon SpaceStation { get; } = new(Macrocosm.SymbolsPath + "SpaceStation");
        public static Icon QuestionMark { get; } = new(Macrocosm.SymbolsPath + "QuestionMark");
        public static Icon Drill { get; } = new(Macrocosm.SymbolsPath + "Drill");

        public record Icon(string texturePath, float scale = 1f)
        {
            public float Scale => scale;

            private Asset<Texture2D> texture;
            public Asset<Texture2D> Texture
            {
                get
                {
                    texture ??= ModContent.Request<Texture2D>(texturePath);
                    return texture;
                }
            }
        }

        public static Icon Current { get; set; } 

        public override void Load()
        {
            On_Main.DrawInterface_40_InteractItemIcon += On_Main_DrawInterface_40_InteractItemIcon;
        }

        public override void Unload()
        {
            On_Main.DrawInterface_40_InteractItemIcon -= On_Main_DrawInterface_40_InteractItemIcon;
        }

        private void On_Main_DrawInterface_40_InteractItemIcon(On_Main.orig_DrawInterface_40_InteractItemIcon orig, Main self)
        {
            if (Current is not null)
            {
                if (!Main.HoveringOverAnNPC && !Main.LocalPlayer.mouseInterface)
                {
                    if (Main.LocalPlayer.cursorItemIconText != "")
                        self.MouseText(Main.LocalPlayer.cursorItemIconText, 0, 0);

                    Main.spriteBatch.Draw(Current.Texture.Value, new Vector2(Main.mouseX + 10, Main.mouseY + 10), null, Color.White, 0f, Vector2.Zero, Main.cursorScale * 0.75f, SpriteEffects.None, 0f);
                }

                return;
            }

            orig(self);
        }

        public override void UpdateUI(GameTime gameTime)
        {
            Current = null;
        }
    }

    
}
