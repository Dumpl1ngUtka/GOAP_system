namespace Services.SaveLoad.Interfaces
{
    public interface ISaveLoadRepository<T> where T : struct
    {
        void Save(T data);
        T Load();
        bool HasSave();
        void RemoveSave();
    }
}