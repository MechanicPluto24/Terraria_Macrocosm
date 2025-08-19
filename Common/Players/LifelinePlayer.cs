using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Players;

public class LifelinePlayer : ModPlayer
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
        if (Main.rand.NextBool(5) && Lifeline && Cooldown < 1)
        {
            int type = 58;
            int itemIdx = Item.NewItem(new EntitySource_Misc("Lifeline"), new Vector2(x, y), type);
            NetMessage.SendData(MessageID.SyncItem, -1, -1, null, itemIdx, 1f);
            Cooldown = 180;
        }
    }

}
