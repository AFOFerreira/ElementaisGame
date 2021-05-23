using UnityEngine;
using UnityEngine.UI;

public class BtnVisitante : MonoBehaviour
{

    Button btn;

    // Start is called before the first frame update
    void Start()
    {

        btn = GetComponent<Button>();
        btn.onClick.AddListener(clicado);
    }

    void clicado()
    {
        Main.Instance.usuario.Nickname = "Visitante" + Random.Range(1, 10000).ToString();
        Main.Instance.MainMenu();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
