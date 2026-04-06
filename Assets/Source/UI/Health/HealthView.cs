using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HealthView : MonoBehaviour
{
    private const float HealthBarChangingDuration = 0.3f;
    private const float ImpactViewDelay = 0.7f;
    private const float Tolerance = 1f;

    [SerializeField] private Slider _healthSliderBar;
    [SerializeField] private Slider _impactSliderBar;

    private static HealthView _instance;

    private Tween _impactBarTween;
    private Tween _healthBarTween;

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(_instance.gameObject);

        _instance = this;

        _healthSliderBar.minValue = 0f;
        _impactSliderBar.minValue = 0f;
        UpdateBar(100f, 100f, true);
    }

    private void OnDisable()
    {
        ResetAnimations();
    }

    public static void UpdateBar(float currentValue, float maxValue, bool instant = false)
    {
        if (_instance != null)
            _instance.UpdateBarInternal(currentValue, maxValue, instant);
    }

    private void UpdateBarInternal(float currentValue, float maxValue, bool instant = false)
    {
        ResetAnimations();

        _healthSliderBar.maxValue = maxValue;
        _impactSliderBar.maxValue = maxValue;

        if (Mathf.Abs(currentValue - _healthSliderBar.value) < Tolerance)
            instant = true;

        if (instant)
        {
            _healthSliderBar.value = currentValue;
            _impactSliderBar.value = currentValue;
        }
        else
        {
            _healthBarTween = _healthSliderBar.DOValue(currentValue, HealthBarChangingDuration);
            _impactBarTween = _impactSliderBar.DOValue(currentValue, HealthBarChangingDuration).SetDelay(ImpactViewDelay);
        }
    }

    private void ResetAnimations()
    {
        _impactBarTween?.Kill();
        _healthBarTween?.Kill();
    }
}