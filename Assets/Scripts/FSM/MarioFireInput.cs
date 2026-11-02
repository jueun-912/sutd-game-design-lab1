using UnityEngine;
using UnityEngine.InputSystem;

public class MarioFireInput : MonoBehaviour
{
    private MarioStateController states;
    private PlayerMovement player;

    private void Awake()
    {
        states = GetComponent<MarioStateController>();
        player = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (Keyboard.current == null ||
            !player.alive ||
            Time.timeScale == 0f)
        {
            return;
        }

        if (Keyboard.current.zKey.wasPressedThisFrame)
            states.Fire();
    }
}