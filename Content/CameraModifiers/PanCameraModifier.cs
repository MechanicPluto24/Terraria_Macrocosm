using Macrocosm.Content.Players;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Graphics.CameraModifiers;

namespace Macrocosm.Content.CameraModifiers
{
    public class PanCameraModifier : ICameraModifier
    {
        /// <summary> The unique ID of this </summary>
        public string UniqueIdentity { get; private set; }

        /// <summary> Whether this camera modifier has concluded </summary>
        public bool Finished { get; private set; }

        /// <summary> Smoothly return to the regular camera position </summary>
        public bool ReturnToNormalPosition { get; set; } = false;

        /// <summary> The position the camera modifier aims for </summary>
        public Vector2 TargetPosition { get; set; }


        private Vector2 startScreenPosition;

        private float panSpeed;

        private Func<float, float> easingFunction;

        private float panProgress;

        /// <summary>
        /// Create a new panning camera modifier towards a target position.
        /// </summary>
        /// <param name="targetPosition"> The target position, can be updated afterwards (<see cref="TargetPosition">) </param>
        /// <param name="startScreenPosition"> The screen position on camera modifier creation </param>
        /// <param name="panSpeed"> The panning speed </param>
        /// <param name="uniqueId"> The unique ID of this </param>
        /// <param name="easingFunction"> The easing function on the pan progress </param> 
        public PanCameraModifier(Vector2 targetPosition, Vector2 startScreenPosition, float panSpeed, string uniqueId, Func<float, float> easingFunction = null)
        {
            ReturnToNormalPosition = false;
            TargetPosition = targetPosition;

            this.startScreenPosition = startScreenPosition;
            this.panSpeed = panSpeed;
            UniqueIdentity = uniqueId;
            this.easingFunction = easingFunction;
        }

        public void Update(ref CameraInfo cameraPosition)
        {

            if (!Main.LocalPlayer.GetModPlayer<RocketPlayer>().InRocket && !ReturnToNormalPosition)
                ReturnToNormalPosition = true;

            // Finished when the pan towards the regular position is done
            if (ReturnToNormalPosition && panProgress < 0f)
            {
                Finished = true;
            }
            else if (Vector2.DistanceSquared(cameraPosition.OriginalCameraPosition, TargetPosition) > 0f)
            {
                // On return, use the regular camera position (e.g. centered on player)
                Vector2 originalPosition = cameraPosition.OriginalCameraPosition;

                if (ReturnToNormalPosition)
                {
                    panProgress -= panSpeed * 0.9f;
                }
                else if (panProgress < 1f)
                {
                    // Use the provided screen position as a starting point, not the regular screen position
                    originalPosition = startScreenPosition;
                    panProgress += panSpeed;
                }

                float localProgress = panProgress;
                if (easingFunction is not null)
                    localProgress = easingFunction(panProgress);

                cameraPosition.CameraPosition = Vector2.Lerp(originalPosition, TargetPosition, localProgress);
            }
        }
    }
}
