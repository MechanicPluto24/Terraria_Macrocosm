using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria.ModLoader;

namespace Macrocosm.Common.Customization
{
    public readonly struct Detail
    {
        public string Name { get; }
        public string Context { get; }

        public string TexturePath => GetType().Namespace.Replace('.', '/') + "/Details/" + Context + "/" + Name;
        public string IconTexturePath => GetType().Namespace.Replace('.', '/') + "/Details/Icons/" + Name;

        private readonly Asset<Texture2D> texture;
        private readonly Asset<Texture2D> iconTexture;

        public Asset<Texture2D> Texture => texture;
        public Asset<Texture2D> IconTexture => iconTexture;


        public Detail(string context, string patternName)
        {
            Name = patternName;
            Context = context;

            if (ModContent.RequestIfExists(TexturePath, out Asset<Texture2D> detailTexture))
                texture = detailTexture;
            else
                texture = Macrocosm.EmptyTex;

            if (ModContent.RequestIfExists(IconTexturePath, out Asset<Texture2D> detailIconTexture))
                iconTexture = detailIconTexture;
            else
                iconTexture = Macrocosm.EmptyTex;
        }

        public override bool Equals(object obj)
        {
            return obj is Detail detail &&
                   Name == detail.Name;
        }

        public static bool operator ==(Detail left, Detail right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Detail left, Detail right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Context);
        }
    }
}
