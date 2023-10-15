using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class Player
    {
        public string key;
        public Int64 exp;
        public int gold;
        public int stage;
    }

    [Serializable]
    public class Shop
    {
        public Defines.EShop shopID;
        public int step;
        public int gold;
    }

    [Serializable]
    public class ExtractData<T> : ILoader<string, T> where T : class
    {
        public List<Player> playerList = new List<Player>();
        public List<Shop> shopList = new List<Shop>();

        public Dictionary<string, T> MakeDict()
        {
            Dictionary<string, T> dict = new Dictionary<string, T>();

            if (typeof(T) == typeof(Player))
            {
                foreach (Player data in playerList)
                    dict.Add(data.key, data as T);
            }
            if (typeof(T) == typeof(Shop))
            {
                foreach (Shop data in shopList)
                    dict.Add(data.shopID.ToString(), data as T);
            }

            return dict;
        }

        public List<T> MakeList(Dictionary<string, T> dict)
        {
            List<T> list = new List<T>();

            foreach (T info in dict.Values)
                list.Add(info);

            return list;
        }
    }
}