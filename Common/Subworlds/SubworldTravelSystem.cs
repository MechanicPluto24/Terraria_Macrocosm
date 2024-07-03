using Macrocosm.Common.Netcode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.Localization;
using Terraria;
using Terraria.ModLoader;
using Macrocosm.Content.Players;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Microsoft.Xna.Framework;

namespace Macrocosm.Common.Subworlds
{
    public class SubworldTravelSystem : ModSystem
    {
        public override void Load()
        {
            On_UIWorldListItem.DrawSelf += DrawWorldListPlanetIcons;
        }

        public override void Unload()
        {
            On_UIWorldListItem.DrawSelf -= DrawWorldListPlanetIcons;
        }

        // Send current main world GUID and ask player to travel to last known subworld  
        public override bool HijackSendData(int whoAmI, int msgType, int remoteClient, int ignoreClient, NetworkText text, int number, float number2, float number3, float number4, int number5, int number6, int number7)
        {
            if (Main.netMode == NetmodeID.Server && msgType == MessageID.FinishedConnectingToServer && remoteClient >= 0 && remoteClient < 255)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MessageType.LastSubworldCheck);
                Guid guid = MacrocosmSubworld.MainWorldUniqueID;
                packet.Write(guid.ToString());
                packet.Send(remoteClient);
            }

            return false;
        }

        // Draw last known subworld icon in world selection screen
        private void DrawWorldListPlanetIcons(On_UIWorldListItem.orig_DrawSelf orig, UIWorldListItem uIItem, SpriteBatch spriteBatch)
        {
            orig(uIItem, spriteBatch);

            var player = Main.LocalPlayer.GetModPlayer<SubworldTravelPlayer>();
            string subworld = "Earth";
            Texture2D texture = Macrocosm.EmptyTex.Value;

            if (player.TryGetReturnSubworld(uIItem.Data.UniqueId, out string id))
                subworld = MacrocosmSubworld.SanitizeID(id, out _);

            if (ModContent.RequestIfExists<Texture2D>(Macrocosm.TexturesPath + "Icons/" + subworld, out var asset))
                texture = asset.Value;

            var dims = uIItem.GetOuterDimensions();
            var pos = new Vector2(dims.X + texture.Width + 102, dims.Y + dims.Height - texture.Height + 1);

            Rectangle bounds = new((int)pos.X, (int)pos.Y, texture.Width, texture.Height);
            spriteBatch.Draw(texture, pos, null, Color.White, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0f);

            if (bounds.Contains(Main.mouseX, Main.mouseY))
                Main.instance.MouseText(Language.GetTextValue("Mods.Macrocosm.Subworlds." + subworld + ".DisplayName"));
        }
    }
}
