using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EngagementView : MonoBehaviour
{
    private const float BarChangingDuration = 0.3f;
    private const float Tolerance = 1f;

    [SerializeField] private Slider _engagementSliderBar;

    private static EngagementView _instance;

    private Tween _healthBarTween;

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(_instance.gameObject);

        _instance = this;
    }

    public static void UpdateBar(float currentValue, float maxValue, bool instant = false)
    {
        if (_instance != null)
            _instance.UpdateBarInternal(currentValue, maxValue, instant);
    }

    private void UpdateBarInternal(float currentValue, float maxValue, bool instant = false)
    {
        ResetAnimations();

        _engagementSliderBar.maxValue = maxValue;

        if (Mathf.Abs(currentValue - _engagementSliderBar.value) < Tolerance)
            instant = true;

        if (instant)
            _engagementSliderBar.value = currentValue;
        else
            _healthBarTween = _engagementSliderBar.DOValue(currentValue, BarChangingDuration);
    }

    private void ResetAnimations()
    {
        _healthBarTween?.Kill();
    }
}
