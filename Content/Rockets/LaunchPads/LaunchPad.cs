using Macrocosm.Common.Drawing;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Netcode;
using Macrocosm.Common.Storage;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Systems.UI;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Achievements;
using Macrocosm.Content.Particles;
using Macrocosm.Content.Rockets.Modules;
using Macrocosm.Content.Tiles.Special;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets.LaunchPads;

public partial class LaunchPad : IInventoryOwner
{
    public static int MinWidth => 18;
    public static int MaxWidth => 34;

    [NetSync] public bool Active;
    [NetSync] public Point16 StartTile;
    [NetSync] public Point16 EndTile;
    [NetSync] public int RocketID = -1;
    [NetSync] public string CustomName = "";

    public string DisplayName => !string.IsNullOrEmpty(CustomName) ? CustomName : Language.GetTextValue("Mods.Macrocosm.Common.LaunchPad");

    public Rocket Rocket => HasActiveRocket ? RocketManager.Rockets[RocketID] : internalRocket;
    public bool HasActiveRocket => RocketID >= 0;

    private Rocket internalRocket;
    private Inventory assemblyInventory;

    public int Width => EndTile.X + 1 - StartTile.X;
    public Rectangle Hitbox => new((int)(StartTile.X * 16f), (int)(StartTile.Y * 16f), Width * 16, 16);
    public Point CenterTile => new((StartTile.X + (EndTile.X - StartTile.X) / 2), StartTile.Y);
    public Vector2 CenterWorld => new(((StartTile.X + (EndTile.X - StartTile.X) / 2f) * 16f), StartTile.Y * 16f);
    public string CompassCoordinates => Utility.GetCompassCoordinates(CenterWorld);

    public Inventory Inventory
    {
        get => assemblyInventory;
        set => assemblyInventory = value;
    }

    public InventoryOwnerType InventoryOwnerType => InventoryOwnerType.TileEntity;
    public int InventoryIndex => ((StartTile.Y & 0xFFFF) << 16) | (StartTile.X & 0xFFFF);
    public Vector2 InventoryPosition => CenterWorld;


    private bool isMouseOver = false;
    private bool spawned = false;


    private Dictionary<string, Range> _moduleSlotRanges = new();

    public RocketModule.ConfigurationType CurrentConfiguration { get; private set; } = RocketModule.ConfigurationType.Manned;
    private List<RocketModule.ConfigurationType> Configurations
        => Enum.GetValues(typeof(RocketModule.ConfigurationType)).Cast<RocketModule.ConfigurationType>().Where(c => c != RocketModule.ConfigurationType.Any).ToList();


    public LaunchPad()
    {
        StartTile = new();
        EndTile = new();

        RocketID = -1;
        internalRocket = new();

        assemblyInventory = new(CountRequiredAssemblyItemSlots(out _moduleSlotRanges), this);
        ReserveAssemblySlots();
    }

    public LaunchPad(int startTileX, int startTileY, int endTileX, int endTileY) : this()
    {
        StartTile = new(startTileX, startTileY);
        EndTile = new(endTileX, endTileY);
    }

    public LaunchPad(Point startTile, Point endTile) : this(startTile.X, startTile.Y, endTile.X, endTile.Y) { }
    public LaunchPad(Point16 startTile, Point16 endTile) : this(startTile.X, startTile.Y, endTile.X, endTile.Y) { }

    public static LaunchPad Create(int startTileX, int startTileY, int endTileX, int endTileY, bool shouldSync = true)
    {
        LaunchPad launchPad = new(startTileX, startTileY, endTileX, endTileY);
        launchPad.Active = true;

        if (shouldSync)
            launchPad.NetSync(MacrocosmSubworld.CurrentID);

        LaunchPadManager.Add(MacrocosmSubworld.CurrentID, launchPad);
        return launchPad;
    }

    public static LaunchPad Create(Point startTile, Point endTile, bool shouldSync = true) => Create(startTile.X, startTile.Y, endTile.X, endTile.Y, shouldSync);
    public static LaunchPad Create(Point16 startTile, Point16 endTile, bool shouldSync = true) => Create(startTile.X, startTile.Y, endTile.X, endTile.Y, shouldSync);

