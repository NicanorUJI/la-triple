using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    void Start()
    {
        // Verifica si hay un punto guardado
        string entryPoint = GameManager.Instance.lastExitName;
        if (!string.IsNullOrEmpty(entryPoint))
        {
            // Busca el objeto con ese nombre en la escena
            GameObject spawnPoint = GameObject.Find(entryPoint);
            if (spawnPoint != null)
            {
                // Mueve al player a esa posición
                transform.position = spawnPoint.transform.position;
            }
            else
            {
                Debug.LogWarning("No se encontró el punto de entrada: " + entryPoint);
            }
        }
    }
}
