using UnityEngine;

[CreateAssetMenu(menuName = "PluggableSM/Actions/ClearStar")]
public class ClearStarAction : Action
{
    public AudioClip sound;

    public override void Act(StateController controller)
    {
        BuffStateController buffs =
            (BuffStateController)controller;

        buffs.ClearBuff();

        AudioSource audio = controller.GetComponent<AudioSource>();

        if (audio != null && sound != null &&
            controller.previousState != null)
        {
            audio.PlayOneShot(sound);
        }
    }
}