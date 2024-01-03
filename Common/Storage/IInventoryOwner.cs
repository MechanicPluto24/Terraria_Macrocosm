using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets;
using Microsoft.Xna.Framework;
using Terraria;

namespace Macrocosm.Common.Storage
{
    public interface IInventoryOwner
    {
        public Inventory Inventory { get; set; }

        public string InventoryOwnerType => GetType().Name;

        public Vector2 InventoryItemDropLocation => Main.LocalPlayer.Center;

        public int InventorySerializationIndex => -1;

        // TODO: Unhardcode this, make getting the instance part of the interface
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

                default:
                    Utility.LogChatMessage($"IInventoryOwner: Unknown owner type {ownerType}", Utility.MessageSeverity.Error);
                    return null;
            }
        }
    }
}
