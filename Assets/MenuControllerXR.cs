using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// Gestisce apertura/chiusura del menu con tasto X (controller sinistro) e salva stati Toggle.
public class MenuControllerXR : MonoBehaviour {
  [SerializeField] private GameObject menuCanvas;

  private readonly List<Toggle> menuToggles = new List<Toggle>();
  private readonly Dictionary<Toggle, bool> savedToggleStates = new Dictionary<Toggle, bool>();

  private void Start() {
    if (menuCanvas != null) {
      menuCanvas.SetActive(false);
      RescanToggles();
      SaveToggleStates();
      Debug.Log("[MENU] Ready (hidden)");
    } else {
      Debug.LogError("XRMenuControllerLeft: menuCanvas non assegnato!");
    }
  }

  private void Update() {
    if (OVRInput.GetDown(OVRInput.RawButton.X)) {
      ToggleMenu();
    }
  }

  private void ToggleMenu() {
    if (menuCanvas == null) return;
    bool isActive = menuCanvas.activeSelf;

    if (isActive) {
      SaveToggleStates();
    } else {
      RescanToggles();
      RestoreToggleStates();
    }

    menuCanvas.SetActive(!isActive);
    if (!isActive) AnchorMenuToHead();

    Debug.Log($"[MENU] {(menuCanvas.activeSelf ? "OPEN" : "CLOSE")}");
  }

  private void SaveToggleStates() {
    RescanToggles();
    savedToggleStates.Clear();
    foreach (Toggle toggle in menuToggles) {
      if (!toggle) continue;
      savedToggleStates[toggle] = toggle.isOn;
    }
  }

  private void RestoreToggleStates() {
    RescanToggles();
    foreach (Toggle toggle in menuToggles) {
      if (!toggle) continue;
      if (savedToggleStates.TryGetValue(toggle, out bool on)) {
        toggle.SetIsOnWithoutNotify(on);
      }
    }
  }

  private void RescanToggles() {
    menuToggles.Clear();
    if (!menuCanvas) return;
    menuToggles.AddRange(menuCanvas.GetComponentsInChildren<Toggle>(true));
  }

  private void AnchorMenuToHead() {
    if (OVRManager.instance != null) {
      OVRCameraRig rig = OVRManager.instance.GetComponent<OVRCameraRig>();
      if (rig != null && rig.centerEyeAnchor != null) {
        Transform head = rig.centerEyeAnchor;
        Vector3 forwardOffset = head.forward * 0.6f;
        Vector3 verticalOffset = Vector3.down * 0.3f;
        menuCanvas.transform.position = head.position + forwardOffset + verticalOffset;
        menuCanvas.transform.LookAt(head.position);
        menuCanvas.transform.forward *= -1;
      }
    }
  }
}
