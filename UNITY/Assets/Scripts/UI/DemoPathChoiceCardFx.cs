using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PathOfTenThousandWays.Demo.UI
{
    public sealed class DemoPathChoiceCardFx : MonoBehaviour
    {
        private sealed class MotionEntry
        {
            public RectTransform Rect;
            public Graphic Graphic;
            public Vector2 BasePosition;
            public Vector3 BaseScale;
            public Color BaseColor;
            public Vector2 Amplitude;
            public float Speed;
            public float Phase;
            public float RotationAmplitude;
            public float ScaleAmplitude;
            public float AlphaAmplitude;
        }

        private readonly List<MotionEntry> motions = new List<MotionEntry>();

        public void Register(
            RectTransform rect,
            Vector2 amplitude,
            float speed,
            float phase,
            float rotationAmplitude = 0f,
            float scaleAmplitude = 0f,
            Graphic graphic = null,
            float alphaAmplitude = 0f)
        {
            if (rect == null)
            {
                return;
            }

            motions.Add(new MotionEntry
            {
                Rect = rect,
                Graphic = graphic,
                BasePosition = rect.anchoredPosition,
                BaseScale = rect.localScale,
                BaseColor = graphic != null ? graphic.color : Color.white,
                Amplitude = amplitude,
                Speed = speed,
                Phase = phase,
                RotationAmplitude = rotationAmplitude,
                ScaleAmplitude = scaleAmplitude,
                AlphaAmplitude = alphaAmplitude
            });
        }

        private void LateUpdate()
        {
            float time = Time.unscaledTime;

            for (int i = 0; i < motions.Count; i++)
            {
                MotionEntry entry = motions[i];
                if (entry.Rect == null)
                {
                    continue;
                }

                float primary = Mathf.Sin(time * entry.Speed + entry.Phase);
                float secondary = Mathf.Cos(time * (entry.Speed * 0.78f) + entry.Phase * 1.37f);

                entry.Rect.anchoredPosition = entry.BasePosition + new Vector2(entry.Amplitude.x * primary, entry.Amplitude.y * secondary);
                entry.Rect.localRotation = Quaternion.Euler(0f, 0f, entry.RotationAmplitude * primary);

                float scale = 1f + entry.ScaleAmplitude * secondary;
                entry.Rect.localScale = entry.BaseScale * scale;

                if (entry.Graphic != null)
                {
                    Color color = entry.BaseColor;
                    color.a = Mathf.Clamp01(entry.BaseColor.a + entry.AlphaAmplitude * primary);
                    entry.Graphic.color = color;
                }
            }
        }
    }
}
