using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LV
{
	public class ItemEntity : MonoBehaviour, IPickupEntity
	{
        public int identity;

        public Item item;
        public int amount;

        private GameManager gameManager;

        public Rigidbody rb;
        public Collider col;

        public virtual void Init(Item it, int amt, int id)
        {
            item = it;
            amount = amt;
            identity = id;

            gameManager = GameManager.instance;

            rb = GetComponent<Rigidbody>();
            col = GetComponent<Collider>();
        }

        public void OnPickUp()
        {
            if (gameManager.localPlayer)
            {
                gameManager.localPlayer.vicinity.RemoveFromVicinity(this);
            }
        }
    }
}