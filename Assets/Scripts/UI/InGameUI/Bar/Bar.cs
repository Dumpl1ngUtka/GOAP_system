using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI.InGameUI.Bar
{
    public class Bar : MonoBehaviour, IUIBar
    {
        [SerializeField] private Image _healthImage;
        [SerializeField] private CanvasGroup _canvasGroup;

        private const float FadeDuration = 0.5f;

        public void Show()
        {
            StopAllCoroutines();
            StartCoroutine(Fade(1f));
        }

        public void Hide()
        {
            StopAllCoroutines();
            StartCoroutine(Fade(0f));
        }

        private IEnumerator Fade(float targetAlpha)
        {
            var startAlpha = _canvasGroup.alpha;
            var time = 0f;

            while (time < FadeDuration)
            {
                time += Time.deltaTime;
                _canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / FadeDuration);
                yield return null;
            }

            _canvasGroup.alpha = targetAlpha;
        }

        public void UpdateValue(float currentValue, float maxValue)
        {
            _healthImage.fillAmount = currentValue / maxValue;
        }

        public void UpdateValue(float currentValue)
        {
            _healthImage.fillAmount = currentValue;
        }
    }
}
