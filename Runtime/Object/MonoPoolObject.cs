using UnityEngine;
using System;

namespace Leopotam.EcsLite.MonoPool.Object
{
    public abstract class MonoPoolObject<TTemplate> : MonoBehaviour
        where TTemplate : IObjectTemplate<MonoPoolObject<TTemplate>>
    {
        public event Action OnReturn;

        public int Entity { get; private set; }
        public TTemplate Template { get; private set; }

        private event Action<MonoPoolObject<TTemplate>> OnReset;

        public static (TObject obj, Action<int> SetIssuingNewIdentifier) Create<TObject, TObjectTemplate>(TObjectTemplate template, Transform parent, Action<TObject> onReset)
            where TObject : MonoPoolObject<TObjectTemplate>
            where TObjectTemplate : IObjectTemplate<TObject>
        {
            TObject answer = Instantiate(template.ObjectInstance, parent);
            answer.Template = template;
            answer.OnReset += (MonoPoolObject<TObjectTemplate> obj) => onReset(obj as TObject);
            return (answer, (int entity) => { answer.Entity = entity; });
        }
        public void Return()
        {
            OnReturn?.Invoke();
            OnReset?.Invoke(this);
            OnReturn = null;
        }
    }
}
