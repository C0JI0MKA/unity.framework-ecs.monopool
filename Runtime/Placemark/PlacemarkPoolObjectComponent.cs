namespace Leopotam.EcsLite.MonoPool.Placemark
{
    using Object;

    public struct PlacemarkPoolObjectComponent<TObject, TObjectTemplate>
        where TObject : MonoPoolObject<TObjectTemplate>
        where TObjectTemplate : IObjectTemplate<TObject>
    {
        public readonly TObject Signature;

        public PlacemarkPoolObjectComponent(TObject signature)
        {
            Signature = signature;
        }
    }
}
