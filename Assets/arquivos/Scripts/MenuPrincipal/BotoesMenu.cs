using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotoesMenu : MonoBehaviour
{
    public void abrirUrl(string url)
    {
        Application.OpenURL(url);
    }
}
