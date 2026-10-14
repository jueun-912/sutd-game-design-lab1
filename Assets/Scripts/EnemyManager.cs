using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    void Awake()
    {
        GameManager.instance.gameRestart.AddListener(GameRestart);
    }

    void OnDestroy()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.gameRestart.RemoveListener(GameRestart);
        }
    }

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