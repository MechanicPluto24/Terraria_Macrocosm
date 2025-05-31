using Macrocosm.Common.Bases.Buffs;
using Macrocosm.Content.Items.Weapons.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Buffs.Weapons
{
    public class HandheldEngineOverheat : ComplexBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override bool CanUseItem(Player player, Item item)
        {
            if (item.type == ModContent.ItemType<HandheldEngine>())
                return false;

            return base.CanUseItem(player, item);
        }

        public override void Update(Player player, ref int buffIndex)
        {
        }
    }
}
