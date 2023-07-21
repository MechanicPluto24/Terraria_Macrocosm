using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Graphics.CameraModifiers;

namespace Macrocosm.Content.CameraModifiers
{
	internal class PanCameraModifier : ICameraModifier
	{
		public string UniqueIdentity { get; private set; }
		public bool Finished { get; private set; }
		public bool ReturnToOriginalPosition { get; set; } = false;

		private Vector2 targetPosition;

		private Vector2 originalScreenPosition;

		private float panSpeed;

		private Func<float, float> easingFunction;

		private float panProgress;

		public PanCameraModifier(Vector2 targetPosition, Vector2 originalScreenPosition, float panSpeed, string context, Func<float, float> easingFunction = null)
		{
			ReturnToOriginalPosition = false;
			this.originalScreenPosition = originalScreenPosition;
			this.targetPosition = targetPosition;
			this.panSpeed = panSpeed;
			UniqueIdentity = context;
			this.easingFunction = easingFunction;
		}

		public PanCameraModifier(float offsetFromNormalPosition, float targetDirectionAngle, float panSpeed, string uniqueIdentity, Func<float, float> easingFunction = null)
		{
			ReturnToOriginalPosition = false;
			this.targetPosition = Utility.PolarVector(offsetFromNormalPosition, targetDirectionAngle);
			this.panSpeed = panSpeed;
			UniqueIdentity = uniqueIdentity;
			this.easingFunction = easingFunction;
		}

		public void Update(ref CameraInfo cameraPosition)
		{
			if (ReturnToOriginalPosition && panProgress < 0f)
			{
				Finished = true;
			}
			else if(Vector2.DistanceSquared(cameraPosition.OriginalCameraPosition, targetPosition) > 0f)
			{
				Vector2 originalPosition = cameraPosition.OriginalCameraPosition;

				if (ReturnToOriginalPosition)
				{
					panProgress -= panSpeed * 0.9f;
				}
				else if(panProgress < 1f)
				{
					// use original screen position on right click, not the one where the player is in the rocket
					originalPosition = originalScreenPosition;
					panProgress += panSpeed;
				}

				float localProgress = panProgress;

				if (easingFunction is not null)
					localProgress = easingFunction(panProgress);

				cameraPosition.CameraPosition = Vector2.Lerp(originalPosition, targetPosition, localProgress);
 			}
		}
	}
}
