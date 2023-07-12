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

		private float panSpeed;

		private Func<float, float> easingFunction;

		private float panProgress;

		public PanCameraModifier(Vector2 targetPosition, float panSpeed, string context, Func<float, float> easingFunction = null)
		{
			ReturnToOriginalPosition = false;
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
			if(Vector2.DistanceSquared(cameraPosition.OriginalCameraCenter, targetPosition) > 0f)
			{
				if (ReturnToOriginalPosition)
					panProgress -= panSpeed;
				else
					panProgress += panSpeed;

				if (easingFunction is not null)
					panProgress = easingFunction(panProgress);

				cameraPosition.OriginalCameraCenter = Vector2.Lerp(cameraPosition.OriginalCameraCenter, targetPosition, panProgress);
			}
			else if (ReturnToOriginalPosition)
			{
				Finished = true;
			}
		}
	}
}
