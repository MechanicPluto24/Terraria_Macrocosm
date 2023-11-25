using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets.Customization
{
    public class Detail : IUnlockable
    {
        public string Name { get; set; }
        public string ModuleName { get; set; }

        public string GetKey() => ModuleName + "_" + Name;

        public bool Unlocked { get; set; }
        public bool UnlockedByDefault { get; private set; }

        public string TexturePath => GetType().Namespace.Replace('.', '/') + "/Details/" + ModuleName + "/" + Name;
        public Texture2D Texture
        {
            get
            {
                if (ModContent.RequestIfExists(TexturePath, out Asset<Texture2D> paintMask))
                    return paintMask.Value;
                else
                    return Macrocosm.EmptyTex;
            }
        }

        //public Texture2D IconTexture { get; set; }
        //public int ItemType{ get; set; }

        public Detail(string moduleName, string patternName, bool unlockedByDefault)
        {
            ModuleName = moduleName;
            Name = patternName;

            UnlockedByDefault = unlockedByDefault;
            Unlocked = unlockedByDefault;
        }

    }
}
