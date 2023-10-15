using Macrocosm.Common.Bases;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Security.Cryptography.X509Certificates;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Melee
{
    public class Procellarum_HalberdProjectile : HalberdProjectile
    {
        public override int baseSpeed => 30;
        public override int regularHoldOffset => 28;
        public override int midHoldOffset => -20;
        public override int farHoldOffset => -72;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 200;
            Projectile.height = 200;
        }
    }
}
