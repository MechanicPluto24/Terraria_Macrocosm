using Macrocosm.Content.Items.Global;
using Macrocosm.Content.Tiles.Global;
using SubworldLibrary;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Common.Hooks
{
    public class RemoveItemLight : ILoadable
    {
        public void Load(Mod mod)
        {
            On_Item.UpdateItem_VisualEffects += RemoveDroppedItemLight; // removes lighting for dropped items 
            On_Player.CanVisuallyHoldItem += DisableTorchHolding;       // disables holding torches in subworlds
            On_Player.ItemCheck_EmitHeldItemLight += RemoveHeldLight;   // removes lighting for held torches (i.e. swinging when placing) 
            On_WorldGen.KillTile_MakeTileDust += RemoveTorchDust;       // removes particles if tile torch is disabled  
        }

        public void Unload()
        {
            On_Item.UpdateItem_VisualEffects -= RemoveDroppedItemLight;
            On_Player.CanVisuallyHoldItem -= DisableTorchHolding;
            On_Player.ItemCheck_EmitHeldItemLight -= RemoveHeldLight;
            On_WorldGen.KillTile_MakeTileDust -= RemoveTorchDust;
        }

        private static void RemoveDroppedItemLight(On_Item.orig_UpdateItem_VisualEffects orig, Item item)
        {
            if (SubworldSystem.AnyActive<Macrocosm>() && TorchGlobalItem.IsTorch(item))
                return;

            orig(item);
        }

        private static bool DisableTorchHolding(On_Player.orig_CanVisuallyHoldItem orig, Player self, Item item)
        {
            if (SubworldSystem.AnyActive<Macrocosm>() && TorchGlobalItem.IsTorch(item))
                return false;

            return orig(self, item);
        }

        private static void RemoveHeldLight(On_Player.orig_ItemCheck_EmitHeldItemLight orig, Player self, Item item)
        {
            if (SubworldSystem.AnyActive<Macrocosm>() && TorchGlobalItem.IsTorch(item))
                return;

            orig(self, item);
        }

        private static int RemoveTorchDust(On_WorldGen.orig_KillTile_MakeTileDust orig, int i, int j, Tile tileCache)
        {
            if (SubworldSystem.AnyActive<Macrocosm>() && LightSourceGlobalTile.IsTileWithFlame(i, j, tileCache.TileType))
                return -1;

            return orig(i, j, tileCache);
        }
    }
}