using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Netcode;

public class NetHooks : ILoadable
{
    public void Load(Mod mod)
    {
        On_MessageBuffer.GetData += On_MessageBuffer_GetData;
    }

    public void Unload()
    {
        On_MessageBuffer.GetData -= On_MessageBuffer_GetData;
    }

    private void On_MessageBuffer_GetData(On_MessageBuffer.orig_GetData orig, MessageBuffer self, int start, int length, out int messageType)
    {
        orig(self, start, length, out messageType);
        if(messageType == MessageID.SpawnTileData)
        {
            foreach (var system in Macrocosm.Instance.GetContent<ModSystem>())
            {
                if(system is IOnPlayerJoining syncable)
                {
                    syncable.OnPlayerJoining(playerIndex: self.whoAmI);
                }
            }
        }
    }
}
