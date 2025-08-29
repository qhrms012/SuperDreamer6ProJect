using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIHitTester : MonoBehaviour
{
    [SerializeField] GraphicRaycaster raycaster;
    [SerializeField] EventSystem eventSystem;

    void Reset()
    {
        if (!raycaster) raycaster = GetComponentInParent<Canvas>()?.GetComponent<GraphicRaycaster>();
        if (!eventSystem) eventSystem = FindObjectOfType<EventSystem>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ped = new PointerEventData(eventSystem) { position = Input.mousePosition };
            var results = new List<RaycastResult>();
            raycaster.Raycast(ped, results);
            Debug.Log("UI hits:\n" + string.Join("\n", results.Select(r => r.gameObject.name)));
        }
    }
}
