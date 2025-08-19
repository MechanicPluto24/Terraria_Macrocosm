using Macrocosm.Common.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace Macrocosm.Common.Utils;

public static partial class Utility
{
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

    public static Projectile FindClosestProjectile(Vector2 position, int type)
    {
        Projectile bestProj = null;
        float bestDistance = float.MaxValue;
        foreach (Projectile projectile in Main.ActiveProjectiles)
        {
            if (projectile.type == type && Vector2.Distance(projectile.Center, position) < bestDistance)
            {
                bestDistance = Vector2.Distance(projectile.Center, position);
                bestProj = projectile;
            }
        }
        return bestProj;
    }

    /// <summary>
    /// Hostile projectiles deal:  
    /// <br> - 2x the <paramref name="damage"/> in Normal Mode </br>
    /// <br> - 4x the <paramref name="damage"/> in Expert Mode </br>
    /// <br> - 6x the <paramref name="damage"/> in Master Mode </br>
    /// <br> This helper method lets you use the actual damage desired, and apply your own difficulty scaling. </br>
    /// </summary>
    public static int TrueDamage(int damage)
    {
        damage /= 2;
        if (Main.expertMode)
            damage /= Main.masterMode ? 3 : 2;

        return damage;
    }

    public static Rectangle GetDamageHitbox(this Projectile projectile) => typeof(Projectile).InvokeMethod<Rectangle>("Damage_GetHitbox", projectile);

    public static void UpdateTrail(this Projectile projectile, bool updatePos = true, bool updateRot = true, bool updateDir = true, float smoothAmount = 0.65f, bool playerFollow = false)
    {
        var oldPos = projectile.oldPos;
        var oldRot = projectile.oldRot;
        var oldDir = projectile.oldSpriteDirection;

        if (oldPos is null || oldPos.Length == 0)
            return;


        for (int i = oldPos.Length - 1; i > 0; i--)
        {
            if (updatePos) oldPos[i] = oldPos[i - 1];
            if (updateRot) oldRot[i] = oldRot[i - 1];
            if (updateDir) oldDir[i] = oldDir[i - 1];
        }

        if (updatePos) oldPos[0] = projectile.position;
        if (updateRot) oldRot[0] = projectile.rotation;
        if (updateDir) oldDir[0] = projectile.spriteDirection;

        Vector2 offset = playerFollow && projectile.owner >= 0 && projectile.owner < Main.maxPlayers ? Main.player[projectile.owner].position - Main.player[projectile.owner].oldPosition : Vector2.Zero;
        if (playerFollow && updatePos && projectile.numUpdates == 0)
        {
            for (int i = 0; i < oldPos.Length; i++)
            {
                if (oldPos[i] != Vector2.Zero)
                    oldPos[i] += offset;
            }
        }

        if (smoothAmount > 0f && updatePos)
        {
            for (int i = oldPos.Length - 1; i > 0; i--)
            {
                if (oldPos[i] == Vector2.Zero) continue;

                if (oldPos[i].Distance(oldPos[i - 1]) > 2f)
                    oldPos[i] = Vector2.Lerp(oldPos[i], oldPos[i - 1], smoothAmount);

                if (updateRot)
                    oldRot[i] = (oldPos[i - 1] - oldPos[i]).SafeNormalize(Vector2.Zero).ToRotation();
            }
        }
    }


