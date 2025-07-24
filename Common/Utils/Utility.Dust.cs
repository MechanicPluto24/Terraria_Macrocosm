using Macrocosm.Common.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Utils;

public static partial class Utility
{
    /// <summary> 
    /// Get the spritesheet of this particular dust type.  
    /// Use with <c>dust.frame</c> as <c>sourceRectangle</c> when drawing. For vanilla dust types, see <see cref="VanillaDustFrame(int)"/>
    /// </summary>
    public static Texture2D GetDustTexture(int dustType)
        => ModContent.GetModDust(dustType) is ModDust modDust ? modDust.Texture2D.Value : TextureAssets.Dust.Value;

    /// <inheritdoc cref="GetDustTexture(int)"></inheritdoc>
    public static Texture2D GetTexture(this Dust dust) => GetDustTexture(dust.type);

    /// <summary> Gets the respective vanilla dust sourceRectangle from the <see cref="TextureAssets.Dust"/> spritesheet </summary>
    public static Rectangle VanillaDustFrame(int dustType)
    {
        int frameX = dustType * 10 % 1000;
        int frameY = (dustType * 10 / 1000 * 30) + (Main.rand.Next(3) * 10);
        return new Rectangle(frameX, frameY, 8, 8);
    }

    public static int GetDustTypeFromLuminiteStyle(LuminiteStyle style)
    {
        return style switch
        {
            LuminiteStyle.Luminite => DustID.LunarOre,
            LuminiteStyle.Heavenforge => DustID.Heavenforge,
            LuminiteStyle.LunarRust => DustID.LunarRust,
            LuminiteStyle.Astra => DustID.Astra,
            LuminiteStyle.DarkCelestial => DustID.DarkCelestial,
            LuminiteStyle.Mercury => DustID.Mercury,
            LuminiteStyle.StarRoyale => DustID.StarRoyale,
            LuminiteStyle.Cryocore => DustID.Cryocore,
            LuminiteStyle.CosmicEmber => DustID.CosmicEmber,
            _ => -1,
        };
    }

    /// <summary>
    /// <br> Adaption of <see cref="WorldGen.KillTile_MakeTileDust(int, int, Tile)"/> that only returns the dust type, without spawning it. </br>
    /// <br> Returns -1 if the tile should not spawn dusts. </br>
    /// </summary>
    /// <param name="i"> X tile coordinates </param>
    /// <param name="j"> Y tile coordinates</param>
    /// <param name="tile"> The tile evaluated </param>
    public static int GetTileDust(int i, int j, Tile tile)
        => GetTileDust(i, j, tile, out _, out _, out _, out _, out _, out _, out _, out _, out _);

