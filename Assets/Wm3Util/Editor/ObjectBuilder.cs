using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wm3Util
{
    public static class ObjectBuilder
    {

        public static GameObject SpawnSpriteObject(Vector3 position, Quaternion rotation, Vector3 scale, Sprite sprite)
        {
            GameObject obj = new GameObject()
            {
                name = sprite.name
            };
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            scale.z = 0;
            obj.transform.localScale = scale;
            SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;

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
}
