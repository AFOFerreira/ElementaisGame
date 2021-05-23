using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;

[System.Serializable]
public class config_usuario : MonoBehaviour
{
    int id;
    string nome;
    string nickname;
    string nivel;
    string xp;
    public int Id { get => id; set => id = value; }
    public string Nome { get => nome; set => nome = value; }
    public string Nickname { get => nickname; set => nickname = value; }
    public string Nivel { get => nivel; set => nivel = value; }
    public string Xp { get => xp; set => xp = value; }
}
