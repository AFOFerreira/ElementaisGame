using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;

public class zonaColecao : MonoBehaviour
{
    [Header("Teste")]
    public MonsterCard[] CartasTeste;
    public int quantidadeCopias;

    //Tamanho da colecao;
    [SerializeField] List<MonsterCard> ColecaoCartas;
    public int TamanhoDeck;
    public int espacos;

    public GameObject content;

    private void Awake()
    {
        ColecaoCartas = new List<MonsterCard>();

        espacos = TamanhoDeck;
    }

    private void Start()
    {

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {

            var c = Random.Range(0, CartasTeste.Length);
            AddCartaTeste(CartasTeste[c].GetComponent<MonsterCard>());
        }

        //InstanciarCartas();
    }

    void InstanciarCartas()
    {
        for (int x = 0; x < ColecaoCartas.Count; x++)
        {
            if (ColecaoCartas[x] != null)
            {
                Instantiate(ColecaoCartas[x], content.transform);
            }
            else
            {
                return;
            }

        }
    }
    void AddCartaTeste(MonsterCard carta)
    {

    }


}