    /// <summary>
    /// <br> Adaption of <see cref="WorldGen.KillTile_MakeTileDust(int, int, Tile)"/> that only returns the dust type, without spawning it. </br>
    /// <br> Returns -1 if the tile should not spawn dusts. </br>
    /// <br> This particular overload also outputs some tile-specific settings. </br>
    /// </summary>
    /// <param name="i"> X tile coordinates </param>
    /// <param name="j"> Y tile coordinates</param>
    /// <param name="tile"> The tile evaluated </param>
    /// <param name="offset"> The spawn offset from the tile position in world </param>
    /// <param name="velocity"> The dust velocity </param>
    /// <param name="scale"> The dust scale </param>
    /// <param name="color"> The dust color </param>
    /// <param name="alpha"> The dust alpha (transparency). 255 is completely transparent </param>
    /// <param name="fadeIn"> The dust fade in variable. Not  </param>
    /// <param name="noGravity"> Whether the dust should be affected by gravity or not </param>
    /// <param name="noLight"> Whether the dust should emit light. </param>
    /// <param name="noLightEmittence"> Whether the dust should emit light, 2.0 (hahah Red) </param>
    public static int GetTileDust(int i, int j, Tile tile, out Vector2 offset, out Vector2 velocity, out float scale, out Color color, out int alpha, out float fadeIn, out bool noGravity, out bool noLight, out bool noLightEmittence)
    {
        offset = Vector2.Zero;
        velocity = new(Main.rand.Next(-20, 21) * 0.1f, Main.rand.Next(-20, 21) * 0.1f);
        scale = 1f;
        color = default;
        alpha = 0;
        fadeIn = 0f;
        noGravity = false;
        noLight = false;
        noLightEmittence = false;

        int dustType = DustID.Dirt;
        if (tile.TileType == TileID.Firework)
        {
            dustType = -1;
        }

        if (tile.TileType == TileID.BeachPiles)
        {
            dustType = (tile.TileFrameY != 0) ? (281 + (tile.TileFrameX / 18)) : 280;
        }

        if (tile.TileType == TileID.Firework)
        {
            dustType = -1;
        }

        if (tile.TileType == TileID.FireworksBox)
        {
            dustType = -1;
        }

        if (tile.TileType == TileID.FireworkFountain)
        {
            dustType = -1;
        }

        if (tile.TileType == TileID.Dirt)
        {
            dustType = DustID.Dirt;
        }

        if (tile.TileType == TileID.LeafBlock)
        {
            dustType = DustID.GrassBlades;
        }

        if (tile.TileType == TileID.Shadewood)
        {
            dustType = 126;
        }
        else if (tile.TileType is TileID.LunarOre or TileID.LunarBrick)
        {
            dustType = DustID.LunarOre;
        }
        else if (tile.TileType == TileID.LunarRustBrick)
        {
            dustType = DustID.LunarRust;
        }
        else if (tile.TileType == TileID.DarkCelestialBrick)
        {
            dustType = DustID.DarkCelestial;
        }
        else if (tile.TileType == TileID.AstraBrick)
        {
            dustType = DustID.Astra;
        }
        else if (tile.TileType == TileID.CosmicEmberBrick)
        {
            dustType = DustID.CosmicEmber;
        }
        else if (tile.TileType == TileID.CryocoreBrick)
        {
            dustType = DustID.Cryocore;
        }
        else if (tile.TileType == TileID.MercuryBrick)
        {
            dustType = DustID.Mercury;
        }
        else if (tile.TileType == TileID.StarRoyaleBrick)
        {
            dustType = DustID.StarRoyale;
        }
        else if (tile.TileType == TileID.HeavenforgeBrick)
        {
            dustType = DustID.Heavenforge;
        }

        if (tile.TileType == TileID.Anvils)
        {
            dustType = DustID.Stone;
            if (tile.TileFrameX >= 36)
            {
                dustType = DustID.Lead;
            }
        }
        else if (tile.TileType is TileID.LunarBlockSolar or TileID.SolarBrick)
        {
            dustType = DustID.Torch;
        }
        else if (tile.TileType is TileID.LunarBlockVortex or TileID.VortexBrick)
        {
            dustType = DustID.GreenTorch;
        }
        else if (tile.TileType is TileID.LunarBlockNebula or TileID.NebulaBrick)
        {
            dustType = DustID.PinkTorch;
        }
        else if (tile.TileType is TileID.LunarBlockStardust or TileID.StardustBrick)
        {
            dustType = DustID.IceTorch;
        }
        else if (tile.TileType == TileID.LesionBlock)
        {
            dustType = DustID.CorruptGibs;
        }

        if (tile.TileType == TileID.Stone || tile.TileType == TileID.Furnaces || tile.TileType == TileID.GrayBrick || tile.TileType == TileID.RedBrick || tile.TileType == TileID.BlueDungeonBrick || tile.TileType == TileID.GreenDungeonBrick || tile.TileType == TileID.PinkDungeonBrick || tile.TileType == TileID.CrackedBlueDungeonBrick || tile.TileType == TileID.CrackedGreenDungeonBrick || tile.TileType == TileID.CrackedPinkDungeonBrick || tile.TileType == TileID.Spikes || Main.tileStone[tile.TileType] || tile.TileType == TileID.Tombstones || tile.TileType == TileID.Bathtubs || tile.TileType == TileID.Lampposts || tile.TileType == TileID.CookingPots || tile.TileType == TileID.Safes || tile.TileType == TileID.TrashCan || tile.TileType == TileID.Pearlstone || tile.TileType == TileID.ActiveStoneBlock || tile.TileType == TileID.InactiveStoneBlock || tile.TileType == TileID.Lever || tile.TileType == TileID.PressurePlates || tile.TileType == TileID.PressurePlates || tile.TileType == TileID.InletPump || tile.TileType == TileID.OutletPump || tile.TileType == TileID.Timers || tile.TileType == TileID.LandMine || tile.TileType == TileID.WaterFountain || tile.TileType == TileID.Teleporter || tile.TileType == TileID.Autohammer || tile.TileType == TileID.Cog || tile.TileType == TileID.StoneSlab || tile.TileType == TileID.HeavyWorkBench || tile.TileType == TileID.LunarMonolith || tile.TileType == TileID.BloodMoonMonolith || tile.TileType == TileID.VoidMonolith || tile.TileType == TileID.AccentSlab || tile.TileType == TileID.EchoMonolith || tile.TileType == TileID.ShimmerMonolith || tile.TileType == TileID.AncientBlueBrick || tile.TileType == TileID.AncientGreenBrick || tile.TileType == TileID.AncientPinkBrick)
        {
            dustType = DustID.Stone;
        }

        if (tile.TileType == TileID.Bubble)
        {
            dustType = DustID.BubbleBlock;
        }

        if (tile.TileType == TileID.DynastyWood)
        {
            dustType = 207;
        }

        if (tile.TileType == TileID.RedDynastyShingles)
        {
            dustType = 208;
        }

        if (tile.TileType == TileID.BlueDynastyShingles)
        {
            dustType = 209;
        }

        if (tile.TileType == TileID.GrandfatherClocks)
        {
            dustType = -1;
        }

        if (tile.TileType is TileID.ChineseLanterns or TileID.SkullLanterns or TileID.Candelabras or TileID.PlatinumCandle or TileID.PlatinumCandelabra)
        {
            dustType = DustID.Torch;
        }

        if (tile.TileType is TileID.WoodBlock or TileID.Loom or TileID.Kegs or TileID.Sawmill or TileID.TinkerersWorkbench or TileID.WoodenBeam or TileID.Mannequin or TileID.Womannequin)
        {
            dustType = 7;
        }

        if (tile.TileType == TileID.PeaceCandle)
        {
            dustType = DustID.PinkTorch;
        }

        if (tile.TileType == TileID.ShadowCandle)
        {
            dustType = DustID.WaterCandle;
        }

        if (tile.TileType == TileID.WaterCandle)
        {
            dustType = DustID.WaterCandle;
        }

        if (tile.TileType == TileID.PinkSlimeBlock)
        {
            dustType = DustID.PinkSlime;
        }

        if (tile.TileType == TileID.WeaponsRack)
        {
            dustType = 7;
        }

        switch (tile.TileType)
        {
            case TileID.ClosedDoor:
            case TileID.OpenDoor:
            case TileID.Pianos:
            case TileID.Benches:
            case TileID.Lamps:
            case TileID.MusicBoxes:
            case TileID.Cannon:
            case TileID.ShipInABottle:
            case TileID.SeaweedPlanter:
            case TileID.TrapdoorOpen:
            case TileID.TrapdoorClosed:
            case TileID.LavaLamp:
            case TileID.Fireplace:
            case TileID.Chimney:
            case TileID.Detonator:
            case TileID.LunarCraftingStation:
            case TileID.LogicGateLamp:
            case TileID.LogicGate:
            case TileID.ConveyorBeltLeft:
            case TileID.ConveyorBeltRight:
            case TileID.LogicSensor:
            case TileID.WirePipe:
            case TileID.AnnouncementBox:
            case TileID.WeightedPressurePlate:
            case TileID.WireBulb:
            case TileID.FakeContainers:
            case TileID.ProjectilePressurePad:
            case TileID.PixelBox:
            case TileID.SillyBalloonPink:
            case TileID.SillyBalloonPurple:
            case TileID.SillyBalloonGreen:
            case TileID.SillyStreamerBlue:
            case TileID.SillyStreamerGreen:
            case TileID.SillyStreamerPink:
            case TileID.SillyBalloonMachine:
            case TileID.SillyBalloonTile:
            case TileID.PartyMonolith:
            case TileID.PartyBundleOfBalloonTile:
            case TileID.PartyPresent:
            case TileID.DjinnLamp:
            case TileID.DefendersForge:
            case TileID.WarTable:
            case TileID.WarTableBanner:
            case TileID.ElderCrystalStand:
            case TileID.FakeContainers2:
            case TileID.GolfHole:
            case TileID.DrumSet:
            case TileID.PicnicTable:
            case TileID.PinWheel:
            case TileID.WeatherVane:
            case TileID.VoidVault:
            case TileID.GolfCupFlag:
            case TileID.GolfTee:
            case TileID.Toilets:
            case TileID.ArrowSign:
            case TileID.PaintedArrowSign:
            case TileID.FoodPlatter:
            case TileID.BlackDragonflyJar:
            case TileID.BlueDragonflyJar:
            case TileID.GreenDragonflyJar:
            case TileID.OrangeDragonflyJar:
            case TileID.RedDragonflyJar:
            case TileID.YellowDragonflyJar:
            case TileID.GoldDragonflyJar:
            case TileID.BoulderStatue:
            case TileID.LawnFlamingo:
            case TileID.PottedPlants1:
            case TileID.PottedPlants2:
            case TileID.GolfTrophies:
            case TileID.PlasmaLamp:
            case TileID.FogMachine:
            case TileID.GardenGnome:
            case TileID.SoulBottles:
            case TileID.RockGolemHead:
            case TileID.PotsSuspended:
            case TileID.BrazierSuspended:
            case TileID.VolcanoSmall:
            case TileID.VolcanoLarge:
            case TileID.PottedLavaPlants:
            case TileID.PottedLavaPlantTendrils:
            case TileID.SliceOfCake:
            case TileID.TeaKettle:
            case TileID.PottedCrystalPlants:
            case TileID.AbigailsFlower:
            case TileID.StinkbugHousingBlocker:
            case TileID.StinkbugHousingBlockerEcho:
            case TileID.GlowTulip:
                dustType = -1;
                break;
            case TileID.DirtiestBlock:
                dustType = DustID.Dirt;
                break;
            case TileID.FossilOre:
                dustType = DustID.t_Golden;
                break;
            case TileID.Pigronata:
                dustType = DustID.Confetti;
                break;
            case TileID.BlueDungeonBrick:
            case TileID.CrackedBlueDungeonBrick:
            case TileID.AncientBlueBrick:
                dustType = DustID.DungeonBlue;
                break;
            case TileID.GreenDungeonBrick:
            case TileID.CrackedGreenDungeonBrick:
            case TileID.AncientGreenBrick:
                dustType = DustID.DungeonGreen;
                break;
            case TileID.PinkDungeonBrick:
            case TileID.CrackedPinkDungeonBrick:
            case TileID.AncientPinkBrick:
                dustType = DustID.DungeonPink;
                break;
            case TileID.LeadBrick:
                dustType = DustID.Lead;
                break;
            case TileID.IronBrick:
            case TileID.Grate:
            case TileID.GrateClosed:
                dustType = DustID.t_SteampunkMetal;
                break;
            case TileID.Spider:
                dustType = DustID.Web;
                break;
            case TileID.LavaMossBrick:
            case TileID.LavaMossBlock:
                dustType = DustID.LavaMoss;
                break;
            case TileID.KryptonMossBrick:
            case TileID.KryptonMossBlock:
                dustType = DustID.KryptonMoss;
                break;
            case TileID.XenonMossBrick:
            case TileID.XenonMossBlock:
                dustType = DustID.XenonMoss;
                break;
            case TileID.ArgonMossBrick:
            case TileID.ArgonMossBlock:
                dustType = DustID.ArgonMoss;
                break;
            case TileID.VioletMossBrick:
            case TileID.VioletMossBlock:
                dustType = DustID.VioletMoss;
                break;
            case TileID.LongMoss:
                {
                    int frameX = tile.TileFrameX / 22;
                    dustType = frameX switch
                    {
                        TileID.Trees => DustID.LavaMoss,
                        TileID.Iron => DustID.KryptonMoss,
                        TileID.Copper => DustID.XenonMoss,
                        TileID.Gold => DustID.ArgonMoss,
                        TileID.Silver => DustID.VioletMoss,
                        TileID.ClosedDoor => DustID.RainbowMk2,
                        _ => 93 + frameX,
                    };
                    break;
                }
            case TileID.BlueMossBrick:
                dustType = DustID.BlueMoss;
                break;
            case TileID.PurpleMossBrick:
                dustType = DustID.PurpleMoss;
                break;
            case TileID.RedMossBrick:
                dustType = DustID.RedMoss;
                break;
            case TileID.BrownMossBrick:
                dustType = DustID.BrownMoss;
                break;
            case TileID.GreenMossBrick:
                dustType = DustID.GreenMoss;
                break;
            case TileID.EchoBlock:
                dustType = DustID.t_Martian;
                break;
            case TileID.GemSaplings:
                dustType = DustID.Stone;
                break;
            case TileID.TreeTopaz:
                dustType = (!WorldGen.genRand.NextBool(10)) ? 1 : 87;
                break;
            case TileID.TreeAmethyst:
                dustType = (!WorldGen.genRand.NextBool(10)) ? 1 : 86;
                break;
            case TileID.TreeSapphire:
                dustType = (!WorldGen.genRand.NextBool(10)) ? 1 : 88;
                break;
            case TileID.TreeEmerald:
                dustType = (!WorldGen.genRand.NextBool(10)) ? 1 : 89;
                break;
            case TileID.TreeRuby:
                dustType = (!WorldGen.genRand.NextBool(10)) ? 1 : 90;
                break;
            case TileID.TreeDiamond:
                dustType = (!WorldGen.genRand.NextBool(10)) ? 1 : 91;
                break;
            case TileID.TreeAmber:
                dustType = (!WorldGen.genRand.NextBool(10)) ? 1 : 138;
                break;
            case TileID.VanityTreeSakuraSaplings:
                dustType = DustID.t_PearlWood;
                break;
            case TileID.VanityTreeSakura:
                dustType = DustID.t_PearlWood;
                break;
            case TileID.VanityTreeWillowSaplings:
                dustType = DustID.t_PearlWood;
                break;
            case TileID.VanityTreeYellowWillow:
                dustType = DustID.t_PearlWood;
                break;
            case TileID.AshGrass:
                dustType = (!WorldGen.genRand.NextBool(6)) ? 237 : 36;
                break;
            case TileID.AshPlants:
            case TileID.AshVines:
                dustType = DustID.Mothron;
                break;
            case TileID.TreeAsh:
                dustType = (!WorldGen.genRand.NextBool(10)) ? 36 : 31;
                if (WorldGen.genRand.NextBool(12))
                {
                    dustType = DustID.Torch;
                }

                break;
        }

        if (Main.tileMoss[tile.TileType])
        {
            dustType = (tile.TileType == TileID.LavaMoss) ? 258 : ((tile.TileType == TileID.KryptonMoss) ? 299 : ((tile.TileType == TileID.XenonMoss) ? 300 : ((tile.TileType == TileID.ArgonMoss) ? 301 : ((tile.TileType == TileID.VioletMoss) ? 305 : ((tile.TileType != 627) ? (tile.TileType - 179 + 93) : 267)))));
        }

        if (tile.TileType == TileID.Painting3X3)
        {
            int frameX = tile.TileFrameX / 54;
            if (tile.TileFrameY >= 54)
            {
                frameX += 36 * (tile.TileFrameY / 54);
            }

            dustType = 7;
            if (frameX is 16 or 17)
            {
                dustType = DustID.Bone;
            }

            if (frameX is >= 46 and <= 49)
            {
                dustType = -1;
            }
        }

        if (tile.TileType == TileID.Painting4X3)
        {
            dustType = DustID.Stone;
        }

        if (tile.TileType == TileID.Painting6X4)
        {
            dustType = -1;
        }

        if (tile.TileType == TileID.SeaOats)
        {
            dustType = Main.tile[i, j + 1].TileType switch
            {
                TileID.Pearlsand => 47,
                TileID.Crimsand => 125,
                TileID.Ebonsand => 17,
                _ => (i >= WorldGen.beachDistance && i <= Main.maxTilesX - WorldGen.beachDistance) ? 289 : 290,
            };
        }

        if (tile.TileType == TileID.Sundial)
        {
            dustType = -1;
        }

        if (tile.TileType == TileID.Moondial)
        {
            dustType = -1;
        }

        if (tile.TileType == TileID.ChimneySmoke)
        {
            dustType = -1;
        }

        if (tile.TileType == TileID.Painting3X2)
        {
            dustType = -1;
        }

        if (tile.TileType == TileID.Presents)
        {
            dustType = -1;
        }

        if (tile.TileType == TileID.SilkRope)
        {
            dustType = DustID.Silk;
        }

        if (tile.TileType == TileID.WebRope)
        {
            dustType = DustID.Web;
        }

        if (tile.TileType == TileID.MysticSnakeRope)
        {
            dustType = -1;
        }

        if (tile.TileType is TileID.MarbleBlock or TileID.Marble or TileID.MarbleColumn)
        {
            dustType = DustID.t_Marble;
        }

        if (tile.TileType is TileID.Granite or TileID.GraniteBlock or TileID.GraniteColumn)
        {
            dustType = DustID.t_Granite;
        }

        if (tile.TileType == TileID.PineTree)
        {
            dustType = 196;
        }

        if (tile.TileType == TileID.Coralstone)
        {
            dustType = 225;
        }

        if (tile.TileType == TileID.ReefBlock)
        {
            dustType = (!WorldGen.genRand.NextBool(2)) ? 161 : 243;
        }

        if (tile.TileType == TileID.ShimmerBlock)
        {
            dustType = DustID.ShimmerSplash;
        }

        if (tile.TileType == TileID.ShimmerBrick)
        {
            dustType = DustID.ShimmerSplash;
        }

        if (tile.TileType == TileID.ChlorophyteBrick)
        {
            dustType = 128;
        }

        if (tile.TileType == TileID.CrimtaneBrick)
        {
            dustType = 117;
        }

        if (tile.TileType == TileID.ShroomitePlating)
        {
            dustType = 42;
        }

        if (tile.TileType == TileID.MartianConduitPlating)
        {
            dustType = DustID.t_Martian;
        }

        if (tile.TileType == TileID.MeteoriteBrick)
        {
            dustType = (!WorldGen.genRand.NextBool(2)) ? 23 : 6;
        }

        if (tile.TileType == TileID.ChristmasTree)
        {
            dustType = (!WorldGen.genRand.NextBool(2)) ? (-1) : 196;
        }

        if (tile.TileType == TileID.Waterfall)
        {
            dustType = 13;
        }

        if (tile.TileType == TileID.Lavafall)
        {
            dustType = 13;
        }

        if (tile.TileType == TileID.Honeyfall)
        {
            dustType = 13;
        }

        if (tile.TileType == TileID.SandFallBlock)
        {
            dustType = 13;
        }

        if (tile.TileType == TileID.SnowFallBlock)
        {
            dustType = 13;
        }

        if (tile.TileType == TileID.LivingFire)
        {
            dustType = DustID.Torch;
        }

        if (tile.TileType == TileID.LivingCursedFire)
        {
            dustType = DustID.CursedTorch;
        }

        if (tile.TileType == TileID.LivingDemonFire)
        {
            dustType = DustID.DemonTorch;
        }

        if (tile.TileType == TileID.LivingFrostFire)
        {
            dustType = DustID.IceTorch;
        }

        if (tile.TileType == TileID.LivingIchor)
        {
            dustType = DustID.IchorTorch;
        }

        if (tile.TileType == TileID.LivingUltrabrightFire)
        {
            dustType = DustID.UltraBrightTorch;
        }

        if (tile.TileType == TileID.Confetti)
        {
            dustType = 13;
        }

        if (tile.TileType == TileID.ConfettiBlack)
        {
            dustType = 13;
        }

        if (tile.TileType == TileID.GoldStarryGlassBlock)
        {
            dustType = 13;
        }

        if (tile.TileType == TileID.BlueStarryGlassBlock)
        {
            dustType = 13;
        }

        if (tile.TileType == TileID.BambooBlock)
        {
            dustType = -1;
        }

        if (tile.TileType == TileID.Bamboo)
        {
            dustType = DustID.t_Cactus;
        }

        if (tile.TileType == TileID.LargeBambooBlock)
        {
            dustType = -1;
        }

        if (tile.TileType == TileID.CopperCoinPile)
        {
            dustType = DustID.Copper;
        }

        if (tile.TileType == TileID.SilverCoinPile)
        {
            dustType = DustID.Silver;
        }

        if (tile.TileType == TileID.GoldCoinPile)
        {
            dustType = 19;
        }

        if (tile.TileType == TileID.PlatinumCoinPile)
        {
            dustType = DustID.Silver;
        }

        if (tile.TileType == TileID.Bookcases)
        {
            dustType = -1;
        }

        if (tile.TileType == TileID.Platforms)
        {
            dustType = (tile.TileFrameY / 18) switch
            {
                TileID.Dirt => 7,
                TileID.Stone => 77,
                TileID.Grass => DustID.t_PearlWood,
                TileID.Plants => 79,
                TileID.Torches => DustID.Bone,
                TileID.Trees => 126,
                TileID.Iron => DustID.DungeonBlue,
                TileID.Copper => DustID.DungeonPink,
                TileID.Gold => DustID.DungeonGreen,
                TileID.Silver => DustID.Stone,
                TileID.ClosedDoor => DustID.t_BorealWood,
                TileID.OpenDoor => DustID.t_BorealWood,
                TileID.Heart => DustID.t_BorealWood,
                TileID.Bottles => 109,
                TileID.Tables => 13,
                TileID.Chairs => 189,
                TileID.Anvils => 191,
                TileID.Furnaces => 215,
                TileID.WorkBenches => DustID.Bone,
                TileID.Platforms => DustID.t_BorealWood,
                TileID.Saplings => DustID.t_Slime,
                TileID.Containers => DustID.t_Golden,
                TileID.Demonite => DustID.Sand,
                TileID.CorruptGrass => DustID.t_PearlWood,
                TileID.CorruptPlants => DustID.t_Honey,
                TileID.Ebonstone => DustID.t_Cactus,
                TileID.DemonAltar => DustID.t_Martian,
                TileID.Sunflower => DustID.t_Meteor,
                TileID.Pots => DustID.t_Granite,
                TileID.PiggyBank => DustID.t_Marble,
                TileID.WoodBlock => 68 + Main.rand.Next(3),
                TileID.ShadowOrbs => DustID.t_Golden,
                TileID.CorruptThorns => DustID.t_PearlWood,
                TileID.Candles => DustID.t_Lihzahrd,
                TileID.Chandeliers => DustID.t_Flesh,
                TileID.Jackolanterns => DustID.t_Frozen,
                TileID.Meteorite => DustID.CorruptGibs,
                TileID.GrayBrick => DustID.Torch,
                TileID.RedBrick => DustID.GreenTorch,
                TileID.ClayBlock => DustID.PinkTorch,
                TileID.BlueDungeonBrick => DustID.IceTorch,
                TileID.HangingLanterns => DustID.DesertPot,
                TileID.PinkDungeonBrick => DustID.GreenBlood,
                TileID.GoldBrick => DustID.PinkSlime,
                TileID.SilverBrick => DustID.PinkSlime,
                TileID.CopperBrick => DustID.Ash,
                TileID.Spikes => DustID.t_Martian,
                _ => DustID.Stone,
            };
        }

        if (tile.TileType == TileID.Beds)
        {
            int frameY = tile.TileFrameY / 36;
            dustType = (frameY == 0) ? 7 : ((frameY == 1) ? 77 : ((frameY == 2) ? 78 : ((frameY == 3) ? 79 : ((frameY == 4) ? 126 : ((frameY == 8) ? 109 : ((frameY < 9) ? 1 : (-1)))))));
        }

        if (tile.TileType == TileID.WorkBenches)
        {
            dustType = (tile.TileFrameX / 36) switch
            {
                TileID.Dirt => 7,
                TileID.Stone => 77,
                TileID.Grass => DustID.t_PearlWood,
                TileID.Plants => 79,
                TileID.Torches => DustID.Bone,
                TileID.Trees => DustID.t_Cactus,
                TileID.Iron => DustID.t_Flesh,
                TileID.Copper => DustID.Bone,
                TileID.Gold => DustID.t_Slime,
                TileID.Silver => 126,
                TileID.ClosedDoor => DustID.t_Lihzahrd,
                TileID.OpenDoor or TileID.Heart or TileID.Bottles => DustID.Stone,
                TileID.Tables => 109,
                TileID.Chairs => 126,
                _ => -1,
            };
        }

        if (tile.TileType is TileID.Tables or TileID.Pianos or TileID.Dressers or TileID.Tables2)
        {
            dustType = -1;
        }

        if (tile.TileType is >= TileID.AmethystGemsparkOff and <= TileID.AmberGemsparkOff)
        {
            int gem = tile.TileType - TileID.AmethystGemsparkOff;
            dustType = DustID.GemAmethyst + gem;
            if (gem == 6)
                dustType = DustID.GemAmber;
        }

        if (tile.TileType is >= TileID.AmethystGemspark and <= TileID.AmethystGemspark)
        {
            int gem = tile.TileType - TileID.AmethystGemspark;
            dustType = DustID.GemAmethyst + gem;
            if (gem == 6)
                dustType = DustID.GemAmber;
        }

        if (tile.TileType == TileID.ExposedGems)
        {
            int frameX = tile.TileFrameX / 18;
            dustType = DustID.GemAmethyst + frameX;
            if (frameX == 6)
                dustType = DustID.GemAmber;
        }

        if (tile.TileType == TileID.GemLocks)
        {
            dustType = (tile.TileFrameX / 54) switch
            {
                TileID.Dirt => DustID.GemRuby,
                TileID.Stone => DustID.GemSapphire,
                TileID.Grass => DustID.GemEmerald,
                TileID.Plants => DustID.GemTopaz,
                TileID.Torches => DustID.GemAmethyst,
                TileID.Trees => DustID.GemDiamond,
                TileID.Iron => DustID.GemAmber,
                _ => -1,
            };
            if (tile.TileFrameY < 54)
            {
                dustType = -1;
            }
        }

        switch (tile.TileType)
        {
            case TileID.TeamBlockRed:
            case TileID.TeamBlockRedPlatform:
                dustType = DustID.GemRuby;
                break;
            case TileID.TeamBlockGreen:
            case TileID.TeamBlockGreenPlatform:
                dustType = DustID.GemEmerald;
                break;
            case TileID.TeamBlockBlue:
            case TileID.TeamBlockBluePlatform:
                dustType = DustID.GemSapphire;
                break;
            case TileID.TeamBlockYellow:
            case TileID.TeamBlockYellowPlatform:
                dustType = DustID.GemTopaz;
                break;
            case TileID.TeamBlockPink:
            case TileID.TeamBlockPinkPlatform:
                dustType = DustID.GemAmethyst;
                break;
            case TileID.TeamBlockWhite:
            case TileID.TeamBlockWhitePlatform:
                dustType = DustID.GemDiamond;
                break;
            case TileID.AntiPortalBlock:
                dustType = 109;
                break;
            case TileID.Seaweed:
                dustType = DustID.GrassBlades;
                break;
            case TileID.Sandcastles:
                dustType = DustID.Sand;
                break;
        }

        if (tile.TileType == TileID.LargePiles)
        {
            dustType = (tile.TileFrameX <= 360) ? 26 : ((tile.TileFrameX <= 846) ? 1 : ((tile.TileFrameX <= 954) ? 9 : ((tile.TileFrameX <= 1062) ? 11 : ((tile.TileFrameX <= 1170) ? 10 : ((tile.TileFrameX > 1332) ? ((tile.TileFrameX > 1386) ? 80 : 10) : 0)))));
        }

        if (tile.TileType == TileID.LargePiles2)
        {
            if (tile.TileFrameX <= 144)
            {
                dustType = DustID.Stone;
            }
            else if (tile.TileFrameX <= 306)
            {
                dustType = 38;
            }
            else if (tile.TileFrameX <= 468)
            {
                dustType = DustID.Ash;
            }
            else if (tile.TileFrameX <= 738)
            {
                dustType = DustID.Web;
            }
            else if (tile.TileFrameX <= 970)
            {
                dustType = DustID.Stone;
            }
            else if (tile.TileFrameX <= 1132)
            {
                dustType = DustID.t_Lihzahrd;
            }
            else if (tile.TileFrameX <= 1132)
            {
                dustType = 155;
            }
            else if (tile.TileFrameX <= 1348)
            {
                dustType = DustID.Stone;
            }
            else if (tile.TileFrameX <= 1564)
            {
                dustType = DustID.Dirt;
            }
            else if (tile.TileFrameX <= 1890)
            {
                dustType = DustID.Sluggy;
            }
            else if (tile.TileFrameX <= 2196)
            {
                dustType = DustID.t_Granite;
            }
            else if (tile.TileFrameX <= 2520)
            {
                dustType = DustID.t_Marble;
            }
        }

        if (tile.TileType == TileID.LargePilesEcho)
        {
            int frameX = tile.TileFrameX / 54;
            if (frameX < 7)
            {
                dustType = DustID.Bone;
            }
            else if (frameX < 16)
            {
                dustType = DustID.Stone;
            }
            else if (frameX < 18)
            {
                dustType = DustID.Copper;
            }
            else if (frameX < 20)
            {
                dustType = DustID.Silver;
            }
            else if (frameX < 22)
            {
                dustType = DustID.t_Golden;
            }
            else if (frameX < 26)
            {
                dustType = 7;
            }
            else if (frameX < 32)
            {
                dustType = DustID.t_Frozen;
            }
            else if (frameX < 35)
            {
                dustType = DustID.t_Frozen;
            }
        }

        if (tile.TileType == TileID.LargePiles2Echo)
        {
            int frameX = tile.TileFrameX / 54;
            frameX += tile.TileFrameY / 36 * 35;
            if (frameX < 3)
            {
                dustType = DustID.Stone;
            }
            else if (frameX < 6)
            {
                dustType = 38;
            }
            else if (frameX < 9)
            {
                dustType = DustID.Ash;
            }
            else if (frameX < 14)
            {
                dustType = DustID.Web;
            }
            else if (frameX < 17)
            {
                dustType = DustID.Stone;
            }
            else if (frameX < 18)
            {
                dustType = DustID.Stone;
            }
            else if (frameX < 21)
            {
                dustType = DustID.t_Lihzahrd;
            }
            else if (frameX < 29)
            {
                dustType = 155;
            }
            else if (frameX < 35)
            {
                dustType = DustID.DesertPot;
            }
            else if (frameX < 41)
            {
                dustType = DustID.t_Granite;
            }
            else if (frameX < 47)
            {
                dustType = DustID.t_Marble;
            }
            else if (frameX < 50)
            {
                dustType = DustID.Dirt;
            }
            else if (frameX < 52)
            {
                dustType = DustID.Grass;
            }
            else if (frameX < 55)
            {
                dustType = DustID.Bone;
            }
        }

        if (tile.TileType == TileID.Statues)
        {
            dustType = DustID.Stone;
            if (tile.TileFrameX >= 1548 && tile.TileFrameX <= 1654 && tile.TileFrameY < 54)
            {
                dustType = DustID.t_Lihzahrd;
            }
        }

        if (tile.TileType == TileID.MushroomStatue)
        {
            dustType = DustID.Stone;
        }

        if (tile.TileType is TileID.AlphabetStatues or TileID.CatBast)
        {
            dustType = DustID.Stone;
        }

        if (tile.TileType == TileID.MetalBars)
        {
            int frameX = tile.TileFrameX / 18;
            if (frameX == 0)
            {
                dustType = DustID.Copper;
            }

            if (frameX == 1)
            {
                dustType = DustID.Tin;
            }

            if (frameX == 2)
            {
                dustType = DustID.t_SteampunkMetal;
            }

            if (frameX == 3)
            {
                dustType = DustID.Lead;
            }

            if (frameX == 4)
            {
                dustType = DustID.Silver;
            }

            if (frameX == 5)
            {
                dustType = DustID.Tungsten;
            }

            if (frameX == 6)
            {
                dustType = DustID.t_Golden;
            }

            if (frameX == 7)
            {
                dustType = DustID.Platinum;
            }

            if (frameX == 8)
            {
                dustType = 14;
            }

            if (frameX == 9)
            {
                dustType = DustID.t_Meteor;
            }

            if (frameX == 10)
            {
                dustType = 25;
            }

            if (frameX == 11)
            {
                dustType = 48;
            }

            if (frameX == 12)
            {
                dustType = 144;
            }

            if (frameX == 13)
            {
                dustType = 49;
            }

            if (frameX == 14)
            {
                dustType = 145;
            }

            if (frameX == 15)
            {
                dustType = 50;
            }

            if (frameX == 16)
            {
                dustType = 146;
            }

            if (frameX == 17)
            {
                dustType = 128;
            }

            if (frameX == 18)
            {
                dustType = DustID.Platinum;
            }

            if (frameX == 19)
            {
                dustType = 117;
            }

            if (frameX == 20)
            {
                dustType = 42;
            }

            if (frameX == 21)
            {
                dustType = -1;
            }

            if (frameX == 22)
            {
                dustType = DustID.LunarOre;
            }
        }

        if (tile.TileType == TileID.SmallPiles)
        {
            if (tile.TileFrameY == 18)
            {
                int frameX = tile.TileFrameX / 36;
                if (frameX < 6)
                {
                    dustType = DustID.Stone;
                }
                else if (frameX < 16)
                {
                    dustType = DustID.Bone;
                }
                else if (frameX == 16)
                {
                    dustType = DustID.Copper;
                }
                else if (frameX == 17)
                {
                    dustType = DustID.Silver;
                }
                else if (frameX == 18)
                {
                    dustType = DustID.t_Golden;
                }
                else if (frameX == 19)
                {
                    dustType = DustID.GemAmethyst;
                }
                else if (frameX == 20)
                {
                    dustType = DustID.GemTopaz;
                }
                else if (frameX == 21)
                {
                    dustType = DustID.GemSapphire;
                }
                else if (frameX == 22)
                {
                    dustType = DustID.GemEmerald;
                }
                else if (frameX == 23)
                {
                    dustType = DustID.GemRuby;
                }
                else if (frameX == 24)
                {
                    dustType = DustID.GemDiamond;
                }
                else if (frameX < 31)
                {
                    dustType = DustID.t_Frozen;
                }
                else if (frameX < 33)
                {
                    dustType = 7;
                }
                else if (frameX < 34)
                {
                    dustType = DustID.t_SteampunkMetal;
                }
                else if (frameX < 38)
                {
                    dustType = DustID.Web;
                }
                else if (frameX < 41)
                {
                    dustType = DustID.Stone;
                }
                else if (frameX < 47)
                {
                    dustType = DustID.DesertPot;
                }
                else if (frameX < 53)
                {
                    dustType = DustID.t_Granite;
                }
                else if (frameX < 59)
                {
                    dustType = DustID.t_Marble;
                }
            }
            else
            {
                int frameX = tile.TileFrameX / 18;
                if (frameX < 6)
                {
                    dustType = DustID.Stone;
                }
                else if (frameX < 12)
                {
                    dustType = DustID.Dirt;
                }
                else if (frameX < 28)
                {
                    dustType = DustID.Bone;
                }
                else if (frameX < 33)
                {
                    dustType = DustID.Stone;
                }
                else if (frameX < 36)
                {
                    dustType = DustID.Dirt;
                }
                else if (frameX < 48)
                {
                    dustType = DustID.t_Frozen;
                }
                else if (frameX < 54)
                {
                    dustType = DustID.Web;
                }
                else if (frameX < 60)
                {
                    dustType = DustID.DesertPot;
                }
                else if (frameX < 66)
                {
                    dustType = DustID.t_Granite;
                }
                else if (frameX < 72)
                {
                    dustType = DustID.t_Marble;
                }
                else if (frameX < 73)
                {
                    dustType = DustID.Dirt;
                }
                else if (frameX < 77)
                {
                    dustType = DustID.Sand;
                }
            }
        }

        if (tile.TileType == TileID.SmallPiles2x1Echo)
        {
            int frameXY = (tile.TileFrameX / 36) + (tile.TileFrameY / 18 * 53);
            if (frameXY < 6)
            {
                dustType = DustID.Stone;
            }
            else if (frameXY < 16)
            {
                dustType = DustID.Bone;
            }
            else if (frameXY == 16)
            {
                dustType = DustID.Copper;
            }
            else if (frameXY == 17)
            {
                dustType = DustID.Silver;
            }
            else if (frameXY == 18)
            {
                dustType = DustID.t_Golden;
            }
            else if (frameXY == 19)
            {
                dustType = DustID.GemAmethyst;
            }
            else if (frameXY == 20)
            {
                dustType = DustID.GemTopaz;
            }
            else if (frameXY == 21)
            {
                dustType = DustID.GemSapphire;
            }
            else if (frameXY == 22)
            {
                dustType = DustID.GemEmerald;
            }
            else if (frameXY == 23)
            {
                dustType = DustID.GemRuby;
            }
            else if (frameXY == 24)
            {
                dustType = DustID.GemDiamond;
            }
            else if (frameXY < 31)
            {
                dustType = DustID.t_Frozen;
            }
            else if (frameXY < 33)
            {
                dustType = 7;
            }
            else if (frameXY < 34)
            {
                dustType = DustID.t_SteampunkMetal;
            }
            else if (frameXY < 38)
            {
                dustType = DustID.Web;
            }
            else if (frameXY < 41)
            {
                dustType = DustID.Stone;
            }
            else if (frameXY < 47)
            {
                dustType = DustID.DesertPot;
            }
            else if (frameXY < 53)
            {
                dustType = DustID.t_Granite;
            }
            else if (frameXY < 59)
            {
                dustType = DustID.t_Marble;
            }
            else if (frameXY < 62)
            {
                dustType = DustID.Dirt;
            }
            else if (frameXY < 65)
            {
                dustType = DustID.Sand;
            }
        }

        if (tile.TileType == TileID.SmallPiles1x1Echo)
        {
            int frameX = tile.TileFrameX / 18;
            if (frameX < 6)
            {
                dustType = DustID.Stone;
            }
            else if (frameX < 12)
            {
                dustType = DustID.Dirt;
            }
            else if (frameX < 28)
            {
                dustType = DustID.Bone;
            }
            else if (frameX < 33)
            {
                dustType = DustID.Stone;
            }
            else if (frameX < 36)
            {
                dustType = DustID.Dirt;
            }
            else if (frameX < 48)
            {
                dustType = DustID.t_Frozen;
            }
            else if (frameX < 54)
            {
                dustType = DustID.Web;
            }
            else if (frameX < 60)
            {
                dustType = DustID.DesertPot;
            }
            else if (frameX < 66)
            {
                dustType = DustID.t_Granite;
            }
            else if (frameX < 72)
            {
                dustType = DustID.t_Marble;
            }
            else if (frameX < 73)
            {
                dustType = DustID.Dirt;
            }
            else if (frameX < 77)
            {
                dustType = DustID.Sand;
            }
        }

        if (tile.TileType == TileID.LihzahrdAltar)
        {
            dustType = DustID.t_Lihzahrd;
        }

        if (tile.TileType == TileID.Ebonwood)
        {
            dustType = 77;
        }

        if (tile.TileType is TileID.RichMahogany or TileID.WoodenSpikes or TileID.LivingMahogany or TileID.RichMahoganyBeam)
        {
            dustType = DustID.t_PearlWood;
        }

        if (tile.TileType == TileID.Pearlwood)
        {
            dustType = DustID.t_PearlWood;
        }

        if (tile.TileType == TileID.Chairs)
        {
            dustType = -1;
        }

        if (tile.TileType == TileID.LivingWood)
        {
            dustType = 7;
        }

        if (tile.TileType == TileID.Trees)
        {
            dustType = 7;
            if (i > 5 && i < Main.maxTilesX - 5)
            {
                int tileX = i;
                int k = j;
                if (tile.TileFrameX == 66 && tile.TileFrameY <= 45)
                {
                    tileX++;
                }

                if (tile.TileFrameX == 88 && tile.TileFrameY >= 66 && tile.TileFrameY <= 110)
                {
                    tileX--;
                }

                if (tile.TileFrameX == 22 && tile.TileFrameY >= 132 && tile.TileFrameY <= 176)
                {
                    tileX--;
                }

                if (tile.TileFrameX == 44 && tile.TileFrameY >= 132 && tile.TileFrameY <= 176)
                {
                    tileX++;
                }

                if (tile.TileFrameX == 44 && tile.TileFrameY >= 132 && tile.TileFrameY <= 176)
                {
                    tileX++;
                }

                if (tile.TileFrameX == 44 && tile.TileFrameY >= 198)
                {
                    tileX++;
                }

                if (tile.TileFrameX == 66 && tile.TileFrameY >= 198)
                {
                    tileX--;
                }

                for (; Main.tile[tileX, k] != null && (!Main.tile[tileX, k].HasTile || !Main.tileSolid[Main.tile[tileX, k].TileType]); k++)
                {
                }

                if (Main.tile[tileX, k] != null)
                {
                    if (Main.tile[tileX, k].HasTile && Main.tile[tileX, k].TileType == 23)
                    {
                        dustType = 77;
                    }

                    if (Main.tile[tileX, k].HasTile && Main.tile[tileX, k].TileType == 661)
                    {
                        dustType = 77;
                    }

                    if (Main.tile[tileX, k].HasTile && Main.tile[tileX, k].TileType == 60)
                    {
                        dustType = DustID.t_PearlWood;
                    }

                    if (Main.tile[tileX, k].HasTile && Main.tile[tileX, k].TileType == 70)
                    {
                        dustType = DustID.Bone;
                    }

                    if (Main.tile[tileX, k].HasTile && Main.tile[tileX, k].TileType == 109)
                    {
                        dustType = 79;
                    }

                    if (Main.tile[tileX, k].HasTile && Main.tile[tileX, k].TileType == 199)
                    {
                        dustType = 121;
                    }

                    if (Main.tile[tileX, k].HasTile && Main.tile[tileX, k].TileType == 662)
                    {
                        dustType = 121;
                    }

                    // Extra patch context.
                    if (Main.tile[tileX, k].HasTile && Main.tile[tileX, k].TileType == 147)
                    {
                        dustType = 122;
                    }

                    TileLoader.TreeDust(Main.tile[tileX, k], ref dustType);
                }
            }
        }

        if (tile.TileType == TileID.PalmTree)
        {
            dustType = 215;
            if (i > 5 && i < Main.maxTilesX - 5)
            {
                int l;
                for (l = j; Main.tile[i, l] != null && (!Main.tile[i, l].HasTile || !Main.tileSolid[Main.tile[i, l].TileType]); l++)
                {
                }

                if (Main.tile[i, l] != null)
                {
                    if (Main.tile[i, l].HasTile && Main.tile[i, l].TileType == 234)
                    {
                        dustType = 121;
                    }

                    if (Main.tile[i, l].HasTile && Main.tile[i, l].TileType == 116)
                    {
                        dustType = 79;
                    }

                    // Extra patch context.
                    if (Main.tile[i, l].HasTile && Main.tile[i, l].TileType == 112)
                    {
                        dustType = 77;
                    }

                    TileLoader.PalmTreeDust(Main.tile[i, l], ref dustType);
                }
            }
        }

        if (tile.TileType == TileID.Traps)
        {
            dustType = (tile.TileFrameY / 18) switch
            {
                TileID.Stone or TileID.Grass or TileID.Plants or TileID.Torches => DustID.t_Lihzahrd,
                TileID.Trees => DustID.Stone,
                _ => (int)DustID.Stone,
            };
        }

        if (tile.TileType == TileID.GeyserTrap)
        {
            dustType = DustID.Stone;
        }

        if (tile.TileType == TileID.BeeHive)
        {
            dustType = -1;
        }

        if (tile.TileType == TileID.SnowballLauncher)
        {
            dustType = -1;
        }

        if (tile.TileType == TileID.Rope)
        {
            dustType = 129;
        }

        if (tile.TileType == TileID.Chain)
        {
            dustType = DustID.Stone;
        }

        if (tile.TileType == TileID.Campfire)
        {
            dustType = -6;
        }

        if (tile.TileType == TileID.TinPlating)
        {
            dustType = DustID.Tin;
        }

        if (tile.TileType == TileID.PumpkinBlock)
        {
            dustType = 189;
        }

        if (tile.TileType == TileID.HayBlock)
        {
            dustType = 190;
        }

        if (tile.TileType == TileID.SpookyWood)
        {
            dustType = 191;
        }

        if (tile.TileType == TileID.Pumpkins)
        {
            if (tile.TileFrameX < 72)
            {
                dustType = DustID.GrassBlades;
            }
            else if (tile.TileFrameX < 108)
            {
                dustType = DustID.GrassBlades;
                if (WorldGen.genRand.NextBool(3))
                {
                    dustType = 189;
                }
            }
            else if (tile.TileFrameX < 144)
            {
                dustType = DustID.GrassBlades;
                if (WorldGen.genRand.NextBool(2))
                {
                    dustType = 189;
                }
            }
            else
            {
                dustType = DustID.GrassBlades;
                if (!WorldGen.genRand.NextBool(4))
                {
                    dustType = 189;
                }
            }
        }

        if (tile.TileType == TileID.Containers2)
        {
            dustType = -1;
        }

        if (tile.TileType == TileID.Containers)
        {
            dustType = (tile.TileFrameX >= 1008) ? (-1) : ((tile.TileFrameX >= 612) ? 11 : ((tile.TileFrameX >= 576) ? 148 : ((tile.TileFrameX >= 540) ? 26 : ((tile.TileFrameX >= 504) ? 126 : ((tile.TileFrameX >= 468) ? 116 : ((tile.TileFrameX >= 432) ? 7 : ((tile.TileFrameX >= 396) ? 11 : ((tile.TileFrameX >= 360) ? 10 : ((tile.TileFrameX >= 324) ? 79 : ((tile.TileFrameX >= 288) ? 78 : ((tile.TileFrameX >= 252) ? 77 : ((tile.TileFrameX >= 216) ? 1 : ((tile.TileFrameX >= 180) ? 7 : ((tile.TileFrameX >= 108) ? 37 : ((tile.TileFrameX < 36) ? 7 : 10)))))))))))))));
        }

        if (tile.TileType == TileID.VineFlowers)
        {
            dustType = DustID.GrassBlades;
        }

        if (tile.TileType is TileID.Grass or TileID.GolfGrass)
        {
            dustType = (!WorldGen.genRand.NextBool(2)) ? 2 : 0;
        }

        if (tile.TileType == TileID.MagicalIceBlock)
        {
            dustType = 67;
        }

        if (tile.TileType == TileID.Banners)
        {
            dustType = -1;
        }

        if (tile.TileType == TileID.Asphalt)
        {
            dustType = 109;
        }

        if (tile.TileType == TileID.DemonAltar)
        {
            dustType = (tile.TileFrameX < 54) ? 8 : 5;
        }

        if (tile.TileType == TileID.Chandeliers)
        {
            dustType = -1;
        }

        if (tile.TileType == TileID.Iron)
        {
            dustType = DustID.t_SteampunkMetal;
        }

        if (tile.TileType is TileID.Copper or TileID.CopperBrick or TileID.CopperPlating or TileID.AncientCopperBrick)
        {
            dustType = DustID.Copper;
        }

        if (tile.TileType is TileID.Gold or TileID.GoldBrick or TileID.Thrones or TileID.AncientGoldBrick)
        {
            dustType = DustID.t_Golden;
        }

        if (tile.TileType is TileID.Silver or TileID.HangingLanterns or TileID.SilverBrick or TileID.DiscoBall or TileID.Switches or TileID.AncientSilverBrick)
        {
            dustType = DustID.Silver;
        }

        if (tile.TileType is TileID.Tin or TileID.TinBrick)
        {
            dustType = DustID.Tin;
        }

        if (tile.TileType == TileID.Lead)
        {
            dustType = DustID.Lead;
        }

        if (tile.TileType is TileID.Tungsten or TileID.TungstenBrick)
        {
            dustType = DustID.Tungsten;
        }

        if (tile.TileType is TileID.Platinum or TileID.PlatinumBrick)
        {
            dustType = DustID.Platinum;
        }

        if (tile.TileType is TileID.CrimsonGrass or TileID.CrimsonJungleGrass)
        {
            dustType = 117;
        }

        if (tile.TileType == TileID.CrimsonVines)
        {
            dustType = DustID.CrimsonPlants;
        }

        if (tile.TileType == TileID.CrimsonPlants)
        {
            dustType = DustID.CrimsonPlants;
        }

        if (tile.TileType == TileID.Chlorophyte)
        {
            dustType = 128;
        }

        if (tile.TileType == TileID.DyePlants)
        {
            switch (tile.TileFrameX / 34)
            {
                case TileID.Dirt:
                case TileID.Stone:
                    dustType = DustID.Bone;
                    break;
                case TileID.Plants:
                    dustType = DustID.GrassBlades;
                    break;
                case TileID.Grass:
                case TileID.Torches:
                case TileID.Trees:
                case TileID.Iron:
                    dustType = DustID.t_Cactus;
                    break;
                case TileID.Copper:
                    dustType = 117;
                    break;
                case TileID.Gold:
                    dustType = DustID.CorruptPlants;
                    break;
                case TileID.Silver:
                    dustType = DustID.Torch;
                    break;
                case TileID.ClosedDoor:
                    dustType = DustID.GrassBlades;
                    break;
                case TileID.OpenDoor:
                    dustType = DustID.Bone;
                    break;
            }
        }

        if (tile.TileType is TileID.Crimtane or TileID.CrimstoneBrick)
        {
            dustType = 117;
            if (WorldGen.genRand.NextBool(2))
            {
                dustType = DustID.Stone;
            }
        }

        if (tile.TileType == TileID.Crimstone)
        {
            dustType = 117;
        }

        if (tile.TileType == TileID.ImbuingStation)
        {
            dustType = (!WorldGen.genRand.NextBool(2)) ? 13 : 7;
        }

        if (tile.TileType == TileID.Extractinator)
        {
            dustType = -1;
        }

        if (tile.TileType == TileID.ChlorophyteExtractinator)
        {
            dustType = -128;
        }

        if (tile.TileType == TileID.BubbleMachine)
        {
            dustType = (WorldGen.genRand.NextBool(2)) ? 1 : 13;
        }

        if (tile.TileType == TileID.TeleportationPylon)
        {
            dustType = -1;
        }
        else if (tile.TileType is >= 358 and <= 364 or >= 275 and <= 282 or TileID.SnailCage or TileID.GlowingSnailCage or >= 288 and <= 297 or >= 316 and <= 318 or TileID.FrogCage or TileID.MouseCage or TileID.PenguinCage or TileID.WormCage or TileID.GrasshopperCage or TileID.LadybugCage or TileID.SquirrelOrangeCage or TileID.SquirrelGoldCage or TileID.GoldGoldfishBowl or TileID.BlackDragonflyJar or TileID.BlueDragonflyJar or TileID.GreenDragonflyJar or TileID.OrangeDragonflyJar or TileID.RedDragonflyJar or TileID.YellowDragonflyJar or TileID.GoldDragonflyJar or TileID.MaggotCage or TileID.PupfishBowl or TileID.GoldLadybugCage or TileID.TurtleCage or TileID.TurtleJungleCage or TileID.RatCage or TileID.GrebeCage or TileID.SeagullCage or TileID.WaterStriderCage or TileID.GoldWaterStriderCage or TileID.SeahorseCage or TileID.GoldSeahorseCage or TileID.OwlCage or TileID.CageEnchantedNightcrawler or TileID.CageSluggy or TileID.CageBuggy or TileID.CageGrubby or TileID.PinkFairyJar or TileID.GreenFairyJar or TileID.BlueFairyJar or TileID.MagmaSnailCage or TileID.HellButterflyJar or TileID.LavafishBowl or TileID.AmethystBunnyCage or TileID.TopazBunnyCage or TileID.SapphireBunnyCage or TileID.EmeraldBunnyCage or TileID.RubyBunnyCage or TileID.DiamondBunnyCage or TileID.AmberBunnyCage or TileID.AmethystSquirrelCage or TileID.TopazSquirrelCage or TileID.SapphireSquirrelCage or TileID.EmeraldSquirrelCage or TileID.RubySquirrelCage or TileID.DiamondSquirrelCage or TileID.AmberSquirrelCage or TileID.TruffleWormCage or TileID.EmpressButterflyJar or TileID.StinkbugCage or TileID.ScarletMacawCage or TileID.BlueMacawCage or TileID.ToucanCage or TileID.YellowCockatielCage or TileID.GrayCockatielCage)
        {
            dustType = 13;
            if (!WorldGen.genRand.NextBool(3))
            {
                dustType = -1;
            }
        }

        if (tile.TileType == TileID.Bottles)
        {
            dustType = (tile.TileFrameX < 90) ? 13 : (-1);
        }

        if (tile.TileType == TileID.Cloud)
        {
            dustType = DustID.Cloud;
        }

        if (tile.TileType == TileID.SnowCloud)
        {
            dustType = DustID.Cloud;
        }

        if (tile.TileType == TileID.OasisPlants)
        {
            dustType = Main.tile[i, j + 2 - (tile.TileFrameY / 18)].TileType switch
            {
                TileID.Pearlsand => DustID.HallowedPlants,
                TileID.Crimsand => DustID.CrimsonPlants,
                TileID.Ebonsand => DustID.CorruptPlants,
                _ => (tile.TileFrameX >= 270) ? 291 : 40,
            };
        }

        if (tile.TileType == TileID.LilyPad)
        {
            if (tile.TileFrameY == 0)
            {
                dustType = DustID.GrassBlades;
            }
            else if (tile.TileFrameY == 18)
            {
                dustType = DustID.HallowedPlants;
            }
            else if (tile.TileFrameY == 36)
            {
                dustType = DustID.t_Cactus;
            }
        }
        else if (tile.TileType == TileID.Cattail)
        {
            if (tile.TileFrameY == 0)
            {
                dustType = DustID.GrassBlades;
            }
            else if (tile.TileFrameY == 18)
            {
                dustType = DustID.t_Cactus;
            }
            else if (tile.TileFrameY == 36)
            {
                dustType = DustID.HallowedPlants;
            }
            else if (tile.TileFrameY == 54)
            {
                dustType = DustID.CrimsonPlants;
            }
            else if (tile.TileFrameY == 72)
            {
                dustType = DustID.CorruptPlants;
            }
            else if (tile.TileFrameY == 90)
            {
                dustType = DustID.Bone;
            }
        }
        else if (tile.TileType == TileID.CorruptVines)
        {
            dustType = DustID.CorruptPlants;
        }
        else if (tile.TileType == TileID.MushroomVines)
        {
            dustType = DustID.Bone;
        }

        if (tile.TileType == TileID.Heart)
        {
            dustType = DustID.LifeCrystal;
        }

        if (tile.TileType == TileID.ManaCrystal)
        {
            dustType = 48;
        }

        if (tile.TileType is TileID.Plants or TileID.Plants2)
        {
            dustType = DustID.GrassBlades;
        }

        if (tile.TileType == TileID.Glass)
        {
            dustType = 13;
        }

        if (tile.TileType is TileID.Demonite or TileID.DemoniteBrick)
        {
            dustType = 14;
        }

        if (tile.TileType == TileID.ClayPot)
        {
            dustType = 22;
        }

        if (tile.TileType is TileID.Pots or TileID.PotsEcho)
        {
            dustType = 22;
            if (tile.TileFrameY is >= 72 and <= 90)
            {
                dustType = DustID.Stone;
            }

            if (tile.TileFrameY is >= 144 and <= 234)
            {
                dustType = 48;
            }

            if (tile.TileFrameY is >= 252 and <= 358)
            {
                dustType = 85;
            }

            if (tile.TileFrameY is >= 360 and <= 466)
            {
                dustType = DustID.Bone;
            }

            if (tile.TileFrameY is >= 468 and <= 574)
            {
                dustType = DustID.Ash;
            }

            if (tile.TileFrameY is >= 576 and <= 790)
            {
                dustType = DustID.CorruptGibs;
            }

            if (tile.TileFrameY is >= 792 and <= 898)
            {
                dustType = DustID.t_Flesh;
            }

            if (tile.TileFrameY is >= 900 and <= 1006)
            {
                dustType = DustID.Dirt;
            }

            if (tile.TileFrameY is >= 1008 and <= 1114)
            {
                dustType = DustID.t_Lihzahrd;
            }

            if (tile.TileFrameY is >= 1116 and <= 1222)
            {
                dustType = DustID.MarblePot;
            }

            if (tile.TileFrameY is >= 1224 and <= 1330)
            {
                dustType = DustID.DesertPot;
            }
        }

        if (tile.TileType == TileID.CorruptIce)
        {
            dustType = 118;
        }

        if (tile.TileType == TileID.HallowedIce)
        {
            dustType = 119;
        }

        if (tile.TileType == TileID.FleshIce)
        {
            dustType = 120;
        }

        if (tile.TileType is TileID.Palladium or TileID.PalladiumColumn)
        {
            dustType = 144;
        }

        if (tile.TileType is TileID.Orichalcum or TileID.BubblegumBlock)
        {
            dustType = 145;
        }

        if (tile.TileType is TileID.Titanium or TileID.Titanstone)
        {
            dustType = 146;
        }

        if (tile.TileType == TileID.Slush)
        {
            dustType = 149;
        }

        if (tile.TileType == TileID.Hive)
        {
            dustType = DustID.t_Honey;
        }

        if (tile.TileType == TileID.HoneyBlock)
        {
            dustType = 153;
        }

        if (tile.TileType == TileID.Larva)
        {
            dustType = 153;
            if (WorldGen.genRand.NextBool(3))
            {
                dustType = DustID.Bone;
            }
        }

        if (tile.TileType == TileID.LihzahrdBrick)
        {
            dustType = DustID.t_Lihzahrd;
        }

        if (tile.TileType == TileID.Bowls)
        {
            dustType = -1;
        }

        if (tile.TileType == TileID.PiggyBank)
        {
            dustType = DustID.t_Meteor;
        }

        if (tile.TileType == TileID.ClayBlock)
        {
            dustType = 28;
        }

        if (tile.TileType == TileID.Books)
        {
            dustType = 22;
        }

        if (tile.TileType == TileID.Cobweb)
        {
            dustType = DustID.Web;
        }

        if (tile.TileType is TileID.Vines or TileID.VineRope)
        {
            dustType = DustID.GrassBlades;
        }

        if (tile.TileType is TileID.Sand or TileID.Coral or TileID.SandstoneBrick or TileID.Sunplate or TileID.SandStoneSlab or TileID.ShellPile)
        {
            dustType = DustID.Sand;
        }

        if (tile.TileType is TileID.Obsidian or TileID.EbonstoneBrick)
        {
            dustType = 37;
        }

        if (tile.TileType is TileID.ObsidianBrick or TileID.AncientObsidianBrick)
        {
            dustType = 109;
        }

        if (tile.TileType is TileID.Ash or TileID.IridescentBrick or TileID.Explosives or TileID.Crimsand or TileID.AshWood or TileID.TNTBarrel)
        {
            dustType = DustID.Ash;
        }

        if (tile.TileType is TileID.Mud or TileID.Mudstone)
        {
            dustType = 38;
        }

        if (tile.TileType is TileID.JunglePlants or TileID.JungleVines or TileID.JunglePlants2 or TileID.Cactus or TileID.CactusBlock or TileID.PlantDetritus or TileID.LifeFruit or TileID.LivingMahoganyLeaves or TileID.PlantDetritus2x2Echo or TileID.PlantDetritus3x2Echo)
        {
            dustType = DustID.t_Cactus;
        }

        if (tile.TileType == TileID.AntlionLarva)
        {
            dustType = DustID.Sand;
        }

        if (tile.TileType == TileID.PlanteraBulb)
        {
            dustType = (!WorldGen.genRand.NextBool(3)) ? 166 : 167;
        }

        if (tile.TileType == TileID.JungleThorns)
        {
            dustType = 7;
        }

        if (tile.TileType == TileID.PlanteraThorns)
        {
            dustType = 166;
        }

        if (tile.TileType is TileID.MushroomPlants or TileID.MushroomTrees or TileID.MushroomBlock or TileID.MushroomBeam)
        {
            dustType = DustID.Bone;
        }

        if (tile.TileType == TileID.MushroomGrass)
        {
            dustType = DustID.CorruptPlants;
        }

        if (tile.TileType == TileID.Ebonsand)
        {
            dustType = 14;
        }

        if (tile.TileType == TileID.Silt)
        {
            dustType = 53;
        }

        if (tile.TileType == TileID.IceBlock)
        {
            dustType = DustID.t_Frozen;
        }

        if (tile.TileType == TileID.IceBrick)
        {
            dustType = DustID.t_Frozen;
        }

        if (tile.TileType == TileID.BreakableIce)
        {
            dustType = DustID.t_Frozen;
        }

        if (tile.TileType == TileID.Stalactite)
        {
            dustType = (tile.TileFrameX / 54) switch
            {
                TileID.Dirt => DustID.t_Frozen,
                TileID.Stone => DustID.Stone,
                TileID.Grass => DustID.Web,
                TileID.Plants => DustID.t_Honey,
                TileID.Torches => DustID.Stone,
                TileID.Trees => 14,
                TileID.Iron => 117,
                TileID.Copper => DustID.Sluggy,
                TileID.Gold => DustID.t_Granite,
                TileID.Silver => DustID.t_Marble,
                _ => DustID.Stone,
            };
        }

        if (tile.TileType == TileID.PoopBlock)
        {
            dustType = DustID.Poop;
        }

        if (tile.TileType == TileID.SlimeBlock)
        {
            dustType = DustID.t_Slime;
        }

        if (tile.TileType == TileID.BoneBlock)
        {
            dustType = DustID.Bone;
        }

        if (tile.TileType == TileID.FleshBlock)
        {
            dustType = DustID.t_Flesh;
        }

        if (tile.TileType == TileID.RainCloud)
        {
            dustType = 108;
        }

        if (tile.TileType == TileID.SnowCloud)
        {
            dustType = 108;
        }

        if (tile.TileType == TileID.FrozenSlimeBlock)
        {
            dustType = DustID.t_Slime;
        }

        if (tile.TileType == TileID.RedStucco)
        {
            dustType = DustID.Bone;
        }

        if (tile.TileType == TileID.YellowStucco)
        {
            dustType = DustID.Sand;
        }

        if (tile.TileType == TileID.GreenStucco)
        {
            dustType = DustID.Grass;
        }

        if (tile.TileType == TileID.GrayStucco)
        {
            dustType = DustID.Stone;
        }

        if (tile.TileType is TileID.Pearlsand or TileID.PearlstoneBrick or TileID.SnowBlock or TileID.SnowBrick)
        {
            dustType = 51;
        }

        if (tile.TileType is TileID.HallowedGrass or TileID.GolfGrassHallowed)
        {
            dustType = (!WorldGen.genRand.NextBool(2)) ? 47 : 0;
        }

        if (tile.TileType is TileID.HallowedPlants or TileID.HallowedPlants2 or TileID.HallowedVines)
        {
            dustType = DustID.HallowedPlants;
        }

        if (tile.TileType is TileID.Cobalt or TileID.CobaltBrick or TileID.AncientCobaltBrick)
        {
            dustType = 48;
        }

        if (tile.TileType is TileID.Mythril or TileID.MythrilBrick or TileID.GreenCandyCaneBlock or TileID.AncientMythrilBrick)
        {
            dustType = 49;
        }

        if (tile.TileType is TileID.Adamantite or TileID.CandyCaneBlock or TileID.AdamantiteBeam)
        {
            dustType = 50;
        }

        if (tile.TileType == TileID.AdamantiteForge)
        {
            dustType = 50;
            if (tile.TileFrameX >= 54)
            {
                dustType = 146;
            }
        }

        if (tile.TileType == TileID.MythrilAnvil)
        {
            dustType = 49;
            if (tile.TileFrameX >= 36)
            {
                dustType = 145;
            }
        }

        if (tile.TileType == TileID.HolidayLights)
        {
            dustType = 49;
        }

        if (Main.tileAlch[tile.TileType])
        {
            int frameX = tile.TileFrameX / 18;
            if (frameX == 0)
            {
                dustType = DustID.GrassBlades;
            }

            if (frameX == 1)
            {
                dustType = DustID.GrassBlades;
            }

            if (frameX == 2)
            {
                dustType = 7;
            }

            if (frameX == 3)
            {
                dustType = DustID.CorruptPlants;
            }

            if (frameX == 4)
            {
                dustType = DustID.SeaOatsOasis;
            }

            if (frameX == 5)
            {
                dustType = DustID.Torch;
            }

            if (frameX == 6)
            {
                dustType = 224;
            }
        }

        if (tile.TileType is TileID.Hellstone or TileID.HellstoneBrick or TileID.Hellforge or TileID.AncientHellstoneBrick)
        {
            dustType = (!WorldGen.genRand.NextBool(2)) ? 25 : 6;
        }

        if (tile.TileType == TileID.Meteorite)
        {
            dustType = (!WorldGen.genRand.NextBool(2)) ? 23 : 6;
        }

        if (tile.TileType == TileID.CorruptThorns)
        {
            dustType = (!WorldGen.genRand.NextBool(2)) ? 24 : 14;
        }

        if (tile.TileType == TileID.CrimsonThorns)
        {
            dustType = (!WorldGen.genRand.NextBool(3)) ? 125 : 5;
        }

        if (tile.TileType is TileID.CorruptGrass or TileID.CorruptPlants or TileID.CorruptJungleGrass)
        {
            dustType = (!WorldGen.genRand.NextBool(2)) ? 17 : 14;
        }

        if (tile.TileType is TileID.Ebonstone or TileID.ShadowOrbs)
        {
            dustType = (tile.TileType == TileID.ShadowOrbs && tile.TileFrameX >= 36) ? 5 : ((!WorldGen.genRand.NextBool(2)) ? 1 : 14);
        }

        if (tile.TileType == TileID.Saplings)
        {
            dustType = (tile.TileFrameX / 54) switch
            {
                TileID.Stone => 122,
                TileID.Grass => DustID.t_PearlWood,
                TileID.Plants => 77,
                TileID.Torches => 121,
                TileID.Trees => 79,
                _ => 7,
            };
        }

        if (tile.TileType == TileID.Sunflower)
        {
            dustType = (!WorldGen.genRand.NextBool(2)) ? 19 : 3;
        }

        if (tile.TileType == TileID.Crystals)
        {
            if (tile.TileFrameX >= 324)
            {
                dustType = DustID.PinkCrystalShard;
            }

            dustType = (tile.TileFrameX is not 0 and not 54 and not 108) ? ((tile.TileFrameX is not 18 and not 72 and not 126) ? 70 : 69) : 68;
        }

        if (tile.TileType == TileID.CrystalBlock)
        {
            dustType = WorldGen.genRand.Next(68, 71);
        }

        if (tile.TileType == TileID.Torches)
        {
            int torch = (int)MathHelper.Clamp(tile.TileFrameY / 22, 0f, TorchID.Count - 1);
            dustType = TorchID.Dust[torch];
        }

        if (tile.TileType == TileID.Jackolanterns)
        {
            dustType = 189;
            if (tile.TileFrameX < 36 && WorldGen.genRand.NextBool(2))
            {
                dustType = DustID.Torch;
            }
        }

        if ((tile.TileType == TileID.Chandeliers || tile.TileType == TileID.HangingLanterns) && WorldGen.genRand.NextBool(2))
        {
            dustType = DustID.Torch;
        }

        if (tile.TileType == TileID.FireflyinaBottle)
        {
            dustType = -1;
        }

        if (tile.TileType == TileID.LightningBuginaBottle)
        {
            dustType = -1;
        }

        if (tile.TileType == TileID.LavaflyinaBottle)
        {
            dustType = -1;
        }

        if (tile.TileType == TileID.ShimmerflyinaBottle)
        {
            dustType = -1;
        }

        if (tile.TileType is TileID.Beds or TileID.Bathtubs or TileID.Bookcases)
        {
            dustType = -1;
        }

        if (tile.TileType is TileID.Candles or TileID.Chandeliers or TileID.HangingLanterns or TileID.Lamps or TileID.Candelabras)
        {
            dustType = -1;
        }

        if (tile.TileType is TileID.BorealWood or TileID.BorealBeam)
        {
            dustType = DustID.t_BorealWood;
        }

        if (tile.TileType == TileID.PalmWood)
        {
            dustType = 215;
        }

        if (tile.TileType == TileID.AshWood)
        {
            dustType = DustID.Ash;
        }

        if (TileLoader.GetTile(tile.TileType) is ModTile modTile)
        {
            dustType = modTile.DustType;
        }

        if (TileLoader.CreateDust(i, j, tile.TileType, ref dustType) && dustType >= 0)
        {
            if (tile.TileType == TileID.RainbowMoss || tile.TileType == TileID.RainbowMossBrick || (tile.TileType == TileID.LongMoss && tile.TileFrameX / 22 == 10))
            {
                color = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB);
                alpha = 0;
                noGravity = true;
                noLightEmittence = true;
            }

            if (tile.TileType == TileID.LilyPad)
            {
                int offsetY = (int)tile.LiquidAmount / 16;
                offsetY -= 3;
                if (WorldGen.SolidTile(i, j - 1) && offsetY > 8)
                    offsetY = 8;

                offset = new Vector2(0, offsetY);
            }

            if (tile.TileType == TileID.CrimsonThorns && dustType == 5)
            {
                scale = 1.5f;
                noGravity = true;
                velocity *= 1.65f;
                fadeIn = 1.6f;
                return DustID.Blood;
            }

            if (tile.TileType is TileID.RainbowBrick or TileID.RainbowMossBlock)
            {
                color = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB);
                alpha = 100;
                scale = 0.75f;
                noGravity = true;
                return DustID.RainbowTorch;
            }

