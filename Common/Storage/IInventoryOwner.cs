using Macrocosm.Common.Utils;
using Macrocosm.Content.Machines;
using Macrocosm.Content.Rockets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace Macrocosm.Common.Storage
{
    public interface IInventoryOwner
    {
        public Inventory Inventory { get; set; }

        public string InventoryOwnerType => GetType().Name;

        public Vector2 InventoryItemDropLocation => Main.LocalPlayer.Center;

        public int InventorySerializationIndex => -1;

        // TODO: Unhardcode this, make getting the instance part of the interface... somehow
        public static IInventoryOwner GetInventoryOwnerInstance(string ownerType, int serializationIndex)
        {
            if(serializationIndex < 0)
            {
                Utility.LogChatMessage($"IInventoryOwner: Invalid serialization index ({serializationIndex})", Utility.MessageSeverity.Error);
                return null;
            }

            switch (ownerType)
            {
                case "Rocket":
                    return (serializationIndex >= 0 && serializationIndex < RocketManager.MaxRockets) ? RocketManager.Rockets[serializationIndex] : new();

                //case "Launchpad":
                //return ...

                case "OreExcavatorTE":
                    return TileEntity.ByID[serializationIndex] as OreExcavatorTE;

                default:
                    Utility.LogChatMessage($"IInventoryOwner: Unknown owner type {ownerType}", Utility.MessageSeverity.Error);
                    return null;
            }
        }
    }
}
