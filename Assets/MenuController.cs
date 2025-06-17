using UnityEngine;

public class XRMenuController : MonoBehaviour
{
    [SerializeField] private GameObject menuCanvas;

    private void Start()
    {
        if (menuCanvas != null)
        {
            menuCanvas.SetActive(false); // Nasconde il menu all'avvio
        }
        else
        {
            Debug.LogError("XRMenuController: menuCanvas non assegnato!");
        }
    }

    private void Update()
    {
        // Pulsante A del controller destro (Meta Quest)
        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
        {
            ToggleMenu();
        }
    }

    private void ToggleMenu()
    {
        if (menuCanvas == null) return;

        bool isActive = menuCanvas.activeSelf;
        menuCanvas.SetActive(!isActive);

        if (!isActive)
        {
            AnchorMenuToHead();
        }

        Debug.Log("Menu attivo: " + !isActive + " | Chiamato da: " + this.name + " | Stack: " + System.Environment.StackTrace);
    }

    private void AnchorMenuToHead()
    {
        if (OVRManager.instance != null && OVRManager.instance.GetComponent<OVRCameraRig>() != null)
        {
            Transform head = OVRManager.instance.GetComponent<OVRCameraRig>().centerEyeAnchor;
            if (head != null)
            {
                // Posiziona il menu davanti alla testa
                Vector3 forwardOffset = head.forward * 0.6f; 
                Vector3 verticalOffset = Vector3.down * 0.4f; // Leggermente più in basso
                menuCanvas.transform.position = head.position + forwardOffset + verticalOffset;

                // Fa guardare il menu verso l’utente
                menuCanvas.transform.LookAt(head.position);
                menuCanvas.transform.forward *= -1;
            }
        }
    }
}
