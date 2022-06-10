using UnityEngine;

namespace Save
{
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }
        
        public WorldSave CurrentSave { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this) 
            { 
                Destroy(this); 
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            CreateSave("Test Save");
        }

        public void CreateSave(string saveName)
        {
            CurrentSave = new WorldSave(saveName);
        }
    }
}