using UnityEngine;

public class ControllerInputDebug : MonoBehaviour {
  public BasicSpawner spawner;  // trascina qui il tuo BasicSpawner
  public int index = 0;         // indice del cubo che vuoi testare

  void Update() {
    // A premuto
    if (OVRInput.GetDown(OVRInput.Button.One)) {
      spawner?.HoldDown(index);
      Debug.Log($"[DEBUG] Tasto A premuto -> Cubo {index} acceso");
    }
    // A rilasciato
    if (OVRInput.GetUp(OVRInput.Button.One)) {
      spawner?.HoldUp(index);
      Debug.Log($"[DEBUG] Tasto A rilasciato -> Cubo {index} spento");
    }
  }
}
