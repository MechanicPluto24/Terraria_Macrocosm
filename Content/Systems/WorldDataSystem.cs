﻿using SubworldLibrary;
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
		//The world system instance
		//Should be safe on mod unload, event if it has an event handler
		public static WorldDataSystem Instance => ModContent.GetInstance<WorldDataSystem>();


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

		public void CopySubworldData()
		{
			// Moon flags
			SubworldSystem.CopyWorldData(nameof(downedCraterDemon), downedCraterDemon);
			SubworldSystem.CopyWorldData(nameof(downedMoonBeast), downedMoonBeast);
			SubworldSystem.CopyWorldData(nameof(downedDementoxin), downedDementoxin);
			SubworldSystem.CopyWorldData(nameof(downedLunalgamate), downedLunalgamate);

			// Global flags
			SubworldSystem.CopyWorldData(nameof(foundVulcan), foundVulcan);
		}

		public void ReadCopiedSubworldData() 
		{
			// Moon flags
			SubworldSystem.ReadCopiedWorldData<bool>(nameof(downedCraterDemon));
			SubworldSystem.ReadCopiedWorldData<bool>(nameof(downedMoonBeast));
			SubworldSystem.ReadCopiedWorldData<bool>(nameof(downedDementoxin));
			SubworldSystem.ReadCopiedWorldData<bool>(nameof(downedLunalgamate));

			// Global flags 
			SubworldSystem.ReadCopiedWorldData<bool>(nameof(foundVulcan));
		}

		// Should these be different?
		public void CopyMainWorldData() => CopySubworldData();
		public void ReadCopiedMainWorldData() => ReadCopiedSubworldData();

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