    /// <summary> Clone of vanilla trail update logic, for extra control </summary>
    public static void UpdateTrail(this Projectile projectile, int trailingMode = 3)
    {
        if (trailingMode == 0)
        {
            for (int i = projectile.oldPos.Length - 1; i > 0; i--)
                projectile.oldPos[i] = projectile.oldPos[i - 1];

            projectile.oldPos[0] = projectile.position;
        }
        else if (trailingMode == 1)
        {
            if (projectile.frameCounter == 0 || projectile.oldPos[0] == Vector2.Zero)
            {
                for (int i = projectile.oldPos.Length - 1; i > 0; i--)
                    projectile.oldPos[i] = projectile.oldPos[i - 1];

                projectile.oldPos[0] = projectile.position;
            }
        }
        else if (trailingMode == 2)
        {
            for (int i = projectile.oldPos.Length - 1; i > 0; i--)
            {
                projectile.oldPos[i] = projectile.oldPos[i - 1];
                projectile.oldRot[i] = projectile.oldRot[i - 1];
                projectile.oldSpriteDirection[i] = projectile.oldSpriteDirection[i - 1];
            }

            projectile.oldPos[0] = projectile.position;
            projectile.oldRot[0] = projectile.rotation;
            projectile.oldSpriteDirection[0] = projectile.spriteDirection;
        }
        else if (trailingMode == 3)
        {
            for (int i = projectile.oldPos.Length - 1; i > 0; i--)
            {
                projectile.oldPos[i] = projectile.oldPos[i - 1];
                projectile.oldRot[i] = projectile.oldRot[i - 1];
                projectile.oldSpriteDirection[i] = projectile.oldSpriteDirection[i - 1];
            }

            projectile.oldPos[0] = projectile.position;
            projectile.oldRot[0] = projectile.rotation;
            projectile.oldSpriteDirection[0] = projectile.spriteDirection;

            float amount = 0.65f;
            for (int i = projectile.oldPos.Length - 1; i > 0; i--)
            {
                if (projectile.oldPos[i] == Vector2.Zero) continue;

                if (projectile.oldPos[i].Distance(projectile.oldPos[i - 1]) > 2f)
                    projectile.oldPos[i] = Vector2.Lerp(projectile.oldPos[i], projectile.oldPos[i - 1], amount);

                projectile.oldRot[i] = (projectile.oldPos[i - 1] - projectile.oldPos[i]).SafeNormalize(Vector2.Zero).ToRotation();
            }
        }
        else if (trailingMode == 4)
        {
            Vector2 playerOffset = Main.player[projectile.owner].position - Main.player[projectile.owner].oldPosition;
            for (int i = projectile.oldPos.Length - 1; i > 0; i--)
            {
                projectile.oldPos[i] = projectile.oldPos[i - 1];
                projectile.oldRot[i] = projectile.oldRot[i - 1];
                projectile.oldSpriteDirection[i] = projectile.oldSpriteDirection[i - 1];
                if (projectile.numUpdates == 0 && projectile.oldPos[i] != Vector2.Zero)
                    projectile.oldPos[i] += playerOffset;
            }

            projectile.oldPos[0] = projectile.position;
            projectile.oldRot[0] = projectile.rotation;
            projectile.oldSpriteDirection[0] = projectile.spriteDirection;
        }
        else if (trailingMode == 5)
        {
            for (int i = projectile.oldPos.Length - 1; i > 0; i--)
            {
                projectile.oldPos[i] = projectile.oldPos[i - 1];
                projectile.oldRot[i] = projectile.oldRot[i - 1];
                projectile.oldSpriteDirection[i] = projectile.oldSpriteDirection[i - 1];
            }

            projectile.oldPos[0] = projectile.position;
            projectile.oldRot[0] = projectile.velocity.ToRotation();
            projectile.oldSpriteDirection[0] = projectile.spriteDirection;
        }
    }

    /// <summary>
    /// Draws an animated projectile, leave texture null to draw as entity with the loaded texture
    /// (Only tested for held projectiles)  
    /// </summary>
    /// <param name="proj"> Projectile instance to draw </param>
    /// <param name="lightColor"> Computed environment color </param>
    /// <param name="drawOffset"> Offset to draw from texture center at 0 rotation </param>
    /// <param name="texture"> Leave null to draw as entity with the loaded texture </param>
    public static void DrawAnimated(this Projectile proj, Color lightColor, SpriteEffects effect, Vector2 drawOffset = default, Texture2D texture = null, float? scale = null, Rectangle? frame = null, Effect shader = null)
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

        Main.EntitySpriteDraw(texture, position, sourceRect, proj.GetAlpha(lightColor), proj.rotation, origin, scale ?? proj.scale, effect, 0);

        if (shader is not null)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(state);
        }
    }

    /// <summary>
    /// Helper to draw an animated projectile extra, such as a glowmask
    /// </summary>
    public static void DrawAnimatedExtra(this Projectile proj, Texture2D texture, Color lightColor, SpriteEffects effect, Vector2 drawOffset = default, float? scale = null, Rectangle? frame = null)
        => proj.DrawAnimated(lightColor, effect, drawOffset + new Vector2(0, -2), texture, scale, frame);


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
