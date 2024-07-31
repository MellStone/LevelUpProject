using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Bobbing : MonoBehaviour
{
    public GameObject obj;

    private void Start()
    {
        obj.transform.DOShakePosition(1f, 0.2f, 1, 30)
            .SetLoops(-1, LoopType.Incremental)
            .SetRelative();
    }
}
