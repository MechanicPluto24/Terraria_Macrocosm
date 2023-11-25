using Macrocosm.Common.Drawing.Trails;
using Macrocosm.Common.Netcode;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace Macrocosm.Common.Drawing.Particles
{
    /// <summary> Particle system by sucss, Nurby & Feldy @ PellucidMod (RIP) </summary>
    public abstract partial class Particle : ModType
    {
        /// <summary> Cached particle type as integer index, used for netcode purposes </summary>
        public int Type => type == -1 ? (type = ParticleManager.Types.IndexOf(this.GetType())) : type;
        private int type = -1;

        /// <summary> Index of this particle in the active particle collection </summary>
        public int WhoAmI => ParticleManager.Particles.IndexOf(this);

        #region Loading

        public override void Load()
        {
            ParticleManager.Textures.Add(ModContent.Request<Texture2D>(TexturePath, AssetRequestMode.ImmediateLoad).Value);
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

        #endregion

        #region Fields & Properties
        protected Particle()
        {
            TimeLeft = SpawnTimeLeft;
            Active = true;

            if (SetRandomFrameOnSpawn)
                currentFrame = Main.rand.Next(FrameNumber);

            //OnSpawn();
        }

        /// <summary> The <c>Particle</c>'s texture, autoloaded </summary>
        public Texture2D Texture => ParticleManager.Textures[Type];

        /// <summary> The texture size of this <c>Particle</c> </summary>
        // TODO: Maybe replace this to a configurable size if ever implementing particle collision
        public Vector2 Size
        {
            get
            {
                // BANDAID: Returns (1,1) on servers... now I really have to do that ^
                if (GetFrame() is null)
                    return Texture is null ? new Vector2(1, 1) : Texture.Size();

                return GetFrame().Value.Size();
            }
        }

        /// <summary> Whether the current particle instance is active </summary>
        [NetSync] public bool Active;

        /// <summary> Time left before despawining, in ticks </summary>
        [NetSync] public int TimeLeft;

        /// <summary> The <c>Particle</c>'s position in the world </summary>
        [NetSync] public Vector2 Position;

        /// <summary> The <c>Particle</c>'s velocity vector </summary>
        [NetSync] public Vector2 Velocity;

        /// <summary> The <c>Particle</c>'s rotation </summary>
        [NetSync] public float Rotation = 0f;

        /// <summary> The <c>Particle</c>'s scale as a 2d vector </summary>
        [NetSync] public Vector2 ScaleV = new(1f);

        /// <summary> The <c>Particle</c>'s scale as a scalar </summary>
        public float Scale
        {
            get => ScaleV.X;
            set { ScaleV.X = value; ScaleV.Y = value; }
        }

        /// <summary> The <c>Particle</c>'s center coordinates in the world </summary>
        public Vector2 Center => Position + Size / 2;

        /// <summary> Path to the <c>Particle</c>'s texture, override for custom loading (non-autoload) </summary>
        public virtual string TexturePath => Utility.GetNamespacePath(this);

        /// <summary> The  <c>Particle</c>'s total lifetime. If <see cref="DespawnOnAnimationComplete"/> is true, this defaults to the animation duration. </summary>
        public virtual int SpawnTimeLeft => DespawnOnAnimationComplete ? FrameNumber * FrameSpeed - 1 : 300;

        /// <summary> Whether the <c>Particle</c> should update its position </summary>
        public virtual bool ShouldUpdatePosition => true;

        /// <summary> The draw layer of this <c>Particle</c>, see <see cref="ParticleDrawLayer"/> </summary>
        public virtual ParticleDrawLayer DrawLayer => ParticleDrawLayer.AfterProjectiles;

        #endregion

        #region Hooks

        /// <summary> Used for loading tasks, called on Mod load </summary>
        public virtual void OnLoad() { }


        /// <summary> Used for unloading tasks, called on Mod unload </summary>
        public virtual void OnUnload() { }


        /// <summary> Called when the <c>Particle</c> is spawned </summary>
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

        /// <summary> Whether to pick a random frame on spawn </summary>
        public virtual bool SetRandomFrameOnSpawn => false;

        /// <summary> If true, particle will despawn on the end of animation </summary>
        public virtual bool DespawnOnAnimationComplete => false;

        /// <summary> Number of animation frames of this particle </summary>
        public virtual int FrameNumber => 1;

        /// <summary> Particle animation update speed, in ticks per frame </summary>
        public virtual int FrameSpeed => 1;

        protected int currentFrame = 0;
        protected int frameCnt = 0;

        /// <summary> Used for animating the <c>Particle</c>. By default, updates with <see cref="FrameNumber"/> and <see cref="FrameSpeed"/> </summary>
        public virtual void UpdateFrame()
        {
            // if not animated or frame was picked on spawn, don't update frame
            if (FrameNumber <= 1 || SetRandomFrameOnSpawn)
                return;

            if (Main.hasFocus || Main.netMode == NetmodeID.MultiplayerClient)
            {
                frameCnt++;
                if (frameCnt == FrameSpeed)
                {
                    frameCnt = 0;
                    currentFrame++;

                    if (currentFrame >= FrameNumber)
                        currentFrame = 0;
                }
            }
        }

        /// <summary> 
        /// The current frame, as a nullabe <see cref="Rectangle"/>, representing the source <see cref="Texture"/> coordinates. 
        /// If null, draws the entire texture.
        /// </summary>
        public virtual Rectangle? GetFrame()
        {

            if (Main.netMode == NetmodeID.Server)
                return null;

            // if not animated or frame is not picked randomly on spawn, draw the entire texture
            if (FrameNumber <= 1 && !SetRandomFrameOnSpawn)
                return null;


            int frameHeight = Texture.Height / FrameNumber;
            return new Rectangle(0, frameHeight * currentFrame, Texture.Width, frameHeight);
        }

        #endregion

        #region Logic

        /// <summary>
        /// Used for drawing things before the particle, with additive blending. Modify the spritebatch only when absolutely necessary.
        /// Substract <see cref="Main.screenPosition"> screenPosition </see> from the <see cref="Particle.Position">Position</see> position before drawing.
        /// Return false to stop the default, alpha blended, drawing logic. Returns true by default.
        /// </summary>
        /// <param name="spriteBatch"> The spritebatch </param>
        /// <param name="screenPosition"> The top-left screen position in the world coordinates </param>
        /// <param name="lightColor"> The light color at the particle's position </param>
        public virtual bool PreDrawAdditive(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
        {
            Trail?.Draw();
            return true;
        }

        /// <summary> 
        /// Used for drawing the particle with alpha blending. Modify the spritebatch only when absolutely necessary.
        /// Substract <see cref="Main.screenPosition"> screenPosition </see> from the <see cref="Particle.Position">Position</see> position before drawing.
        /// Only called if <see cref="PreDrawAdditive(SpriteBatch, Vector2, Color)"/> returns true.
        /// </summary>
        /// <param name="spriteBatch"> The spritebatch </param>
        /// <param name="screenPosition"> The top-left screen position in the world coordinates </param>
        /// <param name="lightColor"> The light color at the particle's position </param>
        public virtual void Draw(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
        {
            spriteBatch.Draw(Texture, Position - screenPosition, GetFrame(), lightColor, Rotation, Size * 0.5f, ScaleV, SpriteEffects.None, 0f);
        }

        /// <summary> 
        /// Used for drawing things after the particle, with additive blending. Modify the spritebatch only when absolutely necessary.
        /// Substract <see cref="Main.screenPosition"> screenPosition </see> from the <see cref="Particle.Position">Position</see> position before drawing 
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="screenPosition"></param>
        /// <param name="lightColor"></param>
        public virtual void PostDrawAdditive(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
        {
        }

        public void Update()
        {
            if (ShouldUpdatePosition)
                Position += Velocity;

            PopulateTrailData();
            AI();
            UpdateFrame();

            if (TimeLeft-- <= 0)
                Kill();
        }

        public void Kill(bool shouldSync = false)
        {
            if (!Active)
                return;

            OnKill();
            Active = false;

            if (shouldSync)
                NetSync();
        }

        #endregion

        // TODO: This could use some touching up, maybe make trails a component or something
        #region Trails
        public void DrawMagicPixelTrail(Vector2 rotatableOffsetFromCenter, float startWidth, float endWidth, Color startColor, Color? endColor = null)
                => Utility.DrawMagicPixelTrail(Size / 2f, OldPositions, OldRotations, rotatableOffsetFromCenter, startWidth, endWidth, startColor, endColor);

        /// <summary> The <see cref="Trails.VertexTrail"> VertexTrail </see> object bound to this <c>Particle</c> </summary>
        public VertexTrail Trail { get; private set; }
        public VertexTrail GetTrail() => Trail;

        /// <summary> Binds the <c>Particle</c>'s trail to the specified <see cref="Trails.VertexTrail"> Trail </see> type </summary>
        /// <typeparam name="T"> The trail type </typeparam>
        public void SetTrail<T>() where T : VertexTrail
        {
            Trail = Activator.CreateInstance<T>();
            Trail.Owner = this;
        }

        public virtual int TrailCacheLenght { get; set; } = 1;

        public Vector2 OldPosition => OldPositions[0];
        public float OldRotation => OldRotations[0];

        public Vector2[] OldPositions = new Vector2[1];
        public float[] OldRotations = new float[1];

        private void PopulateTrailData()
        {
            if (OldPositions.Length != TrailCacheLenght)
            {
                Array.Resize(ref OldPositions, TrailCacheLenght);
                Array.Fill(OldPositions, Position);
            }

            if (OldRotations.Length != TrailCacheLenght)
            {
                Array.Resize(ref OldRotations, TrailCacheLenght);
                Array.Fill(OldRotations, Rotation);
            }

            OldPositions[0] = Position;
            OldRotations[0] = Rotation;

            for (int i = TrailCacheLenght - 1; i > 0; i--)
            {
                OldPositions[i] = OldPositions[i - 1];
                OldRotations[i] = OldRotations[i - 1];
            }
        }

        #endregion
    }
}
