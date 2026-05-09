using Macrocosm.Common.Drawing.Sky;
using Macrocosm.Common.Events;
using Macrocosm.Common.Subworlds;
using Macrocosm.Content.Projectiles.Environment.Meteors;
using Macrocosm.Content.Skies.Ambience.Moon;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace Macrocosm.Content.Events;

public class MeteorStormEvent : MacrocosmEvent
{
    public override MacrocosmEventScope Scope => MacrocosmEventScope.SubworldLocal;

    public override MacrocosmEventState CreateState() => new MeteorStormEventState();

    public override void OnInitializeState(MacrocosmEventState state, string subworldIdOrNull)
    {
        MeteorStormEventState meteorStormState = (MeteorStormEventState)state;
        meteorStormState.WaitTimeToStart = Main.rand.Next(62000, 82000);
        meteorStormState.WaitTimeToEnd = Main.rand.Next(3600, 7200);
    }

    public override void Update(MacrocosmEventContext context, MacrocosmEventState state)
    {
        if (context.CurrentSubworld?.SupportsMeteorStorms != true)
            return;

        MeteorStormEventState meteorStormState = (MeteorStormEventState)state;
        meteorStormState.FrequencyMultiplier = meteorStormState.Active ? 300f : 1f;

        if (Main.netMode == NetmodeID.MultiplayerClient)
            return;

        meteorStormState.Counter += (int)context.WorldUpdateDelta;

        if (!meteorStormState.Active && meteorStormState.WaitTimeToStart <= meteorStormState.Counter)
            context.Start();

        if (meteorStormState.Active && meteorStormState.WaitTimeToEnd <= meteorStormState.Counter)
            context.End();

        if (context.AppliesToCurrentSubworld)
            meteorStormState.UpdateMeteorSpawn(context);
    }

    public override void OnStarted(MacrocosmEventContext context, MacrocosmEventState state)
    {
        MeteorStormEventState meteorStormState = (MeteorStormEventState)state;
        Main.NewText(Language.GetTextValue("Mods.Macrocosm.StatusMessages.MeteorStorm.Start"), Color.Gray);
        meteorStormState.Counter = 0;
        meteorStormState.WaitTimeToEnd = Main.rand.Next(3600, 7200);
    }

    public override void OnEnded(MacrocosmEventContext context, MacrocosmEventState state)
    {
        MeteorStormEventState meteorStormState = (MeteorStormEventState)state;
        Main.NewText(Language.GetTextValue("Mods.Macrocosm.StatusMessages.MeteorStorm.End"), Color.Gray);
        meteorStormState.Counter = 0;
        meteorStormState.WaitTimeToStart = Main.rand.Next(62000, 82000);
    }

    public override void SaveState(MacrocosmEventState state, TagCompound tag)
    {
        MeteorStormEventState meteorStormState = (MeteorStormEventState)state;
        tag[nameof(MeteorStormEventState.Counter)] = meteorStormState.Counter;
        tag[nameof(MeteorStormEventState.WaitTimeToStart)] = meteorStormState.WaitTimeToStart;
        tag[nameof(MeteorStormEventState.WaitTimeToEnd)] = meteorStormState.WaitTimeToEnd;
    }

    public override void LoadState(MacrocosmEventState state, TagCompound tag)
    {
        MeteorStormEventState meteorStormState = (MeteorStormEventState)state;
        if (tag.ContainsKey(nameof(MeteorStormEventState.Counter)))
            meteorStormState.Counter = tag.GetInt(nameof(MeteorStormEventState.Counter));
        if (tag.ContainsKey(nameof(MeteorStormEventState.WaitTimeToStart)))
            meteorStormState.WaitTimeToStart = tag.GetInt(nameof(MeteorStormEventState.WaitTimeToStart));
        if (tag.ContainsKey(nameof(MeteorStormEventState.WaitTimeToEnd)))
            meteorStormState.WaitTimeToEnd = tag.GetInt(nameof(MeteorStormEventState.WaitTimeToEnd));
    }

    public override void NetSendState(MacrocosmEventState state, System.IO.BinaryWriter writer)
    {
        MeteorStormEventState meteorStormState = (MeteorStormEventState)state;
        writer.Write(meteorStormState.Counter);
        writer.Write(meteorStormState.WaitTimeToStart);
        writer.Write(meteorStormState.WaitTimeToEnd);
    }

    public override void NetReceiveState(MacrocosmEventState state, System.IO.BinaryReader reader)
    {
        MeteorStormEventState meteorStormState = (MeteorStormEventState)state;
        meteorStormState.Counter = reader.ReadInt32();
        meteorStormState.WaitTimeToStart = reader.ReadInt32();
        meteorStormState.WaitTimeToEnd = reader.ReadInt32();
    }
}

