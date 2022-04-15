using System.Collections.Generic;
using UnityEngine;

namespace ViralVial.Utilities
{
    public class CoroutineYielderCache
    {
        private Dictionary<string, YieldInstruction> yielderDictionary;

        private static CoroutineYielderCache instance = null;
        public static CoroutineYielderCache Instance
        {
            get
            {
                if (instance == null) instance = new CoroutineYielderCache();
                return instance;
            }
        }

        public CoroutineYielderCache()
        {
            yielderDictionary = new Dictionary<string, YieldInstruction>();
        }

        public static YieldInstruction AddNewYielder(string name, YieldInstruction yielder)
        {
            Instance.yielderDictionary.Add(name, yielder);
            return yielder;
        }

        public static YieldInstruction GetYielder(string name)
        {
            return Instance.yielderDictionary[name];
        }

        public static YieldInstruction ModifyYielder(string name, YieldInstruction yielder)
        {
            return Instance.yielderDictionary[name] = yielder;
        }

        public static YieldInstruction AddOrModifyYielder(string name, YieldInstruction yielder)
        {
            if (Instance.yielderDictionary.ContainsKey(name)) Instance.yielderDictionary[name] = yielder;
            else Instance.yielderDictionary.Add(name, yielder);
            return yielder;
        }
    }
}