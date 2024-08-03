using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Starting : MonoBehaviour
{
    public CanvasGroup canvas;

    private void Start()
    {
        canvas.DOFade(0f, 6f);
    }
}
