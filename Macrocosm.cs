using Macrocosm.Common.Config;
using Macrocosm.Common.Netcode;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;

namespace Macrocosm
{
    public class Macrocosm : Mod
    {
        public static Mod Instance => ModContent.GetInstance<Macrocosm>();

        public const string TexturesPath = "Macrocosm/Assets/Textures/";
        public const string EmptyTexPath = TexturesPath + "Empty";
        public const string SymbolsPath = TexturesPath + "UI/Symbols/";
        public const string ButtonsPath = TexturesPath + "UI/Buttons/";
        public static string TextureEffectsPath => TexturesPath + (MacrocosmConfig.Instance.HighResolutionEffects ? "HighRes/" : "LowRes/");

        public const string ShadersPath = "Macrocosm/Assets/Effects/";
        public const string MusicPath = "Macrocosm/Assets/Music/";
        public const string SFXPath = "Macrocosm/Assets/Sounds/SFX/";

        public const int ItemShoot_UsesAmmo = 10;

        public static Asset<Texture2D> EmptyTex { get; set; }
        public static Type[] GetTypes() => AssemblyManager.GetLoadableTypes(Instance.Code);

        public override void Load()
        {
            if (!Main.dedServ)
            {
                EmptyTex = ModContent.Request<Texture2D>(EmptyTexPath);

                LoadResprites();
                LoadEffects();
            }
        }

        public override void Unload()
        {
            UnloadResprites();
            UnloadEffects();
        }

        public override void PostSetupContent()
        {
        }

        private static void LoadResprites()
        {
            string respritePath = TexturesPath + "Resprites/";
            TextureAssets.Moon[0] = ModContent.Request<Texture2D>(respritePath + "Moon_0");
        }

        private static void UnloadResprites()
        {
            TextureAssets.Moon[0] = Main.Assets.Request<Texture2D>("Images/Moon_0", AssetRequestMode.ImmediateLoad);
        }

        private static void LoadEffects()
        {
            AssetRequestMode mode = AssetRequestMode.ImmediateLoad;

            Filters.Scene["Macrocosm:RadiationNoise"] = new Filter(new ScreenShaderData(ModContent.Request<Effect>(ShadersPath + "RadiationNoise", mode), "RadiationNoise"));
            Filters.Scene["Macrocosm:RadiationNoise"].Load();
        }

        private static void UnloadEffects()
        {
            // What goes here?
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            PacketHandler.HandlePacket(reader, whoAmI);
        }
    }
}