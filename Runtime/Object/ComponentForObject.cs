using UnityEngine;
using System;

namespace Leopotam.EcsLite.MonoPool.Object
{
    [Serializable]
    public struct ComponentForObject
    {
        public Type Type => Type.GetType(_type);
        [SerializeField] private string _type;
        public object Value => _value;
        [SerializeReference] public object _value;

        public ComponentForObject(Type type, object value)
        {
            _type = type.Name;
            _value = value;
        }
    }
}