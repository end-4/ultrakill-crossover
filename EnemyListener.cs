namespace Crossover;

public static class EnemyListener {
    public static event OnSomeEnemyDied EnemyCountChanged;
    public delegate void OnSomeEnemyDied();

    internal static void NotifyChange() {
        EnemyCountChanged?.Invoke();
    }
}
