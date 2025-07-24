using Macrocosm.Common.Netcode;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Drawing.Particles;

/// <summary> Particle system by sucss, Nurby & Feldy @ PellucidMod (RIP) </summary>
public abstract partial class Particle : ModTexturedType
{
    /// <summary> Cached particle type as integer index, used for netcode purposes </summary>
    public int Type => type == -1 ? (type = ParticleManager.Types.IndexOf(this.GetType())) : type;
    private int type = -1;

    /// <summary> Index of this particle in the active particle collection </summary>
    public int WhoAmI => ParticleManager.Particles.IndexOf(this);

    #region Loading
    public override void Load()
    {
        ParticleManager.Textures.Add(ModContent.Request<Texture2D>(Texture));
        OnLoad();
    }

    public override void Unload()
    {
        OnUnload();
    }

    protected sealed override void Register()
    {
        ParticleManager.Types.Add(GetType());
    }

    public sealed override void SetupContent() => SetStaticDefaults();

    #endregion

    #region Common Fields
    /// <summary> Whether the current particle instance is active </summary>
    [NetSync] public bool Active;

    /// <summary> Time left before despawining, in ticks </summary>
    [NetSync] public int TimeLeft;

    /// <summary> The<c> Particle</c>'s total lifetime </summary>
    [NetSync] public int TimeToLive = 300;

    /// <summary> The <c>Particle</c>'s position in the world </summary>
    [NetSync] public Vector2 Position;

    /// <summary> The <c>Particle</c>'s velocity vector </summary>
    [NetSync] public Vector2 Velocity;

    /// <summary> The <c>Particle</c>'s acceleration vector </summary>
    [NetSync] public Vector2 Acceleration;

    /// <summary> The <c>Particle</c>'s rotation </summary>
    [NetSync] public float Rotation = 0f;

    /// <summary> The <c>Particle</c>'s <see cref="Rotation"> change per tick </summary>
    [NetSync] public float RotationVelocity;

    /// <summary> The <c>Particle</c>'s <see cref="RotationVelocity"> change per tick </summary>
    [NetSync] public float RotationAcceleration;

    /// <summary> The <c>Particle</c>'s scale vector </summary>
    [NetSync] public Vector2 Scale = new(1f);

    /// <summary> <see cref="ScaleVelocity"/> change per tick </summary>
    [NetSync] public Vector2 ScaleAcceleration;

    /// <summary> <see cref="Scale"/> change per tick </summary>
    [NetSync] public Vector2 ScaleVelocity;

    /// <summary> Allow negative scale (scale is absolute, but negative flips the sprite) </summary>
    [NetSync] public bool AllowNegativeScale = false;

    /// <summary> Normalized time (with respect to <see cref="TimeToLive"/> and <see cref="TimeLeft"/>) of the fade in </summary>
    [NetSync] public float FadeInNormalizedTime = float.Epsilon;

    /// <summary> Normalized time (with respect to <see cref="TimeToLive"/> and <see cref="TimeLeft"/>) of the fade out </summary>
    [NetSync] public float FadeOutNormalizedTime = 1f;

    /// <summary> The draw color </summary>
    [NetSync] public Color Color = Color.White;

    /// <summary> Particle animation update speed, in ticks per frame </summary>
    [NetSync] public int FrameSpeed;

    /// <summary> The draw layer of this <c>Particle</c>, see <see cref="ParticleDrawLayer"/>. Unused if <see cref="HasCustomDrawer"/>. </summary>
    [NetSync] public ParticleDrawLayer DrawLayer = ParticleDrawLayer.AfterProjectiles;

    protected int currentFrame = 0;
    protected int frameCounter = 0;

    #endregion

    #region Common Properties
    /// <summary> The <c>Particle</c>'s texture, autoloaded </summary>
    public Asset<Texture2D> TextureAsset => ParticleManager.Textures[Type];

    /// <summary> The texture size of this <c>Particle</c> </summary>
    // TODO: Maybe replace this to an overridable size if ever implementing particle collision
    public Vector2 Size
    {
        get
        {
            // BANDAID: Returns (1,1) on servers because I'm dumb -- Feldy
            if (GetFrame() is null)
                return TextureAsset is null ? new Vector2(1, 1) : TextureAsset.Size();

            return GetFrame().Value.Size();
        }
    }

    /// <summary> The <c>Particle</c>'s center coordinates in the world </summary>
    public Vector2 Center => Position + Size / 2;

    /// <summary> Whether the <c>Particle</c> should update its position based on velocity </summary>
    public virtual bool ShouldUpdatePosition => true;

