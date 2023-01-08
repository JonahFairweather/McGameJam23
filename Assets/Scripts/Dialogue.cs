using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName="ScriptableObjects/Dialogue")]
    public class Dialogue : ScriptableObject
    {
        public List<Image> Speakers;
        public List<String> Sentences;
        public List<String> Names;
    }

    [CreateAssetMenu(menuName="ScriptableObjects/Fish")]
    public class Fish : ScriptableObject
    {
        public enum FishRarity
        {
            Common, Rare, Legendary
        }

        public FishRarity rarity;
        public Sprite FishSprite;
        public String name;
    }
    
    [CreateAssetMenu(menuName="ScriptableObjects/FishTable")]
    public class FishTable : ScriptableObject
    {
        public Dictionary<Fish, float> ProbabilityTable;
    }
}