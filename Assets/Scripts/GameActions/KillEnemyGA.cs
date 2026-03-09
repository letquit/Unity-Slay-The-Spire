using UnityEngine;

public class KillEnemyGA : GameAction
{
    public EnemyView EnemyView { get; set; }

    public KillEnemyGA(EnemyView enemyView)
    {
        EnemyView = enemyView;
    }
}
