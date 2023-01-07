using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using ReLogic.Content;
using Terraria.ModLoader;
using Macrocosm.Common.Netcode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Macrocosm.Common.Drawing.Trails;
using static Terraria.ModLoader.PlayerDrawLayer;
using Macrocosm.Common.Utils;

namespace Macrocosm.Common.Drawing.Particles
{
	/// <summary> Particle system by sucss, Nurby & Feldy @ PellucidMod </summary>
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

			OnSpawn();
		}

		/// <summary> The <c>Particle</c>'s texture, autoloaded </summary>
		public Texture2D Texture => ParticleManager.Textures[Type];

		/// <summary> The texture size of this <c>Particle</c> </summary>
		// TODO: Maybe replace this to a configurable size if ever implementing particle collision
		public Vector2 Size => Texture.Size();
					
		/// <summary> Whether the current particle instance is active </summary>
		public bool Active { get; private set; }

		/// <summary> Time left before despawining, in ticks </summary>
		[NetSync] public int TimeLeft;

		/// <summary> The <c>Particle</c>'s position in the world </summary>
		[NetSync] public Vector2 Position;

		/// <summary> The <c>Particle</c>'s velocity vector </summary>
		[NetSync] public Vector2 Velocity;

		/// <summary> The <c>Particle</c>'s rotation </summary>
		[NetSync] public float Rotation = 0f;

		/// <summary> The <c>Particle</c>'s scale </summary>
		[NetSync] public float Scale = 1f;

		/// <summary> The <c>Particle</c>'s center coordinates in the world </summary>
		public Vector2 Center => Position + Size / 2;

		/// <summary> Path to the <c>Particle</c>'s texture, override for custom loading (non-autoload) </summary>
		public virtual string TexturePath => (this.GetType().Namespace + "." + this.GetType().Name).Replace('.', '/');

		/// <summary> The  <c>Particle</c>'s total lifetime </summary>
		public virtual int SpawnTimeLeft => 300;

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
		
		/// <summary> Used for animating the <c>Particle</c> </summary>
		/// <returns> The current frame as a nullabe <see cref="Rectangle"/>, representing the <see cref="Texture"/> coordinates </returns>
		public virtual Rectangle? GetFrame() => null;

		#endregion

		#region Logic
		
		/// <summary> Used for drawing the particle. Substract <see cref="Main.screenPosition"> screenPosition </see> from the <see cref="Particle.Position">Position</see> position before drawing </summary>
		/// <param name="spriteBatch"> The spritebatch </param>
		/// <param name="screenPosition"> The top-left screen position in the world coordinates </param>
		/// <param name="lightColor"> The light color at the particle's position </param>
		public virtual void Draw(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor) 
		{
			spriteBatch.Draw(Texture, Position - screenPosition, GetFrame(), lightColor, Rotation, Size * 0.5f, Scale, SpriteEffects.None, 0f);

			if (Trail is not null)
				Trail.Draw();
		}

		public void Update()
		{
			if(ShouldUpdatePosition)
 				Position += Velocity;
			
			PopulateTrailParameters();

			AI();
			
			if(TimeLeft-- <= 0)
  				Kill();
  		}

		public void Kill()
		{
			if (!Active)
				return;

			OnKill();
			Active = false;
		}

		#endregion

		#region Trails

		public void DrawSimpleTrail(Vector2 rotatableOffsetFromCenter, float startWidth, float endWidth, Color startColor, Color? endColor = null)
				=> Utility.DrawSimpleTrail(Size / 2f, OldPositions, OldRotations, rotatableOffsetFromCenter, startWidth, endWidth, startColor, endColor);


		/// <summary> The <see cref="Trails.Trail"> Trail </see> object bound to this <c>Particle</c> </summary>
		public Trail Trail { get; private set; }
		public Trail GetTrail() => Trail;

		/// <summary> Binds the <c>Particle</c>'s trail to the specified <see cref="Trails.Trail"> Trail </see> type </summary>
		/// <typeparam name="T"> The trail type </typeparam>
		public void SetTrail<T>() where T : Trail
		{
			Trail = Activator.CreateInstance<T>();
			Trail.Owner = this;
		}

 		public virtual int TrailCacheLenght { get; set; } = 1;

		public Vector2 OldPosition => OldPositions[0];
		public float OldRotation => OldRotations[0];

		public Vector2[] OldPositions = new Vector2[1];
		public float[] OldRotations = new float[1];

		private void PopulateTrailParameters() 
		{
			InitializeTrailArrays();

			OldPositions[0] = Position;
			OldRotations[0] = Rotation;

			for (int i = TrailCacheLenght - 1; i > 0; i--)
			{
 				OldPositions[i] = OldPositions[i - 1];
				OldRotations[i] = OldRotations[i - 1];
 			}
		}

		private void InitializeTrailArrays()
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
		}

		#endregion
	}
}
