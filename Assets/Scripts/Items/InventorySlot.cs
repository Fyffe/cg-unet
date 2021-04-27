using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace LV
{
	public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        public Transform origin;

        public InventoryUI ui;

        public Text nameTxt;
        public Text amountTxt;

        public RectTransform rect;

        public Image icon;
        public Image rootImg;

        public void Init()
        {
            rect = GetComponent<RectTransform>();
            ui = GetComponentInParent<InventoryUI>();

            origin = transform.parent;
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {

        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == 0)
            {
                Pick();
            }
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button == 0)
            {
                Release();
            }
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
        }

        protected void Pick()
        {
            ui.PickItem(this);

            transform.SetParent(ui.transform);

            ToggleTexts(false);

            rootImg.enabled = false;

            OnPick();
        }

        protected virtual void OnPick()
        {

        }

        public void Release()
        {
            ToggleTexts(true);

            OnRelease();
        }

        public virtual void OnRelease()
        {
            transform.SetParent(origin);
            transform.localPosition = new Vector3(0, 0, 0);

            ui.selectedSlot = null;

            rootImg.enabled = true;
        }

        public void ToggleTexts(bool b)
        {
            nameTxt.gameObject.SetActive(b);
            amountTxt.gameObject.SetActive(b);
        }
    }
}