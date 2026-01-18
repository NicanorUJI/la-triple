using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.InputSystem;

public class CambioDeEscenaPorIndice : MonoBehaviour
{
    public int indiceEscena;
    public Animator transicion;

    public string tagJugador = "Player";

    [Header("Requisito de input (opcional)")]
    [Tooltip("Si es None, el TP funciona como siempre (al entrar). Si asignas una tecla, el TP se activa al pulsarla mientras el jugador está dentro del trigger.")]
    public Key teclaRequerida = Key.None;

    public float retardo = 0f;
    public string nombrePuntoEntrada;

    private bool jugadorDentro = false;
    private bool teleporteEnCurso = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(tagJugador))
            return;

        jugadorDentro = true;

        if (teclaRequerida == Key.None)
            IntentarTeletransportar();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag(tagJugador))
            return;

        jugadorDentro = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (teclaRequerida == Key.None)
            return;

        if (!collision.CompareTag(tagJugador))
            return;

        if (!jugadorDentro || teleporteEnCurso)
            return;

        if (CumpleInputRequerido())
            IntentarTeletransportar();
    }

    private bool CumpleInputRequerido()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null)
            return false;

        return keyboard[teclaRequerida].isPressed;
    }

    private void IntentarTeletransportar()
    {
        if (teleporteEnCurso)
            return;

        teleporteEnCurso = true;

        if (GameManager.Instance != null)
            GameManager.Instance.lastExitName = nombrePuntoEntrada;

        StartCoroutine(CambiarEscena());
    }

    IEnumerator CambiarEscena()
    {
        if (transicion != null)
            transicion.SetTrigger("Start");

        yield return new WaitForSeconds(1 + retardo);

        SceneManager.LoadScene(indiceEscena);
    }
}
