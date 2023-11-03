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
        public override int baseSpeed => 60;
        //public override int regularHoldOffset => 36;
        //public override int midHoldOffset => -40;
        //public override int farHoldOffset => -72;
        //public override int rotationPoint => 38;

        public override int halberdSize => 200;
        public override int rotationOffset => 39;
        public override int startOffset => 72 - rotationOffset;
        public override int farOffset => 168 - rotationOffset;

        public override void SetDefaults()
        {
            base.SetDefaults();
        }
    }
}
