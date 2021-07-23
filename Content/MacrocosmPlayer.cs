using System.IO;
using Terraria;
using Terraria.ModLoader;
using SubworldLibrary;
using Microsoft.Xna.Framework.Graphics;
using Macrocosm.Content.Buffs.Debuffs;
using Microsoft.Xna.Framework;
using Macrocosm.Content.Subworlds.Moon;

namespace Macrocosm.Content
{
    public class MacrocosmPlayer : ModPlayer
    {
        public bool accMoonArmor = false;
        public bool ZoneMoon = false;
        public bool ZoneBasalt = false;
        public override void ResetEffects()
        {
            accMoonArmor = false;
        }
        public override void PostUpdateBuffs()
        {
            if (Subworld.IsActive<Moon>())
            {
                if (!player.GetModPlayer<MacrocosmPlayer>().accMoonArmor)
                {
                    player.AddBuff(ModContent.BuffType<SuitBreach>(), 2);
                }
            }
        }
        public override void UpdateBiomes()
        {
            ZoneMoon = Subworld.IsActive<Moon>();
            ZoneBasalt = MacrocosmWorld.moonBiome > 20;
        }
        public override bool CustomBiomesMatch(Player other)
        {
            var modOther = other.GetModPlayer<MacrocosmPlayer>();
            return ZoneMoon == modOther.ZoneMoon && ZoneBasalt == modOther.ZoneBasalt;
        }
        public override void CopyCustomBiomesTo(Player other)
        {
            var modOther = other.GetModPlayer<MacrocosmPlayer>();
            modOther.ZoneMoon = ZoneMoon;
            modOther.ZoneBasalt = ZoneBasalt;
        }
        public override void SendCustomBiomes(BinaryWriter writer)
        {
            var flags = new BitsByte();
            flags[0] = ZoneMoon;
            flags[1] = ZoneBasalt;
            writer.Write(flags);
        }
        public override void ReceiveCustomBiomes(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            ZoneMoon = flags[0];
            ZoneBasalt = flags[1];
        }
        public override void PostUpdateMiscEffects()
        {
            if (ZoneMoon)
            {
                player.gravity = 0.068f;
            }
        }
        public override void UpdateBiomeVisuals()
        {
            player.ManageSpecialBiomeVisuals("Macrocosm:MoonSky", ZoneMoon, player.Center);
        }
        public override Texture2D GetMapBackgroundImage()
        {
            if (ZoneMoon)
            {
                return ModContent.GetTexture($"{typeof(Macrocosm).Name}/Assets/Map/Moon");
            }
            return null;
        }

        public override void ModifyScreenPosition()
        {
            if (Subworld.AnyActive(mod))
            {
                if (Main.screenPosition.Y >= 11864f) // Ignore the magic numbers :peepohappy:
                {
                    Main.screenPosition = new Vector2(Main.screenPosition.X, 11864f);
                }
            }
        }
    }
}
