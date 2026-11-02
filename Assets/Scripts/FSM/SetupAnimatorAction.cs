using UnityEngine;

[CreateAssetMenu(menuName = "PluggableSM/Actions/SetupAnimator")]
public class SetupAnimatorAction : Action
{
    public MarioState marioState;
    public RuntimeAnimatorController animatorController;
    public AudioClip sound;

    [Range(0.1f, 2f)]
    public float sizeMultiplier = 1f;

    public override void Act(StateController controller)
    {
        MarioStateController mario =
            (MarioStateController)controller;

        PlayerMovement player =
            controller.GetComponent<PlayerMovement>();

        Animator animator = controller.GetComponent<Animator>();
        SpriteRenderer sprite =
            controller.GetComponent<SpriteRenderer>();

        BoxCollider2D box =
            controller.GetComponent<BoxCollider2D>();

        Rigidbody2D body =
            controller.GetComponent<Rigidbody2D>();

        // 변신 전 발이 서 있던 높이
        float feetY = box.bounds.min.y;

        mario.StopFlickering();
        mario.currentMarioState = marioState;

        bool dead = marioState == MarioState.DeadMario;

        player.alive = !dead;

        if (animatorController != null)
        {
            animator.runtimeAnimatorController = animatorController;
        }

        bool large =
            marioState == MarioState.SuperMario ||
            marioState == MarioState.FireMario;

        float multiplier = large ? sizeMultiplier : 1f;

        controller.transform.localScale =
            player.OriginalScale * multiplier;

        player.RefreshAnimationState();

        if (dead)
        {
            animator.Play("mario-die", 0, 0f);
        }
        else
        {
            // 切り替え後のSpriteを反映
            animator.Update(0f);

            if (large && sprite.sprite != null)
            {
                // 大きいMarioは表示中のSpriteにColliderを合わせる
                Bounds bounds = sprite.sprite.bounds;

                box.size = new Vector2(
                    bounds.size.x,
                    bounds.size.y
                );

                box.offset = new Vector2(
                    bounds.center.x,
                    bounds.center.y
                );
            }
            else
            {
                // 元の小さいMarioのCollider
                box.size = new Vector2(1f, 1.210146f);
                box.offset = new Vector2(0f, 0.10626f);
            }

            // ScaleやColliderを変更しても足元の高さを保つ
            Physics2D.SyncTransforms();

            float adjustment = feetY - box.bounds.min.y;

            Vector3 position = controller.transform.position;
            position.y += adjustment;

            controller.transform.position = position;

            if (body != null)
            {
                body.position = new Vector2(
                    position.x,
                    position.y
                );
            }

            Physics2D.SyncTransforms();
        }

        AudioSource audio =
            controller.GetComponent<AudioSource>();

        if (audio != null && sound != null)
        {
            audio.PlayOneShot(sound);
        }
    }
}