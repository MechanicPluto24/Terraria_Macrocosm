using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
            base.DrawSelf(spriteBatch);

            if (string.IsNullOrEmpty(currentModuleName))
                return;

            Recalculate();
            CalculatedStyle dimensions = GetDimensions();
            Vector2 iconPosition = dimensions.Center();
            Texture2D icon = ModContent.Request<Texture2D>("Macrocosm/Content/Rockets/Modules/" + currentModuleName, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            spriteBatch.Draw(icon, iconPosition, null, Color.White, 0f, new Vector2(icon.Width * 0.5f, icon.Height * 0.5f), GetIconScale(currentModuleName), SpriteEffects.None, 0);
        }

        private float GetIconScale(string moduleName)
        {
            return moduleName switch
            {
                "ServiceModule" => 0.9f,
                "ReactorModule" => 1.15f,
                "CommandPod" => 1.25f,
                "EngineModule" or "BoosterLeft" or "BoosterRight" => 0.34f,
                _ => 1f,
            };
        }
    }
}
