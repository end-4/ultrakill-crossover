using System;
using System.Transactions;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Crossover;

public class CrossBehavior : EnemyTrackingBehavior {
    private enum State {
        Hidden,
        Marking
    }

    // Params
    public bool startMarkImmediately = true;
    public float markLifetime = 0.5333f;
    public float scalingDelay = 0.0000f;
    public float scalingDuration = 0.1333f;
    public float startScale = 1f;
    public float endScale = 0.2f;

    public Tuple<float, Color>[] GetUpdatedColorSteps() {
        Color backLayerImageColor = backLayerImage.color;
        Color transparentizedBackLayerImageColor = Utils.Transparentize(backLayerImageColor);
        return [
            Tuple.Create(0.0000f, Color.white),
            Tuple.Create(0.0333f, Color.black),
            Tuple.Create(0.0667f, Color.white),
            Tuple.Create(0.1333f, Color.black),
            Tuple.Create(0.2000f, backLayerImage.color),
            Tuple.Create(0.4000f, transparentizedBackLayerImageColor),
            Tuple.Create(0.4667f, backLayerImage.color),
            Tuple.Create(0.5333f, transparentizedBackLayerImageColor)
        ];
    }

    public Tuple<float, Color>[] colorSteps = [];

    // Object info
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Image frontLayerImage;
    private Image backLayerImage;

    // Animation
    private State state = State.Hidden;
    private float startMarkTime;
    private float scaleDiff => (startScale - endScale);

    private void Awake() {
        frontLayerImage = transform.Find("DeathCrossFrontLayer").gameObject.GetComponent<Image>();
        backLayerImage = gameObject.GetComponent<Image>();
        backLayerImage.color = ConfigManager.AccentColor.value;
        colorSteps = GetUpdatedColorSteps();
        startScale = ConfigManager.StartScale.value;
        endScale = ConfigManager.EndScale.value;
        scalingDuration = ConfigManager.ScalingDuration.value;
        scalingDelay = ConfigManager.ScalingDelay.value;
        markLifetime = ConfigManager.VisibleDuration.value;
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = ConfigManager.CrossMarkOpacity.value;
        if (canvasGroup.alpha < 1) { // Disable shadow if transparent
            backLayerImage.color = Utils.Transparentize(backLayerImage.color);
        }
    }

    protected override void Start() {
        rectTransform = transform as RectTransform;
        base.Start();
    }

    private float Curve(float x) {
        float t = Math.Clamp(x, 0, 1);
        // return t * (2 - t);
        float i = 1 - t;
        return 1 - (i * i * i);
    }

    private void UpdateColor(float elapsedTime) {
        int i = 0;
        if (elapsedTime < 0) return;
        while (i < colorSteps.Length && elapsedTime >= colorSteps[i].Item1) i++;
        frontLayerImage.color = colorSteps[i - 1].Item2;
    }

    private void SetScale(float newScale) {
        rectTransform.localScale = new Vector3(newScale, newScale, newScale);
    }

    private void UpdateScale(float percentage) {
        float newScale = startScale - Curve(percentage) * scaleDiff;
        SetScale(newScale);
    }

    protected override void Update() {
        if (state == State.Hidden && (startMarkImmediately || enemy == null || enemy.health <= 0f)) {
            state = State.Marking;
            startMarkTime = Time.time;
        }

        if (state == State.Hidden) return;

        float elapsedTime = Time.time - startMarkTime;
        UpdateColor(elapsedTime);
        UpdateScale((elapsedTime - scalingDelay) / scalingDuration);
        base.Update();
        rectTransform.position = canvasPoint;

        if (Time.time - startMarkTime >= markLifetime) {
            Destroy(gameObject);
            return;
        }
    }
}
