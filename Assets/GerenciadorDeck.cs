using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class GerenciadorDeck : MonoBehaviour
{
    public GameObject mensagem;
    public GameObject ViewPort;

    ColecaoDeck d1 = new ColecaoDeck
    {
        IdCarta = 1,
        Quantidade = 3,
        TituloDeck = "A"
    }; ColecaoDeck d2 = new ColecaoDeck
    {
        IdCarta = 2,
        Quantidade = 2,
        TituloDeck = "A"
    }; ColecaoDeck d3 = new ColecaoDeck
    {
        IdCarta = 3,
        Quantidade = 1,
        TituloDeck = "B"
    }; ColecaoDeck d4 = new ColecaoDeck
    {
        IdCarta = 2,
        Quantidade = 3,
        TituloDeck = "A"
    }; ColecaoDeck d5 = new ColecaoDeck
    {
        IdCarta = 1,
        Quantidade = 3,
        TituloDeck = "C"
    }; ColecaoDeck d6 = new ColecaoDeck
    {
        IdCarta = 2,
        Quantidade = 3,
        TituloDeck = "A"
    };


    // Start is called before the first frame update
    void Start()
    {
        Main.Instance.Banco.InserirCartaDeck(d1);
        Main.Instance.Banco.InserirCartaDeck(d2);
        Main.Instance.Banco.InserirCartaDeck(d3);
        Main.Instance.Banco.InserirCartaDeck(d4);
        Main.Instance.Banco.InserirCartaDeck(d5);
        Main.Instance.Banco.InserirCartaDeck(d6);
        var deckTemp = Main.Instance.Banco.ConsultarDeck();
        if (deckTemp.Count() == 0)
        {
            mensagem.SetActive(true);
            ViewPort.SetActive(false);
        }
        else
        {
            mensagem.SetActive(true);
            ViewPort.SetActive(true);

            mensagem.GetComponent<TextMeshProUGUI>().text = "Você possui " + deckTemp.Count() + " cartas em seu deck";
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
