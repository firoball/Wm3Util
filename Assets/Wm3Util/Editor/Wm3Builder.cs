using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEditor;
using UnityEngine;

using Wm3Util;

public class Wm3Builder
{
    private Material[] m_materials;

    private string m_searchFolder;
    private string m_generatedFolder;
    private string m_generatedPath;

    public const int c_solid = 0;
    public const int c_overlay = 1;
    public const int c_sky = 2;
    public const int c_sprite = 3;

    public Wm3Builder(Material[] materials)
    {
        m_materials = materials;
        m_searchFolder = "Assets/Import";
        m_generatedFolder = "Generated";
        m_generatedPath = "Assets/Generated";
    }

    public Wm3Builder() : this (null)
    {
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
        if (data == null)
        {
            return false;
        }

        CreateAndClearFolder(); //temp
        Wm3Texture.BaseMaterials = m_materials;
        Wm3Bitmap.SearchFolder = m_searchFolder;

        int matIdx = 0;
        foreach (Wm3Texture texture in data.Textures)
        {
            texture.SetupTexture();
            texture.BuildMaterial("material " + matIdx);
            Material material = texture.Material;
            AssetDatabase.CreateAsset(material, m_generatedPath + "/" + material.name + ".mat");
            matIdx++;
        }

        //hack
        GameObject level = new GameObject("Wm3Level");

        int idx = 0;
        foreach (Wm3Mesh mesh in data.Meshes)
        {
            if (mesh.Construct("mesh " + idx))
            {

                //this is all hack
                GameObject gameobj = MeshBuilder.SpawnMeshObject(mesh.Position, mesh.Mesh);
                gameobj.transform.SetParent(level.transform);
                gameobj.name = mesh.Mesh.name;
                gameobj.isStatic = true;
                MeshRenderer renderer = gameobj.GetComponent<MeshRenderer>();
                Collider collider = gameobj.GetComponent<Collider>();

                renderer.materials = mesh.Materials;

                //meshes carry ambient additive to material ambient
                float ambient = mesh.Ambient * 0.01f;
                //only single material meshes supported by overlay (wm3 exporter takes care of this)
                if (((mesh.Flags & Wm3Flags.Overlay) != 0) && (mesh.Materials.Length == 1))
                {
                    Material m = new Material(GetBaseMaterial(c_overlay));
                    m.mainTexture = renderer.sharedMaterial.mainTexture;
                    Color col = renderer.sharedMaterial.color + new Color(ambient, ambient, ambient, 0.0f);
                    m.color = col;// renderer.sharedMaterial.color;
                    renderer.sharedMaterial = m;
                }
                else if (Convert.ToInt16(mesh.Ambient) != 0)
                {
                    Material[] materials = renderer.sharedMaterials;
                    for (int mIdx = 0; mIdx < materials.Length; mIdx++)
                    {
                        Material m = new Material(materials[mIdx]);
                        m.mainTexture = materials[mIdx].mainTexture;
                        Color col = materials[mIdx].color + new Color(ambient, ambient, ambient, 0.0f);
                        m.color = col;
                        materials[mIdx] = m;
                    }
                    renderer.sharedMaterials = materials;
                }
                //TODO: set shader properties on renderer.materials not material directly!!

                if ((mesh.Flags & Wm3Flags.Invisible) != 0)
                {
                    renderer.enabled = false;
                }

                if ((mesh.Flags & Wm3Flags.Passable) != 0)
                {
                    collider.enabled = false;
                }

            }
            idx++;

        }

        GameObject objects = new GameObject("Wm3Objects");
        foreach (Wm3Object obj in data.Objects)
        {
            if (obj.Construct())
            {
                //this is all hack
                GameObject gameobj = MeshBuilder.SpawnSpriteObject(obj.Position, obj.Rotation, obj.Scale, obj.Sprite);
                gameobj.transform.SetParent(objects.transform);
                SpriteRenderer renderer = gameobj.GetComponent<SpriteRenderer>();
                Collider collider = gameobj.GetComponent<Collider>();

                if ((obj.Flags & Wm3Flags.Invisible) != 0)
                {
                    renderer.enabled = false;
                }

                if ((obj.Flags & Wm3Flags.Passable) != 0)
                {
                    collider.enabled = false;
                }

                renderer.sharedMaterial = obj.Material;
                Material m = new Material(GetBaseMaterial(c_sprite));
                m.mainTexture = renderer.sharedMaterial.mainTexture;
                //WM3 format already merges texture and object ambient (special treatment for A8 engine)
                //--> only use object ambient and drop any material ambient
                //m.color = renderer.sharedMaterial.color;
                float ambient = (obj.Ambient * 0.01f) + 0.5f;
                m.color = new Color(ambient, ambient, ambient, renderer.sharedMaterial.color.a);
                renderer.sharedMaterial = m;

                gameobj.isStatic = true;
            }
        }

        return true;
    }

    private Material GetBaseMaterial(int index)
    {
        index = Math.Min(index, m_materials.Length - 1);
        return m_materials[index];
    }
}
