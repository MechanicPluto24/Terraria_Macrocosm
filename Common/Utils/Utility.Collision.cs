using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;

namespace Macrocosm.Common.Utils
{
    public partial class Utility
    {
        public static Vector2 Cast(Vector2 start, Vector2 direction, float length, bool platformCheck = false)
        {
            Vector2 output = start;
            direction.Normalize();
            for (int i = 0; i < length; i++)
            {
                if (Collision.CanHitLine(output, 0, 0, output + direction, 0, 0) && (platformCheck ? !Collision.SolidTiles(output, 1, 1, platformCheck) && Main.tile[(int)output.X / 16, (int)output.Y / 16].TileType != TileID.Platforms : true))
                    output += direction;
                else
                    break;
            }

            return output;
        }

        public static float CastLength(Vector2 start, Vector2 direction, float length, bool platformCheck = false)
        {
            Vector2 end = Cast(start, direction, length, platformCheck);
            return (end - start).Length();
        }
    }
}
