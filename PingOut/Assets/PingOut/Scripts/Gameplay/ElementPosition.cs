using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CircleCollider2D))]
public class ElementPosition : MonoBehaviour, IPointerClickHandler
{
    public UnityEvent<ElementPosition> OnTouchPos;

    public bool isInteractive = true;

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (!isInteractive)return;

        OnTouchPos?.Invoke(this);
        //Debug.Log($"Touch {gameObject.name}", this);
    }

}
