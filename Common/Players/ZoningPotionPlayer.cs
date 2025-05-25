using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Macrocosm.Content.Projectiles.Friendly.Misc;
using Terraria.DataStructures;
using Macrocosm.Common.Utils;
using Terraria.ID;

namespace Macrocosm.Common.Players
{
    public class ZoningPotionPlayer : ModPlayer
    {
        public bool Zoning { get; set; }

        public override void ResetEffects()
        {
            Zoning = false;
        }    
    }

    
}
