
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

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

				if (interactingPlayer != value && Main.netMode != NetmodeID.SinglePlayer)
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

		public void LootAll()
		{
			Player player = Main.LocalPlayer;
			for (int i = 0; i < Size; i++)
			{
				if (items[i].type > ItemID.None)
				{
					items[i].position = player.Center;
					items[i] = player.GetItem(Main.myPlayer, items[i], GetItemSettings.LootAllSettingsRegularChest);

					if(Main.netMode == NetmodeID.MultiplayerClient)
						SyncItem(i);
				}
			}
		}

		public void DepositAll(ContainerTransferContext context)
		{
			Player player = Main.LocalPlayer;

			for (int slot = 49; slot >= 10; slot--)
			{
				if (player.inventory[slot].stack > 0 && player.inventory[slot].type > ItemID.None && !player.inventory[slot].favorited)
				{
					if (player.inventory[slot].maxStack > 1)
					{
						for (int i = 0; i < Size; i++)
						{
							if (items[i].stack >= items[i].maxStack || player.inventory[slot].type != items[i].type)
								continue;

							if (!ItemLoader.TryStackItems(items[i], player.inventory[slot], out _))
								continue;

							SoundEngine.PlaySound(SoundID.Grab);
							if (player.inventory[slot].stack <= 0)
							{
								player.inventory[slot].SetDefaults();
								if (Main.netMode == NetmodeID.MultiplayerClient)
									SyncItem(i);

								break;
							}

							if (items[i].type == ItemID.None)
							{
								items[i] = player.inventory[slot].Clone();
								player.inventory[slot].SetDefaults();
							}

							if (Main.netMode == NetmodeID.MultiplayerClient)
								SyncItem(i);
						}
					}

					if (player.inventory[slot].stack > 0)
					{
						for (int i = 0; i < Size; i++)
						{
							if (items[i].stack == 0)
							{
								SoundEngine.PlaySound(SoundID.Grab);

								items[i] = player.inventory[slot].Clone();
								player.inventory[slot].SetDefaults();

								if (Main.netMode == NetmodeID.MultiplayerClient)
									SyncItem(i);

								break;
							}
						}
					}
				}
			}
		}

		public void QuickStack(ContainerTransferContext context)
		{
			Player player = Main.LocalPlayer;
			Item[] playerInventory = player.inventory;

			Vector2 center = player.Center;
			Vector2 containerWorldPosition = context.GetContainerWorldPosition();
			bool canVisualizeTransfers = context.CanVisualizeTransfers;

			List<int> itemTypes  = new();
			List<int> itemIndexes = new();
			List<int> emptySlotIndexes = new();
			List<int> zeroStackList = new();
			Dictionary<int, int> itemTypesByIndex = new();

			bool[] shouldSyncSlot = new bool[items.Length];

			for (int i = 0; i < Size; i++)
			{
				if (items[i].type > ItemID.None && items[i].stack > 0 && (items[i].type < ItemID.CopperCoin || items[i].type > ItemID.PlatinumCoin))
				{
					itemIndexes.Add(i);
					itemTypes.Add(items[i].netID);
				}

				if (items[i].type == ItemID.None || items[i].stack <= 0)
					emptySlotIndexes.Add(i);
			}

			int endInventoryIndex = 50;
			int startInventoryIndex = 10;

			for (int i = startInventoryIndex; i < endInventoryIndex; i++)
			{
				if (itemTypes.Contains(playerInventory[i].netID) && !playerInventory[i].favorited)
					itemTypesByIndex.Add(i, playerInventory[i].netID);
			}

			for (int i = 0; i < itemIndexes.Count; i++)
			{
				int idx = itemIndexes[i];
				int type = items[idx].netID;
				foreach (var kvp in itemTypesByIndex)
				{
					if (kvp.Value == type && playerInventory[kvp.Key].netID == type)
					{
						int stack = playerInventory[kvp.Key].stack;
						int stackDifference = items[idx].maxStack - items[idx].stack;
						if (stackDifference == 0)
							break;

						if (stack > stackDifference)
							stack = stackDifference;

						SoundEngine.PlaySound(SoundID.Grab);

						ItemLoader.TryStackItems(items[idx], playerInventory[kvp.Key], out stack);

						if (canVisualizeTransfers && stack > 0)
							Chest.VisualizeChestTransfer(center, containerWorldPosition, items[idx], stack);

						shouldSyncSlot[idx] = true;
					}
				}
			}

			foreach (var kvp in itemTypesByIndex)
			{
				if (playerInventory[kvp.Key].stack == 0)
					zeroStackList.Add(kvp.Key);
			}

			foreach (int idx in zeroStackList)
			{
				itemTypesByIndex.Remove(idx);
			}

			for (int i = 0; i < emptySlotIndexes.Count; i++)
			{
				int idx = emptySlotIndexes[i];
				bool beginningOfStack = true;
				int type = items[idx].netID;
				if (type >= ItemID.CopperCoin && type <= ItemID.PlatinumCoin)
					continue;

				foreach (var kvp in itemTypesByIndex)
				{
					if ((kvp.Value != type || playerInventory[kvp.Key].netID != type) && (!beginningOfStack || playerInventory[kvp.Key].stack <= 0))
						continue;

					SoundEngine.PlaySound(SoundID.Grab);

					if (beginningOfStack)
					{
						type = kvp.Value;
						items[idx] = playerInventory[kvp.Key];
						playerInventory[kvp.Key] = new Item();

						if (canVisualizeTransfers)
							Chest.VisualizeChestTransfer(center, containerWorldPosition, items[idx], items[idx].stack);
					}
					else
					{
						int stack = playerInventory[kvp.Key].stack;
						int stackDifference = items[idx].maxStack - items[idx].stack;

						if (stackDifference == 0)
							break;

						if (stack > stackDifference)
							stack = stackDifference;

						ItemLoader.TryStackItems(items[idx], playerInventory[kvp.Key], out stack);
						if (canVisualizeTransfers && stack > 0)
							Chest.VisualizeChestTransfer(center, containerWorldPosition, items[idx], stack);

						if (playerInventory[kvp.Key].stack == 0)
							playerInventory[kvp.Key] = new Item();
					}

					shouldSyncSlot[idx] = true;
					beginningOfStack = false;
				}
			}

			// Essentially this syncs all slots..?
			if (Main.netMode == NetmodeID.MultiplayerClient)
 				for (int i = 0; i < shouldSyncSlot.Length; i++)
 					SyncItem(i);
 
			itemTypes.Clear();
			itemIndexes.Clear();
			emptySlotIndexes.Clear();
			itemTypesByIndex.Clear();
			zeroStackList.Clear();
		}

		//FIXME: this behaves a weird, for some reason
		public void Restock()
		{
			Player player = Main.LocalPlayer;
			Item[] playerInventory = player.inventory;

			HashSet<int> hashSet = new();
			List<int> restockableIndexes = new();
			List<int> emptySlotIndexes = new();
			for (int slot = 57; slot >= 0; slot--)
			{
				if ((slot < 50 || slot >= 54) && (playerInventory[slot].type < ItemID.CopperCoin || playerInventory[slot].type > ItemID.PlatinumCoin))
				{
					if (playerInventory[slot].stack > 0 && playerInventory[slot].maxStack > 1)
					{ 
						hashSet.Add(playerInventory[slot].netID);
						if (playerInventory[slot].stack < playerInventory[slot].maxStack)
							restockableIndexes.Add(slot);
					}
					else if (playerInventory[slot].stack == 0 || playerInventory[slot].netID == 0 || playerInventory[slot].type == ItemID.None)
					{
						emptySlotIndexes.Add(slot);
					}
				}
			}

			bool success = false;
			for (int i = 0; i < items.Length; i++)
			{
				if (items[i].stack < 1 || !hashSet.Contains(items[i].netID)) 
					continue;

				bool restocked = false;
				for (int j = 0; j < restockableIndexes.Count; j++)
				{
					int idx = restockableIndexes[j];
					int context = idx >= 50 ? 2 : 0;

					if (playerInventory[idx].netID != items[j].netID)
						continue;

					if (ItemSlot.PickItemMovementAction(playerInventory, context, idx, items[j]) == -1)
						continue;

					if (!ItemLoader.TryStackItems(playerInventory[idx], items[j], out _)) 
						continue;

					success = true;
					if (playerInventory[idx].stack == playerInventory[idx].maxStack)
					{
						if (Main.netMode == NetmodeID.MultiplayerClient)
							SyncItem(j);

						restockableIndexes.RemoveAt(j);
						j--;
					}

					if (items[j].stack == 0)
					{
						items[j] = new Item();
						restocked = true;

						if (Main.netMode == NetmodeID.MultiplayerClient)
							SyncItem(j);

						break;
					}
				}

				if (restocked || emptySlotIndexes.Count <= 0 || items[i].ammo == 0)
					continue;

				for (int k = 0; k < emptySlotIndexes.Count; k++)
				{
					int context = emptySlotIndexes[k] >= 50 ? 2 : 0;

					if (ItemSlot.PickItemMovementAction(playerInventory, context, emptySlotIndexes[k], items[k]) != -1)
					{
						Utils.Swap(ref playerInventory[emptySlotIndexes[k]], ref items[k]);
						if (Main.netMode == NetmodeID.MultiplayerClient)
							SyncItem(k);

						restockableIndexes.Add(emptySlotIndexes[k]);
						emptySlotIndexes.RemoveAt(k);
						success = true;
						break;
					}
				}
			}

			if (success)
				SoundEngine.PlaySound(SoundID.Grab);
		}


		public void DepositCoinsFromPlayerInventory(bool moveEvenIfNoCoinsAreThereAlready = false)
		{
			MoveCoins(Main.LocalPlayer.inventory, items, moveEvenIfNoCoinsAreThereAlready);
		}

		// Adapted from Terraria.UI.ChestUI.MoveCoins, but to support dynamic inventory size,
		// and the option of moving coins even though there are no coins already in the container.
		// Didn't really understand the purpose of some parts of the code, hence the non-descriptive variable names -- Feldy
		private long MoveCoins(Item[] source, Item[] destination, bool moveEvenIfNoCoinsAreThereAlready = false)
		{
			bool success = false;
			bool foundCoinsInDestination = false;

			int[] coinsPerType = new int[4];

			int[] tempArray = new int[destination.Length];
			List<int> tempList = new();
			List<int> tempList2 = new();

			long coinCount = Utils.CoinsCount(out bool overflowing, source);

			int coinId;

			for (int i = 0; i < destination.Length; i++)
			{
				tempArray[i] = -1;
				if (destination[i].stack < 1 || destination[i].type <= ItemID.None)
				{
					tempList2.Add(i);
					destination[i] = new Item();
				}

				if (destination[i] != null && destination[i].stack > 0)
				{
					coinId = 0;
					if (destination[i].type == ItemID.CopperCoin)
						coinId = 1;

					if (destination[i].type == ItemID.SilverCoin)
						coinId = 2;

					if (destination[i].type == ItemID.GoldCoin)
						coinId = 3;

					if (destination[i].type == ItemID.PlatinumCoin)
						coinId = 4;

					tempArray[i] = coinId - 1;
					if (coinId > 0)
					{
						coinsPerType[coinId - 1] += destination[i].stack;
						tempList2.Add(i);
						destination[i] = new Item();
						foundCoinsInDestination = true;
					}
				}
			}

			if (!foundCoinsInDestination && !moveEvenIfNoCoinsAreThereAlready)
				return 0L;

			for (int j = 0; j < source.Length; j++)
			{
				if (j != 58 && source[j] != null && source[j].stack > 0 && !source[j].favorited)
				{
					coinId = 0;
					if (source[j].type == ItemID.CopperCoin)
						coinId = 1;

					if (source[j].type == ItemID.SilverCoin)
						coinId = 2;

					if (source[j].type == ItemID.GoldCoin)
						coinId = 3;

					if (source[j].type == ItemID.PlatinumCoin)
						coinId = 4;

					if (coinId > 0)
					{
						success = true;
						coinsPerType[coinId - 1] += source[j].stack;
						tempList.Add(j);
						source[j] = new Item();
					}
				}
			}

			for (int k = 0; k < 3; k++)
			{
				while (coinsPerType[k] >= 100)
				{
					coinsPerType[k] -= 100;
					coinsPerType[k + 1]++;
				}
			}

			for (int l = 0; l < destination.Length; l++)
			{
				if (tempArray[l] < 0 || destination[l].type != ItemID.None)
					continue;

				int idx = l;
				coinId = tempArray[l];
				if (coinsPerType[coinId] > 0)
				{
					destination[idx].SetDefaults(ItemID.CopperCoin + coinId);
					destination[idx].stack = coinsPerType[coinId];
					if (destination[idx].stack > destination[idx].maxStack)
						destination[idx].stack = destination[idx].maxStack;

					coinsPerType[coinId] -= destination[idx].stack;
					tempArray[l] = -1;
				}

				if (Main.netMode == NetmodeID.MultiplayerClient)
					SyncItem(idx);

				tempList2.Remove(idx);
			}

			for (int m = 0; m < destination.Length; m++)
			{
				if (tempArray[m] < 0 || destination[m].type != ItemID.None)
					continue;

				int idx = m;
				coinId = 3;
				while (coinId >= 0)
				{
					if (coinsPerType[coinId] > 0)
					{
						destination[idx].SetDefaults(ItemID.CopperCoin + coinId);
						destination[idx].stack = coinsPerType[coinId];
						if (destination[idx].stack > destination[idx].maxStack)
							destination[idx].stack = destination[idx].maxStack;

						coinsPerType[coinId] -= destination[idx].stack;
						tempArray[m] = -1;
						break;
					}

					if (coinsPerType[coinId] == 0)
						coinId--;
				}

				if (Main.netMode == NetmodeID.MultiplayerClient)
					SyncItem(idx);

				tempList2.Remove(idx);
			}

			while (tempList2.Count > 0)
			{
				int idx = tempList2[0];
				coinId = 3;
				while (coinId >= 0)
				{
					if (coinsPerType[coinId] > 0)
					{
						destination[idx].SetDefaults(ItemID.CopperCoin + coinId);
						destination[idx].stack = coinsPerType[coinId];
						if (destination[idx].stack > destination[idx].maxStack)
							destination[idx].stack = destination[idx].maxStack;

						coinsPerType[coinId] -= destination[idx].stack;
						break;
					}

					if (coinsPerType[coinId] == 0)
						coinId--;
				}

				if (Main.netMode == NetmodeID.MultiplayerClient)
					SyncItem(tempList2[0]);

				tempList2.RemoveAt(0);
			}

			coinId = 3;
			while (coinId >= 0 && tempList.Count > 0)
			{
				int idx = tempList[0];
				if (coinsPerType[coinId] > 0)
				{
					source[idx].SetDefaults(ItemID.CopperCoin + coinId);
					source[idx].stack = coinsPerType[coinId];
					if (source[idx].stack > source[idx].maxStack)
						source[idx].stack = source[idx].maxStack;

					coinsPerType[coinId] -= source[idx].stack;
					success = false;
					tempList.RemoveAt(0);
				}

				if (coinsPerType[coinId] == 0)
					coinId--;
			}

			if (success)
				SoundEngine.PlaySound(SoundID.Grab);

			long coinsCountPostMove = Utils.CoinsCount(out bool overflowPostMove, source);
			if (overflowing || overflowPostMove)
				return 0L;

			return coinCount - coinsCountPostMove;
		}

	}
}
