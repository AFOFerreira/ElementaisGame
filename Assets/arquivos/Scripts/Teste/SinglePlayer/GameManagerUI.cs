using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManagerUI : MonoBehaviour
{
    public TextMeshProUGUI turnoAtual;
    public TextMeshProUGUI faseAtual;
    public static GameManagerUI _instance;
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (GameManagerIa.instance.EmJogo)
        {
            if(GameManagerIa.instance.turno == TipoJogador.PLAYER)
            {
                turnoAtual.text = "Turno atual: PLAYER";
            }
            else
            {
                turnoAtual.text = "Turno atual: IA";
            }

       
            faseAtual.text = "Fase atual: " + GameManagerIa.instance.faseAtual.ToString();
           // Debug.Log("Em Jogo -GameManagerUI-");
        }
    }
}
