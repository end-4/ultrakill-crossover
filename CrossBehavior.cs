using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Crossover;

public class CrossBehavior : MonoBehaviour {
    private enum State {
        Hidden,
        Marking
    }

    // Params
    public float markLifetime = 0.4f;
    public float scalingDuration = 0.1333f;
    public float startScale = 1f;
    public float endScale = 0.2f;

    public Tuple<float, Color>[] colorSteps = {
        Tuple.Create(0f, Color.white),
        Tuple.Create(0.0333f, Color.black),
        Tuple.Create(0.0667f, Color.white),
        Tuple.Create(0.1333f, Color.black),
        Tuple.Create(0.2f, Color.red),
    };


    // Object info
    public EnemyIdentifier enemy;
    private CanvasGroup canvasGroup;
    private float enemyHeight;
    private RectTransform rectTransform;
    private Image frontLayerImage;

    // Animation
    private State state = State.Hidden;
    private float startMarkTime;
    private Vector3 targetPoint;
    private Vector3 lastEnemyPosition = Vector3.zero;
    private float scaleDiff => (startScale - endScale);

    private void Awake() {
        canvasGroup = GetComponent<CanvasGroup>();
        frontLayerImage = transform.Find("DeathCrossFrontLayer").gameObject.GetComponent<Image>();
        Image backLayerImage = gameObject.GetComponent<Image>();
        backLayerImage.color = ConfigManager.AccentColor.value;
        colorSteps[^1] = Tuple.Create(colorSteps[^1].Item1, backLayerImage.color);
        startScale = ConfigManager.StartScale.value;
        endScale = ConfigManager.EndScale.value;
    }

    private void Start() {
        rectTransform = transform as RectTransform;
        Collider collider = enemy.GetComponent<Collider>();
        enemyHeight = (collider.bounds.center - enemy.transform.position).y + collider.bounds.extents.y;
    }

    private float Curve(float x) {
        float t = Math.Clamp(x, 0, 1);
        // return t * (2 - t);
        float i = 1 - t;
        return 1 - (i * i * i);
    }

    private Vector3 GetMarkPosition() {
        if (enemy != null && enemy.transform != null)
            lastEnemyPosition = enemy.transform.position;
        return lastEnemyPosition + enemyHeight / 2 * Vector3.up;
    }

    private void UpdateTargetPoint() {
        targetPoint = MonoSingleton<CameraController>.Instance.cam.WorldToScreenPoint(GetMarkPosition());
        int width = PostProcessV2_Handler.Instance.GetPrivateField<int>("width");
        int height = PostProcessV2_Handler.Instance.GetPrivateField<int>("height");

        targetPoint.x *= Screen.width;
        targetPoint.y *= Screen.height;
        targetPoint.x /= width;
        targetPoint.y /= height;

        rectTransform.position = targetPoint;
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

    private void Update() {
        if (state == State.Hidden && (enemy == null || enemy.health <= 0f)) {
            state = State.Marking;
            startMarkTime = Time.time;
            canvasGroup.alpha = ConfigManager.CrossMarkOpacity.value;
        }

        if (state == State.Hidden) return;

        float elapsedTime = Time.time - startMarkTime;
        UpdateColor(elapsedTime);
        UpdateScale(elapsedTime / scalingDuration);
        UpdateTargetPoint();

        if (Time.time - startMarkTime >= markLifetime) {
            Destroy(gameObject);
            return;
        }
    }
}
