using UnityEditor;
using UnityEngine;

using Wm3Util;

public class LevelBuilder
{
    private string m_searchFolder;
    private string m_generatedFolder;
    private string m_generatedPath;
    private TextureManager m_textureManager;

    public LevelBuilder()
    {
        //TODO: move to configuration panel / Wm3Import
        m_searchFolder = "Assets/Import";
        m_generatedFolder = "Generated";
        m_generatedPath = "Assets/Generated";

        Initialize();
    }

    private void Initialize()
    {
        m_textureManager = new TextureManager(m_searchFolder, m_generatedPath);
    }

    private bool CreateAndClearFolder()
    {
        bool doCreate = true;
        string folder = "assets/" + m_generatedFolder;
        if (AssetDatabase.IsValidFolder(folder))
        {
            if (EditorUtility.DisplayDialog
                (
                "XML Importer",
                "Folder <" + m_generatedFolder + "> already exists.\nProceed and delete folder with all contents?\nNote: this cannot be undone!",
                "Yes", "No"
                )
             )
            {
                AssetDatabase.DeleteAsset(folder);
            }
            else
            {
                doCreate = false;
            }
        }

        if (doCreate)
        {
            string guid = AssetDatabase.CreateFolder("assets", m_generatedFolder);
            m_generatedPath = AssetDatabase.GUIDToAssetPath(guid);
        }

        return doCreate;
    }

    public bool Construct(Wm3Data data)
    {
        Initialize();
        if (data == null)
        {
            return false;
        }

        //TODO: move somewhere to Wm3Import
        if(!CreateAndClearFolder())
        {
            return false;
        }

        //MESHES
        GameObject level = new GameObject("Wm3Level");
        foreach (Wm3Mesh wm3Mesh in data.Meshes)
        {
            GameObject obj;
            if (MeshBuilder.Build(wm3Mesh, m_textureManager, out obj))
            {
                obj.transform.SetParent(level.transform);
            }
        }

        //OBJECTS
        GameObject objects = new GameObject("Wm3Objects");
        foreach (Wm3Object wm3Object in data.Objects)
        {
            GameObject obj;
            if (ObjectBuilder.Build(wm3Object, m_textureManager, out obj))
            {
                obj.transform.SetParent(objects.transform);
            }
        }

        m_textureManager.Report();

        return true;
    }

}
