using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TabGroup : MonoBehaviour
{

    public List<TabButton> tabButtons;
    public Color tabIdle;
    public Color tabHover;
    public Color tabActive;
    public TabButton selectedTab;
    public List<GameObject> objectsToSwap;
    public int indexBotaoSelecionado = 0;
    public void Substribe(TabButton button)
    {
        if (tabButtons == null)
        {
            tabButtons = new List<TabButton>();
        }
        tabButtons.Add(button);
    }

    //public void OnTabEnter(TabButton button)
    //{
    //    ResetTabs();
    //    if (selectedTab == null || button != selectedTab)
    //    {
    //        button.corBackground.color = tabHover;
    //    }
    //}
    //public void OnTabExit(TabButton button)
    //{
    //    ResetTabs();
    //}
    private void Update()
    {
        Debug.Log("INDEX SELECIONADO: " + indexBotaoSelecionado);
    }
    public void OnTabSelected(TabButton button)
    {
        selectedTab = button;
        ResetTabs();
        button.corBackground.color = tabActive;
        //button.enabled = false;
        indexBotaoSelecionado = button.transform.GetSiblingIndex();
        if (indexBotaoSelecionado > 0)
        {
            if (indexBotaoSelecionado == 1)
            {
                if (!objectsToSwap[0].activeSelf)
                {
                    objectsToSwap[0].SetActive(true);
                    objectsToSwap[1].SetActive(false);

                }
            }
            else if (indexBotaoSelecionado > 1)
            {
                if (!objectsToSwap[1].activeSelf)
                {
                    objectsToSwap[0].SetActive(false);
                    objectsToSwap[1].SetActive(true);
                }
            }

        }
        else
        {
            foreach (var o in objectsToSwap)
            {
                o.SetActive(false);
            }
        }
    }

    public void ResetTabs()
    {
        foreach (TabButton button in tabButtons)
        {
            if (selectedTab != null && button == selectedTab) { continue; }
            button.corBackground.color = tabIdle;
            button.btn.interactable = true;
        }
    }
}
