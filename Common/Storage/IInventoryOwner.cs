using Macrocosm.Common.Subworlds;
using Macrocosm.Content.Rockets;
using Macrocosm.Content.Rockets.LaunchPads;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace Macrocosm.Common.Storage
{
    public enum InventoryOwnerType
    {
        None,
        Rocket,
        Launchpad,
        TileEntity
    }

    public interface IInventoryOwner
    {
        public Inventory Inventory { get; set; }
        public InventoryOwnerType InventoryOwnerType { get; }
        public int InventoryIndex => 0;
        public Vector2 InventoryPosition => Main.LocalPlayer.Center;

        public static IInventoryOwner GetInventoryOwnerInstance(InventoryOwnerType type, int index)
        {
            return type switch
            {
                InventoryOwnerType.Rocket => (index >= 0 && index < RocketManager.MaxRockets) ? RocketManager.Rockets[index] : new(),
                InventoryOwnerType.Launchpad => LaunchPadManager.TryGetLaunchPadAtStartTile(MacrocosmSubworld.CurrentID, new Point16(index & 0xFFFF, (index >> 16) & 0xFFFF), out LaunchPad launchPad) ? launchPad : null,
                InventoryOwnerType.TileEntity => TileEntity.ByID.TryGetValue(index, out var te) && te is IInventoryOwner owner ? owner : null,
                _ => null,
            };
        }
    }
}