    public void Update()
    {
        CheckMarkers();
        CheckRocket();
        CheckBlueprint();

        if (spawned)
            Interact();

        if (!spawned)
        {
            spawned = true;
        }
    }

    private void CheckMarkers()
    {
        if (Main.tile[StartTile].TileType != ModContent.TileType<LaunchPadMarker>())
        {
            Active = false;
            LaunchPadMarker.SetState(StartTile, MarkerState.Inactive);
            LaunchPadMarker.SetState(EndTile, MarkerState.Inactive);
        }

        if (Main.tile[EndTile].TileType != ModContent.TileType<LaunchPadMarker>())
        {
            Active = false;
            LaunchPadMarker.SetState(StartTile, MarkerState.Inactive);
            LaunchPadMarker.SetState(EndTile, MarkerState.Inactive);
        }

        if (!Active)
            NetSync(MacrocosmSubworld.CurrentID);
    }

    private void CheckRocket()
    {
        int prevRocketId = RocketID;
        RocketID = -1;

        for (int i = 0; i < RocketManager.MaxRockets; i++)
        {
            Rocket rocket = RocketManager.Rockets[i];
            if (rocket.ActiveInCurrentWorld && Hitbox.Intersects(rocket.Bounds))
                RocketID = i;
        }

        if (RocketID != prevRocketId || !spawned)
        {
            if (RocketID < 0)
            {
                LaunchPadMarker.SetState(StartTile, MarkerState.Vacant);
                LaunchPadMarker.SetState(EndTile, MarkerState.Vacant);
            }
            else
            {
                LaunchPadMarker.SetState(StartTile, MarkerState.Occupied);
                LaunchPadMarker.SetState(EndTile, MarkerState.Occupied);
            }

            NetSync(MacrocosmSubworld.CurrentID);
        }
    }

    private void CheckTiles()
    {
        int tileY = StartTile.Y;
        bool foundObstruction = false;
        for (int tileX = StartTile.X; tileX <= EndTile.X; tileX++)
        {
            Tile tile = Main.tile[tileX, tileY];
            if (tile.HasTile)
            {
                if (tile.TileType != ModContent.TileType<LaunchPadMarker>() && WorldGen.SolidOrSlopedTile(tileX, tileY))
                {
                    foundObstruction = true;
                }
            }
        }

        if (foundObstruction)
        {
            LaunchPadMarker.SetState(StartTile, MarkerState.Invalid);
            LaunchPadMarker.SetState(EndTile, MarkerState.Invalid);
            Active = false;
        }
    }

    private void CheckBlueprint()
    {
        foreach (var module in Rocket.Modules)
        {
            if (HasActiveRocket)
            {
                module.IsBlueprint = false;
            }
            else
            {
                TryGetAssemblySlotRangeForModule(module, out Range range);
                module.IsBlueprint = !module.Recipe.Check(consume: false, Inventory.Items[range]);
            }
        }
    }

    private void Interact()
    {
        isMouseOver = Hitbox.Contains(Main.MouseWorld.ToPoint()) && Hitbox.InPlayerInteractionRange(TileReachCheckSettings.Simple);
        if (isMouseOver)
        {
            if (Main.mouseRight && Main.mouseRightRelease)
            {
                UISystem.ShowAssemblyUI(this);
            }
            else
            {
                if (!UISystem.Active)
                {
                    Main.LocalPlayer.noThrow = 2;
                    CursorIcon.Current = CursorIcon.LaunchPad;
                }
            }
        }
    }

