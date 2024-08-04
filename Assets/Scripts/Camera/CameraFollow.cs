using DG.Tweening;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player1;
    public Transform player2;
    public Vector3 offset;

    public bool isGameStarted = false;

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
        if (isGameStarted == true)
        {
            Vector3 midpoint = (player1.position + player2.position) / 2;
            transform.position = midpoint + offset;
        }
    }
}