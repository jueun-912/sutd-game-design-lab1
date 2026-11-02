using UnityEngine;

[CreateAssetMenu(menuName = "PluggableSM/Actions/SetupInvincibility")]
public class InvincibleAction : Action
{
    public AudioClip sound;

    public override void Act(StateController controller)
    {
        MarioStateController mario =
            (MarioStateController)controller;

        mario.SetRendererToFlicker();

        AudioSource audio = controller.GetComponent<AudioSource>();

        if (audio != null && sound != null)
        {
            audio.PlayOneShot(sound);
        }
    }
}