using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampoSingle : MonoBehaviour
{
    public GameObject containerCarta;

    public TipoJogador donoCampo;
    public TipoCarta tipoCartaCampo;
    public int idCampo;
    public bool vazio;

    // Start is called before the first frame update
    void Start()
    {
        vazio = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManagerIa.instance.EmJogo)
        {
            if (containerCarta.transform.childCount >0)
            {
                vazio = false;
            }
            else
            {
                vazio = true;
            }
        }
    }
}
