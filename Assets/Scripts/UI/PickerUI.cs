using UnityEngine;
using UnityEngine.UI;

namespace LV
{
	public class PickerUI : MonoBehaviour 
	{
        private CanvasGroup display;

        public ItemEntity setEntity;

        public string pickUpSize = "PICK UP $ [E]";
        public string pickUpText = "PICK UP <color='#DF2935'>$</color> [E]";

        public Text baseTxt;
        public Text displayTxt;

        bool isVisible = false;

        void Awake()
        {
            display = GetComponent<CanvasGroup>();
        }

        public void SetItem(ItemEntity ent)
        {
            setEntity = ent;

            string name = ent.item.displayName;
            name = name.ToUpper();

            string txt = pickUpText;
            string sTxt = pickUpSize;

            sTxt = sTxt.Replace("$", name);
            txt = txt.Replace("$", name);

            baseTxt.text = sTxt;
            displayTxt.text = txt;

            ToggleDisplay(true);
        }

        public void ToggleDisplay(bool b)
        {
            if(isVisible == b)
            {
                return;
            }

            isVisible = b;
            display.alpha = b ? 1 : 0;

            if(!b)
            {
                setEntity = null;
            }
        }
	}
}