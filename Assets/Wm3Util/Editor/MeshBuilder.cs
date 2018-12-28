using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshBuilder
{
    public static GameObject SpawnMeshObject(Vector3 position, Mesh mesh)
    {
        GameObject obj = new GameObject();
        obj.transform.position = position;

        MeshFilter filter = obj.AddComponent<MeshFilter>();
        filter.sharedMesh = mesh;

        MeshRenderer renderer = obj.AddComponent<MeshRenderer>();
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.receiveShadows = false;

        MeshCollider collider = obj.AddComponent<MeshCollider>();
        collider.sharedMesh = filter.sharedMesh;

        return obj;
    }

    public static GameObject SpawnSpriteObject(Vector3 position, Quaternion rotation, Vector3 scale, Sprite sprite)
    {
        GameObject obj = new GameObject();
        obj.name = sprite.name;

        obj.transform.position = position;
        obj.transform.rotation = rotation;
        scale.z = 0;
        obj.transform.localScale = scale;
        SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;

        //BoxCollider collider = obj.AddComponent<BoxCollider>();
        //collider.size = sprite.bounds.size;
        CapsuleCollider collider = obj.AddComponent<CapsuleCollider>();
        Vector2 size = new Vector2(sprite.bounds.extents.x, collider.height = sprite.bounds.extents.y);
        if (size.y >= size.x)
        {
            collider.direction = 1;
            collider.radius = size.x;
            collider.height = size.y * 2;
        }
        else
        {
            collider.direction = 0;
            collider.radius = size.y;
            collider.height = size.x * 2;
        }
        //        Debug.Log(obj.name + " " + sprite.bounds + " " + sprite.bounds.size+" "+collider.bounds);

        return obj;
    }
}
