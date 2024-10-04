using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Threading.Tasks;
using static SerializationAssistant;
using Yarn;
using Yarn.Unity;

/// <summary>
/// A singleton class that handles saving and loading
/// </summary>
public class SaveSystem : Singleton<SaveSystem>
{
    /// <summary>
    /// An inner class representing a save file. Should not do
    /// anything besides hold values.
    /// </summary>
    [System.Serializable]
    public class Save
    {
        public string filename;
        public SerializableVector3 sylviePosition;
        public HashSet<string> visitedAreas;
        public Dictionary<string, float> floatDialogueVariables;
        public Dictionary<string, string> stringDialogueVariables;
        public Dictionary<string, bool> boolDialogueVariables;
    }

    public const string DUMMY_FILE_NAME = "save0";

    void Awake()
    {
        InitializeSingleton(gameObject);
    }

    private void Start()
    {

    }

    /// <summary>
    /// Creates a new save file based on the current conditions of the game.
    /// Should only be called in the "World" section (TODO: check for that?)
    /// </summary>
    /// <returns>A save file representing the state of the world</returns>
    public static Save GenerateSave(string filename = DUMMY_FILE_NAME)
    {
        GameObject sylvie = GameObject.FindWithTag("Player");
        GameObject dialogueSystem = GameObject.Find("Dialogue System");
        (var f, var s, var b) = dialogueSystem.GetComponent<InMemoryVariableStorage>().GetAllVariables();
        Save save = new()
        {
            filename = filename,
            sylviePosition = sylvie.transform.position.ToSerializable(),
            visitedAreas = VisitedAreaManager.visitedAreas,
            floatDialogueVariables = f,
            stringDialogueVariables = s,
            boolDialogueVariables = b,
        };

        return save;
    }

    /// <summary>
    /// Write the given Save to a file based on its path name.
    /// Blocks when run synchronously, should be run in a background thread.
    /// </summary>
    /// <param name="save">The save file to write</param>
    public static void SaveToFile(Save save)
    {
        Debug.Log($"Saving to {Application.dataPath}");

        // The size of the buffer for the save file
        // (Shouldn't affect performance too much, as this file should
        // remain fairly small)
        const int BUF_SIZE = 1_000;

        BinaryFormatter bf = new();
        string realPath = Path.ChangeExtension(Path.Combine(Application.dataPath, save.filename), ".sylvie");
        using FileStream fs = new(
            realPath,
            FileMode.Create,
            FileAccess.ReadWrite,
            FileShare.ReadWrite,
            bufferSize: BUF_SIZE,
            useAsync: true);
        bf.Serialize(fs, save);
    }

    /// <summary>
    /// Load a Save from the given file.
    /// Runs synchronously (blocks), so it could be run on a background thread.
    /// </summary>
    /// <param name="filename">The name of the file to load (just the part 
    /// before the extension, "save0" right now</param>
    /// <returns>A Save if the file is found</returns>
    public static Save LoadFromFile(string filename)
    {
        try
        {
            // The size of the buffer for the save file
            // (Shouldn't affect performance too much, as this file should
            // remain fairly small)
            const int BUF_SIZE = 1_000;

            BinaryFormatter bf = new();
            string realPath = Path.ChangeExtension(Path.Combine(Application.dataPath, filename), ".sylvie");
            using FileStream fs = new(
                realPath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                bufferSize: BUF_SIZE,
                useAsync: true);
            Save save = (Save)bf.Deserialize(fs);

            Debug.Log($"Loaded from {Application.dataPath}");

            return save;
        }
        catch (System.Exception e)
        {
            Debug.Log($"Couldn't load from {Application.dataPath}: {e.Message}");

            return null;
        }
    }

    /// <summary>
    /// Takes a given Save and transforms the state of the world to match it.
    /// </summary>
    /// <param name="save">The Save file to be loaded</param>
    public static void LoadSave(Save save)
    {
        if (save == null) return;

        GameObject sylvie = GameObject.FindWithTag("Player");
        GameObject dialogueSystem = GameObject.Find("Dialogue System");
        sylvie.transform.position = save.sylviePosition.ToVector3();
        VisitedAreaManager.visitedAreas = save.visitedAreas;
        dialogueSystem.GetComponent<InMemoryVariableStorage>().SetAllVariables(
            save.floatDialogueVariables,
            save.stringDialogueVariables,
            save.boolDialogueVariables,
            clear: false
        );
        foreach ((string v, string s) in save.stringDialogueVariables)
        {
            Debug.Log($"Loaded {v} with value {s}");
        }
    }

    /// <summary>
    /// Generates a new save file and saves the game.
    /// </summary>
    public static void SaveGame(string filename = DUMMY_FILE_NAME)
    {
        Save save = GenerateSave(filename);
        Task.Run(() => SaveToFile(save));
    }

    /// <summary>
    /// Loads the game from the save at the default file name.
    /// </summary>
    /// NOTE: If the file does not exist, this will do nothing.
    public static void TryLoadGame(string filename = DUMMY_FILE_NAME)
    {
        LoadSave(LoadFromFile(filename));
    }
}
