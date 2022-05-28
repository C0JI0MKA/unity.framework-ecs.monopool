using System.Collections.Generic;
using UnityEngine;

namespace Leopotam.EcsLite.MonoPool.Object
{
    public interface IObjectTemplate<out TObject>
        where TObject : MonoBehaviour
    {
        public TObject ObjectInstance { get; }
        public IReadOnlyList<ComponentForObject> Components { get; }
    }
}
