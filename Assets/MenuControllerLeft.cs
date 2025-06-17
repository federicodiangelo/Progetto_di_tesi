using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class XRMenuControllerLeft : MonoBehaviour
{
    [SerializeField] private GameObject menuCanvas;

    private List<Toggle> menuToggles = new List<Toggle>();
    private Dictionary<Toggle, bool> savedToggleStates = new Dictionary<Toggle, bool>();

    private void Start()
    {
        if (menuCanvas != null)
        {
            menuCanvas.SetActive(false); // Nasconde il menu all'avvio
            // Trova tutti i toggle nel canvas
            menuToggles.AddRange(menuCanvas.GetComponentsInChildren<Toggle>(true));
            // Inizializza i loro stati (di default o tutti falsi)
            foreach (Toggle toggle in menuToggles)
            {
                savedToggleStates[toggle] = toggle.isOn;
            }
        }
        else
        {
            Debug.LogError("XRMenuControllerLeft: menuCanvas non assegnato!");
        }
    }

    private void Update()
    {
        // Pulsante X sul controller sinistro
        if (OVRInput.GetDown(OVRInput.RawButton.X))
        {
            ToggleMenu();
        }
    }

    private void ToggleMenu()
    {
        if (menuCanvas == null) return;

        bool isActive = menuCanvas.activeSelf;

        if (isActive)
        {
            // 👉 Salva gli stati dei toggle PRIMA di chiudere
            SaveToggleStates();
        }
        else
        {
            // 👉 Ripristina gli stati dei toggle PRIMA di mostrare
            RestoreToggleStates();
        }

        menuCanvas.SetActive(!isActive);

        if (!isActive)
        {
            AnchorMenuToHead();
        }

        Debug.Log("📋 Menu attivo: " + !isActive + " | Chiamato da: " + this.name);
    }

    private void SaveToggleStates()
    {
        foreach (Toggle toggle in menuToggles)
        {
            savedToggleStates[toggle] = toggle.isOn;
        }
    }

    private void RestoreToggleStates()
    {
        foreach (Toggle toggle in menuToggles)
        {
            if (savedToggleStates.ContainsKey(toggle))
            {
                toggle.isOn = savedToggleStates[toggle];
            }
        }
    }

    private void AnchorMenuToHead()
    {
        if (OVRManager.instance != null)
        {
            OVRCameraRig rig = OVRManager.instance.GetComponent<OVRCameraRig>();
            if (rig != null && rig.centerEyeAnchor != null)
            {
                Transform head = rig.centerEyeAnchor;
                Vector3 forwardOffset = head.forward * 1f;
                Vector3 verticalOffset = Vector3.down * 0.8f;
                menuCanvas.transform.position = head.position + forwardOffset + verticalOffset;

                menuCanvas.transform.LookAt(head.position);
                menuCanvas.transform.forward *= -1;
            }
        }
    }
}
