using UnityEngine;

public class ToggleCubeBinder : MonoBehaviour {
  [Range(0,31)]
  public int index;

  public void OnToggleChanged(bool isOn) {
    var sp = BasicSpawner.Instance;
    if (sp == null) { Debug.LogWarning("[BINDER] BasicSpawner.Instance NULL"); return; }

    if (isOn) {
      Debug.Log($"[BINDER] DOWN idx={index}");
      sp.HoldDown(index);
    } else {
      Debug.Log($"[BINDER] UP   idx={index}");
      sp.HoldUp(index);
    }
  }
}
