namespace Leopotam.EcsLite.MonoPool.Placemark
{
    using Object;

    public struct PlacemarkObjectRequestComponent<TObject, TObjectTemplate>
        where TObject : MonoPoolObject<TObjectTemplate>
        where TObjectTemplate : IObjectTemplate<TObject>
    {
        public readonly TObjectTemplate Template;

        public PlacemarkObjectRequestComponent(TObjectTemplate template)
        {
            Template = template;
        }
    }
}
