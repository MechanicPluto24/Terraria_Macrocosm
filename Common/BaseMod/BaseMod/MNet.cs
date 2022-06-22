using Terraria;
using Terraria.ID;

namespace Macrocosm {
	public class MNet {
		public static void SendBaseNetMessage(int msg, params object[] param) {
			if (Main.netMode == NetmodeID.SinglePlayer) { return; } //nothing to sync in SP
            BaseNet.WriteToPacket(Macrocosm.Instance.GetPacket(), (byte)msg, param).Send();
		}
	}
}