using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Liquids;
using ModLiquidLib.ID;
using ModLiquidLib.ModLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Bases.Items;
public abstract class Bucket : ModItem
{
    public abstract int BucketLiquidType { get; }

    public override void SetStaticDefaults()
    {
        ItemID.Sets.IsLavaImmuneRegardlessOfRarity[Type] = true;
        ItemID.Sets.AlsoABuildingItem[Type] = true;
        ItemID.Sets.DuplicationMenuToolsFilter[Type] = true;

        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5;

        LiquidID_TLmod.Sets.CreateLiquidBucketItem[BucketLiquidType] = Type;
        ItemSets.LiquidContainerData[Type] = new LiquidContainerData(BucketLiquidType, 50, ItemID.EmptyBucket);
    }

    public override void SetDefaults()
    {
        Item.width = 20;
        Item.height = 24;
        Item.maxStack = Item.CommonMaxStack;
        Item.useTurn = true;
        Item.autoReuse = true;
        Item.useAnimation = 15;
        Item.useTime = 10;
        Item.useStyle = ItemUseStyleID.Swing;
    }

    public override void HoldItem(Player player)
    {
        if (!player.JustDroppedAnItem)
        {
            if (player.whoAmI != Main.myPlayer)
                return;

            if (!player.ItemInTileReach(Item))
                return;

            if (!Main.GamepadDisableCursorItemIcon)
            {
                player.cursorItemIconEnabled = true;
                Main.ItemIconCacheUpdate(Item.type);
            }

            if (!player.ItemTimeIsZero || player.itemAnimation <= 0 || !player.controlUseItem)
                return;

            Tile tile = Main.tile[Player.tileTargetX, Player.tileTargetY];
            if (tile.LiquidAmount >= 200)
                return;

            if (tile.HasUnactuatedTile)
            {
                bool[] tileSolid = Main.tileSolid;
                if (tileSolid[tile.TileType])
                {
                    bool[] tileSolidTop = Main.tileSolidTop;
                    if (!tileSolidTop[tile.TileType])
                    {
                        if (tile.TileType != TileID.Grate)
                            return;
                    }
                }
            }

            if (tile.LiquidAmount != 0)
            {
                if (tile.LiquidType != BucketLiquidType)
                    return;
            }

            SoundEngine.PlaySound(SoundID.SplashWeak, player.position);
            tile.LiquidType = BucketLiquidType;
            tile.LiquidAmount = byte.MaxValue;
            WorldGen.SquareTileFrame(Player.tileTargetX, Player.tileTargetY);
            Item.stack--;
            player.PutItemInInventoryFromItemUsage(ItemID.EmptyBucket, player.selectedItem);
            player.ApplyItemTime(Item);

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.sendWater(Player.tileTargetX, Player.tileTargetY);
            }
        }
    }
}