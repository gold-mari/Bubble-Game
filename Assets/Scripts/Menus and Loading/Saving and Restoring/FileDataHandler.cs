// File IO implementation written with the help of Shaped by Rain Studios:
// https://www.youtube.com/watch?v=aUi9aijvpgs

using UnityEngine;
using System.IO;
using System;

public static class FileDataHandler
{
    private static readonly string dirPath = Application.persistentDataPath;
    private static readonly string fileName = "save.dat";
    private static readonly string fullPath = Path.Combine(dirPath, fileName);
    private static readonly string codeword = ")Mx␎-PA␟Z␇C&␆|(5hx%Zp-(IB␙␞HEg)d";

    public static SaveHandler.SaveData Load()
    {
        // Loads our data from the location specified by fullPath.
        // ================

        if (File.Exists(fullPath)) {
            try {
                // Begin reading from file. We use using() to ensure garbage collection.
                using FileStream stream = new(fullPath, FileMode.Open);
                using StreamReader reader = new(stream);
                string JSON = reader.ReadToEnd();
                JSON = EncryptDecrypt(JSON);

                // Deserialize from JSON.
                return JsonUtility.FromJson<SaveHandler.SaveData>(JSON);
            } catch (Exception e) {
                Debug.LogError($"FileDataHandler Error: Load failed. "
                                + $"Ran into exception while trying to load from {fullPath}:\n{e}");
                return null;
            }
        } else {
            // No save file, return null SaveData.
            // Not necessarily a bug- this is the initial state before any Save() calls, for example.
            return null;
        }
    }

    public static void Save(SaveHandler.SaveData data)
    {
        // Saves our data, as JSON (for now), to the location specified by fullPath.
        // ================

        try {
            // Create directory if it doesn't exist!
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            // Serialize our save to JSON.
            string JSON = JsonUtility.ToJson(data, true);
            JSON = EncryptDecrypt(JSON);
            // Begin writing to file. We use using() to ensure garbage collection.
            using FileStream stream = new(fullPath, FileMode.Create);
            using StreamWriter writer = new(stream);
            writer.Write(JSON);
            Debug.Log($"FileDataHandler: Finished saving to {fullPath}");
        } catch (Exception e) {
            Debug.LogError($"FileDataHandler Error: Save failed. "
                         + $"Ran into exception while trying to save to {fullPath}:\n{e}");
        }
    }

    private static string EncryptDecrypt(string data)
    {
        string modifiedData = "";

        for (int i = 0; i < data.Length; i++) {
            modifiedData += (char)(data[i] ^ codeword[i % codeword.Length]);
        }

        return modifiedData;
    }
}