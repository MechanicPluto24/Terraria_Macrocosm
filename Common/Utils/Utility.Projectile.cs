using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Trails;
using Macrocosm.Content.Projectiles.Global;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace Macrocosm.Common.Utils
{
    public static partial class Utility
    {
        public static void SetTrail<T>(this Projectile projectile) where T : VertexTrail
        {
            if (projectile.TryGetGlobalProjectile(out MacrocosmProjectile globalProj))
            {
                globalProj.Trail = Activator.CreateInstance<T>();
                globalProj.Trail.Owner = projectile;
            }
        }

        public static VertexTrail GetTrail(this Projectile projectile) => projectile.GetGlobalProjectile<MacrocosmProjectile>().Trail;
        public static bool TryGetTrail(this Projectile projectile, out VertexTrail trail)
        {
            trail = projectile.GetGlobalProjectile<MacrocosmProjectile>().Trail;
            return trail is not null;
        }

        public static void Explode(this Projectile projectile, float blastRadius, int timeLeft = 1)
        {
            projectile.tileCollide = false;
            projectile.timeLeft = timeLeft;
            projectile.penetrate = -1;
            projectile.alpha = 255;

            projectile.velocity = Vector2.Zero;
            projectile.Resize((int)blastRadius, (int)blastRadius);
            projectile.netUpdate = true;
        }

        /// <summary>
        /// Hostile projectiles deal 2x the <paramref name="damage"/> in Normal Mode and 4x the <paramref name="damage"/> in Expert Mode.
        /// This helper method remedies that. TODO: check how it behaves in Journey & Master Modes
        /// </summary>
        public static int TrueDamage(int damage)
            => damage / (Main.expertMode ? 4 : 2);

        public static Rectangle GetDamageHitbox(this Projectile proj)
        {
            MethodInfo dynMethod = proj.GetType().GetMethod("Damage_GetHitbox",
                BindingFlags.NonPublic | BindingFlags.Instance);
            return (Rectangle)dynMethod.Invoke(proj, null);
        }

        /// <summary>
        /// Draws an animated projectile, leave texture null to draw as entity with the loaded texture
        /// (Only tested for held projectiles)  
        /// </summary>
        /// <param name="proj"> Projectile instance to draw </param>
        /// <param name="lightColor"> Computed environment color </param>
        /// <param name="drawOffset"> Offset to draw from texture center at 0 rotation </param>
        /// <param name="texture"> Leave null to draw as entity with the loaded texture </param>
        public static void DrawAnimated(this Projectile proj, Color lightColor, SpriteEffects effect, Vector2 drawOffset = default, Texture2D texture = null, Rectangle? frame = null, Effect shader = null)
        {
            texture ??= TextureAssets.Projectile[proj.type].Value;

            Vector2 position = proj.Center - Main.screenPosition;

            int numFrames = Main.projFrames[proj.type];
            Rectangle sourceRect = frame ?? texture.Frame(1, numFrames, frameY: proj.frame);

            Vector2 origin = new Vector2(texture.Width / 2, texture.Height / numFrames / 2) - new Vector2(drawOffset.X, drawOffset.Y * proj.spriteDirection);

            SpriteBatchState state = default;
            if (shader is not null)
            {
                state.SaveState(Main.spriteBatch);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(shader, state);
            }

            Main.EntitySpriteDraw(texture, position, sourceRect, lightColor, proj.rotation, origin, proj.scale, effect, 0);

            if (shader is not null)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(state);
            }
        }

        /// <summary>
        /// Draws an animated projectile extra, such as a glowmask
        /// (Only tested for held projectiles)  
        /// </summary>
        public static void DrawAnimatedExtra(this Projectile proj, Texture2D glowmask, Color lightColor, SpriteEffects effect, Vector2 drawOffset = default, Rectangle? frame = null)
            => proj.DrawAnimated(lightColor, effect, drawOffset + new Vector2(0, -2), glowmask, frame);


        public static void DrawWhipLine(List<Vector2> list, Color color)
        {
            Texture2D texture = TextureAssets.FishingLine.Value;
            Rectangle frame = texture.Frame();
            Vector2 origin = new(frame.Width / 2, 2);

            Vector2 pos = list[0];
            for (int i = 0; i < list.Count - 1; i++)
            {
                Vector2 element = list[i];
                Vector2 diff = list[i + 1] - element;

                float rotation = diff.ToRotation() - MathHelper.PiOver2;
                Color lightColor = Lighting.GetColor(element.ToTileCoordinates(), color);
                Vector2 scale = new(1, (diff.Length() + 2) / frame.Height);

                Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, lightColor, rotation, origin, scale, SpriteEffects.None, 0);

                pos += diff;
            }
        }


        public static void AIFallingBlock(Projectile projectile, bool falling)
        {
            projectile.tileCollide = true;
            projectile.localAI[1] = 0f;

            if (projectile.ai[0] == 1f)
            {
                if (!falling)
                {
                    projectile.ai[1] += 1f;

                    if (projectile.ai[1] >= 60f)
                    {
                        projectile.ai[1] = 60f;
                        projectile.velocity.Y += 0.2f;
                    }
                }
                else
                    projectile.velocity.Y += 0.41f;
            }
            else if (projectile.ai[0] == 2f)
            {
                projectile.velocity.Y += 0.2f;

                if (projectile.velocity.X < -0.04f)
                    projectile.velocity.X += 0.04f;
                else if (projectile.velocity.X > 0.04f)
                    projectile.velocity.X -= 0.04f;
                else
                    projectile.velocity.X = 0f;
            }

            projectile.rotation += 0.1f;

            if (projectile.velocity.Y > 10f)
                projectile.velocity.Y = 10f;
        }

        public static void FallingBlockCreateTile(Projectile projectile, int createTileType, int createItemType)
        {
            if (projectile.owner == Main.myPlayer && !projectile.noDropItem)
            {
                int tileX = (int)(projectile.position.X + projectile.width / 2) / 16;
                int tileY = (int)(projectile.position.Y + projectile.width / 2) / 16;

                Tile tile = Main.tile[tileX, tileY];

                bool placed = false;
                int item = -1;

                if (tile.HasUnactuatedTile && tile.IsHalfBlock && projectile.velocity.Y > 0f && Math.Abs(projectile.velocity.Y) > Math.Abs(projectile.velocity.X))
                    tileY--;

                if (!tile.HasTile)
                {
                    Tile tileBelow = Main.tile[tileX, tileY + 1];
                    bool canNotPlace = tileY < Main.maxTilesY - 2 && tileBelow.HasTile && tileBelow.TileType == TileID.MinecartTrack && WorldGen.BlockBelowMakesSandFall(tileX, tileY);

                    if (!canNotPlace)
                        placed = WorldGen.PlaceTile(tileX, tileY, createTileType, false, true);

                    if (!canNotPlace && tile.HasTile && tile.IsHalfBlock && tile.TileType == createTileType)
                    {
                        if (tileBelow.IsHalfBlock || tileBelow.Slope != 0)
                        {
                            WorldGen.SlopeTile(tileX, tileY + 1, 0);

                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 14, tileX, tileY + 1);
                        }

                        if (Main.netMode != NetmodeID.SinglePlayer)
                            NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 1, tileX, tileY, createTileType);
                    }
                    else if (!placed)
                    {
                        item = Item.NewItem(projectile.GetSource_DropAsItem(), (int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height, createItemType);
                    }
                }
                else
                {
                    item = Item.NewItem(projectile.GetSource_DropAsItem(), (int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height, createItemType);
                }
            }
        }
    }
}
