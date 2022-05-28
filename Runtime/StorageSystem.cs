using System.Collections.Generic;
using UnityEngine;
using System;

namespace Leopotam.EcsLite.MonoPool
{
    using Placemark;
    using Object;

    public sealed class StorageSystem<TObject, TObjectTemplate> : IEcsInitSystem, IEcsRunSystem
        where TObject : MonoPoolObject<TObjectTemplate>
        where TObjectTemplate : IObjectTemplate<TObject>
    {
        private EcsPool<PlacemarkObjectRequestComponent<TObject, TObjectTemplate>> _placemarkObjectRequestPool = null;
        private EcsPool<PlacemarkPoolObjectComponent<TObject, TObjectTemplate>> _placemarkPoolObjectPool = null;
        private EcsPool<PlacemarkJustCreateObjectComponent> _placemarkJustCreateObjectPool = null;
        private EcsFilter _filter = null;
        private EcsWorld _world = null;

        private readonly Dictionary<TObject, Action<int>> _objectsThierInstallToEntity = new(); 
        private readonly Dictionary<TObjectTemplate, Queue<TObject>> _freeForUse = new();
        private readonly Transform _storageLocation;

        public StorageSystem(Transform storageLocation)
        {
            _storageLocation = storageLocation;
        }
        public void Init(EcsSystems systems)
        {
            _world = systems.GetWorld();
            _filter = _world.Filter<PlacemarkObjectRequestComponent<TObject, TObjectTemplate>>().End();
            _placemarkPoolObjectPool = _world.GetPool<PlacemarkPoolObjectComponent<TObject, TObjectTemplate>>();
            _placemarkObjectRequestPool = _world.GetPool<PlacemarkObjectRequestComponent<TObject, TObjectTemplate>>();
            _placemarkJustCreateObjectPool = _world.GetPool<PlacemarkJustCreateObjectComponent>();
        }
        public void Run(EcsSystems systems)
        {
            foreach(int entity in _filter)
            {
                PlacemarkObjectRequestComponent<TObject, TObjectTemplate> placemarkObjectRequest = _placemarkObjectRequestPool.Get(entity);
                Receive(placemarkObjectRequest.Template);
                _world.DelEntity(entity);
            }
        }
        private void Receive(TObjectTemplate template)
        {
            TObject obj = GetObject(template);
            int entity = _world.NewEntity();

            _objectsThierInstallToEntity[obj].Invoke(entity);
            for(int index = 0; index < template.Components.Count; index++)
            {
                ComponentForObject component = template.Components[index];
                AddComponentToObject(entity, component);
            }
            obj.transform.parent = null;
            _placemarkPoolObjectPool.Add(entity) = new PlacemarkPoolObjectComponent<TObject, TObjectTemplate>(obj);
            _placemarkJustCreateObjectPool.Add(entity);
        }
        private TObject GetObject(TObjectTemplate template)
        {
            if (_freeForUse.ContainsKey(template) is not true)
                _freeForUse.Add(template, new Queue<TObject>());

            return _freeForUse[template].Count switch
            {
                0 => Create(template),
                _ => _freeForUse[template].Dequeue()
            };
        }
        private TObject Create(TObjectTemplate template)
        {
            (TObject obj, Action<int> setIssuingNewIdentifier) = MonoPoolObject<TObjectTemplate>.Create<TObject, TObjectTemplate>(template, _storageLocation, Returning);
            _objectsThierInstallToEntity.Add(obj, setIssuingNewIdentifier);

            return obj;
        }
        private void AddComponentToObject(int entity, ComponentForObject component)
        {
            IEcsPool pool = typeof(EcsWorld).GetMethod("GetPool").MakeGenericMethod(component.Type).Invoke(_world, null) as IEcsPool;
            pool.AddRaw(entity, component.Value);
        }
        private void Returning(TObject obj)
        {
            obj.transform.parent = _storageLocation;
            _freeForUse[obj.Template].Enqueue(obj);
            _world.DelEntity(obj.Entity);
            _objectsThierInstallToEntity[obj].Invoke(-1);
        }
    }
}
