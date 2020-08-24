using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System.ComponentModel;

namespace Macrocosm.Buffs.Debuffs
{
    public class SuitBreach : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Suit Breach");
            Description.SetDefault("There is a hole in your spacesuit!");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.lifeRegen -= 20;
        }
    }
}