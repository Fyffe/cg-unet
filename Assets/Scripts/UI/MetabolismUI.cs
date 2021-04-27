using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LV
{
	public class MetabolismUI : MonoBehaviour 
	{
        public PlayerMetabolism meta;

        [Header("Health")]
        public Image healthFill;
        public Image healthIcon;

        public Color healthyColor;
        public Color deadColor;

        private float startHp;
        private  float targetHp;
        private  float lerpHpTime;

        private bool lerpHp;     

        [Header("Stamina")]
        public Image staminaFill;

        public Color restedColor;
        public Color tiredColor;

        private float startSt;
        private float targetSt;
        private float lerpStTime;

        private bool lerpSt;

        private bool isInit;

        public void Init(PlayerMetabolism meta)
        {
            this.meta = meta;

            meta.onHealthChanged += UpdateHealth;
            meta.onStaminaChanged += UpdateStamina;

            isInit = true;
        }

        void Update()
        {
            if(!isInit)
            {
                return;
            }

            if(Input.GetKeyDown(KeyCode.G))
            {
                meta.DecreaseHealth(10);
            }

            if (Input.GetKeyDown(KeyCode.H))
            {
                meta.IncreaseHealth(10);
            }

            if (lerpHp)
            {
                float perc = (Time.time - lerpHpTime) / 0.25f;

                float fill = Mathf.Lerp(startHp, targetHp, perc);

                Color c = healthFill.color;

                c = Color.Lerp(deadColor, healthyColor, fill);

                healthFill.fillAmount = fill;

                healthFill.color = c;
                healthIcon.color = c;
            }
        }

        public void UpdateEnemy()
        {

        }

        public void UpdateHealth(int change)
        {
            float perc = (float)meta.currentHealth / (float)meta.maxHealth;

            startHp = healthFill.fillAmount;
            targetHp = perc;
            lerpHpTime = Time.time;

            lerpHp = true;
        }

        public void UpdateStamina(int change)
        {

        }
	}
}