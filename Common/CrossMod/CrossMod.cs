using Macrocosm.Common.CrossMod;
using Macrocosm.Common.Systems.Flags;
using Macrocosm.Content.Items.Armor.Vanity.BossMasks;
using Macrocosm.Content.Items.Consumables.BossSummons;
using Macrocosm.Content.Items.MusicBoxes;
using Macrocosm.Content.Items.Pets;
using Macrocosm.Content.Items.Relics;
using Macrocosm.Content.Items.Trophies;
using Macrocosm.Content.NPCs.Bosses.CraterDemon;
using Macrocosm.Content.Subworlds;
using Macrocosm.Content.Tiles.Blocks.Terrain;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SubworldLibrary;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Common.CrossMod
{
    public partial class CrossMod : ModSystem
    {
        public static ModEntry Redemption { get; } = new("Redemption");
        public static ModEntry BossChecklist { get; } = new("BossChecklist");
        public static ModEntry Fargowiltas { get; } = new("Fargowiltas");
        public static ModEntry MusicDisplay { get; } = new("MusicDisplay");
        public static ModEntry TerrariaAmbience { get; } = new("TerrariaAmbience");
        public static ModEntry TerrariaAmbienceAPI { get; } = new("TerrariaAmbienceAPI");
        public static ModEntry Wikithis { get; } = new("Wikithis");
        public static ModEntry TMLAchievements { get; } = new("TMLAchievements");

        /// <summary> The names and instances of loaded crossmod mods per <see cref="ModEntry"/>. </summary>
        public static Dictionary<string, Mod> LoadedMods { get; } = [];

        public override void PostSetupContent()
        {
            Call_BossChecklist();
            Call_Fargowiltas();
            Call_MusicDisplay();
            Call_Wikithis();
            Call_TerrariaAmbience();

            foreach (var achievement in Mod.GetContent<TMLAchievement>())
                achievement.PostSetupContent();
        }

        private void Call_BossChecklist()
        {
            #region Vanilla value reference
            /*
            KingSlime = 1f;
            EyeOfCthulhu = 2f;
            EaterOfWorlds = 3f;  
            BrainOfCthulhu = 3f;  
            QueenBee = 4f;
            Skeletron = 5f;
            DeerClops = 6f;
            WallOfFlesh = 7f;
            QueenSlime = 8f;
            TheTwins = 9f;
            TheDestroyer = 10f;
            SkeletronPrime = 11f;
            Plantera = 12f;
            Golem = 13f;
            Betsy = 14f;
            EmpressOfLight = 15f;
            DukeFishron = 16f;
            LunaticCultist = 17f;
            Moonlord = 18f;
            */
            #endregion

            if (BossChecklist.Enabled)
            {
                #region CraterDemon
                BossChecklist.Instance.Call("LogBoss", Mod, nameof(CraterDemon), 20f, () => WorldData.DownedCraterDemon, ModContent.NPCType<CraterDemon>(), new Dictionary<string, object>()
                {
                    ["spawnItems"] = ModContent.ItemType<CraterDemonSummon>(),
                    ["collectibles"] = new List<int>
                        {
                            ModContent.ItemType<CraterDemonRelic>(),
                            ModContent.ItemType<CraterDemonPet>(),
                            ModContent.ItemType<CraterDemonTrophy>(),
                            ModContent.ItemType<CraterDemonMask>(),
                            ModContent.ItemType<SpaceInvaderMusicBox>()
                        },
                    ["availability"] = () => true,
                    ["spawnInfo"] = ModContent.GetInstance<CraterDemon>().GetLocalization("SpawnInfo"),
                    ["despawnMessage"] = (NPC npc) => npc.ModNPC.GetLocalization("DespawnMessage").WithFormatArgs(npc.FullName),
                    ["customPortrait"] = (SpriteBatch spriteBatch, Rectangle rect, Color color) =>
                    {
                        Texture2D texture = ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/BossChecklist/CraterDemon").Value;
                        Vector2 centered = new(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
                        spriteBatch.Draw(texture, centered, color);
                    }
                });
                #endregion
            }
        }

        private void Call_Fargowiltas()
        {
            if (Fargowiltas.Enabled)
            {
                Fargowiltas.Instance.Call("AddSummon", 19.0f, "Macrocosm", "CraterDemonSummon", () => WorldData.DownedCraterDemon, Item.buyPrice(gold: 15));
            }
        }

        private void Call_MusicDisplay()
        {
            if (MusicDisplay.Enabled)
            {
                void AddMusic(string file, string name, string author) => MusicDisplay.Instance.Call("AddMusic", (short)MusicLoader.GetMusicSlot(Mod, "Assets/Music/" + file), name, "by " + author, nameof(Macrocosm));

                AddMusic("Deadworld", "\"Deadworld\" - Theme of the Moon (Day)", "Lincoln Ennis");
                AddMusic("Requiem", "\"Requiem\" - Theme of the Moon (Night)", "Lincoln Ennis");
                AddMusic("Stygia", "\"Stygia\" - Theme of the Moon (Underground)", "Lincoln Ennis");
                AddMusic("SpaceInvader", "\"Space Invader\" - Theme of Crater Demon", "Lincoln Ennis");
                AddMusic("IntoTheUnknown", "\"Into The Unknown\" - Macrocosm - Title Theme", "Lincoln Ennis");
            }
        }

        private void Call_TerrariaAmbience()
        {
            if (TerrariaAmbience.Enabled)
            {
                TerrariaAmbience.Instance.Call("AddTilesToList", null, "Stone", Array.Empty<string>(), new int[]
                {
                    ModContent.TileType<Regolith>(),
                    ModContent.TileType<Protolith>(),
                    ModContent.TileType<Cynthalith>(),
                    ModContent.TileType<IrradiatedRock>(),
                    ModContent.TileType<Astrolith>()
                });
            }

            if (TerrariaAmbienceAPI.Enabled)
            {
                TerrariaAmbienceAPI.Instance.Call("Ambience", Mod, "MoonAmbience", "Assets/Sounds/Ambient/Moon", 1f, 0.0075f, new Func<bool>(SubworldSystem.IsActive<Moon>));
            }
        }

        private void Call_Wikithis()
        {
            if (Wikithis.Enabled && !Main.dedServ)
            {
                Wikithis.Instance.Call("AddModURL", Mod, "https://terrariamods.wiki.gg/wiki/Macrocosm/{}");
            }
        }
    }
}
