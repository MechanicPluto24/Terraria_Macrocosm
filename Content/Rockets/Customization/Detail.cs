using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets.Customization
{
	public readonly struct Detail
	{
		public string Name { get;  }
        public string ModuleName { get; }


        public Detail(string moduleName, string patternName)
        {
            Name = patternName;
            ModuleName = moduleName;
        }

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

        public string IconTexturePath => GetType().Namespace.Replace('.', '/') + "/Details/Icons/" + Name;
        public Texture2D IconTexture
        {
            get
            {
                if (ModContent.RequestIfExists(IconTexturePath, out Asset<Texture2D> paintMask))
                    return paintMask.Value;
                else
                    return Macrocosm.EmptyTex;
            }
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
            return HashCode.Combine(Name, ModuleName);
        }
    }
}
