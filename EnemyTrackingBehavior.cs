using UnityEngine;

namespace Crossover;

public class EnemyTrackingBehavior: MonoBehaviour {
    public Vector3 canvasPoint;
    protected Vector3 lastEnemyPosition = Vector3.zero;

    public EnemyIdentifier enemy;
    protected float enemyHeight;

    public void SetEnemy(EnemyIdentifier enemy) {
        this.enemy = enemy;
    }

    protected Vector3 GetEnemyPosition() {
        if (enemy != null && enemy.transform != null)
            lastEnemyPosition = enemy.transform.position;
        return lastEnemyPosition + enemyHeight / 2 * Vector3.up;
    }

    private void UpdateCanvasPoint() {
        canvasPoint = MonoSingleton<CameraController>.Instance.cam.WorldToScreenPoint(GetEnemyPosition());
        int width = PostProcessV2_Handler.Instance.GetPrivateField<int>("width");
        int height = PostProcessV2_Handler.Instance.GetPrivateField<int>("height");

        canvasPoint.x *= Screen.width;
        canvasPoint.y *= Screen.height;
        canvasPoint.x /= width;
        canvasPoint.y /= height;
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