    /// <summary> Fade factor, taking into account the <see cref="FadeInNormalizedTime"/> and <see cref="FadeOutNormalizedTime"/> </summary>
    public float FadeFactor
    {
        get
        {
            float fadeIn = MathHelper.Clamp(FadeInNormalizedTime, float.Epsilon, 1f);
            float fadeOut = MathHelper.Clamp(FadeOutNormalizedTime, float.Epsilon, 1f);
            float progress = (TimeToLive - (float)TimeLeft) / TimeToLive;
            return Utility.InverseLerp(0f, fadeIn, progress, clamped: true) * Utility.InverseLerp(1f, fadeOut, progress, clamped: true);
        }
    }

    #endregion

    #region Pooling

    /// <summary> The maximum number of particles of this type to keep in the pool. </summary>
    public virtual int MaxPoolCount { get; }

    /// <summary> Resets the particle's fields to their default values. </summary>
    public void Reset()
    {
        Active = false;

        TimeToLive = 300;
        TimeLeft = TimeToLive;

        Position = Vector2.Zero;
        Velocity = Vector2.Zero;
        Acceleration = Vector2.Zero;

        Rotation = 0f;
        RotationVelocity = 0f;
        RotationAcceleration = 0f;

        Scale = new Vector2(1f);
        ScaleVelocity = Vector2.Zero;
        ScaleAcceleration = Vector2.Zero;
        AllowNegativeScale = false;

        FadeInNormalizedTime = float.Epsilon;
        FadeOutNormalizedTime = 1f;

        Color = Color.White;

        spawned = false;

        CustomDrawer = null;

        FrameSpeed = int.MaxValue;
        frameCounter = 0;
        currentFrame = 0;

        OldPositions = new Vector2[1];
        OldRotations = new float[1];

        SetDefaults();
    }

    #endregion

    #region Hooks

    public bool HasCustomDrawer => CustomDrawer is not null;
    public object CustomDrawer { get; set; } = null;

    /// <summary> Used for loading tasks, called on Mod load </summary>
    public virtual void OnLoad() { }

    /// <summary> Used for unloading tasks, called on Mod unload </summary>
    public virtual void OnUnload() { }

    /// <summary> Set default values of this particle, called when instanced or fetched from pool. Randomized values are allowed. Runs before <c>CreateParticle</c>, values set here might be overwritten. </summary>
    public virtual void SetDefaults() { }

    /// <summary> Called when the <c>Particle</c> is spawned, used to create effects or run custom spawn logic. Runs after <c>CreateParticle</c> (on first update tick), may use value set here</summary>
    public virtual void OnSpawn() { }

    /// <summary> Used for defining the <c>Particle</c>'s behaviour </summary>
    public virtual void AI() { }

    /// <summary> 
    /// Used for special effects when the <c>Particle</c> is killed,
    /// such as when calling <see cref="Kill">Kill()</see>, or when the 
    /// <c>Particle</c>'s lifetime (<see cref="TimeLeft"/>) has elapsed. 
    /// </summary>
    public virtual void OnKill() { }

    #endregion

    #region Animation

    /// <summary> Whether this particle type picks a random frame on spawn </summary>
    public virtual bool SetRandomFrameOnSpawn => false;

    /// <summary> Whether this particle should despawn on the end of an animation cycle </summary>
    public virtual bool DespawnOnAnimationComplete => false;

    /// <summary> Number of animation frames of this particle </summary>
    public virtual int FrameCount => 1;

    /// <summary> Used for animating the <c>Particle</c>. By default, updates with <see cref="FrameCount"/> and <see cref="FrameSpeed"/> </summary>
    public virtual void UpdateFrame()
    {
        // if not animated or frame was picked on spawn, don't update frame
        if (FrameCount <= 1 || SetRandomFrameOnSpawn)
            return;

        if (Main.hasFocus || Main.netMode == NetmodeID.MultiplayerClient)
        {
            frameCounter++;
            if (frameCounter >= FrameSpeed)
            {
                frameCounter = 0;
                currentFrame++;

                if (currentFrame >= FrameCount)
                    currentFrame = 0;
            }
        }
    }

    /// <summary> 
    /// The current frame, as a nullabe <see cref="Rectangle"/>, representing the source <see cref="TextureAsset"/> coordinates. 
    /// If null, draws the entire texture.
    /// </summary>
    public virtual Rectangle? GetFrame()
    {
        if (Main.netMode == NetmodeID.Server)
            return null;

        // if not animated and frame is not picked randomly on spawn, draw the entire texture
        if (FrameCount <= 1 && !SetRandomFrameOnSpawn)
            return null;

        int frameHeight = TextureAsset.Height() / FrameCount;
        return new Rectangle(0, frameHeight * currentFrame, TextureAsset.Width(), frameHeight);
    }

    #endregion

