using Macrocosm.Common.Global.GlobalItems;
using Macrocosm.Common.Global.GlobalTiles;
using SubworldLibrary;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Common.Hooks
{
	public class RemoveItemLight : ILoadable
	{
		public void Load(Mod mod)
		{
			Terraria.On_Item.UpdateItem_VisualEffects += RemoveDroppedItemLight; // removes lighting for dropped items 
			Terraria.On_Player.CanVisuallyHoldItem += DisableTorchHolding;       // disables holding torches in subworlds
			Terraria.On_Player.ItemCheck_EmitHeldItemLight += RemoveHeldLight;   // removes lighting for held torches (i.e. swinging when placing) 
			Terraria.On_WorldGen.KillTile_MakeTileDust += RemoveTorchDust;       // removes particles if tile torch is disabled  
		}

		public void Unload() 
		{
			Terraria.On_Item.UpdateItem_VisualEffects -= RemoveDroppedItemLight;
			Terraria.On_Player.CanVisuallyHoldItem -= DisableTorchHolding;
			Terraria.On_Player.ItemCheck_EmitHeldItemLight -= RemoveHeldLight;
			Terraria.On_WorldGen.KillTile_MakeTileDust -= RemoveTorchDust;
		}

		private static void RemoveDroppedItemLight(Terraria.On_Item.orig_UpdateItem_VisualEffects orig, Item item)
		{
			if (SubworldSystem.AnyActive<Macrocosm>() && TorchGlobalItem.IsTorch(item))
				return;

			orig(item);
		}

		private static bool DisableTorchHolding(Terraria.On_Player.orig_CanVisuallyHoldItem orig, Player self, Item item)
		{
			if (SubworldSystem.AnyActive<Macrocosm>() && TorchGlobalItem.IsTorch(item))
				return false;

			return orig(self, item);
		}

		private static void RemoveHeldLight(Terraria.On_Player.orig_ItemCheck_EmitHeldItemLight orig, Player self, Item item)
		{
			if (SubworldSystem.AnyActive<Macrocosm>() && TorchGlobalItem.IsTorch(item))
				return;

			orig(self, item);
		}

		private static int RemoveTorchDust(Terraria.On_WorldGen.orig_KillTile_MakeTileDust orig, int i, int j, Tile tileCache)
		{
			if (SubworldSystem.AnyActive<Macrocosm>() && LightSourceGlobalTile.IsTileWithFlame(i, j, tileCache.TileType))
				return -1;

			return orig(i, j, tileCache);
		}
	}
}