using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace Macrocosm.Common.Systems.Power
{
    public class CircuitSystem : ModSystem
    {
        private static readonly Dictionary<int, Circuit> machineToCircuit = new();
        private static readonly List<Circuit> circuits = new();

        public override void Load()
        {
            On_WorldGen.PlaceWire += On_WorldGen_PlaceWire;
            On_WorldGen.PlaceWire2 += On_WorldGen_PlaceWire2;
            On_WorldGen.PlaceWire3 += On_WorldGen_PlaceWire3;
            On_WorldGen.PlaceWire4 += On_WorldGen_PlaceWire4;
            On_WorldGen.KillWire += On_WorldGen_KillWire;
            On_WorldGen.KillWire2 += On_WorldGen_KillWire2;
            On_WorldGen.KillWire3 += On_WorldGen_KillWire3;
            On_WorldGen.KillWire4 += On_WorldGen_KillWire4;
        }

        public override void Unload()
        {
            On_WorldGen.PlaceWire -= On_WorldGen_PlaceWire;
            On_WorldGen.PlaceWire2 -= On_WorldGen_PlaceWire2;
            On_WorldGen.PlaceWire3 -= On_WorldGen_PlaceWire3;
            On_WorldGen.PlaceWire4 -= On_WorldGen_PlaceWire4;
            On_WorldGen.KillWire -= On_WorldGen_KillWire;
            On_WorldGen.KillWire2 -= On_WorldGen_KillWire2;
            On_WorldGen.KillWire3 -= On_WorldGen_KillWire3;
            On_WorldGen.KillWire4 -= On_WorldGen_KillWire4;
        }

        public override void ClearWorld()
        {
            SearchCircuits();
        }

        private static int updateTimer = 0;
        private const int updateRate = 1;
        public override void PostUpdateEverything()
        {
            if (updateTimer++ >= updateRate)
            {
                SolveCircuits();
                updateTimer = 0;
            }
        }

        public static void SearchCircuits()
        {
            machineToCircuit.Clear();
            circuits.Clear();

            foreach (var kvp in TileEntity.ByID)
            {
                if (kvp.Value is MachineTE machine)
                {
                    if (!machineToCircuit.ContainsKey(machine.ID))
                    {
                        Circuit circuit = new();

                        Queue<MachineTE> queue = new();
                        queue.Enqueue(machine);

                        while (queue.Count > 0)
                        {
                            MachineTE currentMachine = queue.Dequeue();
                            circuit.Add(currentMachine);
                            machineToCircuit[currentMachine.ID] = circuit;

                            foreach (var other in TileEntity.ByID.Values.OfType<MachineTE>())
                            {
                                if (currentMachine.IsConnected(other) && !circuit.Contains(other))
                                {
                                    queue.Enqueue(other);
                                }
                            }
                        }

                        if (circuit.NodeCount > 1)
                        {
                            circuits.Add(circuit);
                        }
                        else if (machine.MachineType is MachineType.Consumer && machine.PoweredOn)
                        {
                            machine.PowerOff();
                        }
                    }
                }
            }

            SolveCircuits();
        }

        private static void SolveCircuits()
        {
            foreach (Circuit circuit in circuits)
                circuit.Solve();
        }

        private static void DebugDrawMachines(SpriteBatch spriteBatch)
        {
        }

        private bool On_WorldGen_PlaceWire(On_WorldGen.orig_PlaceWire orig, int i, int j)
        {
            SearchCircuits();
            return orig(i, j);
        }

        private bool On_WorldGen_PlaceWire2(On_WorldGen.orig_PlaceWire2 orig, int i, int j)
        {
            SearchCircuits();
            return orig(i, j);
        }

        private bool On_WorldGen_PlaceWire3(On_WorldGen.orig_PlaceWire3 orig, int i, int j)
        {
            SearchCircuits();
            return orig(i, j);
        }

        private bool On_WorldGen_PlaceWire4(On_WorldGen.orig_PlaceWire4 orig, int i, int j)
        {
            SearchCircuits();
            return orig(i, j);
        }

        private bool On_WorldGen_KillWire(On_WorldGen.orig_KillWire orig, int i, int j)
        {
            SearchCircuits();
            return orig(i, j);
        }

        private bool On_WorldGen_KillWire2(On_WorldGen.orig_KillWire2 orig, int i, int j)
        {
            SearchCircuits();
            return orig(i, j);
        }

        private bool On_WorldGen_KillWire3(On_WorldGen.orig_KillWire3 orig, int i, int j)
        {
            SearchCircuits();
            return orig(i, j);
        }

        private bool On_WorldGen_KillWire4(On_WorldGen.orig_KillWire4 orig, int i, int j)
        {
            SearchCircuits();
            return orig(i, j);
        }

        public override bool HijackGetData(ref byte messageType, ref BinaryReader reader, int playerNumber)
        {
            if (messageType is MessageID.TileManipulation or MessageID.TileSquare or MessageID.MassWireOperation)
                SearchCircuits();

            return false;
        }
    }
}
