using UnityEngine;

[System.Serializable]
public struct StateTransformMap
{
    public MarioState fromState;
    public PowerupType powerupCollected;
}

[CreateAssetMenu(menuName = "PluggableSM/Decisions/Transform")]
public class TransformDecision : Decision
{
    public StateTransformMap[] map = new StateTransformMap[0];

    public override bool Decide(StateController controller)
    {
        MarioStateController mario =
            (MarioStateController)controller;

        foreach (StateTransformMap entry in map)
        {
            if (mario.currentMarioState == entry.fromState &&
                mario.currentPowerupType == entry.powerupCollected)
            {
                return true;
            }
        }

        return false;
    }
}