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
    public class SpaceSuffocation : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Osphyxiation");
            Description.SetDefault("You are suffocating");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.lifeRegen -= 60;
        }
    }
}