using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Macrocosm.Content.Projectiles.Friendly.Misc;
using Terraria.DataStructures;
using Macrocosm.Common.Utils;
using Terraria.ID;

namespace Macrocosm.Common.Players
{
    public class LifelinePotionPlayer : ModPlayer
    {
        public bool Lifeline { get; set; }

        private int Cooldown;


        public override void ResetEffects()
        {
            Lifeline = false;
            Cooldown--;
        }
        public override void OnHitAnything(float x, float y, Entity victim)
        {
            if(Main.rand.NextBool(5)&&Lifeline&&Cooldown<1){
            int type = 58;
            int itemIdx = Item.NewItem(new EntitySource_Misc("Lifeline"), new Vector2(x,y), type);
            NetMessage.SendData(MessageID.SyncItem, -1, -1, null, itemIdx, 1f);
            Cooldown=180;
            }
        }
        
    }
}
