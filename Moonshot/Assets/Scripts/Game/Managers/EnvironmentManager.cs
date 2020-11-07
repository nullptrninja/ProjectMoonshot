using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Game.Common;
using Assets.Scripts.Game.Data;
using Core.Utility;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Game.Managers {
    [ExecuteInEditMode]
    public class EnvironmentManager : MonoBehaviour {
        private static readonly Color BaseColor = new Color(0.5f, 0.5f, 1f, 1f);
        private static readonly Color AtmosphereLineColor = new Color(0.25f, 1f, 0.25f, 1f);
        private static readonly Color CompassLineColor = new Color(0.25f, 1f, 1f, 1f);
        private static readonly Color WindCompassLineColor = new Color(1f, 0.15f, 0.15f, 1f);

        public EnvironmentSetting Settings;

#if UNITY_EDITOR_WIN
        public void OnDrawGizmos() {
            DrawBase();

            // Draw the atmospheric layer viz
            DrawAtmosphere();

            // Draw our compass
            DrawCompass();

            // Wind Data
            DrawWind();
        }

        private void DrawBase() {
            var startPt = this.transform.position;
            Gizmos.color = BaseColor;
            Gizmos.DrawWireSphere(startPt, 0.25f);
        }

        private void DrawAtmosphere() {
            var startPt = this.transform.position;

            Gizmos.color = AtmosphereLineColor;
            for (var i = 0; i < this.Settings.AtmosphericLayers.Length; ++i) {
                var layer = this.Settings.AtmosphericLayers[i];
                var endPt = new Vector3(startPt.x, startPt.y + layer.Height, startPt.z);
                var detentLineEndPt = endPt + new Vector3(2f, 0f, 0f);

                // Main Vertical                
                Gizmos.DrawLine(startPt, endPt);
                Gizmos.DrawLine(endPt, detentLineEndPt);
                Handles.Label(endPt, $"Top of Atmosphere {i} at {layer.Height}m / Drag: {layer.Drag}");

                startPt = endPt;
            }
        }

        private void DrawCompass() {
            // North is defined at moving +1 on the Z axis (V3.Forward)
            var startPt = this.transform.position + new Vector3(0, 10f, 0);
            var mainLineEndPt = startPt + Vector3.forward * 4f;                   // Our reference vector (scaled up for for visibility)
            DrawArrow(CompassLineColor, startPt, 0f, 5f);

            // Draw "N"
            var nBarStartPt = mainLineEndPt + new Vector3(-1f, 0f, 4f);
            var nBarEndPt = mainLineEndPt + new Vector3(1f, 0f, 2f);
            var nLeftLegPt = nBarStartPt + new Vector3(0f, 0f, -2f);
            var nRightLegPt = nBarEndPt + new Vector3(0f, 0f, 2f);
            Gizmos.DrawLine(nBarStartPt, nBarEndPt);
            Gizmos.DrawLine(nBarStartPt, nLeftLegPt);
            Gizmos.DrawLine(nBarEndPt, nRightLegPt);
        }

        private void DrawWind() {
            float WindDirToAngle(CompassDirection cdir) {
                switch (cdir) {
                    case CompassDirection.N:
                        return 0f;
                    case CompassDirection.E:
                        return 90f;
                    case CompassDirection.W:
                        return 270f;
                    case CompassDirection.S:
                        return 180f;

                    case CompassDirection.NE:
                        return 45f;
                    case CompassDirection.NW:
                        return 315f;
                    case CompassDirection.SE:
                        return 125f;
                    case CompassDirection.SW:
                        return 225f;

                    default:
                        return 0f;
                }
            }

            var angle = WindDirToAngle(this.Settings.WindDirection);
            var startPt = this.transform.position + new Vector3(0, 15f, 0);
            DrawArrow(WindCompassLineColor, startPt, angle, 10f, $"Wind: {this.Settings.WindDirection} / Speed: {this.Settings.WindSpeed}");
        }

        private void DrawArrow(Color color, Vector3 basePosition, float rotDegs, float length, string label = "") {
            var legLength = length / 3f;

            // Reference vector.
            var mainLineEndPt = basePosition + (Vector3.forward * length).RotateXZPlanar(rotDegs);
            var leftLeg = mainLineEndPt + (new Vector3(-legLength, 0f, -legLength)).RotateXZPlanar(rotDegs);
            var rightLeg = mainLineEndPt + (new Vector3(legLength, 0f, -legLength)).RotateXZPlanar(rotDegs);
            var labelStartPt = basePosition + (Vector3.forward * (length + (length / 2f))).RotateXZPlanar(rotDegs);

            Gizmos.color = color;
            Gizmos.DrawLine(basePosition, mainLineEndPt);
            Gizmos.DrawLine(mainLineEndPt, leftLeg);
            Gizmos.DrawLine(mainLineEndPt, rightLeg);
            Handles.Label(labelStartPt, label);
        }
#endif
    }
}
