using UnityEngine;

public class ToggleOggetto : MonoBehaviour
{
    public GameObject oggettoDaAttivare;

    public void AttivaDisattiva()
    {
        if (oggettoDaAttivare != null)
        {
            oggettoDaAttivare.SetActive(!oggettoDaAttivare.activeSelf);
        }
        else
        {
            Debug.LogWarning("Nessun oggetto assegnato al bottone!");
        }
    }
}
