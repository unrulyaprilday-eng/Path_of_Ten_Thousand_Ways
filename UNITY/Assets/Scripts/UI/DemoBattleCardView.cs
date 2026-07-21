using System;
using PathOfTenThousandWays.Demo.Cards;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PathOfTenThousandWays.Demo.UI
{
    public sealed class DemoBattleCardView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        private RectTransform rect;
        private DemoCard card;
        private Action<int> playAction;
        private Action<DemoCard, bool> hoverAction;
        private int handIndex;
        private bool playable;
        private bool hovered;
        private int restSiblingIndex;
        private float blockedPulse;
        private Vector2 restPosition;
        private float restRotation;

        public void Configure(
            DemoCard value,
            int index,
            bool canPlay,
            Action<int> onPlay,
            Action<DemoCard, bool> onHover)
        {
            rect = GetComponent<RectTransform>();
            card = value;
            handIndex = index;
            playable = canPlay;
            playAction = onPlay;
            hoverAction = onHover;
            restSiblingIndex = transform.GetSiblingIndex();
        }

        public void SetRestPose(Vector2 position, float rotation)
        {
            restPosition = position;
            restRotation = rotation;
            if (!hovered)
            {
                rect.anchoredPosition = position;
                rect.localRotation = Quaternion.Euler(0f, 0f, rotation);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            hovered = true;
            rect.SetAsLastSibling();
            hoverAction?.Invoke(card, true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            hovered = false;
            transform.SetSiblingIndex(Mathf.Clamp(restSiblingIndex, 0, transform.parent.childCount - 1));
            hoverAction?.Invoke(card, false);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            if (playable)
            {
                playAction?.Invoke(handIndex);
            }
            else
            {
                blockedPulse = 0.28f;
            }
        }

        private void Update()
        {
            if (rect == null)
            {
                return;
            }

            float delta = Time.unscaledDeltaTime;
            blockedPulse = Mathf.Max(0f, blockedPulse - delta);
            float shake = blockedPulse > 0f ? Mathf.Sin(blockedPulse * 92f) * 5f : 0f;
            Vector2 targetPosition = restPosition + new Vector2(shake, hovered ? 30f : 0f);
            float interpolation = 1f - Mathf.Exp(-delta * 18f);
            rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, targetPosition, interpolation);
            float targetScale = hovered ? 1.065f : 1f;
            rect.localScale = Vector3.Lerp(rect.localScale, new Vector3(targetScale, targetScale, 1f), interpolation);
            float rotation = Mathf.LerpAngle(rect.localEulerAngles.z, hovered ? 0f : restRotation, interpolation);
            rect.localRotation = Quaternion.Euler(0f, 0f, rotation);
        }
    }
}
