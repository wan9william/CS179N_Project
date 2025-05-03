using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemySettings", menuName = "Enemies/Enemy Settings")]
public class EnemySettings : ScriptableObject
{
    public float speed = 3f;
    public float stopDistance = 1.5f;
    public float nextWaypointDistance = 0.3f;
    public bool flipSprite = true;
}
