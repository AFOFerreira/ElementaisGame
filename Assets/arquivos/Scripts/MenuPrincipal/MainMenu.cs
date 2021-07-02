using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public TextMeshProUGUI nickname;
    public TextMeshProUGUI XP;
    public TextMeshProUGUI Nivel;
    public Image profile_image;

    

    private void Awake()
    {
        //nickname.text = Main.Instance.usuario.Nickname;
        //XP.text = Main.Instance.usuario.Xp + " xp";
        //Nivel.text = Main.Instance.usuario.Nivel;
        //Main.Instance.facebook.GetPicture(profile_image);
    }
    // Start is called before the first frame update
    void Start()
    {
        AudioBase._instance.playMusicaMenu();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Treino()
    {
        Main.Instance.Treino();
    }
}
