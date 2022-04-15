using System;
using System.Collections.Generic;
using UnityEngine;
namespace ViralVial.Utilities
{
    [Serializable]
    public class SerializableDictionary<TValue> : Dictionary<string, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<SertializableDictionaryStruct> elements = new List<SertializableDictionaryStruct>();

        // save the dictionary to lists
        public void OnBeforeSerialize()
        {
            elements.Clear();
            foreach (var pair in this)
            {
                elements.Add(new SertializableDictionaryStruct
                {
                    key = pair.Key,
                    value = pair.Value
                });
            }
        }

        // load dictionary from lists
        public void OnAfterDeserialize()
        {
            this.Clear();

            foreach (var element in elements)
            {
                this.Add(!this.ContainsKey(element.key) ? element.key : element.key + "_copy", element.value);
            }
        }

        [Serializable]
        public struct SertializableDictionaryStruct
        {
            public string key;
            public TValue value;
        }

        public string FindKeyForValue(TValue value)
        {
            foreach (var keyValuePair in this)
            {
                if (keyValuePair.Value.Equals(value)) return keyValuePair.Key;
            }
            return null;
        }
    }
}