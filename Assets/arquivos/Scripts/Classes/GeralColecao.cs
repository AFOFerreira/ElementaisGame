using Unity;
using UnityEngine;
using SQLite4Unity3d;
using System.Collections.Generic;
using System.Linq;


[System.Serializable]
public class GeralColecao
{

    List<Magica> Magica;
    List<Monstro> Monstro;

    public GeralColecao(List<Magica> magica, List<Monstro> monstro)
    {
        this.Magica = magica;
        this.Monstro = monstro;
    }

    public GeralColecao()
    {
    }

    public List<Monstro> ConsultarMonstros()
    {
        return Monstro;
    }

    public List<Magica> ConsultarMagicas()
    {
        return Magica;
    }

    public List<Monstro> ConsultarMonstroFogo()
    {
        return Monstro.Where(x => x.TipoElemental == TipoElemental.FOGO).ToList();
    }
    public List<Monstro> ConsultarMonstroAgua()
    {
        return Monstro.Where(x => x.TipoElemental == TipoElemental.AGUA).ToList(); ;
    }
    public List<Monstro> ConsultarMonstroTerra()
    {
        return Monstro.Where(x => x.TipoElemental == TipoElemental.TERRA).ToList();
    }
    public List<Monstro> ConsultarMonstroAr()
    {
        return Monstro.Where(x => x.TipoElemental == TipoElemental.AR).ToList();
    }

    public List<Magica> ConsultarAuxiliares()
    {
        return Magica.Where(x => x.TipoMagica == TipoMagica.AUXILIAR).ToList();
    }
    public List<Magica> ConsultarArmadilhas()
    {
        return Magica.Where(x => x.TipoMagica == TipoMagica.ARMAILHA).ToList();
    }
    public List<Magica> ConsultarArmadilhasContinua()
    {
        return Magica.Where(x => x.TipoMagica == TipoMagica.ARMAILHA && x.TipoEfeito == TipoEfeito.CONTINUO).ToList();
    }
    public List<Magica> ConsultarArmadilhasUnica()
    {
        return Magica.Where(x => x.TipoMagica == TipoMagica.ARMAILHA && x.TipoEfeito == TipoEfeito.UNICO).ToList();
    }
    public List<Magica> ConsultarAuxiliarContinua()
    {
        return Magica.Where(x => x.TipoMagica == TipoMagica.AUXILIAR && x.TipoEfeito == TipoEfeito.CONTINUO).ToList();
    }
    public List<Magica> ConsultarAuxiliarUnica()
    {
        return Magica.Where(x => x.TipoMagica == TipoMagica.AUXILIAR && x.TipoEfeito == TipoEfeito.UNICO).ToList();
    }
}

