using Macrocosm.Common.Bases.Tiles;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.TileFrame
{
    // TODO: disable when tML ModifyFrameMerge is available
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
            // Fallback, each tile should set them in anticipation to tML Sept stable update which adds a similar hook
            for (int type = 0; type < TileLoader.TileCount; type++)
            {
                if (TileLoader.GetTile(type) is IModifyTileFrame)
                    TileID.Sets.ChecksForMerge[type] = true;
            }
        }

        private void IL_WorldGen_TileFrame(ILContext il)
        {
            var c = new ILCursor(il);

            int up = 0;
            int down = 0;
            int left = 0;
            int right = 0; 
            int upLeft = 0;
            int upRight = 0;
            int downLeft = 0;
            int downRight = 0;

            // Match:
            // TileMergeAttempt(num, Main.tileMerge[num], ref up, ref down, ref left, ref right, ref upLeft, ref upRight, ref downLeft, ref downRight);
            if (!c.TryGotoNext(MoveType.After,
                    i => i.OpCode == OpCodes.Ldloc_3, // ldloc.3
                    i => i.OpCode == OpCodes.Ldsfld && i.Operand is FieldReference field && field.Name == "tileMerge", // ldsfld bool[][] Terraria.Main::tileMerge
                    i => i.OpCode == OpCodes.Ldloc_3, // ldloc.3
                    i => i.OpCode == OpCodes.Ldelem_Ref, // ldelem.ref
                    i => i.MatchLdloca(out up), 
                    i => i.MatchLdloca(out down),  
                    i => i.MatchLdloca(out left),  
                    i => i.MatchLdloca(out right), 
                    i => i.MatchLdloca(out upLeft),  
                    i => i.MatchLdloca(out upRight), 
                    i => i.MatchLdloca(out downLeft),  
                    i => i.MatchLdloca(out downRight),  
                    i => i.MatchCall("Terraria.WorldGen", "TileMergeAttempt")
                ))
            {
                Macrocosm.Instance.Logger.Error("Failed to inject ILHook: Terraria.WorldGen.TileFrame");
                return;
            }

            c.Emit(OpCodes.Ldarg_0); // i
            c.Emit(OpCodes.Ldarg_1); // j
            c.Emit(OpCodes.Ldloca, up);
            c.Emit(OpCodes.Ldloca, down);
            c.Emit(OpCodes.Ldloca, left);
            c.Emit(OpCodes.Ldloca, right);
            c.Emit(OpCodes.Ldloca, upLeft);
            c.Emit(OpCodes.Ldloca, upRight);
            c.Emit(OpCodes.Ldloca, downLeft);
            c.Emit(OpCodes.Ldloca, downRight);

            c.EmitDelegate(ModifyTileFrame);
        }

        public void ModifyTileFrame(int i, int j, ref int up, ref int down, ref int left, ref int right, ref int upLeft, ref int upRight, ref int downLeft, ref int downRight)
        {
            Tile tile = Main.tile[i, j];

            if (TileLoader.GetTile(tile.TileType) is IModifyTileFrame modifyTile)
                modifyTile.ModifyTileFrame(i, j, ref up, ref down, ref left, ref right, ref upLeft, ref upRight, ref downLeft, ref downRight);
        }
    }
}
