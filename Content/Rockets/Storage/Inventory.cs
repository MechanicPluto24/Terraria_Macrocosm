
using Macrocosm.Common.Netcode;
using Macrocosm.Content.Rockets;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Content.Rockets.Storage
{
	public partial class Inventory
	{
		private Item[] items;
		public Item[] Items => items;

		public Item this[int index]
		{
			get => items[index];
			set => items[index] = value;
		}

		private int size;
		public const int MaxInventorySize = ushort.MaxValue;
		public int Size
		{
			get => size;
			set => Resize(value);
		}

		// TODO: entity or spacecraft abstraction (?)
		public Rocket Owner { get; }

		public Inventory(int size, Rocket owner)
		{
			this.size = (int)MathHelper.Clamp(size, 0, MaxInventorySize);

			items = new Item[size];
			for (int i = 0; i < items.Length; i++)
				items[i] = new Item();

			Owner = owner;
		}

		public void DropItems(Range indexRange)
		{
			if (Main.netMode == NetmodeID.MultiplayerClient)
				return;

			(int offset, int length) = indexRange.GetOffsetAndLength(Size);
			if (offset < 0 || length >= Size)
				return;

			Enumerable.Range(offset, length).ToList().ForEach(DropItem);
		}

		public void DropItem(int index)
		{
			if (Main.netMode == NetmodeID.MultiplayerClient)
				return;

			if (index < 0 || index >= Size)
				return;

			Item.NewItem(items[index].GetSource_Misc("Rocket"), Owner.Center, items[index]);
			items[index] = new();
		}

		private void Resize(int newSize)
		{
			newSize = (int)MathHelper.Clamp(newSize, 0, MaxInventorySize);

			int oldSize = size;
			size = newSize;

			if (oldSize > size)
				DropItems(size..(oldSize-1));

			if (oldSize != size)
				Array.Resize(ref items, size);

			if (oldSize < size)
				Array.Fill(items, new Item(), oldSize, size - oldSize);
		}
	}
}
