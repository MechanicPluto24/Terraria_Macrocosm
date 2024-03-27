using Macrocosm.Common.Utils;
using Macrocosm.Content.Machines;
using Macrocosm.Content.Rockets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace Macrocosm.Common.Storage
{
    // This needs some attention, it's not that great as it was in my mind.
    // Owner references for Inventories are currently only needed for the InventoryItemDropLocation,
    // in case the inventory is resized or the container is destroyed.
    // Maybe there's a better way to do this. 
    // -- Feldy
    public interface IInventoryOwner
    {
        public Inventory Inventory { get; set; }
        public string InventoryOwnerType
        {
            get
            {
                if (this is TileEntity)
                    return nameof(TileEntity);

                return GetType().Name;
            }
        }
        public Vector2 InventoryItemDropLocation => Main.LocalPlayer.Center;
        public int InventorySerializationIndex => 0;

        // TODO: Unhardcode this, make getting the instance part of the interface... somehow
        public static IInventoryOwner GetInventoryOwnerInstance(string ownerType, int serializationIndex)
        {
            if(serializationIndex < 0)
            {
                Utility.LogChatMessage($"IInventoryOwner: Invalid serialization index ({serializationIndex})", Utility.MessageSeverity.Error);
                return null;
            }

            return ownerType switch
            {
                "Rocket" => (serializationIndex >= 0 && serializationIndex < RocketManager.MaxRockets) ? RocketManager.Rockets[serializationIndex] : new(),

                //"Launchpad" => ...

                // TEs are added to the ByID dictionary after the deserialization... the owner association has to be done manually 
                "TileEntity" => null,

                _ => null,
            };
        }
    }
}
