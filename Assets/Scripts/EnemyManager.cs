using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public void GameRestart()
    {
        foreach (Transform child in transform)
        {
            EnemyMovement enemyMovement =
                child.GetComponent<EnemyMovement>();

            GoombaStomp goombaStomp =
                child.GetComponent<GoombaStomp>();

            if (enemyMovement != null)
            {
                enemyMovement.GameRestart();
            }

            if (goombaStomp != null)
            {
                goombaStomp.GameRestart();
            }
        }
    }
}