using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButtonBinder : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    [Tooltip("Trascina qui il GameObject che ha BasicSpawner (quello con NetworkRunner)")]
    public BasicSpawner spawner;

    [Tooltip("Indice del cubo da attivare (0 = primo, 1 = secondo, ecc.)")]
    public int index;

    // Quando premi il pulsante UI
    public void OnPointerDown(PointerEventData e) {
        if (spawner) spawner.HoldDown(index);
    }

    // Quando rilasci il pulsante UI
    public void OnPointerUp(PointerEventData e) {
        if (spawner) spawner.HoldUp(index);
    }
}
