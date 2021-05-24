using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IA : MonoBehaviour
{
    public bool possoJogar;

    [HideInInspector] public List<CampoSingle> camposParaJogarMonstro = new List<CampoSingle>();

    public CampoSingle campo;

    public TipoJogador tipoIA;
    public TipoFase fase;
    GameManagerIa gameManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManagerIa.instance;
        VerificaCampos();
        VerificaMeuTurno();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.EmJogo)
        {
            VerificaMeuTurno();

            if (possoJogar)
            {
                VerificaCampos();
                fase = gameManager.faseAtual;
                if (fase == TipoFase.MONSTRO)
                {
                    if (camposParaJogarMonstro.Count > 0)
                    {
                        if (campo == null)
                        {
                            for (int i = 0; i < camposParaJogarMonstro.Count; i++)
                            {
                                campo = camposParaJogarMonstro[i];
                            }
                        }
                        else
                        {
                            Debug.Log(tipoIA + ": Posso jogar no campo: " + campo.idCampo);
                            if (campo.vazio)
                            {
                                Debug.Log("Esperando jogada!");
                            }
                            else
                            {
                                Debug.Log(tipoIA + ": Joguei no campo " + campo.idCampo);
                                gameManager.faseAtual = TipoFase.MAGICA;
                            }         
                        }
                    }
                    else
                    {
                        Debug.Log(tipoIA + ": Não posso jogar, nao há campos disponiveis!");
                        //gameManager.PassaFase();
                        gameManager.faseAtual = TipoFase.MAGICA;

                    }
                }
                else if (fase == TipoFase.MAGICA)
                {
                    //TURNO PARA JOGAR CARTAS MAGICAS.
                    Debug.Log(tipoIA + ": Esperando para jogar cartas magicas");
                    gameManager.PassaTurno();

                }
                else
                {
                    //TURNO PARA DEFESA.
                    Debug.Log(tipoIA + ": Realizando defesa");

                }
            }
            else
            {
                Debug.Log(tipoIA + ": Não posso jogar, não é a minha vez!!");
                campo = null;
                //DO NOTHING!
            }
        }
    }

    void VerificaMeuTurno()
    {
        if (tipoIA == gameManager.turno)
            possoJogar = true;
        else
            possoJogar = false;
    }

    void VerificaCampos()
    {
        if (tipoIA == TipoJogador.IA)
        {
            camposParaJogarMonstro = gameManager.VerificaCampoDisponivelIA();
        }
        else
        {
            camposParaJogarMonstro = gameManager.VerificaCampoDisponivelPlayer();
        }
    }
}
