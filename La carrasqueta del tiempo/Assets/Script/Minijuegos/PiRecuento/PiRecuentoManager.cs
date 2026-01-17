using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PiRecuentoManager : MonoBehaviour
{
    [Header("Niños y Spots")]
    public List<GameObject> childrenSprites; // 3 ni�os �nicos
    public List<HidingSpot> spots;           // 5 spots

    [Header("Rondas")]
    public int rounds = 5;

    [Header("UI")]
    public TextMeshProUGUI rondaText;
    public TextMeshProUGUI mensajeText;

    [Header("Paneles")]
    public GameObject panelReglas;
    public GameObject panelFinPartida; // Panel de fin de juego
    public Button botonContinuar;  // Botón Continuar

    [Header("Fin de partida - Imagen")]
    public Image imagenResultado;
    public TextMeshProUGUI textoResultado;
    public Sprite spriteVictoria;
    public Sprite spriteDerrota;

    private int currentRound = 1;
    private List<GameObject> availableChildren; // ni�os que a�n no han sido encontrados

    public bool inputEnabled = false;
    private bool haGanado = false;


    void Start()
    {
        // Inicializar lista de ni�os disponibles
        availableChildren = new List<GameObject>(childrenSprites);

        // Ocultamos mensaje al iniciar
        mensajeText.gameObject.SetActive(false);
        rondaText.gameObject.SetActive(false);

        if (panelFinPartida != null)
            panelFinPartida.SetActive(false);
    }




    public void Aceptar() 
    {
        inputEnabled = true;
        if (panelReglas != null)
            panelReglas.SetActive(false);
        IniciarPartida();
    }

    public void IniciarPartida()
    {
        currentRound = 1;
        availableChildren = new List<GameObject>(childrenSprites);
        rondaText.gameObject.SetActive(true);
        StartRound();
    }

    void StartRound()
    {
        // Reiniciamos todos los spots
        foreach (var s in spots)
        {
            s.ResetSpot();
            s.childSprite = null;
        }

        // Fin del juego
        if (availableChildren.Count == 0 || currentRound > rounds)
        {
            haGanado = availableChildren.Count == 0;
            MostrarPanelFin();
            return;
        }

        // Mezclar spots
        List<HidingSpot> shuffledSpots = new List<HidingSpot>(spots);
        for (int i = 0; i < shuffledSpots.Count; i++)
        {
            int rand = Random.Range(i, shuffledSpots.Count);
            var temp = shuffledSpots[i];
            shuffledSpots[i] = shuffledSpots[rand];
            shuffledSpots[rand] = temp;
        }

        // Mezclar ni�os disponibles
        List<GameObject> shuffledChildren = new List<GameObject>(availableChildren);
        for (int i = 0; i < shuffledChildren.Count; i++)
        {
            int rand = Random.Range(i, shuffledChildren.Count);
            var temp = shuffledChildren[i];
            shuffledChildren[i] = shuffledChildren[rand];
            shuffledChildren[rand] = temp;
        }

        // Asignar cada ni�o disponible a un spot diferente cada ronda
        for (int i = 0; i < shuffledChildren.Count; i++)
        {
            HidingSpot spot = shuffledSpots[i];
            GameObject child = shuffledChildren[i];

            spot.hasChild = true;
            spot.childSprite = child;

            Niño n = child.GetComponent<Niño>();

            if (n != null)
                n.Ocultar(); // restauramos escala y sortingOrder

            // Ahora s� ponemos al ni�o en el spot
            child.transform.SetParent(spot.transform);
            child.transform.localPosition = Vector3.zero;
        }

        rondaText.text = $"Ronda: {currentRound}/{rounds}";
    }

    public void OnSpotClicked(HidingSpot spot)
    {
        if (!spot.hasChild)
        {
            StartCoroutine(MostrarMensaje());
        }
        else
        {
            Niño n = spot.childSprite.GetComponent<Niño>();
            if (n != null)
            {
                n.Mostrar(); // mostrar en primer plano
            }

            StartCoroutine(MostrarMensaje(n.nombre));

            // Quitar ni�o de la lista de disponibles
            availableChildren.Remove(spot.childSprite);

            // Esperar un momento antes de moverlo fuera
            StartCoroutine(RemoverNiñoDelay(spot.childSprite));
        }

        currentRound++;
        StartCoroutine(NextRoundDelay());
    }



    //PANEL FIN
    private void MostrarPanelFin()
    {
        inputEnabled = false;

        if (panelFinPartida != null)
            panelFinPartida.SetActive(true);

        if (haGanado)
        {
            imagenResultado.sprite = spriteVictoria;
            textoResultado.text = "Has trobat a tots el xiquets!";
        }
        else
        {
            imagenResultado.sprite = spriteDerrota;
            textoResultado.text = "Els xiquets han guanyat... :(";
        }

        if (botonContinuar != null)
            botonContinuar.interactable = haGanado;
        rondaText.gameObject.SetActive(false);
    }
    public void ReintentarJuego() 
    {
        panelFinPartida.SetActive(false);
        ResetGame();
    }
    public void ResetGame()
    {
        SceneManager.LoadScene("PiRecuentoMinijuego");
    }
    public void ContinuarJuego() 
    {
        if (!haGanado)
            return;
        panelFinPartida.SetActive(false);
        FinishGame();
    }
    public void FinishGame()
    {
        GameManager.Change("Act2_Q_ESQUELLES_HasBracelet");
        Debug.Log("[Esquelles/PiRecuento] Joaquín ha aconseguit la polsera.");

        var mc = MissionController.Instance ?? FindObjectOfType<MissionController>();
        if (mc != null)
        {
            mc.SetActiveMission(
                4,
                "Portar la polsera a Raúl",
                "Has aconseguit una polsera al col·legi. Torna al passat i dóna-li-la a Raúl a les calderetes."
            );
        }

        StartCoroutine(ReturnToSchoolAfterDelay(0.5f));
    }








    private IEnumerator RemoverNiñoDelay(GameObject child)
    {
        float showTime = 3f;          // tiempo que el niño se muestra
        float swayAngle = 15f;        // ángulo máximo de balanceo
        float swaySpeed = 2f;         // velocidad del balanceo

        Niño n = child.GetComponent<Niño>();

        child.transform.localScale = n.originalScale * 2;

        float elapsedTime = 0f;
        while (elapsedTime <= showTime)
        {
            if (child != null)
            {
                // Balanceo lateral
                float angle = Mathf.Sin(Time.time * swaySpeed) * swayAngle;
                child.transform.rotation = Quaternion.Euler(0, 0, angle);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Restaurar valores y remover
        if (child != null)
        {
            child.transform.rotation = Quaternion.identity;
            child.transform.position = new Vector3(9999, 9999, 0);
            child.transform.SetParent(null);
        }
    }

    private IEnumerator NextRoundDelay()
    {
        inputEnabled = false;

        yield return new WaitForSeconds(3f);

        inputEnabled = true;
        StartRound();
    }

    private IEnumerator MostrarMensaje()
    {
        mensajeText.gameObject.SetActive(true);
        mensajeText.text = "Ací no hi ha ningú...";
        yield return new WaitForSeconds(3f);
        mensajeText.gameObject.SetActive(false);
    }

    private IEnumerator MostrarMensaje(string nombre)
    {
        mensajeText.gameObject.SetActive(true);
        mensajeText.text = $"Has trobat a {nombre}!"; ;
        yield return new WaitForSeconds(3f);
        mensajeText.gameObject.SetActive(false);
    }

    private IEnumerator ReturnToSchoolAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // 🔹 Guardamos el spawn donde queremos que aparezca el Player
        if (GameManager.Instance != null)
        {
            GameManager.Instance.lastExitName = "EscuM"; // nombre del Empty en la escena colegio
        }

        // Cambiamos de escena
        SceneManager.LoadScene("colegio"); // nombre real de la escena
    }
}
