using System;
using Microsoft.Xna.Framework;
using Terraria.WorldBuilding;

namespace Macrocosm.Common.WorldGeneration.GenActions
{
	// By GroxTheGreat
	public class RadialDitherTopMiddle : GenAction
    {
        private int _width, _height;
        private float _innerRadius, _outerRadius;

        public RadialDitherTopMiddle(int width, int height, float innerRadius, float outerRadius)
        {
            _width = width;
            _height = height;
            _innerRadius = innerRadius;
            _outerRadius = outerRadius;
        }

        public override bool Apply(Point origin, int x, int y, params object[] args)
        {
            Vector2 value = new((float)origin.X + _width / 2, origin.Y);
            Vector2 value2 = new(x, y);
            float num = Vector2.Distance(value2, value);
            float num2 = Math.Max(0f, Math.Min(1f, (num - _innerRadius) / (_outerRadius - _innerRadius)));
            if (_random.NextDouble() > num2)
            {
                return UnitApply(origin, x, y, args);
            }
            return Fail();
        }
    }
}