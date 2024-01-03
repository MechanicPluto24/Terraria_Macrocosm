using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets;
using Microsoft.Xna.Framework;

namespace Macrocosm.Common.Storage
{
    public interface IInventoryOwner
    {
        public Inventory Inventory { get; set; }

        public string InventoryOwnerType => GetType().Name;

        public Vector2 InventoryItemDropLocation { get; }

        public int InventorySerializationIndex { get; }

        // TODO: Unhardcode this, maybe make getting the instance part of the interface
        public static IInventoryOwner GetInventoryOwnerInstance(string ownerType, int serializationIndex)
        {
            switch (ownerType)
            {
                case "Rocket":
                    return (serializationIndex >= 0 && serializationIndex < RocketManager.MaxRockets) ? RocketManager.Rockets[serializationIndex] : new();

                default:
                    Utility.LogChatMessage($"IInventoryOwner: Unknown owner type {ownerType}", Utility.MessageSeverity.Error);
                    return null;
            }
        }
    }
}
