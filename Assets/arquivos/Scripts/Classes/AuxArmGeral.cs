using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AuxArmGeral
{
    //public TipoCarta tipoCarta;
    public Sprite moldura;
    public Sprite icone;
    public Sprite mask;
    public Sprite molduraCampo;

    public AuxArmGeral(Sprite moldura, Sprite icone, Sprite mask, Sprite molduraCampo)
    {
        this.moldura = moldura;
        this.icone = icone;
        this.mask = mask;
        this.molduraCampo = molduraCampo;
    }
    public AuxArmGeral(AuxArmGeral auxArm)
    {
        this.moldura = auxArm.moldura;
        this.icone = auxArm.icone;
        this.mask = auxArm.mask;
        this.molduraCampo = auxArm.molduraCampo;
    }
}
