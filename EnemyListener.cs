namespace Crossover;

public static class EnemyListener {
    public static event OnSomeEnemyDied SomeEnemyDied;
    public delegate void OnSomeEnemyDied();

    internal static void NotifyDeath() {
        SomeEnemyDied?.Invoke();
    }
}
