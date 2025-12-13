using UnityEngine;

public class HideIfFlagActive : MonoBehaviour
{
    [Tooltip("Nombre del flag de GameManager que, si está activo, ocultará este objeto.")]
    public string flagName;

    private void Start()
    {
        if (!string.IsNullOrEmpty(flagName) && GameManager.Check(flagName))
        {
            Debug.Log($"[HideIfFlagActive] Ocultant {name} perquè el flag '{flagName}' està actiu.");
            gameObject.SetActive(false);
        }
    }
}
