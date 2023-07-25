using Macrocosm.Common.Subworlds;
using SubworldLibrary;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Content.Systems
{
	public class WorldDataSystem : ModSystem, INotifyPropertyChanged
	{
		/// <summary> The instance </summary>
		public static WorldDataSystem Instance => ModContent.GetInstance<WorldDataSystem>();

		/// <summary> 
		/// Objects can subscribe to this to get notified about world flags change.
		/// Should be used only for singleton or low instance count things, such 
		///   as UIs or ModSystems, not entities, as they are kept from being GC 
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged([CallerMemberName] string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));


		#region Flags

		private bool downedCraterDemon = false;
		public bool DownedCraterDemon
		{
			get => downedCraterDemon;
			set { if (value != downedCraterDemon) { downedCraterDemon = value; OnPropertyChanged(); } }
		}

		private bool downedMoonBeast = false;
		public bool DownedMoonBeast
		{
			get => downedMoonBeast;
			set { if (value != downedMoonBeast) { downedMoonBeast = value; OnPropertyChanged(); } }
		}

		private bool downedDementoxin = false;
		public bool DownedDementoxin
		{
			get => downedDementoxin;
			set { if (value != downedDementoxin) { downedDementoxin = value; OnPropertyChanged(); } }
		}

		private bool downedLunalgamate = false;
		public bool DownedLunalgamate
		{
			get => downedLunalgamate;
			set { if (value != downedLunalgamate) { downedLunalgamate = value; OnPropertyChanged(); } }
		}

		private bool foundVulcan = false;
		public bool FoundVulcan 
		{
			get => foundVulcan;
			set { if (value != foundVulcan) { foundVulcan = value; OnPropertyChanged(); } }
		}

		#endregion


		public void ResetFlags()
		{
			// Moon flags
			downedCraterDemon = false;
			downedMoonBeast = false;
			downedDementoxin = false;
			downedLunalgamate = false;

			// Global flags
			foundVulcan = false;
		}

		public override void OnWorldLoad() => ResetFlags();

		public override void OnWorldUnload() => ResetFlags();

		public override void SaveWorldData(TagCompound tag) 
		{
			// Moon flags
			if (downedCraterDemon) tag[nameof(downedCraterDemon)] = true;
			if (downedMoonBeast) tag[nameof(downedMoonBeast)] = true;
			if (downedDementoxin) tag[nameof(downedDementoxin)] = true;
			if (downedLunalgamate) tag[nameof(downedLunalgamate)] = true;

			// Global flags
			if (foundVulcan) tag[nameof(foundVulcan)] = true;
		}

		public override void LoadWorldData(TagCompound tag)
		{
			// Moon flags 
			downedCraterDemon = tag.ContainsKey(nameof(downedCraterDemon));
			downedMoonBeast = tag.ContainsKey(nameof(downedMoonBeast));
			downedDementoxin = tag.ContainsKey(nameof(downedDementoxin));
			downedLunalgamate = tag.ContainsKey(nameof(downedLunalgamate));

			// Global flags
			foundVulcan = tag.ContainsKey(nameof(foundVulcan));
		}

		public void CopyWorldData(TagCompound dataCopyTag) => SaveWorldData(dataCopyTag);

		public void ReadCopiedWorldData(TagCompound dataCopyTag) => LoadWorldData(dataCopyTag);

		public override void NetSend(BinaryWriter writer)
		{
 			var flags = new BitsByte();

			// Moon flags
			flags[0] = downedCraterDemon;
			flags[1] = downedMoonBeast;
			flags[2] = downedDementoxin;
			flags[3] = downedLunalgamate;

			// Global flags 
			flags[4] = foundVulcan; 
 			writer.Write(flags);
		}

		public override void NetReceive(BinaryReader reader) 
		{
 			BitsByte flags = reader.ReadByte();

			// Moon flags
			downedCraterDemon = flags[0];
			downedMoonBeast = flags[1];
			downedDementoxin = flags[2];
			downedLunalgamate = flags[3];

			// Global flags
			foundVulcan = flags[4];
		}
	}
}
