using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

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

    private int currentRound = 1;
    private List<GameObject> availableChildren; // ni�os que a�n no han sido encontrados

    public GameObject panelReglas;

    public bool inputEnabled = false;

    public void Aceptar() 
    {
        inputEnabled = true;
        if (panelReglas != null)
            panelReglas.SetActive(false);
        IniciarPartida();
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

        StartCoroutine(ReturnToSchoolAfterDelay(1.5f));
    }

    public void ResetGame()
    {
        SceneManager.LoadScene("PiRecuentoMinijuego");
    }


    void Start()
    {
        // Inicializar lista de ni�os disponibles
        availableChildren = new List<GameObject>(childrenSprites);

        // Ocultamos mensaje al iniciar
        mensajeText.gameObject.SetActive(false);
    }

    public void IniciarPartida() 
    {
        currentRound = 1;
        availableChildren = new List<GameObject>(childrenSprites);
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
        if (availableChildren.Count == 0)
        {
            FinishGame();
            return;
        }
        else if (currentRound > rounds) 
        {
            ResetGame();
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
        SceneManager.LoadScene("colegio");  // usa el nombre real de la escena
    }
}
