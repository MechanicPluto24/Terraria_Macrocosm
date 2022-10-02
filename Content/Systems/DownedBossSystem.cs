using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Content.Systems
{
	public class DownedBossSystem : ModSystem
	{
		public static bool DownedCraterDemon { get; set; } = false;
 
		public override void OnWorldLoad()
		{
			DownedCraterDemon = false;
 		}

		public override void OnWorldUnload() 
		{
			DownedCraterDemon = false;
 		}

		public override void SaveWorldData(TagCompound tag) 
		{
			if (DownedCraterDemon) 
				tag["downedCraterDemon"] = true;
		}

		public override void LoadWorldData(TagCompound tag)
		{
			DownedCraterDemon = tag.ContainsKey("downedCraterDemon");
 		}

		public override void NetSend(BinaryWriter writer)
		{
 			var flags = new BitsByte();
			flags[0] = DownedCraterDemon;

 			writer.Write(flags);
		}

		public override void NetReceive(BinaryReader reader) 
		{
 			BitsByte flags = reader.ReadByte();
			DownedCraterDemon = flags[0];
		}
	}
}
