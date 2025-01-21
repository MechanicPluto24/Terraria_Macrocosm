using Macrocosm.Common.Bases.Tiles;
using Macrocosm.Common.TileFrame;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Macrocosm.Content.Items.Blocks.Bricks;

namespace Macrocosm.Common.Hooks
{
    /// <summary>
    /// TML: Maybe add functionality to tModLoader 
    /// </summary>
    public class HaemonovaHooks : ILoadable
    {
        public void Load(Mod mod)
        {
            On_Item.GetShimmered += On_Item_GetShimmered;
            
        }


        public void Unload()
        {
            On_Item.GetShimmered -= On_Item_GetShimmered;
            
        }
        

        private void On_Item_GetShimmered(On_Item.orig_GetShimmered orig, Item self)
        {
            if(self.type==3461 &&Main.bloodMoon)
            {
                int StackAmount=self.stack;
                self.SetDefaults(ModContent.ItemType<HaemonovaBrick>());
                self.shimmered = true;
                self.stack = StackAmount;
                if (self.stack > 0)
                    self.shimmerTime = 1f;
                else
                    self.shimmerTime = 0f;

                self.shimmerWet = true;
                self.wet = true;
                self.velocity *= 0.1f;
                if (Main.netMode == 0) {
                    Item.ShimmerEffect(self.Center);
                }
                else {
                    NetMessage.SendData(146, -1, -1, null, 0, (int)self.Center.X, (int)self.Center.Y);
                    NetMessage.SendData(145, -1, -1, null, self.whoAmI, 1f);
                }

                Terraria.GameContent.Achievements.AchievementsHelper.NotifyProgressionEvent(27);
                if (self.stack == 0) {
                    self.makeNPC = -1;
                    self.active = false;
                }
            }
            else
            {
                orig(self);
            }
        }

    }
}
