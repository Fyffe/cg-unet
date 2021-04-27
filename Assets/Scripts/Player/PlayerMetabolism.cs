using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LV
{
	public class PlayerMetabolism : MonoBehaviour 
	{
        private PlayerRoot root;
        private MetabolismUI ui;

        public delegate void OnHealthChanged(int change);
        public event OnHealthChanged onHealthChanged;

        public delegate void OnStaminaChanged(int change);
        public event OnStaminaChanged onStaminaChanged;

        public delegate void OnDeath();
        public event OnDeath onDeath;

        public int currentHealth;
        public int maxHealth { get; protected set; }

        public int currentStamina;
        public int maxStamina { get; protected set; }

        public bool isDead = false;

        private bool isInit = false;

        public void Init(PlayerRoot r)
        {
            root = r;
            ui = root.inGameGUI.metaUI;

            maxHealth = 100;
            maxStamina = 100;

            currentHealth = maxHealth;
            currentStamina = maxStamina;

            ui.Init(this);

            isInit = true;
        }

        void ChangeHealth(int amt)
        {
            if(!isInit)
            {
                return;
            }

            Debug.Log("Changing health by " + amt);
            currentHealth += amt;

            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            if(onHealthChanged != null)
            {
                onHealthChanged(amt);
            }

            CheckAlive();
        }

        public void DecreaseHealth(int damage)
        {
            if (!isInit)
            {
                return;
            }

            damage = Mathf.Abs(damage);
            ChangeHealth(-damage);
        }

        public void IncreaseHealth(int heal)
        {
            if (!isInit)
            {
                return;
            }

            heal = Mathf.Abs(heal);
            ChangeHealth(heal);
        }

        public void ChangeStamina(int amt)
        {
            if (!isInit)
            {
                return;
            }

            currentStamina += amt;

            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

            if (onStaminaChanged != null)
            {
                onStaminaChanged(amt);
            }
        }

        public void DecreaseStamina(int subs)
        {
            if (!isInit)
            {
                return;
            }

            subs = Mathf.Abs(subs);
            ChangeStamina(-subs);
        }

        public void IncreaseStamina(int add)
        {
            if (!isInit)
            {
                return;
            }

            add = Mathf.Abs(add);
            ChangeStamina(add);
        }

        public void CheckAlive()
        {
            if (!isInit)
            {
                return;
            }

            if (currentHealth <= 0)
            {
                isDead = true;
            }

            if (isDead)
            {
                if (onDeath != null)
                {
                    onDeath();
                }

                Destroy(gameObject);
            }
        }
    }
}