            if (tile.TileType == TileID.PalmTree)
            {
                offset = new(tile.TileFrameY, 0);
            }

            if (tile.TileType == TileID.MinecartTrack)
            {
                velocity = new(WorldGen.genRand.Next(-2, 3), WorldGen.genRand.Next(-2, 3));
                noGravity = true;
                fadeIn = scale + 1f + (0.01f * WorldGen.genRand.Next(0, 51));
                noGravity = true;
                return DustID.MinecartSpark;
            }

            if (tile.TileType is
                TileID.ExposedGems or TileID.GemLocks or
                TileID.TeamBlockRed or TileID.TeamBlockRedPlatform or
                TileID.TeamBlockBlue or TileID.TeamBlockBluePlatform or
                TileID.TeamBlockGreen or TileID.TeamBlockGreenPlatform or
                TileID.TeamBlockPink or TileID.TeamBlockPinkPlatform or
                TileID.TeamBlockWhite or TileID.TeamBlockWhitePlatform or
                TileID.TeamBlockYellow or TileID.TeamBlockYellowPlatform
            )
            {
                noLight = true;
                scale = 0.75f;
                alpha = 75;
            }

            if (tile.TileType == TileID.SlimeBlock || (tile.TileType == TileID.WorkBenches && dustType == 4) || (tile.TileType == TileID.Platforms && dustType == 4))
            {
                color = new Color(0, 80, 255, 100);
                alpha = 75;
                scale = 0.75f;
            }

            if (tile.TileType == TileID.FrozenSlimeBlock)
            {
                color = new Color(97, 200, 255, 100);
                alpha = 75;
                scale = 0.75f;
            }

            if (tile.TileType == TileID.SmallPiles && dustType >= 86 && dustType <= 91)
            {
                noLight = true;
                alpha = 75;
                scale = 0.75f;
            }

            if (tile.TileType == TileID.Torches && dustType == 66)
            {
                noGravity = true;
                color = new Color(Main.DiscoR / 255f, Main.DiscoG / 255f, Main.DiscoB / 255f);
                alpha = 0;
            }

            if (dustType == 139)
            {
                return dustType + Main.rand.Next(4);
            }

            return dustType;
        }

        return -1;
    }

}
