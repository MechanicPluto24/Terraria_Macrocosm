using Macrocosm.Common.Subworlds;
using Macrocosm.Content.Rockets;
using Macrocosm.Content.Rockets.LaunchPads;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace Macrocosm.Common.Storage
{
    // This needs some attention, it's not that great as it was in my mind.
    // Owner references for Inventories are currently needed for:
    // - finding the correct owner on remote clients
    // - the Position, in case the inventory is resized or the container is destroyed.
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

        public Vector2 InventoryPosition => Main.LocalPlayer.Center;
        public int InventorySerializationIndex => 0;

        // TODO: Unhardcode this, make getting the instance part of the interface... somehow
        public static IInventoryOwner GetInventoryOwnerInstance(string ownerType, int serializationIndex)
        {
            return ownerType switch
            {
                "Rocket" => (serializationIndex >= 0 && serializationIndex < RocketManager.MaxRockets) ? RocketManager.Rockets[serializationIndex] : new(),

                "Launchpad" => LaunchPadManager.TryGetLaunchPadAtStartTile(MacrocosmSubworld.CurrentID, new Point16(serializationIndex & 0xFFFF, (serializationIndex >> 16) & 0xFFFF), out LaunchPad launchPad) ? launchPad as IInventoryOwner : null,

                // TEs are added to the ByID dictionary after the deserialization...
                // the owner association has to be done manually in the TE update hook
                // This might not work as expected in multiplayer... pls help!!!
                "TileEntity" => null,

                _ => null,
            };
        }
    }
}
