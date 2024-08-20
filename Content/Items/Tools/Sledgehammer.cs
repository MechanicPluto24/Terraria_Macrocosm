using Macrocosm.Common.Global.Items;
using Macrocosm.Content.Items.Bars;
using Macrocosm.Content.Rarities;
using StructureHelper.GUI;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Tools
{
    public class Sledgehammer : ModItem
    {
        public override void Load()
        {
            On_Player.ItemCheck_UseMiningTools_TryHittingWall += TryHittingWall_Sledgehammer;
        }

        public override void Unload()
        {
            On_Player.ItemCheck_UseMiningTools_TryHittingWall -= TryHittingWall_Sledgehammer;
        }

        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Item.damage = 70;
            Item.DamageType = DamageClass.Melee;
            Item.width = 44;
            Item.height = 38;
            Item.useTime = 5;
            Item.useAnimation = 12;
            Item.hammer = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<MoonRarityT2>();
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.tileBoost = 5;
        }

        private void TryHittingWall_Sledgehammer(On_Player.orig_ItemCheck_UseMiningTools_TryHittingWall orig, Player player, Item item, int wX, int wY)
        {
            if (item.type == Type)
            {
                for (int i = wX - 1; i <= wX + 1; i++)
                {
                    for (int j = wY - 1; j <= wY + 1; j++)
                    {
                        if (
                            Main.tile[i, j].WallType > 0 && 
                            (!Main.tile[i, j].HasTile || wX != Player.tileTargetX || wY != Player.tileTargetY || (!Main.tileHammer[Main.tile[i, j].TileType] && !player.poundRelease)) && 
                            player.toolTime == 0 && player.itemAnimation > 0 && player.controlUseItem && item.hammer > 0 && Player.CanPlayerSmashWall(i, j)
                        )
                        {
                            int damage = (int)(item.hammer * 1.5f);
                            player.PickWall(i, j, damage);
                        }
                    }
                }

                player.itemTime = item.useTime / 2;
            }
            else
            {
                orig(player, item, wX, wY);
            }
        }
    }
}
