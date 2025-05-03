using UnityEngine;
using Pathfinding;

public class AStarBootstrap : MonoBehaviour
{
    void Start()
    {
        AstarPath.active.Scan();
    }
}
