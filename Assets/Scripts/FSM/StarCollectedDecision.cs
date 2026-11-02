using UnityEngine;

[CreateAssetMenu(menuName = "PluggableSM/Decisions/StarCollected")]
public class StarCollectedDecision : Decision
{
    public override bool Decide(StateController controller)
    {
        BuffStateController buffs =
            (BuffStateController)controller;

        return buffs.starCollected;
    }
}