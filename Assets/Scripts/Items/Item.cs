using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LV
{
    [CreateAssetMenu(menuName = "Items/Base")]
	public class Item : ScriptableObject
    {
        public int id;
        public string slug;

        public EItemType itemType;
        
        public string displayName;
        public string description;

        public int amount = 1;
        public int maxAmount = 1;
        public bool stackable = false;

        public bool onlyEquip = false;

        public GameObject prefab;

        public Sprite thumbnailSprite;
        public Sprite inspectSprite;
    }
}