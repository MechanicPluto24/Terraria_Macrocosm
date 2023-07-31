using Terraria.ID;
using Terraria.Localization;
using Terraria;
using Terraria.UI;

namespace Macrocosm.Common.Utils
{
	public static partial class Utility
	{
		public static void ReplaceChildWith(this UIElement parent, UIElement toRemove, UIElement newElement)
		{
			parent.RemoveChild(toRemove);
			toRemove = newElement;
			parent.Append(toRemove);
			toRemove.Activate();
		}

        public static bool UICloseConditions(this Player player) =>
            player.dead || !player.active || Main.editChest || Main.editSign || player.talkNPC >= 0 || !Main.playerInventory;

        public static void UICloseOthers()
        {
            Player player = Main.LocalPlayer;

            //Should your tile entity bring up a UI, this line is useful to prevent item slots from misbehaving
            Main.mouseRightRelease = false;

            //The following four (4) if-blocks are recommended to be used if your multitile opens a UI when right clicked:
            if (player.sign > -1)
            {
                player.sign = -1;
                Main.editSign = false;
                Main.npcChatText = string.Empty;
            }
            if (Main.editChest)
            {
                Main.editChest = false;
                Main.npcChatText = string.Empty;
            }
            if (player.editedChestName)
            {
                NetMessage.SendData(MessageID.SyncPlayerChest, -1, -1, NetworkText.FromLiteral(Main.chest[player.chest].name), player.chest, 1f, 0f, 0f, 0, 0, 0);
                player.editedChestName = false;
            }
            if (player.talkNPC > -1)
            {
                player.SetTalkNPC(-1);
                Main.npcChatCornerItem = 0;
                Main.npcChatText = string.Empty;
            }
        }
    }
}
