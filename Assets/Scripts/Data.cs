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
        public int whiteAttack;
        public int whiteCoolTime;
        public int brownAttack;
        public int brownCoolTime;
        public int orangeAttack;
        public int orangeCoolTime;
        public int yellowAttack;
        public int yellowCoolTime;
        public int greenAttack;
        public int greenCoolTime;
        public int blueAttack;
        public int blueCoolTime;
        public int pinkAttack;
        public int pinkCoolTime;
        public int redAttack;
        public int redCoolTime;
    }

    [Serializable]
    public class ExtractData<T> : ILoader<string, T> where T : class
    {
        public List<Player> playerList = new List<Player>();

        public Dictionary<string, T> MakeDict()
        {
            Dictionary<string, T> dict = new Dictionary<string, T>();

            if (typeof(T) == typeof(Player))
            {
                foreach (Player data in playerList)
                    dict.Add(data.key, data as T);
            }

            return dict;
        }

        public List<T> MakeList(Dictionary<string, T> dict)
        {
            List<T> list = new List<T>();

            if (typeof(T) == typeof(Player))
            {
                foreach (T info in dict.Values)
                    list.Add(info);
            }

            return list;
        }
    }
}