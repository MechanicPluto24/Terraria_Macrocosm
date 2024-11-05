using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Common.Systems
{
    public class EventSystem : ModSystem
    {
        public static bool DemonSun { get; set; } = false;
        public static bool MoonMeteorStorm { get; set; } = false;
        public static bool SolarStorm { get; set; } = false;
        public static bool XaocRift { get; set; } = false;

        public override void ClearWorld()
        {
            DemonSun = false;
            MoonMeteorStorm = false;
            SolarStorm = false;
            XaocRift = false;
        }

        public override void SaveWorldData(TagCompound tag) => SaveData(tag);
        public override void LoadWorldData(TagCompound tag) => LoadData(tag);

        public static void SaveData(TagCompound tag)
        {
            if (DemonSun) tag[nameof(DemonSun)] = true;
            if (MoonMeteorStorm) tag[nameof(MoonMeteorStorm)] = true;
            if (SolarStorm) tag[nameof(SolarStorm)] = true;
            if (XaocRift) tag[nameof(XaocRift)] = true;
        }

        public static void LoadData(TagCompound tag)
        {
            DemonSun = tag.ContainsKey(nameof(DemonSun));
            MoonMeteorStorm = tag.ContainsKey(nameof(MoonMeteorStorm));
            SolarStorm = tag.ContainsKey(nameof(SolarStorm));
            XaocRift = tag.ContainsKey(nameof(XaocRift));
        }

        public override void NetSend(BinaryWriter writer)
        {
            BitsByte bb = new
            (
                DemonSun,
                MoonMeteorStorm,
                SolarStorm,
                XaocRift
            );

            writer.Write(bb);
        }

        public override void NetReceive(BinaryReader reader)
        {
            BitsByte bb = reader.ReadByte();

            DemonSun = bb[0];
            MoonMeteorStorm = bb[1];
            SolarStorm = bb[2];
            XaocRift = bb[3];
        }
    }
}
