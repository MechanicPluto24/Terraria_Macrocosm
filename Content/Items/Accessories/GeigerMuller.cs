using Macrocosm.Common.Players;
using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Accessories
{
    public class GeigerMuller : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.accessory = true;
            Item.value = Item.sellPrice(gold: 5);
            Item.rare = ModContent.RarityType<MoonRarityT1>();
        }
        int timer=0;
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            timer++;
            var irradiationPlayer = player.GetModPlayer<IrradiationPlayer>();
            int ticksPerSecond = (int)((10/(irradiationPlayer.IrradiationLevel+0.1)));
            if(irradiationPlayer.IrradiationLevel >0.01f &&timer%ticksPerSecond==0)
                SoundEngine.PlaySound(SoundID.Item11, player.position);
        }
    }
}
