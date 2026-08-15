using System;
using System.Drawing;
using UnityEngine;

namespace Crossover;

public class EnemyTrackingBehavior: MonoBehaviour {
    public Vector3 canvasPoint;
    protected Vector3 lastEnemyPosition = Vector3.zero;

    public EnemyIdentifier enemy;
    protected float enemyHeight;

    public bool clampToScreen = false;
    public float clampPadding = 0;

    public void SetEnemy(EnemyIdentifier enemy) {
        this.enemy = enemy;
    }

    protected Vector3 GetEnemyPosition() {
        if (enemy != null && enemy.transform != null)
            lastEnemyPosition = enemy.transform.position;
        return lastEnemyPosition + enemyHeight / 2 * Vector3.up;
    }

    private void UpdateCanvasPoint() {
        Vector3 point = MonoSingleton<CameraController>.Instance.cam.WorldToScreenPoint(GetEnemyPosition());
        bool isBehindCamera = point.z < 0;
        int width = PostProcessV2_Handler.Instance.GetPrivateField<int>("width");
        int height = PostProcessV2_Handler.Instance.GetPrivateField<int>("height");

        point.x *= Screen.width;
        point.y *= Screen.height;
        point.x /= width;
        point.y /= height;

        if (isBehindCamera) {
            float centerX = width / 2f;
            float dirX = (width - point.x) - centerX;
            // Edge case: perfectly centered behind
            if (Mathf.Approximately(dirX, 0f)) {
                dirX = -1f;
            }
            // Push beyond screen bounds for clamp
            point.x = centerX + (dirX / Math.Abs(dirX) * Screen.width / 2);
        }

        if (clampToScreen) {
            point.x = Mathf.Clamp(point.x, clampPadding, width - clampPadding);
            point.y = Mathf.Clamp(point.y, clampPadding, height - clampPadding);
        }

        canvasPoint = point;
    }

    protected virtual void Start() {
        if (enemy == null) return;
        Collider collider = enemy.GetComponent<Collider>();
        enemyHeight = (collider.bounds.center - enemy.transform.position).y + collider.bounds.extents.y;
    }

    protected virtual void Update() {
        UpdateCanvasPoint();
    }
}
