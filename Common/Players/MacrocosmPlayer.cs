﻿using Macrocosm.Common.CrossMod;
using Macrocosm.Common.Netcode;
using Macrocosm.Common.Systems;
using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Achievements;
using Macrocosm.Content.Debuffs.Environment;
using Macrocosm.Content.Subworlds;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using System.IO;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Common.Players
{
    /// <summary>
    /// Miscellaneous class for storing custom player data. 
    /// Complex, very specific systems should be implemented in a separate ModPlayer.
    /// </summary>
    public class MacrocosmPlayer : ModPlayer
    {
        #region Stats
        /// <summary> 
        /// The player's space protection level.
        /// Not synced.
        /// </summary>
        public float SpaceProtection { get; set; } = 0f;

        /// <summary>
        /// The player's movement speed in zero gravity.
        /// </summary>
        public float ZeroGravitySpeed { get; set; } = 2f;

        /// <summary> 
        /// Chance to not consume ammo from equipment and weapons, stacks additively with the vanilla chance.
        /// Not synced.
        /// </summary>
        public float ChanceToNotConsumeAmmo
        {
            get => chanceToNotConsumeAmmo;
            set => chanceToNotConsumeAmmo = MathHelper.Clamp(value, 0f, 1f);
        }

        private float chanceToNotConsumeAmmo = 0f;

        /// <summary>
        /// Percentage of regular shoot spread reduction of ranged weapons.
        /// </summary>
        public float ShootSpreadReduction
        {
            get => shootSpreadReduction;
            set => shootSpreadReduction = MathHelper.Clamp(value, 0f, 1f);
        }
        private float shootSpreadReduction = 0f;


        /// <summary>
        /// Extra crit damage, expressed in percentage.
        /// ExtraCritDamagePercent += 0.25f means 25% extra crit damage, alongside the default +100% damage.
        /// Not synced.
        /// </summary>
        public float ExtraCritDamagePercent { get; set; } = 0;

        /// <summary>
        /// Multiplier for non-crit damage.
        /// Not synced.
        /// </summary>
        public float NonCritDamageMultiplier { get; set; } = 1f;
        #endregion

        #region Item use data

        /// <summary> Use counter per item type. Not synced, not saved. </summary>
        public int[] ItemUseCount { get; private set; }

        /// <summary> Alt use cooldown per item type. Not synced, not saved. </summary>
        public int[] ItemAltUseCooldown { get; private set; }

        /// <summary> Cooldown on held projectile attacks. </summary>
        public int HeldProjectileCooldown = 0;

        #endregion

        #region Specific weapon data
        /// <summary> 
        /// Chandrium whip hit stacks. 
        /// Not synced.
        /// </summary>
        public int ChandriumWhipStacks = 0;
        #endregion

        #region Player flags
        /// <summary> 
        /// Whether this player is aware that they can use zombie fingers to unlock chests.
        /// Persistent. Not synced.
        /// </summary>
        public bool KnowsToUseZombieFinger = false;
        #endregion

        public override void Initialize()
        {
            ItemUseCount = new int[ItemLoader.ItemCount];
            ItemAltUseCooldown = new int[ItemLoader.ItemCount];
        }

        public override void OnEnterWorld()
        {
        }

        public override void ResetEffects()
        {
            SpaceProtection = 0f;

            ZeroGravitySpeed = 2f;

            ChanceToNotConsumeAmmo = 0f;
            ShootSpreadReduction = 0f;

            ExtraCritDamagePercent = 0;
            NonCritDamageMultiplier = 1f;

            Player.buffImmune[BuffType<Depressurized>()] = false;

            for (int type = 0; type < ItemLoader.ItemCount; type++)
            {
                if (ItemAltUseCooldown[type] > 0)
                    ItemAltUseCooldown[type]--;
            }
        }

        public override void PostUpdateBuffs()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                Player.AddBuff(BuffType<Depressurized>(), 2);
        }

        public override void PostUpdateEquips()
        {
            if (SpaceProtection >= 3f)
            {
                Player.buffImmune[BuffType<Depressurized>()] = true;
                Player.setBonus = Language.GetTextValue("Mods.Macrocosm.Items.SetBonuses.SpaceProtection_" + (int)(SpaceProtection / 3f));
            }
            else if(RoomOxygenSystem.IsRoomPressurized(Player.Center))
            {
                Player.buffImmune[BuffType<Depressurized>()] = true;
            }

            if (HeldProjectileCooldown < 600)
                HeldProjectileCooldown++; // 600 ticks as none of these weapons will have a use time above 10s.
        }

        public override bool CanUseItem(Item item)
        {
            // TODO: see why this is broken when useTime != useAnimation
            ItemUseCount[item.type]++;
            return true;
        }

        public override bool CanConsumeAmmo(Item weapon, Item ammo)
        {
            bool consumeAmmo = true;

            if (Main.rand.NextFloat() < ChanceToNotConsumeAmmo)
                consumeAmmo = false;

            return consumeAmmo;
        }

        public override void ModifyShootStats(Item item, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
        }

        public override void ModifyHitNPCWithItem(Item item, NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.CritDamage += ExtraCritDamagePercent;
            modifiers.NonCritDamage *= NonCritDamageMultiplier;
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (proj.owner != Player.whoAmI)
                return;

            modifiers.CritDamage += ExtraCritDamagePercent;
            modifiers.NonCritDamage *= NonCritDamageMultiplier;
        }

        public override void PreUpdateMovement()
        {
        }

        public override void PostUpdate()
        {
            HandleAchievements();
            HandleZeroGravity();
        }

        public void HandleAchievements()
        {
            if(SubworldSystem.IsActive<Moon>())
                CustomAchievement.IncreaseEventValue<SurviveMoon>(nameof(SurviveMoon), 1f);
        }

        private void HandleZeroGravity()
        {
            if (Player.gravity > float.Epsilon)
                return;

            if (Player.mount.Active && (Player.mount._data.usesHover || Player.mount._data.constantJump))
                return;

            if (Collision.TileCollision(Player.position, Player.velocity, Player.width, Player.height, gravDir: (int)Player.gravDir) == Vector2.Zero)
                Player.velocity = new Vector2(0, 0.01f);

            float speed = ZeroGravitySpeed;

            Player.stepSpeed = 0.5f;

            if (Player.controlUp)
                Player.velocity.Y = MathHelper.Lerp(Player.velocity.Y, -speed, 0.5f);
            else if (Player.controlDown)
                Player.velocity.Y = MathHelper.Lerp(Player.velocity.Y, speed, 0.5f);
            else
                Player.velocity.Y *= 0.99f;

            if (Player.controlLeft)
                Player.velocity.X = MathHelper.Lerp(Player.velocity.X, -speed, 0.5f);
            else if (Player.controlRight)
                Player.velocity.X = MathHelper.Lerp(Player.velocity.X, speed, 0.5f);
            else
                Player.velocity.X *= 0.99f;
        }

        #region Netcode
        public override void CopyClientState(ModPlayer clientClone)
        {
            // TODO: copy data that has to be netsynced
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            // TODO: SyncPlayer if netsynced data is different
        }

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)MessageType.SyncMacrocosmPlayer);
            packet.Write((byte)Player.whoAmI);

            // TODO: add netsynced data here 
        }

        public static void ReceiveSyncPlayer(BinaryReader reader, int whoAmI)
        {
            int playerWhoAmI = reader.ReadByte();
            MacrocosmPlayer macrocosmPlayer = Main.player[playerWhoAmI].GetModPlayer<MacrocosmPlayer>();

            // TODO: read netsynced data here
        }

        #endregion

        #region Saving & Loading
        public override void SaveData(TagCompound tag)
        {
            if (KnowsToUseZombieFinger)
                tag[nameof(KnowsToUseZombieFinger)] = true;
        }

        public override void LoadData(TagCompound tag)
        {
            KnowsToUseZombieFinger = tag.ContainsKey(nameof(KnowsToUseZombieFinger));
        }
        #endregion
    }
}
