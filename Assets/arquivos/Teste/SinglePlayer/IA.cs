using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IA : MonoBehaviour
{
    public bool possoJogar;

    public List<CampoSingle> camposParaJogarMonstro = new List<CampoSingle>();
    public List<CampoSingle> camposParaJogarMagicas = new List<CampoSingle>();
    public List<CampoSingle> campoOcupadosMagicas = new List<CampoSingle>();
    public List<CampoSingle> camposOcupadosMonstros = new List<CampoSingle>();
    public CartaSingle[] Mao;

    public CampoSingle campoSelecionado;

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
                        if (campoSelecionado == null)
                        {
                            for (int i = 0; i < camposParaJogarMonstro.Count; i++)
                            {
                                campoSelecionado = camposParaJogarMonstro[i];
                            }
                        }
                        else
                        {
                            Debug.Log(tipoIA + ": Posso jogar no campo: " + campoSelecionado.idCampo);
                            if (campoSelecionado.vazio)
                            {
                                Debug.Log("Esperando jogada!");
                                var cartaA = Mao[0];
                                if (Input.GetButtonDown("Jump"))
                                {
                                    gameManager.JogarCarta(campoSelecionado, cartaA.gameObject);
                                }
                            }
                            else
                            {
                                Debug.Log(tipoIA + ": Joguei no campo " + campoSelecionado.idCampo);
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
                    if (camposParaJogarMagicas.Count > 0)
                    {
                        for (int i = 0; i < camposParaJogarMagicas.Count; i++)
                        {
                            campoSelecionado = camposParaJogarMagicas[i];
                        }
                        Debug.Log(tipoIA + ": Posso jogar no campo: " + campoSelecionado.idCampo);
                        if (campoSelecionado.vazio)
                        {
                            Debug.Log("Esperando jogada!");
                            var cartaA = Mao[1];
                            if (Input.GetButtonDown("Jump"))
                            {
                                gameManager.JogarCarta(campoSelecionado, cartaA.gameObject);
                            }
                        }
                    }
                    else
                    {
                        Debug.Log(tipoIA + ": Não posso jogar, nao há campos disponiveis!");
                        gameManager.PassaTurno();
                    }
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
                campoSelecionado = null;
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
        camposParaJogarMagicas = gameManager.VerificaCampoDisponivelMagicas(tipoIA);
        campoOcupadosMagicas = gameManager.VerificaCampoOcupadoMagicas(tipoIA);
        camposParaJogarMonstro = gameManager.VerificaCampoDisponivelMonstros(tipoIA);
        camposOcupadosMonstros = gameManager.VerificaCampoOcupadoMonstros(tipoIA);
    }


}
