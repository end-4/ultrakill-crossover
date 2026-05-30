using System;
using System.Linq;
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
    internal GameObject indicatorObject;
    private Image frontLayerImage;
    private Image backLayerImage;
    private TMPro.TMP_Text frontLayerText;
    private TMPro.TMP_Text backLayerText;

    private static readonly string DEFAULT_ICON = "dot_circle";

    // Enemy info
    private string enemyName = "Enemy";

    private bool enemyCountBelowThreshold {
        get {
            EnemyTracker tracker = MonoSingleton<EnemyTracker>.Instance;
            int count = tracker.GetCurrentEnemies().Where(e => !e.puppet).Count();
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
        // The specific icons are already colored so don't recolor them
        if (frontLayerImage.sprite.name != DEFAULT_ICON) color = Color.white;
        frontLayerImage.color = color;
        frontLayerText.color = color;
    }

    private void SetBackgroundColor(Color color) {
        if (backLayerImage == null || backLayerText == null) return;
        backLayerImage.color = color;
        backLayerText.color = color;
    }

    private void SetScale(float scale) {
        if (rectTransform == null) return;
        rectTransform.localScale = new Vector3(scale, scale, scale);
        clampPadding = Math.Max(rectTransform.rect.width, rectTransform.rect.height);
    }

    private void SetAlpha(float alpha, Tuple<float, float> _ = null) {
        if (canvasGroup == null) return;
        canvasGroup.alpha = alpha;
        if (canvasGroup.alpha < 1) {
            SetBackgroundColor(Utils.Transparentize(backLayerImage.color));
        } else {
            SetBackgroundColor(Utils.Opacitize(backLayerImage.color));
        }
    }

    private void SetActive(bool active) {
        if (indicatorObject == null) return;
        indicatorObject.SetActive(active);
    }

    private void SetNameActive(bool active) {
        if (frontLayerText != null) frontLayerText.gameObject.SetActive(active);
        if (backLayerText != null) backLayerText.gameObject.SetActive(active);
    }

    private void SetEnemyIcon(bool useSpecificEnemyIcon) {
        string enemyTypeId = enemy.enemyType.ToString();
        string iconName = DEFAULT_ICON;
        if (useSpecificEnemyIcon) {
            string potentialIconName = Utils.ToSnakeCase(enemyTypeId);
            if (Plugin.EnemyIcons.ContainsKey(potentialIconName)) {
                iconName = potentialIconName;
            } else {
                string fullNameLower = enemy.FullName.ToLower();
                // Plugin.Log.LogInfo($"full name {fullName}");
                if (fullNameLower == "earthmover mortar") potentialIconName = "centaur_mortar";
                else if (fullNameLower == "earthmover rocket launcher") potentialIconName = "centaur_rocket";
                else if (fullNameLower == "earthmover tower") potentialIconName = "centaur_orb";
                else if (fullNameLower == "cancerous rodent") potentialIconName = "cancerous_rodent";
                else if (fullNameLower == "very cancerous rodent") potentialIconName = "very_cancerous_rodent";
                else if (fullNameLower == "big johninator") potentialIconName = "big_johninator";
                if (Plugin.EnemyIcons.ContainsKey(potentialIconName)) {
                    iconName = potentialIconName;
                }
            }
        }

        if (Plugin.EnemyIcons.TryGetValue(iconName, out Sprite icon)) {
            frontLayerImage.sprite = icon;
            backLayerImage.sprite = icon;
            SetAccentColor(ConfigManager.TrackerColor.value);
        }
    }

    private void UpdateAppearance() {
        if (indicatorObject == null) return;
        SetAccentColor(ConfigManager.TrackerColor.value);
        SetScale(ConfigManager.TrackerScale.value);
        SetAlpha(ConfigManager.TrackerMarkOpacity.value);
        SetNameActive(ConfigManager.TrackerShowEnemyNames.value);
        SetEnemyIcon(ConfigManager.TrackerUseSpecificEnemyIcons.value);
        indicatorObject.SetActive(ConfigManager.EnableTrackers.value);
    }

    private void UpdateShow() {
        if (!enemyCountBelowThreshold && !ConfigManager.IsEnemyForceTracked(enemy.enemyType)) {
            _show = false;
            if (indicatorObject != null) indicatorObject.SetActive(false);
            return;
        }

        // Init the indicator
        if (indicatorObject == null) {
            indicatorObject = Object.Instantiate(Plugin.EnemyIndicatorPrefab, Plugin.CrossoverCanvas.transform);
            rectTransform = indicatorObject.GetComponent<RectTransform>();
            canvasGroup = indicatorObject.GetComponent<CanvasGroup>();
            frontLayerImage = indicatorObject.transform.Find("EnemyTrackerFrontLayer").GetComponent<Image>();
            backLayerImage = indicatorObject.GetComponent<Image>();
            frontLayerText = indicatorObject.transform.Find("EnemyTrackerFrontLayer/FrontEnemyName")
                .GetComponent<TMP_Text>();
            backLayerText = indicatorObject.transform.Find("EnemyName")
                .GetComponent<TMP_Text>();
        }

        // Set props
        SetText(enemyName);
        UpdateAppearance();
        _show = true;
    }

    private void HookStuff() {
        ConfigManager.EnableTrackers.postValueChangeEvent += SetActive;
        ConfigManager.TrackerUseSpecificEnemyIcons.postValueChangeEvent += SetEnemyIcon;
        ConfigManager.TrackerShowEnemyNames.postValueChangeEvent += SetNameActive;
        ConfigManager.TrackerColor.postValueChangeEvent += SetAccentColor;
        ConfigManager.TrackerScale.postValueChangeEvent += SetScale;
        ConfigManager.TrackerMarkOpacity.postValueChangeEvent += SetAlpha;
        EnemyListener.EnemyCountChanged += UpdateShow;
        enemy.destroyOnDeath.Add(indicatorObject);
    }

    private void UnhookStuff() {
        EnemyListener.EnemyCountChanged -= UpdateShow;
        ConfigManager.EnableTrackers.postValueChangeEvent -= SetActive;
        ConfigManager.TrackerUseSpecificEnemyIcons.postValueChangeEvent -= SetEnemyIcon;
        ConfigManager.TrackerShowEnemyNames.postValueChangeEvent -= SetNameActive;
        ConfigManager.TrackerColor.postValueChangeEvent -= SetAccentColor;
        ConfigManager.TrackerScale.postValueChangeEvent -= SetScale;
        ConfigManager.TrackerMarkOpacity.postValueChangeEvent -= SetAlpha;
    }

    private void Awake() {
        clampToScreen = true;
    }

    protected override void Start() {
        base.Start();
        enemyName = enemy?.enemyType.ToString() ?? "Enemy";
        // Plugin.Log.LogInfo($"[+] {enemyName}");
        UpdateShow();
        HookStuff();
    }

    internal void PrintDebugInfo() {
        string tranzform = enemy?.transform.ToString() ?? "NULL TRANSFORM";
        string pos = $"{(enemy?.transform?.position ?? (Vector3.one * -1)).ToString()}";
        string hp = $"{(enemy?.health ?? -1)}";
        string indicatorObjPos = indicatorObject?.transform.position.ToString() ?? "NULL POS";
        Plugin.Log.LogInfo($"--- Enemy {enemyName} | transform: {tranzform} | pos: {pos} | hp: {hp} | indicatorObject: {indicatorObject} | indicatorObjPos: {indicatorObjPos}");
    }

    protected override void Update() {
        // if (Input.GetKeyDown(KeyCode.RightBracket)) {
        //     PrintDebugInfo();
        // }
        if (enemy == null || enemy.health <= 0f || enemy.transform == null) {
            RemoveIndicatorAndStop();
            // Plugin.Log.LogInfo($"ENEMY DIED/NULL: {enemyName}");
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
        // Plugin.Log.LogInfo($"OBJECT DESTROYED FOR {enemyName}");
        // Plugin.Log.LogInfo($"[-] {enemyName}");
        UnhookStuff();
        if (indicatorObject != null) {
            Destroy(indicatorObject);
        }
    }
}
