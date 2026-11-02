using UnityEngine;

public class PowerupEventRouter : MonoBehaviour
{
    public PowerupGameEvent onPowerupAffectsPlayer;

    public void FilterAndCastPowerup(IPowerup powerup)
    {
        if (powerup == null)
            return;

        int type = (int)powerup.powerupType;

        // Mushroom = 1, StarMan = 3, FireFlower = 4
        if (type == 1 || type == 3 || type == 4)
        {
            onPowerupAffectsPlayer.Raise(powerup);
        }
    }
}