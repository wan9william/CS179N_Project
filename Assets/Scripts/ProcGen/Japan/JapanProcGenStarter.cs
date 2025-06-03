using UnityEngine;

public class JapanProcGenStarter : MonoBehaviour
{
    [SerializeField] JapanCityDungeonGenerator japanCityDungeonGenerator;
    [SerializeField] AStarBootstrap pathfinder;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        japanCityDungeonGenerator.RunProceduralGeneration();
        pathfinder.Scan();
    }

}
