using UnityEngine;

public class SolteronaGeneral : MonoBehaviour
{
    [Header("Cartas rival")]
    public carta carta3;
    public carta carta4;

    [Header("Cartas jugador")]
    public carta carta1;
    public carta carta2;

    public bool turnoJuaquin;
    private carta_controller minijuegoController;






    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        minijuegoController = FindObjectOfType<carta_controller>();
        minijuegoController.carta2Objeto.SetActive(false);
        minijuegoController.animator.gameObject.SetActive(false);
        carta1.tipo = 1; //esta carta es un as 
        carta2.tipo = 0; //esta carta esta vacia
        carta3.tipo = 2; //esta carta es un joker
        carta4.tipo = 1; //esta carta es un as
        minijuegoController.BarajarCartasAlcalde();


        turnoJuaquin = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
