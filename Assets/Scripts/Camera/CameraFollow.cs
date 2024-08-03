using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player1;
    public Transform player2;
    public Vector3 offset;

    private void LateUpdate()
    {
        Vector3 midpoint = (player1.position + player2.position) / 2;
        transform.position = midpoint + offset;
    }
}