using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LV
{
    public class InGameMenu : MonoBehaviour
    {
        private InGameGUI gui;

        public CanvasGroup display;

        private bool isInit = false;

        public void Init(InGameGUI g)
        {
            gui = g;

            display = GetComponent<CanvasGroup>();

            isInit = true;
        }

        public bool isOpen { get; private set; } 

        public void ToggleDisplay(bool b)
        {
            display.alpha = b ? 1 : 0;
            display.blocksRaycasts = b;

            isOpen = b;

            if(isOpen)
            {
                gui.OnOpenMenus();
            }
            else
            {
                gui.OnCloseMenus();
            }
        }

        public void DisconnectFromServer()
        {
            CGNetworkManager.instance.StopHost();
        }
	}
}