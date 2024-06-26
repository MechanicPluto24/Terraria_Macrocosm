using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macrocosm.Common.Bases.Tiles
{
    internal interface IModifyTileFrame
    {
        public void ModifyTileFrame(int i, int j, ref int up, ref int down, ref int left, ref int right, ref int upLeft, ref int upRight, ref int downLeft, ref int downRight);
    }
}
