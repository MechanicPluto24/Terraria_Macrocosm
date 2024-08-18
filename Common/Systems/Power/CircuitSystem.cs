using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
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

        public override void ClearWorld()
        {
            SearchCircuits();
        }

        private static void SolveCircuits()
        {
            foreach (Circuit circuit in circuits)
                circuit.Solve();
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
                        else if (machine.IsConsumer && machine.PoweredOn)
                        {
                            machine.PowerOff();
                        }
                    }
                }
            }

            SolveCircuits();
        }


        private static void WireEffects(bool foundPower, MachineTE machine, HashSet<Point16> visited)
        {
            if (foundPower && machine.PoweredOn && machine.ConsumedPower > 0)
            {
                foreach (var p in visited)
                {
                    if (Main.rand.NextBool(20) && Main.tile[p].AnyWire())
                    {
                        var d = Dust.NewDustDirect(p.ToWorldCoordinates() + new Vector2(2), 8, 8, DustID.Electric, Scale: 0.2f, SpeedX: 0, SpeedY: 0);
                        d.noGravity = false;
                    }
                }
            }
        }

        private static void DebugDrawMachines(SpriteBatch spriteBatch)
        {
            foreach (var kvp in TileEntity.ByID)
            {
                if (kvp.Value is MachineTE machine)
                {
                    string activePower = machine.ActivePower.ToString("F2");
                    string maxPower = (machine.IsGenerator ? machine.GeneratedPower : machine.ConsumedPower).ToString("F2");
                    Vector2 position = machine.Position.ToWorldCoordinates() - new Vector2(8, 16 + 8) - Main.screenPosition;
                    ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, $"{activePower}/{maxPower}", position, Color.Wheat, 0f, Vector2.Zero, Vector2.One);
                }
            }
        }
    }
}
