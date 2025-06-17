using UnityEngine;

public class XRMenuControllerProva : MonoBehaviour
{
    [SerializeField] private GameObject menuCanvas;

    private OVRHand leftHand;
    private bool wasPinchingLastFrame = false;

    private void Start()
    {
        if (menuCanvas != null)
        {
            menuCanvas.SetActive(false);
        }
        else
        {
            Debug.LogError("XRMenuControllerProva: menuCanvas non assegnato!");
        }

        // Trova solo la mano sinistra in modo sicuro
        OVRHand[] hands = Object.FindObjectsByType<OVRHand>(FindObjectsSortMode.None);
        foreach (var hand in hands)
        {
            string name = hand.gameObject.name.ToLower();
            if (name.Contains("left") && !name.Contains("right"))
            {
                leftHand = hand;
                Debug.Log("✅ Mano sinistra assegnata: " + hand.gameObject.name);
                break;
            }
        }

        if (leftHand == null)
        {
            Debug.LogError("❌ Mano sinistra non trovata!");
        }
    }

    private void Update()
    {
        // Pulsante A del controller destro
        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
        {
            ToggleMenu("Controller A");
        }

        // Solo pinch della mano sinistra
        if (leftHand != null)
        {
            string name = leftHand.gameObject.name.ToLower();
            if (name.Contains("left") && !name.Contains("right"))
            {
                bool isPinching = leftHand.GetFingerIsPinching(OVRHand.HandFinger.Index);

                if (isPinching && !wasPinchingLastFrame)
                {
                    ToggleMenu("Pinch mano sinistra");
                }

                wasPinchingLastFrame = isPinching;
            }
        }
    }

    private void ToggleMenu(string source)
    {
        if (menuCanvas == null) return;

        bool isActive = menuCanvas.activeSelf;
        menuCanvas.SetActive(!isActive);

        if (!isActive)
        {
            AnchorMenuToHead();
        }

        Debug.Log($"📋 Menu attivo: {!isActive} | Chiamato da: {source}");
    }

    private void AnchorMenuToHead()
    {
        if (OVRManager.instance != null && OVRManager.instance.GetComponent<OVRCameraRig>() != null)
        {
            Transform head = OVRManager.instance.GetComponent<OVRCameraRig>().centerEyeAnchor;
            if (head != null)
            {
                Vector3 forwardOffset = head.forward * 0.8f;
                Vector3 verticalOffset = Vector3.down * 0.6f;
                menuCanvas.transform.position = head.position + forwardOffset + verticalOffset;

                menuCanvas.transform.LookAt(head.position);
                menuCanvas.transform.forward *= -1;
            }
        }
    }
}