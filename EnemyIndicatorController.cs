using System;
using System.Runtime.Serialization.Formatters;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Crossover;

public class EnemyIndicatorController : EnemyTrackingBehavior {
    private bool _show = false;
    private bool _cleanedUp = false;

    // Indicator info
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private GameObject indicatorObject;
    private Image frontLayerImage;
    private Image backLayerImage;
    private TMPro.TMP_Text frontLayerText;
    private TMPro.TMP_Text backLayerText;

    // Enemy info
    private string enemyName = "Enemy";

    private bool enemyCountBelowThreshold {
        get {
            EnemyTracker tracker = MonoSingleton<EnemyTracker>.Instance;
            int count = tracker.GetCurrentEnemies().Count - tracker.deathcatcherCount;
            return count <= ConfigManager.TrackerThreshold.value;
        }
    }

    private void SetText(string text) {
        if (frontLayerText == null || backLayerText == null) return;
        frontLayerText.text = text;
        backLayerText.text = text;
    }

    private void SetAccentColor(Color color) {
        if (frontLayerImage == null || frontLayerText == null) return;
        frontLayerImage.color = color;
        frontLayerText.color = color;
    }

    private void SetBackgroundColor(Color color) {
        if (backLayerImage == null || backLayerText == null) return;
        backLayerImage.color = color;
        backLayerText.color = color;
    }

    private void SetScale(float scale) {
        if (rectTransform is null) return;
        rectTransform.localScale = new Vector3(scale, scale, scale);
    }

    private void SetAlpha(float alpha, Tuple<float, float> _ = null) {
        if (canvasGroup is null) return;
        canvasGroup.alpha = alpha;
        if (canvasGroup.alpha < 1) {
            SetBackgroundColor(Utils.Transparentize(backLayerImage.color));
        } else {
            SetBackgroundColor(Utils.Opacitize(backLayerImage.color));
        }
    }

    private void SetActive(bool active) {
        if (indicatorObject is null) return;
        indicatorObject.SetActive(active);
    }

    private void SetNameActive(bool active) {
        if (frontLayerText is not null) frontLayerText.gameObject.SetActive(active);
        if (backLayerText is not null) backLayerText.gameObject.SetActive(active);
    }

    private void UpdateAppearance() {
        if (indicatorObject is null) return;
        SetAccentColor(ConfigManager.TrackerColor.value);
        SetScale(ConfigManager.TrackerScale.value);
        SetAlpha(ConfigManager.TrackerMarkOpacity.value);
        SetNameActive(ConfigManager.TrackerShowEnemyNames.value);
        indicatorObject.SetActive(ConfigManager.EnableTrackers.value);
    }

    private void UpdateShow() {
        if (_show || !enemyCountBelowThreshold) return;
        // Init the indicator
        indicatorObject = Object.Instantiate(Plugin.EnemyIndicatorPrefab, Plugin.CrossoverCanvas.transform);
        rectTransform = indicatorObject.GetComponent<RectTransform>();
        canvasGroup = indicatorObject.GetComponent<CanvasGroup>();
        frontLayerImage = indicatorObject.transform.Find("EnemyTrackerFrontLayer").GetComponent<Image>();
        backLayerImage = indicatorObject.GetComponent<Image>();
        frontLayerText = indicatorObject.transform.Find("EnemyTrackerFrontLayer/FrontEnemyName")
            .GetComponent<TMP_Text>();
        backLayerText = indicatorObject.transform.Find("EnemyName")
            .GetComponent<TMP_Text>();
        // Set props
        SetText(enemyName);
        UpdateAppearance();
        _show = true;
    }

    private void HookStuff() {
        ConfigManager.EnableTrackers.postValueChangeEvent += SetActive;
        ConfigManager.TrackerShowEnemyNames.postValueChangeEvent += SetNameActive;
        ConfigManager.TrackerColor.postValueChangeEvent += SetAccentColor;
        ConfigManager.TrackerScale.postValueChangeEvent += SetScale;
        ConfigManager.TrackerMarkOpacity.postValueChangeEvent += SetAlpha;
        EnemyListener.SomeEnemyDied += UpdateShow;
    }

    private void UnhookStuff() {
        EnemyListener.SomeEnemyDied -= UpdateShow;
    }

    protected override void Start() {
        base.Start();
        enemyName = enemy.enemyType.ToString();
        HookStuff();
    }

    protected override void Update() {
        if (enemy == null || enemy.health <= 0f || enemy.transform == null) {
            RemoveIndicatorAndStop();
            return;
        }

        if (!_show) return;

        base.Update();
        rectTransform.position = canvasPoint;
    }

    private void RemoveIndicatorAndStop() {
        if (_cleanedUp) return;
        _cleanedUp = true;
        _show = false;

        // Clean up
        if (indicatorObject != null) {
            Destroy(indicatorObject);
        }

        UnhookStuff();
        Destroy(this);
    }

    private void OnDestroy() {
        UnhookStuff();
        if (indicatorObject != null) {
            Destroy(indicatorObject);
        }
    }
}
