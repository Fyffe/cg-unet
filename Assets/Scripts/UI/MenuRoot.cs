using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LV
{
    public class MenuRoot : MonoBehaviour
    {
        protected MenuManager manager;

        public List<CanvasGroup> SubMenus = new List<CanvasGroup>();

        protected Transform menusRoot;
        protected CanvasGroup uiRoot;

        protected CanvasGroup currentMenu;
        protected CanvasGroup previousMenu;

        public bool isEnabled = true;

        protected virtual void Start()
        {
            manager = GetComponentInParent<MenuManager>();

            GetMenus();
        }

        protected void GetMenus()
        {
            uiRoot = transform.GetChild(0).GetComponent<CanvasGroup>();
            menusRoot = uiRoot.transform.Find("Menus");

            if (menusRoot)
            {
                for (int i = 0; i < menusRoot.childCount; i++)
                {
                    CanvasGroup menu = menusRoot.GetChild(i).GetComponent<CanvasGroup>();

                    if (menu)
                    {
                        SubMenus.Add(menu);
                    }
                }
            }

            if (SubMenus.Count > 0)
            {
                currentMenu = SubMenus[0];
            }

            ToggleRoot(isEnabled);
        }

        public void OpenMenu(int id)
        {
            if (id >= 0 && id < SubMenus.Count)
            {
                CanvasGroup menu = SubMenus[id];

                if (menu)
                {
                    if (currentMenu)
                    {
                        ToggleMenu(currentMenu, false);
                    }

                    ToggleMenu(menu, true);
                }
            }
        }

        public void CloseMenu(int id)
        {
            if (id >= 0 && id < SubMenus.Count)
            {
                CanvasGroup menu = SubMenus[id];

                if (menu)
                {
                    ToggleMenu(menu, false);
                }
            }
        }

        public void PreviousMenu()
        {
            if (previousMenu)
            {
                CanvasGroup prev = previousMenu;

                ToggleMenu(currentMenu, false);
                ToggleMenu(prev, true);
            }
        }

        protected void ToggleMenu(CanvasGroup m, bool b)
        {
            m.blocksRaycasts = b;
            m.alpha = b ? 1 : 0;

            if (b)
            {
                currentMenu = m;
            }
            else
            {
                previousMenu = m;
            }
        }

        public void ToggleRoot(bool b)
        {
            uiRoot.alpha = b ? 1 : 0;
            uiRoot.blocksRaycasts = b;
        }
    }
}