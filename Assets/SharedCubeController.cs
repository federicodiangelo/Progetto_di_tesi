using Fusion;
using UnityEngine;

public class SharedCubeController : NetworkBehaviour {
  [SerializeField] private bool enableDebug = true;

  [SerializeField] private Renderer[] renderers;
  [SerializeField] private Collider[] colliders;
  [SerializeField] private Rigidbody rb;
  [SerializeField] private MonoBehaviour[] xrInteractables;

  [Networked] private bool IsActive { get; set; }
  [Networked] private PlayerRef Owner { get; set; }

  void Awake() {
    if (renderers == null || renderers.Length == 0) renderers = GetComponentsInChildren<Renderer>(true);
    if (colliders == null || colliders.Length == 0) colliders = GetComponentsInChildren<Collider>(true);
    if (!rb) rb = GetComponent<Rigidbody>();
  }

  public override void Spawned() {
    Apply(IsActive);
    if (enableDebug) Debug.Log($"[CUBE] Spawned active={IsActive} owner={Owner}");
  }

  public void HostAssignOwner(PlayerRef owner) {
    if (!HasStateAuthority) return;
    Owner = owner;
    if (enableDebug) Debug.Log($"[CUBE] Owner set -> {Owner}");
  }

  public void HostSetBy(PlayerRef who, bool active) {
    if (!HasStateAuthority) return;
    if (who != Owner) return;

    if (IsActive != active && enableDebug) {
      Debug.Log($"[CUBE] Stato cambiato -> {(active ? "ON" : "OFF")} da {who}");
    }

    IsActive = active;
    Apply(IsActive);
  }

  public void HostPlaceAt(Vector3 pos, Quaternion? rot) {
    if (!HasStateAuthority) return;
    transform.position = pos;
    if (rot.HasValue) transform.rotation = rot.Value;
    if (enableDebug) Debug.Log($"[CUBE] Placed at {pos}");
  }

  private void Apply(bool active) {
    if (renderers != null) foreach (var r in renderers) if (r) r.enabled = active;
    if (colliders != null) foreach (var c in colliders) if (c) c.enabled = active;
    if (rb) rb.isKinematic = !active;
    if (xrInteractables != null) foreach (var x in xrInteractables) if (x) x.enabled = active;
  }
}
