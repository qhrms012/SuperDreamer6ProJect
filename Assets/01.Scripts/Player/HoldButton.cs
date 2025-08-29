using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public bool IsHeld { get; private set; }
    public void OnPointerDown(PointerEventData e) => IsHeld = true;
    public void OnPointerUp(PointerEventData e) => IsHeld = false;
    public void OnPointerExit(PointerEventData e) => IsHeld = false;
}

