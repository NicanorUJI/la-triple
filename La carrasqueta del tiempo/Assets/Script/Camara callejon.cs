using UnityEngine;

[RequireComponent(typeof(Camera))]
public class AjustarCamaraAlFondo : MonoBehaviour
{
    public SpriteRenderer fondo; // arrastra aquí tu fondo desde el inspector

    void Start()
    {
        if (fondo == null)
        {
            Debug.LogWarning("No se asignó fondo al script AjustarCamaraAlFondo.");
            return;
        }

        Camera cam = GetComponent<Camera>();

        // Tamaño del sprite en unidades del mundo
        float alto = fondo.bounds.size.y;
        float ancho = fondo.bounds.size.x;

        // Relación de aspecto de la pantalla
        float aspect = (float)Screen.width / Screen.height;

        // Calculamos el tamaño ortográfico necesario
        float sizeNecesario = (ancho / aspect) / 2f;

        // La cámara toma el tamaño mayor para asegurar que se vea todo
        cam.orthographicSize = Mathf.Max(alto / 2f, sizeNecesario);
    }
}