using UnityEngine;

[CreateAssetMenu(menuName = "PluggableSM/Actions/StartStar")]
public class StartStarAction : Action
{
    public AudioClip sound;

    public override void Act(StateController controller)
    {
        BuffStateController buffs =
            (BuffStateController)controller;

        buffs.StartInvincibility();

        AudioSource audio = controller.GetComponent<AudioSource>();

        if (audio != null && sound != null)
        {
            audio.PlayOneShot(sound);
        }
    }
}