using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptAtacarCachorro : MonoBehaviour
{
    public GameObject ataqueCachorro, cachorroInimigo, charA, miaCopia;

    IEnumerator atacarCachorro(float tempo)
    {

        ataqueCachorro.SetActive(true);
        yield return new WaitForSeconds(tempo);
        Destroy(cachorroInimigo);
        Destroy(ataqueCachorro);
        charA.SetActive(true);

    }

    private void Update()
    {
        if(miaCopia.activeInHierarchy == true)
        {
            StartCoroutine(atacarCachorro(2.5f));
        }
    }

}
