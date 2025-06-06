using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class CompassDirection : MonoBehaviour
{
    public Transform player;
    public Transform ship;

    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        if (ship == null)
        {
            GameObject shipObj = GameObject.FindGameObjectWithTag("Ship");
            if (shipObj != null)
                ship = shipObj.transform;
        }
    }

    void Update()
    {
        if (player == null || ship == null) return;

        Vector2 direction = (ship.position - player.position);
        //rectTransform.LookAt(direction);

        //Vector2 direction = target.position - transform.position;
        transform.rotation = Quaternion.FromToRotation(Vector3.up, direction);
        //float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //rectTransform.rotation = Quaternion.Euler(0, 0, angle - 90); // -90 to point up
    }
}
