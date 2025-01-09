using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;

namespace Macrocosm.Common.Bases.Tiles
{
    public interface ICustomPaintingSettingsTile
    {
        public TreePaintingSettings PaintingSettings { get; }
    }
}
