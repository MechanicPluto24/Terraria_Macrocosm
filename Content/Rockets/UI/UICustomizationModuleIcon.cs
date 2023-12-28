using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Content.Rockets.UI
{
    public class UICustomizationModuleIcon : UIPanel
    {
        private string currentModuleName;
        public void SetModule(string moduleName)
        {
            currentModuleName = moduleName;
        }

        public void ClearModule()
        {
            currentModuleName = null;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (string.IsNullOrEmpty(currentModuleName))
                return;

            Recalculate();
            CalculatedStyle dimensions = GetDimensions();
            Vector2 iconPosition = dimensions.Center();
            Texture2D icon = ModContent.Request<Texture2D>("Macrocosm/Content/Rockets/Modules/Icons/" + currentModuleName, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            spriteBatch.Draw(icon, iconPosition, null, Color.White, 0f, new Vector2(icon.Width * 0.5f, icon.Height * 0.5f), 1f, SpriteEffects.None, 0);
        }
    }
}
