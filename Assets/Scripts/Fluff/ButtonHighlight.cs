using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Bobbing titleBob;
    public float newDuration = 0.2f;
    public float oldDuration = 1f;


    public void OnPointerEnter(PointerEventData eventData)
    {
        titleBob.UpdateDuration(newDuration);
        Debug.Log("Enters");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        titleBob.UpdateDuration(oldDuration);
        Debug.Log("Exits");
    }
}
