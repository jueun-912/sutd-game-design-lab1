using UnityEngine;

[CreateAssetMenu(
    fileName = "GameConstants",
    menuName = "ScriptableObjects/GameConstants",
    order = 1
)]
public class GameConstants : ScriptableObject
{
    // 목숨
    public int maxLives;

    // 마리오 이동
    public int speed;
    public int maxSpeed;
    public int upSpeed;
    public int deathImpulse;
    public Vector3 marioStartingPosition;

    // 굼바 이동
    public float goombaPatrolTime;
    public float goombaMaxOffset;
}