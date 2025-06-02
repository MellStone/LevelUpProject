using DG.Tweening;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player1;
    public Transform player2;
    public Vector3 offset;

    public bool isGameStarted = false;

    private Rigidbody rbPlayer1;
    private Rigidbody rbPlayer2;

    private void Start()
    {
        rbPlayer1 = player1.GetComponent<Rigidbody>();
        rbPlayer2 = player2.GetComponent<Rigidbody>();
    }

    public void CameraIntro()
    {
        this.gameObject.transform.DOMoveZ(-5, 3f)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                isGameStarted = true;
            });
    }

    private void LateUpdate()
    {
        if (isGameStarted)
        {
            if (IsPlayerActive(player1) && IsPlayerActive(player2))
            {
                Vector3 midpoint = (player1.position + player2.position) / 2;
                transform.position = midpoint + offset;
            }
            else if (IsPlayerActive(player1))
            {
                transform.position = player1.position + offset;
            }
            else if (IsPlayerActive(player2))
            {
                transform.position = player2.position + offset;
            }
        }
    }

    private bool IsPlayerActive(Transform player)
    {
        // Проверяем, активен ли игрок (например, по наличию скорости)
        if (player != null && player.gameObject.activeInHierarchy)
        {
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                return rb.linearVelocity.sqrMagnitude > 0.1f; // Проверяем, движется ли игрок
            }
        }
        return false;
    }
}