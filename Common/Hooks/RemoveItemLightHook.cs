﻿using Microsoft.Xna.Framework.Graphics;
using Terraria;
using SubworldLibrary;
using Terraria.ModLoader;
using MonoMod.Cil;
using Terraria.ID;
using Macrocosm.Content.Items.GlobalItems;
using Macrocosm.Content.Tiles.GlobalTiles;

namespace Macrocosm.Common.Hooks
{
    public class RemoveItemLightHook : ILoadable
    {
        public void Load(Mod mod)
        {
            On.Terraria.Item.UpdateItem_VisualEffects += RemoveItemLight;      // removes lighting for dropped items 
            On.Terraria.Player.CanVisuallyHoldItem += DisableTorchHolding;     // disables holding torches in subworlds
            On.Terraria.Player.ItemCheck_EmitHeldItemLight += RemoveHeldLight; // removes lighting for held torches (i.e. swinging when placing) 
            On.Terraria.WorldGen.KillTile_MakeTileDust += RemoveTorchDust;     // removes particles if tile torch is disabled  
        }

        public void Unload() { }

        private static void RemoveItemLight(On.Terraria.Item.orig_UpdateItem_VisualEffects orig, Item item)
        {
            if (SubworldSystem.AnyActive<Macrocosm>() && TorchGlobalItem.IsTorch(item))
            {
                return;
            }
            orig(item);
        }

        private static bool DisableTorchHolding(On.Terraria.Player.orig_CanVisuallyHoldItem orig, Player self, Item item)
        {
            if(SubworldSystem.AnyActive<Macrocosm>() && TorchGlobalItem.IsTorch(item))
            {
                return false;
            }
            return orig(self, item);
        }

        private static void RemoveHeldLight(On.Terraria.Player.orig_ItemCheck_EmitHeldItemLight orig, Player self, Item item)
        {
            if (SubworldSystem.AnyActive<Macrocosm>() && TorchGlobalItem.IsTorch(item))
            {
                return;
            }
            orig(self, item);
        }

        private static int RemoveTorchDust(On.Terraria.WorldGen.orig_KillTile_MakeTileDust orig, int i, int j, Tile tileCache)
        {
            if(SubworldSystem.AnyActive<Macrocosm>() && LightSourceGlobalTile.IsTileWithFire(i, j, tileCache.TileType))
            {
                return -1;
            }
            return orig(i, j, tileCache);
        }
    }
}