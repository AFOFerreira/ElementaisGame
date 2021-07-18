using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class GerenciadorColecao : MonoBehaviour
{
    [Header("Controladores")]
    public GameObject alvo;
    public GameObject _mgs;
    public GameObject _viewport;

    [Header("Para Testes")]
    public Sprite foto1;
    public Sprite foto2;
    public Sprite img1;
    public Sprite img2;
    public GeralColecao _geral;

    GameObject PrefabCarta;
    GerenciadorBancoLocal _banco = new GerenciadorBancoLocal("databaseV2.db");
    GameObjectExtensions.ChildrenEnumerable quantidadeComponentes;

    private void Awake()
    {

        Magica m1 = new Magica()
        {
            Nome = "Caracasana",
            Descricao = "Carta Auxiliar Caracasana",
            Foto = foto1,
            Moldura = img1,
            Id = 5,
            TipoCarta = TipoCarta.AuxArm,
            TipoEfeito = TipoEfeito.CONTINUO,
            TipoMagica = TipoMagica.AUXILIAR,
            qtdTurno = 0
        };

        Magica m2 = new Magica()
        {
            Nome = "Bloqueio",
            Descricao = "Carta Armadilha Bloqueio",
            Foto = foto2,
            Moldura = img2,
            Id = 5,
            TipoCarta = TipoCarta.AuxArm,
            TipoEfeito = TipoEfeito.UNICO,
            TipoMagica = TipoMagica.ARMAILHA,
            qtdTurno = 4
        };
        _banco.InserirCarta(m1);
        _banco.InserirCarta(m2);
        _banco.CartasMonstro();
        _geral = new GeralColecao(_banco.ConsultarCartasMagicas(), _banco.ConsultarCartasMonstro());

    }
    private void Start()
    {
        PrefabCarta = Main.Instance.PrefabCarta;
        SetDefaults();
    }

    private void SetDefaults()
    {
        SetMsg(false);
        MostrarTodas();
    }

    private void Update()
    {
        quantidadeComponentes = alvo.Children();
        if (quantidadeComponentes.Count() > 0)
        {
            Debug.Log("quantidade de cartas: " + quantidadeComponentes.Count());
        }

    }

    void DestuirItens()
    {
        if (quantidadeComponentes.Count() > 0)
        {
            quantidadeComponentes.ForEach((item) =>
            {
                Destroy(item);
            });
        }
        else
            return;
    }

    public void MostrarTodas()
    {
        DestuirItens();
        SelecionarMonstros();
        SelecionarArmadilhas();
        SelecionarAuxiliares();
    }

    public void MostrarMonstros()
    {
        DestuirItens();
        SelecionarMonstros();
    }
    void SelecionarMonstros()
    {
        //Debug.Log("Cartas Monstros: " + _geral.ConsultarMonstros().Count);
        _geral.ConsultarMonstros().ForEach((item) =>
        {
            if (item != null)
            {
                SetMsg(false);
                (Instantiate(PrefabCarta, alvo.transform)).GetComponent<MonsterCard>().SetInformations(item);
            }
            else
                SetMsg(true);
        });
    }

    public void SelecionarMonstrosFogo()
    {
        DestuirItens();
        //Debug.Log("Cartas Monstros: " + _geral.ConsultarMonstros().Count);
        _geral.ConsultarMonstroFogo().ForEach((item) =>
        {
            if (item != null)
            {
                SetMsg(false);
                (Instantiate(PrefabCarta, alvo.transform)).GetComponent<MonsterCard>().SetInformations(item);
            }
            else
            {
                SetMsg(true);
            }
        });
    }

    public void SelecionarMonstrosAgua()
    {
        DestuirItens();
        //Debug.Log("Cartas Monstros: " + _geral.ConsultarMonstros().Count);
        _geral.ConsultarMonstroAgua().ForEach((item) =>
        {
            if (item != null)
            {
                SetMsg(false);
                (Instantiate(PrefabCarta, alvo.transform)).GetComponent<MonsterCard>().SetInformations(item);
            }
            else
            {
                SetMsg(true);
            }
        });
    }

    public void SomenteAuxiliares() 
    {
        DestuirItens();
        SelecionarAuxiliares();
    } 
    public void SomenteArmadilhas() 
    {
        DestuirItens();
        SelecionarArmadilhas();
    }

    public void SelecionarMonstrosTerra()
    {
        DestuirItens();
        //Debug.Log("Cartas Monstros: " + _geral.ConsultarMonstros().Count);
        _geral.ConsultarMonstroTerra().ForEach((item) =>
        {
            if (item != null)
            {
                SetMsg(false);
                (Instantiate(PrefabCarta, alvo.transform)).GetComponent<MonsterCard>().SetInformations(item);
            }
            else
            {
                SetMsg(true);
            }
        });
    }

    public void SelecionarMonstrosAr()
    {
        DestuirItens();
        //Debug.Log("Cartas Monstros: " + _geral.ConsultarMonstros().Count);
        _geral.ConsultarMonstroAr().ForEach((item) =>
        {
            if (item != null)
            {
                SetMsg(false);
                (Instantiate(PrefabCarta, alvo.transform)).GetComponent<MonsterCard>().SetInformations(item);
            }
            else
            {
                SetMsg(true);
            }
        });
    }

    public void SelecionarAuxiliares()
    {
        //DestuirItens();
        //Debug.Log("Cartas Magicas: " + _geral.ConsultarMagicas().Count);
        _geral.ConsultarAuxiliares().ForEach((item) =>
        {
            if (item != null)
            {
                SetMsg(false);
                item.Foto = img1;
                item.Moldura = foto1;

                (Instantiate(PrefabCarta, alvo.transform)).GetComponent<MonsterCard>().SetInformations(item);
            }
            else
            {
                SetMsg(true);
            }
        });
    }
    public void SelecionarArmadilhas()
    {
        //DestuirItens();
        //Debug.Log("Cartas Magicas: " + _geral.ConsultarMagicas().Count);
        _geral.ConsultarArmadilhas().ForEach((item) =>
        {
            if (item != null)
            {
                SetMsg(false);
                item.Foto = img2;
                item.Moldura = foto2;

                (Instantiate(PrefabCarta, alvo.transform)).GetComponent<MonsterCard>().SetInformations(item);
            }
            else
            {
                SetMsg(true);
            }
        });
    }

    public void SelecionarAuxiliarContinuas()
    {
        DestuirItens();
        // Debug.Log("Cartas Magicas: " + _geral.ConsultarMagicas().Count);
        _geral.ConsultarAuxiliarContinua().ForEach((item) =>
        {
            if (item != null)
            {
                SetMsg(false);
                item.Foto = img1;
                item.Moldura = foto1;
                (Instantiate(PrefabCarta, alvo.transform)).GetComponent<MonsterCard>().SetInformations(item);
            }
            else
            {
                SetMsg(true);
            }
        });
    }

    public void SelecionarAuxiliarUnicas()
    {
        DestuirItens();
        //Debug.Log("Cartas Magicas: " + _geral.ConsultarMagicas().Count);
        _geral.ConsultarAuxiliarUnica().ForEach((item) =>
        {
            if (item != null)
            {
                SetMsg(false);
                item.Foto = img1;
                item.Moldura = foto1;
                (Instantiate(PrefabCarta, alvo.transform)).GetComponent<MonsterCard>().SetInformations(item);
            }
            else
            {
                SetMsg(true);
            }
        });
    }
    public void SelecionarArmadilhaUnicas()
    {
        DestuirItens();
        //Debug.Log("Cartas Magicas: " + _geral.ConsultarMagicas().Count);
        _geral.ConsultarArmadilhasUnica().ForEach((item) =>
        {
            if (item != null)
            {
                SetMsg(false);
                item.Foto = img2;
                item.Moldura = foto2;
                (Instantiate(PrefabCarta, alvo.transform)).GetComponent<MonsterCard>().SetInformations(item);
            }
            else
            {
                SetMsg(true);
            }
        });
    }
    public void SelecionarArmadilhaContinuas()
    {
        DestuirItens();
        //Debug.Log("Cartas Magicas: " + _geral.ConsultarMagicas().Count);
        _geral.ConsultarArmadilhasContinua().ForEach((item) =>
        {
            if (_geral.ConsultarArmadilhasContinua().Count> 0)
            {
                SetMsg(false);
                item.Foto = img2;
                item.Moldura = foto2;
                (Instantiate(PrefabCarta, alvo.transform)).GetComponent<MonsterCard>().SetInformations(item);
            }
            else
            {
                Debug.Log("nao ha cartas");

                SetMsg(true);
            }
        });
    }

    void SetMsg(bool b)
    {
        if(b == true)
        {
            _mgs.SetActive(true);
            _viewport.SetActive(false);
        }
        else
        {
            _mgs.SetActive(false);
            _viewport.SetActive(true);
        }
    }
}