    private void ReserveAssemblySlots()
    {
        foreach (var module in RocketModule.Templates)
        {
            if (!module.Recipe.Linked && TryGetAssemblySlotRangeForModule(module, out var range))
            {
                int slotIndex = range.Start.Value;
                foreach (AssemblyRecipeEntry recipeEntry in module.Recipe)
                {
                    if (recipeEntry.ItemType.HasValue)
                        Inventory.SetReserved(slotIndex++, recipeEntry.ItemType.Value, recipeEntry.Description, GetBlueprintTexture(recipeEntry.ItemType.Value), UITheme.Current.InventorySlotStyle.BorderColor);
                    else
                        Inventory.SetReserved(slotIndex++, recipeEntry.ItemCheck, recipeEntry.Description, GetBlueprintTexture(recipeEntry.Description.Key), UITheme.Current.InventorySlotStyle.BorderColor);
                }
            }
        }
    }

    private Asset<Texture2D> GetBlueprintTexture(int itemType)
        => ContentSamples.ItemsByType[itemType].ModItem is ModItem modItem ? (ModContent.RequestIfExists<Texture2D>(modItem.Texture + "_Blueprint", out var blueprint) ? blueprint : null) : null;

    private Asset<Texture2D> GetBlueprintTexture(string key)
        => ModContent.RequestIfExists<Texture2D>(Macrocosm.TexturesPath + "UI/Blueprints/" + key[(key.LastIndexOf('.') + 1)..], out var blueprint) ? blueprint : null;

    public bool CanAssemble() => CheckAssemble(consume: false) && !HasActiveRocket && RocketManager.ActiveRocketCount < RocketManager.MaxRockets;
    public bool CanDisassemble() => HasActiveRocket;

    public bool CheckAssemble(bool consume)
    {
        bool met = true;
        foreach (var module in Rocket.Modules)
        {
            TryGetAssemblySlotRangeForModule(module, out var range);
            met &= module.Recipe.Check(consume, Inventory.Items[range]);
        }
        return met;
    }

    public void Assemble()
    {
        if (!CanAssemble())
            return;

        Assemble_CreateParticles();
        CheckAssemble(consume: true);

        Rocket rocket = Rocket.Create(CenterWorld - new Vector2(Rocket.Width / 2f - 8, Rocket.Height - 16), Rocket.Modules);
        RocketID = rocket.WhoAmI;
        internalRocket = rocket.VisualClone();

        NetSync(MacrocosmSubworld.CurrentID);
        ModContent.GetInstance<BuildRocket>()?.Condition?.Complete();
    }

    private void Assemble_CreateParticles()
    {
        foreach (var module in Rocket.Modules)
        {
            if (module.Recipe.Linked || !_moduleSlotRanges.TryGetValue(module.Name, out var range))
                continue;

            foreach (var item in Inventory.Items[range])
            {
                for (int i = 0; i < item.stack; i++)
                {
                    if (Main.rand.NextBool(6))
                    {
                        Particle.Create<ItemTransferParticle>((p) =>
                        {
                            p.StartPosition = CenterWorld;
                            p.EndPosition = module.Position + new Vector2(module.Width / 2f, module.Height / 2f) + Main.rand.NextVector2Circular(64, 64);
                            p.ItemType = item.type;
                            p.TimeToLive = Main.rand.Next(40, 60);
                        });
                    }
                }
            }
        }
    }

    public void Disassemble()
    {
        if (!CanDisassemble())
            return;

        Disassemble_TransferItems();

        internalRocket = Rocket.VisualClone();
        Rocket.Despawn();
        RocketID = -1;
        NetSync(MacrocosmSubworld.CurrentID);
    }

    private void Disassemble_TransferItems()
    {
        foreach (var module in Rocket.Modules)
        {
            if (module.Recipe.Linked)
                continue;

            foreach (AssemblyRecipeEntry recipeEntry in module.Recipe)
            {
                Item item = null;
                if (recipeEntry.ItemType.HasValue)
                {
                    item = new(recipeEntry.ItemType.Value, recipeEntry.RequiredAmount);
                }
                else
                {
                    int defaultType = ContentSamples.ItemsByType.Values.FirstOrDefault((item) => recipeEntry.ItemCheck(item))?.type ?? 0;
                    item = new(defaultType, recipeEntry.RequiredAmount);
                }

                bool addedToInventory = Inventory.TryPlacingItem(ref item, sound: true);
                if (!addedToInventory)
                    Main.LocalPlayer.QuickSpawnItem(item.GetSource_DropAsItem("Launchpad"), item.type, item.stack);

                if (addedToInventory)
                {
                    for (int i = 0; i < item.stack; i++)
                    {
                        if (Main.rand.NextBool(6))
                            Particle.Create<ItemTransferParticle>((p) =>
                            {
                                p.StartPosition = module.Position + new Vector2(module.Width / 2f, module.Height / 2f) + Main.rand.NextVector2Circular(64, 64);
                                p.EndPosition = CenterWorld;
                                p.ItemType = item.type;
                                p.TimeToLive = Main.rand.Next(50, 70);
                            });
                    }
                }
            }
        }
    }

