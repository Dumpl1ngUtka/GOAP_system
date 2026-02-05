using System.IO;
using Services.SaveLoad.Interfaces;
using UnityEngine;

namespace Services.SaveLoad.Repositories
{
    public class JsonSaveLoadRepository<T> : ISaveLoadRepository<T> where T : struct
    {
        private readonly string _saveFilePath;

        public JsonSaveLoadRepository(
            string saveFilePath)
        {
            _saveFilePath = saveFilePath;
        }
        
        public void Save(T data)
        {
            string json = JsonUtility.ToJson(data);
            File.WriteAllText(GetFullPath(), json);
        }

        public T Load()
        {
            string json = File.ReadAllText(GetFullPath());
            return JsonUtility.FromJson<T>(json);
        }

        public bool HasSave()
        {
            return File.Exists(GetFullPath());
        }

        public void RemoveSave()
        {
            string fullPath = GetFullPath();
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }

        private string GetFullPath()
        {
            return Path.Combine(Application.persistentDataPath, _saveFilePath);
        }
    }
}