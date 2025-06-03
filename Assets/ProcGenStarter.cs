using UnityEngine;

public class ProcGenStarter : MonoBehaviour
{
    [SerializeField] DungeonGenerator dungeonGenerator;
    [SerializeField] AStarBootstrap pathfinder;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dungeonGenerator.RunProceduralGeneration();
        pathfinder.Scan();
    }

}
