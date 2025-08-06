using UnityEngine;
using Oculus.Platform;
using Oculus.Platform.Models;

public class MetaInitializer : MonoBehaviour
{
    void Start()
    {
        Core.Initialize();
        Entitlements.IsUserEntitledToApplication().OnComplete(callback =>
        {
            if (callback.IsError)
            {
                Debug.LogError("Utente non autorizzato: " + callback.GetError().Message);
            }
            else
            {
                Debug.Log("Utente autorizzato all'app!");
            }
        });
    }
}