    #region Logic
    internal bool spawned;
    public void Update()
    {
        if (!spawned)
        {
            OnSpawn();

            if (DespawnOnAnimationComplete)
                TimeToLive = FrameCount * FrameSpeed - 1;

            TimeLeft = TimeToLive;

            if (SetRandomFrameOnSpawn)
                currentFrame = Main.rand.Next(FrameCount);

            spawned = true;
        }

        Velocity += Acceleration;
        Position += Velocity;
        RotationVelocity += RotationAcceleration;
        Rotation += RotationVelocity;
        ScaleVelocity += ScaleAcceleration;
        Scale += ScaleVelocity;

        if ((Scale.X < 0f || Scale.Y < 0f) && !AllowNegativeScale)
            Scale = Vector2.Clamp(Scale, Vector2.Zero, new Vector2(float.MaxValue));

        if (DrawLayer is ParticleDrawLayer.PostInterface && !HasCustomDrawer)
            Position += Main.screenPosition;

        PopulateTrailArrays();
        AI();
        UpdateFrame();

        if (TimeLeft-- <= 0)
            Kill();
    }

    public void Kill(bool shouldSync = false)
    {
        OnKill();
        Active = false;

        ParticleManager.RemoveParticle(this);

        if (shouldSync)
            NetSync();
    }

    #endregion

    #region Drawcode
    /// <summary>
    /// Used for drawing things before the particle, with additive blending. Modify the spritebatch only when absolutely necessary.
    /// Substract <see cref="Main.screenPosition"> screenPosition </see> from the <see cref="Particle.Position">Position</see> before drawing.
    /// Return false to stop the default, alpha blended, drawing logic. Returns true by default.
    /// </summary>
    /// <param name="spriteBatch"> The spritebatch </param>
    /// <param name="screenPosition"> The top-left screen position in the world coordinates </param>
    /// <param name="lightColor"> The light color at the particle's position </param>
    public virtual bool PreDrawAdditive(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
    {
        return true;
    }

    /// <summary> 
    /// Used for drawing the particle with alpha blending. Modify the spritebatch only when absolutely necessary.
    /// Substract <see cref="Main.screenPosition"> screenPosition </see> from the <see cref="Particle.Position">Position</see> before drawing.
    /// Only called if <see cref="PreDrawAdditive(SpriteBatch, Vector2, Color)"/> returns true.
    /// </summary>
    /// <param name="spriteBatch"> The spritebatch </param>
    /// <param name="screenPosition"> The top-left screen position in the world coordinates </param>
    /// <param name="lightColor"> The light color at the particle's position </param>
    public virtual void Draw(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
    {
        spriteBatch.Draw(TextureAsset.Value, Position - screenPosition, GetFrame(), Utility.Colorize(Color, lightColor) * FadeFactor, Rotation, Size * 0.5f, Scale, SpriteEffects.None, 0f);
    }

    /// <summary> 
    /// Used for drawing things after the particle, with additive blending. Modify the spritebatch only when absolutely necessary.
    /// Substract <see cref="Main.screenPosition"> screenPosition </see> from the <see cref="Particle.Position">Position</see> before drawing.
    /// </summary>
    /// <param name="spriteBatch"></param>
    /// <param name="screenPosition"></param>
    /// <param name="lightColor"></param>
    public virtual void PostDrawAdditive(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
    {
    }
    #endregion

    #region Trails

    /// <summary> This particle length of <see cref="OldPositions"/> and <see cref="OldRotations"/> </summary>
    public virtual int TrailCacheLength { get; set; } = 1;

    /// <summary> Previous position </summary>
    public Vector2 OldPosition => OldPositions[0];

    /// <summary> Previous rotation </summary>
    public float OldRotation => OldRotations[0];

    /// <summary> Collection of old position, of length <see cref="TrailCacheLength"/> </summary>
    public Vector2[] OldPositions = new Vector2[1];

    /// <summary> Collection of old rotations, of length <see cref="TrailCacheLength"/> </summary>
    public float[] OldRotations = new float[1];

    private void PopulateTrailArrays()
    {
        if (OldPositions.Length != TrailCacheLength)
        {
            Array.Resize(ref OldPositions, TrailCacheLength);
            Array.Fill(OldPositions, Position);
        }

        if (OldRotations.Length != TrailCacheLength)
        {
            Array.Resize(ref OldRotations, TrailCacheLength);
            Array.Fill(OldRotations, Rotation);
        }

        OldPositions[0] = Position;
        OldRotations[0] = Rotation;

        for (int i = TrailCacheLength - 1; i > 0; i--)
        {
            OldPositions[i] = OldPositions[i - 1];
            OldRotations[i] = OldRotations[i - 1];
        }
    }

    #endregion
}