public sealed class MeteorStormEventState : MacrocosmEventState
{
    public int Counter { get; set; }
    public int WaitTimeToStart { get; set; }
    public int WaitTimeToEnd { get; set; }
    public float FrequencyMultiplier { get; set; } = 1f;
    public double SpawnTimeAccumulator { get; set; }

    public void UpdateMeteorSpawn(MacrocosmEventContext context)
    {
        SpawnTimeAccumulator += context.WorldUpdateDelta;
        for (int iteration = 1; iteration <= (int)SpawnTimeAccumulator; iteration++)
        {
            int closestPlayer;
            int chance = 10000;
            float frequency = FrequencyMultiplier;

            if (Main.rand.Next(chance) < frequency)
            {
                Vector2 position = new((Main.rand.Next(Main.maxTilesX - 50) + 100) * 16, Main.rand.Next((int)(Main.maxTilesY * 0.05)) * 16);

                if (!Main.rand.NextBool(4))
                {
                    closestPlayer = Player.FindClosest(position, 1, 1);
                    if (Main.player[closestPlayer].position.Y < Main.worldSurface * 16.0 && Main.player[closestPlayer].afkCounter < 3600)
                    {
                        int offset = Main.rand.Next(1, 640);
                        position.X = Main.player[closestPlayer].position.X + Main.rand.Next(-offset, offset + 1);
                    }
                }

                if (!Collision.SolidCollision(position, 16, 16))
                {
                    float speedX = Main.rand.Next(-100, 101);
                    float speedY = Main.rand.Next(200) + 100;
                    float mult = 8 / (float)Math.Sqrt(speedX * speedX + speedY * speedY);
                    speedX *= mult;
                    speedY *= mult;

                    WeightedRandom<int> choice = new(Main.rand);
                    choice.Add(ProjectileID.FallingStar, 30.0);

                    choice.Add(ModContent.ProjectileType<MoonMeteorSmall>(), 50.0);
                    choice.Add(ModContent.ProjectileType<MoonMeteorMedium>(), 30.0);
                    choice.Add(ModContent.ProjectileType<MoonMeteorLarge>(), 12.0);

                    choice.Add(ModContent.ProjectileType<IronMeteorSmall>(), 50.0);
                    choice.Add(ModContent.ProjectileType<IronMeteorMedium>(), 30.0);
                    choice.Add(ModContent.ProjectileType<IronMeteorLarge>(), 12.0);

                    choice.Add(ModContent.ProjectileType<TitaniumMeteorSmall>(), 50.0);
                    choice.Add(ModContent.ProjectileType<TitaniumMeteorMedium>(), 30.0);
                    choice.Add(ModContent.ProjectileType<TitaniumMeteorLarge>(), 12.0);

                    choice.Add(ModContent.ProjectileType<SolarMeteor>(), 2.0);
                    choice.Add(ModContent.ProjectileType<NebulaMeteor>(), 2.0);
                    choice.Add(ModContent.ProjectileType<StardustMeteor>(), 2.0);
                    choice.Add(ModContent.ProjectileType<VortexMeteor>(), 2.0);

                    int type = choice;
                    IEntitySource source = type != ProjectileID.FallingStar ? new EntitySource_Misc("Meteor") : new EntitySource_Misc("FallingStar");
                    int damage = 1500;

                    if (type == ProjectileID.FallingStar)
                        damage = 720;
                    else if (type == ModContent.ProjectileType<MoonMeteorSmall>())
                        damage = 500;
                    else if (type == ModContent.ProjectileType<MoonMeteorMedium>())
                        damage = 1000;
                    else if (type == ModContent.ProjectileType<MoonMeteorLarge>())
                        damage = 1500;

                    Projectile.NewProjectile(source, position.X, position.Y, speedX, speedY, type, damage, 0f);
                    break;
                }
            }

            if (Main.rand.Next(chance / 3) < frequency)
            {
                Vector2 position = new((Main.rand.Next(Main.maxTilesX - 50) + 100) * 16, Main.rand.Next((int)(Main.maxTilesY * 0.05)) * 16);
                closestPlayer = Player.FindClosest(position, 1, 1);
                if (Main.player[closestPlayer].position.Y < Main.worldSurface * 16.0 && Main.player[closestPlayer].afkCounter < 3600)
                {
                    int offset = Main.rand.Next(60, 640);
                    position.X = Main.player[closestPlayer].position.X + Main.rand.Next(-offset, offset + 1);
                }

                MacrocosmAmbientSky.Instance.Spawn<MoonMeteor>(Main.player[closestPlayer], Main.rand.Next());
            }
        }

        SpawnTimeAccumulator %= 1.0;
    }
}