    public bool TryGetAssemblySlotRangeForModule(RocketModule module, out Range range)
    {
        if (_moduleSlotRanges.TryGetValue(module.Name, out range))
            return true;

        return false;
    }

    private int CountRequiredAssemblyItemSlots(out Dictionary<string, Range> moduleSlotRanges)
    {
        int currentSlot = 0;
        moduleSlotRanges = new();

        foreach (var module in RocketModule.Templates.Where(m => !m.Recipe.Linked))
        {
            int requiredSlots = module.Recipe.Count();
            moduleSlotRanges[module.Name] = currentSlot..(currentSlot + requiredSlots);
            currentSlot += requiredSlots;
        }

        foreach (var module in RocketModule.Templates.Where(m => m.Recipe.Linked))
        {
            moduleSlotRanges[module.Name] = moduleSlotRanges[module.Recipe.LinkedResult.Name];
        }

        return currentSlot;
    }

    public bool SwitchAssemblyModuleTier(RocketModule module, int direction, bool justCheck = false)
    {
        if (Rocket.Active)
            return false;

        var availableModules = RocketModule.Templates
            .Where(m => m.Slot == module.Slot && (m.Configuration == module.Configuration || m.Configuration == RocketModule.ConfigurationType.Any))
            .OrderBy(m => m.Tier).ToList();

        int currentIndex = availableModules.FindIndex(m => m.Name == module.Name);
        int newIndex = currentIndex + direction;
        if (newIndex < 0 || newIndex >= availableModules.Count)
            return false;

        if (!justCheck)
        {
            RocketModule newModule = availableModules[newIndex].Clone();
            Rocket.Modules[(int)newModule.Slot] = newModule;
            Rocket.Modules[(int)newModule.Slot].Rocket = Rocket;

            NetSync(MacrocosmSubworld.CurrentID);
            return true;
        }

        return true;
    }

    public bool SwitchAssemblyModulesConfiguration(int direction, bool justCheck = false)
    {
        if (Rocket.Active)
            return false;

        int currentIndex = Configurations.IndexOf(CurrentConfiguration);
        int newIndex = currentIndex + direction;

        if (newIndex < 0 || newIndex >= Configurations.Count)
            return false;

        if (!justCheck)
        {
            var targetConfiguration = Configurations[newIndex];
            CurrentConfiguration = targetConfiguration;

            foreach (var module in Rocket.Modules)
            {
                if (module.Configuration == RocketModule.ConfigurationType.Any || module.Configuration == targetConfiguration)
                    continue;

                var replacementModule = RocketModule.Templates.FirstOrDefault(m =>
                    m.Slot == module.Slot &&
                    m.Tier == module.Tier &&
                    m.Configuration == targetConfiguration);

                if (replacementModule != null)
                {
                    Rocket.Modules[(int)module.Slot] = replacementModule.Clone();
                    Rocket.Modules[(int)module.Slot].Rocket = Rocket;
                }
            }

            NetSync(MacrocosmSubworld.CurrentID);
            return true;
        }

        return true;
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 screenPosition)
    {
        Rectangle rect = Hitbox;
        rect.X -= (int)screenPosition.X;
        rect.Y -= (int)screenPosition.Y;

        if (isMouseOver)
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, rect, Color.Gold * 0.25f);
    }
}
