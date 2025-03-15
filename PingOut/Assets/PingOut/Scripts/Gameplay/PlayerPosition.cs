using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CircleCollider2D))]
public class PlayerPosition : MonoBehaviour, IPointerClickHandler
{
    public UnityEvent<PlayerPosition> OnTouchPos;

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        OnTouchPos?.Invoke(this);
        //Debug.Log($"Touch {gameObject.name}", this);
    }

}
