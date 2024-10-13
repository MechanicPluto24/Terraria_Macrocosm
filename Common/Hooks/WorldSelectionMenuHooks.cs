﻿using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Audio;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Common.Hooks
{
    public class WorldSelectionMenuHooks : ILoadable
    {
        public void Load(Mod mod)
        {
            On_UIWorldListItem.ctor += UIWorldListItem_ctor_AddWorldIcons;
        }

        public void Unload()
        {
            On_UIWorldListItem.ctor -= UIWorldListItem_ctor_AddWorldIcons;
        }

        private void UIWorldListItem_ctor_AddWorldIcons(On_UIWorldListItem.orig_ctor orig, UIWorldListItem self, Terraria.IO.WorldFileData data, int orderInList, bool canBePlayed)
        {
            orig(self, data, orderInList, canBePlayed);

            UIText buttonLabel = typeof(UIWorldListItem).GetFieldValue<UIText>("_buttonLabel", obj: self);

            var player = Main.LocalPlayer.GetModPlayer<SubworldTravelPlayer>();
            string subworld = "Earth";
            if (player.TryGetReturnSubworld(self.Data.UniqueId, out string id))
                subworld = MacrocosmSubworld.SanitizeID(id, out _);

            bool exists = ModContent.RequestIfExists(Macrocosm.TexturesPath + "Icons/" + subworld, out Asset<Texture2D> texture, AssetRequestMode.ImmediateLoad);
            UIImage icon = new(exists ? texture : Macrocosm.EmptyTex)
            {
                Left = new(0, 0),
                Top = new(4.2f, 0),
                HAlign = 0f,
                VAlign = 1f,
                ImageScale = 0.76f
            };
            if (exists)
            {
                icon.OnMouseOver += (_, _) =>
                {
                    SoundEngine.PlaySound(SoundID.MenuTick);
                    buttonLabel.SetText(Language.GetTextValue("Mods.Macrocosm.Subworlds." + subworld + ".DisplayName"));
                };
                icon.OnMouseOut += (_, _) =>
                {
                    buttonLabel.SetText("");
                };
            }
            self.Append(icon);
            self.Recalculate();

            icon.Left.Pixels = buttonLabel.Left.Pixels - 6;
            buttonLabel.Left.Pixels += icon.Width.Pixels * 0.84f;
    }    
    }
}
