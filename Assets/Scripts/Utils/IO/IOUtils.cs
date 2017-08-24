
using System.IO;
using UnityEngine;

namespace Drone.Utils.IO
{
    public class IOUtils : MonoBehaviour
    {


        public static ApplicationSettings LoadJson(string pathToJsonFile)
        {
            string filePath = Path.Combine(Application.streamingAssetsPath, pathToJsonFile);

            if (File.Exists(filePath))
            {
                // Read the json from the file into a string
                string dataAsJson = File.ReadAllText(filePath);
                // Pass the json to JsonUtility, and tell it to create a GameData object from it
                return JsonUtility.FromJson<ApplicationSettings>(dataAsJson);
            }
            else
            {
                Debug.LogError("Cannot load application settings!");
                return null;
            }
        }

    }
}