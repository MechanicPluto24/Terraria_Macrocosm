using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Global.Projectiles;

public class ShaderGlobalProjectile : GlobalProjectile
{
    public override bool InstancePerEntity => true;
    public int OverrideDyeShaderID { get; set; } = 0;

    public override void Load()
    {
        On_Main.GetProjectileDesiredShader += On_Main_GetProjectileDesiredShader;
    }

    public override void Unload()
    {
        On_Main.GetProjectileDesiredShader -= On_Main_GetProjectileDesiredShader;
    }

    private int On_Main_GetProjectileDesiredShader(On_Main.orig_GetProjectileDesiredShader orig, Projectile proj)
    {
        if (proj.TryGetGlobalProjectile<ShaderGlobalProjectile>(out var shaderGlobalProjectile) && shaderGlobalProjectile.OverrideDyeShaderID > 0)
            return shaderGlobalProjectile.OverrideDyeShaderID;

        return orig(proj);
    }
}
