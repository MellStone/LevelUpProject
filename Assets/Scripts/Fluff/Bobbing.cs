using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Bobbing : MonoBehaviour
{
    public GameObject obj;
    public float duration = 1f;
    public float strength = 0.2f;
    public int vibrato = 1;

    private Tweener shakeTween;

    private void Start()
    {
        CreateShakeTween();
    }

    private void CreateShakeTween()
    {
        // Kill the existing tween if it's active
        if (shakeTween != null && shakeTween.IsActive())
        {
            shakeTween.Kill();
        }

        // Create a new shake tween with the current duration
        shakeTween = obj.transform.DOShakePosition(duration, strength, vibrato, 30)
            .SetLoops(-1, LoopType.Incremental)
            .SetRelative();
    }

    public void UpdateDuration(float newDuration)
    {
        duration = newDuration;
        CreateShakeTween();
    }
}
