using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm {
	public class MProjectile : GlobalProjectile {
		public override bool PreDrawExtras(Projectile Projectile) {
			BaseArmorData.lastShaderDrawObject = Projectile;
			return base.PreDrawExtras(Projectile);
		}		
	}
}