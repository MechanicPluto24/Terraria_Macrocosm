using Macrocosm.Common.Bases.Tiles;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.TileFrame
{
    public class TileFrameHooks : ModSystem
    {
        public override void Load()
        {
            IL_WorldGen.TileFrame += IL_WorldGen_TileFrame;
        }

        public override void Unload()
        {
            IL_WorldGen.TileFrame -= IL_WorldGen_TileFrame;
        }

        public override void SetStaticDefaults()
        {
            for (int type = 0; type < TileLoader.TileCount; type++)
            {
                if (TileLoader.GetTile(type) is IModifyTileFrame)
                    TileID.Sets.ChecksForMerge[type] = true;
            }
        }

        private void IL_WorldGen_TileFrame(ILContext il)
        {
            var c = new ILCursor(il);

            // Match:
            // TileMergeAttempt(num, Main.tileMerge[num], ref up, ref down, ref left, ref right, ref upLeft, ref upRight, ref downLeft, ref downRight);
            if (!c.TryGotoNext(MoveType.After,
                    i => i.OpCode == OpCodes.Ldloc_3, // ldloc.3
                    i => i.OpCode == OpCodes.Ldsfld && i.Operand is FieldReference field && field.Name == "tileMerge", // ldsfld bool[][] Terraria.Main::tileMerge
                    i => i.OpCode == OpCodes.Ldloc_3, // ldloc.3
                    i => i.OpCode == OpCodes.Ldelem_Ref, // ldelem.ref
                    i => i.MatchLdloca(out _), // ldloca.s up
                    i => i.MatchLdloca(out _), // ldloca.s down
                    i => i.MatchLdloca(out _), // ldloca.s left
                    i => i.MatchLdloca(out _), // ldloca.s right
                    i => i.MatchLdloca(out _), // ldloca.s upLeft
                    i => i.MatchLdloca(out _), // ldloca.s upRight
                    i => i.MatchLdloca(out _), // ldloca.s downLeft
                    i => i.MatchLdloca(out _), // ldloca.s downRight
                    i => i.MatchCall("Terraria.WorldGen", "TileMergeAttempt")
                ))
            {
                Macrocosm.Instance.Logger.Error("Failed to inject ILHook: Terraria.WorldGen.TileFrame");
                return;
            }

            c.Emit(OpCodes.Ldarg_0); // i
            c.Emit(OpCodes.Ldarg_1); // j
            c.Emit(OpCodes.Ldloca, 84); // up
            c.Emit(OpCodes.Ldloca, 89); // down
            c.Emit(OpCodes.Ldloca, 86); // left
            c.Emit(OpCodes.Ldloca, 87); // right
            c.Emit(OpCodes.Ldloca, 83); // upLeft
            c.Emit(OpCodes.Ldloca, 85); // upRight
            c.Emit(OpCodes.Ldloca, 88); // downLeft
            c.Emit(OpCodes.Ldloca, 90); // downRight

            c.EmitDelegate(ModifyTileFrame);
        }

        public void ModifyTileFrame(int i, int j, ref int up, ref int down, ref int left, ref int right, ref int upLeft, ref int upRight, ref int downLeft, ref int downRight)
        {
            Tile tile = Main.tile[i, j];
            
            if(TileLoader.GetTile(tile.TileType) is IModifyTileFrame modifyTile)
                modifyTile.ModifyTileFrame(i,j, ref up, ref down, ref left, ref right, ref upLeft, ref upRight, ref downLeft, ref downRight);
        }
    }
}
