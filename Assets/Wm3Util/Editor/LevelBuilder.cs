using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEditor;
using UnityEngine;

using Wm3Util;

public class LevelBuilder
{
    private Material[] m_baseMaterials;

    private string m_searchFolder;
    private string m_generatedFolder;
    private string m_generatedPath;

    private List<LevelMaterial> m_levelMaterials;

    public const int c_solid = 0;
    public const int c_overlay = 1;
    public const int c_sky = 2;
    public const int c_sprite = 3;
    public const int c_max = 4;

    public LevelBuilder(Material[] materials)
    {
        m_baseMaterials = materials;
        m_searchFolder = "Assets/Import";
        m_generatedFolder = "Generated";
        m_generatedPath = "Assets/Generated";
    }

    public LevelBuilder() : this (null)
    {
        Initialize();
    }

    private void Initialize()
    {
        if (m_levelMaterials == null)
        {
            m_levelMaterials = new List<LevelMaterial>();
        }
        m_levelMaterials.Clear();

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

        CreateAndClearFolder(); //temp

        //MATERIALS
        foreach (Wm3Texture texture in data.Textures)
        {
            LevelMaterial levelMaterial = new LevelMaterial(m_searchFolder);
            if (levelMaterial.Construct(texture, m_baseMaterials))
            {
                m_levelMaterials.Add(levelMaterial);
                AssetDatabase.CreateAsset(levelMaterial.Material, m_generatedPath + "/" + levelMaterial.Material.name + ".mat");
            }
        }

        //MESHES
        //hack
        GameObject level = new GameObject("Wm3Level");

        int idx = 0;
        foreach (Wm3Mesh mesh in data.Meshes)
        {
            Mesh unityMesh = new Mesh();
            //if (mesh.Construct("mesh " + idx))
            if (MeshBuilder.Construct(unityMesh,mesh))
            {
                Material[] materials = GetMaterials(mesh.Textures);

                //this is all hack
                GameObject gameobj = MeshBuilder.SpawnMeshObject(mesh.Position, unityMesh);
                gameobj.transform.SetParent(level.transform);
                gameobj.name = unityMesh.name;
                gameobj.isStatic = true;
                MeshRenderer renderer = gameobj.GetComponent<MeshRenderer>();
                Collider collider = gameobj.GetComponent<Collider>();

                renderer.materials = materials;

                //meshes carry ambient additive to material ambient
                float ambient = mesh.Ambient * 0.01f;
                //only single material meshes supported by overlay (wm3 exporter takes care of this)
                if (mesh.Flags.IsSet(Wm3Flags.Overlay) && (materials.Length == 1))
                {
                    Material m = new Material(GetBaseMaterial(c_overlay))
                    {
                        mainTexture = renderer.sharedMaterial.mainTexture
                    };
                    Color col = renderer.sharedMaterial.color + new Color(ambient, ambient, ambient, 0.0f);
                    m.color = col;// renderer.sharedMaterial.color;
                    renderer.sharedMaterial = m;
                }
                else if (Convert.ToInt16(mesh.Ambient) != 0)
                {
                    Material[] sharedMaterials = renderer.sharedMaterials;
                    for (int mIdx = 0; mIdx < sharedMaterials.Length; mIdx++)
                    {
                        Material m = new Material(sharedMaterials[mIdx])
                        {
                            mainTexture = sharedMaterials[mIdx].mainTexture
                        };
                        Color col = sharedMaterials[mIdx].color + new Color(ambient, ambient, ambient, 0.0f);
                        m.color = col;
                        sharedMaterials[mIdx] = m;
                    }
                    renderer.sharedMaterials = sharedMaterials;
                }
                //TODO: set shader properties on renderer.materials not material directly!!

                if (mesh.Flags.IsSet(Wm3Flags.Invisible))
                {
                    renderer.enabled = false;
                }

                if (mesh.Flags.IsSet(Wm3Flags.Passable))
                {
                    collider.enabled = false;
                }

            }
            idx++;

        }

        //OBJECTS
        GameObject objects = new GameObject("Wm3Objects");
        foreach (Wm3Object obj in data.Objects)
        {
            LevelMaterial levelMaterial = GetLevelMaterial(obj.Texture);
            Material material = levelMaterial.Material;
            Sprite sprite = levelMaterial.Sprite;

            //this is all hack
            GameObject gameobj = ObjectBuilder.SpawnSpriteObject(obj.Position, obj.Rotation, obj.Scale, sprite);
            gameobj.transform.SetParent(objects.transform);
            SpriteRenderer renderer = gameobj.GetComponent<SpriteRenderer>();
            Collider collider = gameobj.GetComponent<Collider>();

            if (obj.Flags.IsSet(Wm3Flags.Invisible))
            {
                renderer.enabled = false;
            }

            if (obj.Flags.IsSet(Wm3Flags.Passable))
            {
                collider.enabled = false;
            }

            renderer.sharedMaterial = material;

            Material m = new Material(GetBaseMaterial(c_sprite))
            {
                mainTexture = renderer.sharedMaterial.mainTexture
            };
            //WM3 format already merges texture and object ambient (special treatment for A8 engine)
            //--> only use object ambient and drop any material ambient
            //m.color = renderer.sharedMaterial.color;
            float ambient = (obj.Ambient * 0.01f) + 0.5f;
            m.color = new Color(ambient, ambient, ambient, renderer.sharedMaterial.color.a);
            m.name = renderer.sharedMaterial.name;
            renderer.sharedMaterial = m;

            gameobj.isStatic = true;
            
        }

        return true;
    }

    private Material[] GetMaterials(Wm3Texture[] textures)
    {
        int count = textures.Length;
        Material[] materials = new Material[count];
        for (int j = 0; j < count; j++)
        {
            materials[j] = GetLevelMaterial(textures[j]).Material;
        }

        return materials;
    }

    private LevelMaterial GetLevelMaterial(Wm3Texture texture)
    {
        LevelMaterial levelMaterial = m_levelMaterials.Where(x => x.TextureReference == texture).FirstOrDefault();
        return levelMaterial;
    }

    private Material GetBaseMaterial(int index)
    {
        index = Math.Min(index, m_baseMaterials.Length - 1);
        return m_baseMaterials[index];
    }
}
