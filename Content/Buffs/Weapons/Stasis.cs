using Macrocosm.Common.Bases.Buffs;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Macrocosm.Content.Buffs.Weapons;

public class Stasis : ComplexBuff
{
    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = true;
        Main.buffNoSave[Type] = true;
        Main.buffNoTimeDisplay[Type] = false;
        Main.pvpBuff[Type] = true;
    }

    public override void Update(Player player, ref int buffIndex)
    {
        player.moveSpeed *= 0.3f;
    }

    public override void AI(NPC npc)
    {
        if (npc.velocity.LengthSquared() > 2 * 2)
        {
            npc.velocity.X = MathHelper.Lerp(npc.velocity.X * 0.5f, npc.velocity.X, 0.01f);
            npc.velocity.Y = MathHelper.Lerp(0f, npc.velocity.Y, 0.01f);
        }
    }

    public override void DrawEffects(NPC npc, ref Color drawColor)
    {
        drawColor = Color.Lerp(drawColor, Color.Cyan, 0.5f);

        if (!npc.dontTakeDamage)
            DustEffects(npc);
    }

    public override void DrawEffects(Player player, PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
    {
        Vector3 cyan = Color.Cyan.ToVector3();
        r = MathHelper.Lerp(r, cyan.X, 0.5f);
        g = MathHelper.Lerp(r, cyan.Y, 0.5f);
        b = MathHelper.Lerp(r, cyan.Z, 0.5f);

        int dustIndex = DustEffects(player);
        if(dustIndex > 0)
            drawInfo.DustCache.Add(dustIndex);
    }

    private int DustEffects(Entity entity)
    {
        if (Main.rand.NextBool())
            return Dust.NewDust(entity.position, entity.width, entity.height, ModContent.DustType<Dusts.StasisDust>(), Scale: Main.rand.NextFloat(0.6f, 0.8f));

        return -1;
    }
}
