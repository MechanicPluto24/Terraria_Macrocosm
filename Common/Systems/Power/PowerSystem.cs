using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using Microsoft.Xna.Framework;
using Macrocosm.Common.Utils;
using System.Collections.Generic;
using System;
using Terraria.ID;
using System.Reflection.PortableExecutable;

namespace Macrocosm.Common.Systems.Power
{
    public class PowerSystem : ModSystem
    {
        private class Circuit
        {
            public HashSet<MachineTE> Machines { get; private set; } = new HashSet<MachineTE>();

            public void Add(MachineTE machine)
            {
                Machines.Add(machine);
            }

            public bool Containts(MachineTE machine)
            {
                return Machines.Contains(machine);
            }
        }

        private List<Circuit> circuits = new();

        public override void PostUpdateEverything()
        {
            circuits.Clear();

            foreach (var kvp in TileEntity.ByID)
            {
                if (kvp.Value is MachineTE machine)
                {
                    foreach(Circuit circuit in circuits)
                    {
                    }
                }
            }


            ResetPower();
            UpdatePowerState();
        }

        private static void ResetPower()
        {
            foreach (var kvp in TileEntity.ByID)
            {
                if (kvp.Value is MachineTE machine)
                {
                }
            }
        }

        private void UpdatePowerState()
        {
            foreach (var kvp in TileEntity.ByID)
            {
                if (kvp.Value is MachineTE machine)
                {
                    if(machine.IsConsumer)
                    {
                        bool foundPower = false;
                        if (foundPower && !machine.PoweredUp)
                            machine.PowerUp();

                        if (!foundPower && machine.PoweredUp)
                            machine.PowerDown();
                    }


                    if (machine.IsGenerator)
                    {
                        if (machine.GeneratedPower > 0 && !machine.PoweredUp)
                            machine.PowerUp();

                        if (machine.GeneratedPower <= 0 && machine.PoweredUp)
                            machine.PowerDown();
                    }
                }
            }
        }

        private void WireEffects(bool foundPower, MachineTE machine, HashSet<Point16> visited)
        {
            if (foundPower && machine.PoweredUp && machine.ConsumedPower > 0)
            {
                foreach (var p in visited)
                {
                    if (Main.rand.NextBool(20) && PowerWiring.Map[p].AnyWire)
                    {
                        var d = Dust.NewDustDirect(p.ToWorldCoordinates() + new Vector2(2), 8, 8, DustID.Electric, Scale: 0.2f, SpeedX: 0, SpeedY: 0);
                        d.noGravity = false;
                    }
                }
            }
        }

        public override void PostDrawTiles()
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, default, default, null, Main.GameViewMatrix.ZoomMatrix);

            DebugDrawMachines(spriteBatch);

            spriteBatch.End();
        }

        private void DebugDrawMachines(SpriteBatch spriteBatch)
        {
            foreach (var kvp in TileEntity.ByID)
            {
                if (kvp.Value is MachineTE machine)
                {
                    string text = $"{(machine.IsGenerator ? machine.GeneratedPower : machine.ConsumedPower)}";
                    Vector2 position = machine.Position.ToWorldCoordinates() - new Vector2(8, 16 + 8) - Main.screenPosition;
                    ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, text, position, Color.Wheat, 0f, Vector2.Zero, Vector2.One);
                }
            }
        }
    }
}
