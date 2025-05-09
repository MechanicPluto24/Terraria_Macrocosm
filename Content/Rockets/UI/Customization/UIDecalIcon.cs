using Macrocosm.Common.Customization;
using Macrocosm.Common.UI;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Localization;
using Terraria.ModLoader;


namespace Macrocosm.Content.Rockets.UI.Customization
{
    public class UIDecalIcon : UIPanelIconButton, IFocusable
    {
        public Decal Decal { get; set; }

        public UIDecalIcon(Decal decal)
        : base
        (
            decal.Icon,
            ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanel", AssetRequestMode.ImmediateLoad),
            ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanelBorder", AssetRequestMode.ImmediateLoad),
            ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanelHoverBorder", AssetRequestMode.ImmediateLoad)
        )
        {
            Decal = decal;
        }

        public override void OnInitialize()
        {
            FocusContext = "DecalSelection";
            OnLeftClick += (_, _) => { HasFocus = true; };
            HoverText = Language.GetOrRegister("Mods.Macrocosm.UI.Rocket.Customization.Decals." + Decal.Name, () => Decal.Name);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            /*
			var dimensions = GetOuterDimensions();
			Texture2D texture = ModContent.Request<Texture2D>(Decal.TexturePath + "_Icon").Value;
			spriteBatch.Draw(texture, dimensions.Position(), null, Color.White, 0f, Vector2.Zero, 0.995f, SpriteEffects.None, 0f);
			*/
        }
    }
}
