
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;

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
			set
			{
				value = (int)MathHelper.Clamp(value, 0, MaxInventorySize);

				if(size != value)
				{
					OnResize(size, value);

					if (Main.netMode != NetmodeID.SinglePlayer)
						SyncSize();
				}

				size = value;		
			}
		}

		public bool CanInteract => interactingPlayer == Main.myPlayer;

		private int interactingPlayer;
		public int InteractingPlayer
		{
			get => interactingPlayer;

			set
			{
				if (value < 0 || value > Main.maxPlayers)
					return;

				if (interactingPlayer != value && Main.netMode == NetmodeID.SinglePlayer)
					SyncInteraction();

				interactingPlayer = value;
			}
		}

		// TODO: entity or spacecraft abstraction (?)
		public Rocket Owner { get; init; }
		public int WhoAmI => Owner.WhoAmI;
		public Vector2 WorldPosition => Owner.Center;

		public Inventory(int size, Rocket owner)
		{
			this.size = (int)MathHelper.Clamp(size, 0, MaxInventorySize);

			items = new Item[size];
			for (int i = 0; i < items.Length; i++)
				items[i] = new Item();

			Owner = owner;

			if (Main.netMode == NetmodeID.SinglePlayer)
				interactingPlayer = Main.myPlayer;
			else
				interactingPlayer = Main.maxPlayers;
		}

		public void DropItem(int index)
		{
			if (Main.netMode == NetmodeID.MultiplayerClient)
				return;

			if (index < 0 || index >= Size)
				return;

			Item.NewItem(items[index].GetSource_Misc(Owner.GetType().Name), WorldPosition, items[index]);
			items[index] = new();
		}

		public void OnResize(int oldSize, int newSize)
		{
			if (oldSize > newSize)
				for(int i = oldSize - 1; i >= newSize; i--)
					DropItem(i);
 
			if (oldSize != newSize)
				Array.Resize(ref items, newSize);

			if (oldSize < newSize)
				Array.Fill(items, new Item(), oldSize, newSize - oldSize);
		}
	}
}
