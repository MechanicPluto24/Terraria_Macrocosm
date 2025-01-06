using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.Localization;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent.UI.Elements;
using Macrocosm.Common.Utils;
using Terraria.Audio;
using SubworldLibrary;

namespace Macrocosm.Common.Subworlds
{
    internal class SubworldTravelSystem : ModSystem
    {
        public override void Load()
        {
            On_UIWorldListItem.ctor += UIWorldListItem_ctor_AddWorldIcons;
        }

        public override void Unload()
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

        public override bool HijackSendData(int whoAmI, int msgType, int remoteClient, int ignoreClient, NetworkText text, int number, float number2, float number3, float number4, int number5, int number6, int number7)
        {
            if (Main.netMode == NetmodeID.Server && msgType == MessageID.FinishedConnectingToServer && remoteClient >= 0 && remoteClient < 255)
            {
                SubworldTravelPlayer.SendLastSubworldCheck(remoteClient);
            }

            return false;
        }
    }
}
