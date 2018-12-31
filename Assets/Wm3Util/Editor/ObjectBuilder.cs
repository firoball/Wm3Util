using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wm3Util
{
    public static class ObjectBuilder
    {

        public static bool Build(Wm3Object obj, TextureManager textureManager, out GameObject gameObj)
        {
            bool success = false;
            gameObj = null;
            Material material;
            Sprite sprite;
            if (textureManager.BuildObjectMaterial(obj.Texture, obj.Ambient, out material, out sprite))
            {
                gameObj = SpawnSpriteObject(sprite, material);
                gameObj.name = sprite.name;
                gameObj.transform.position = obj.Position;
                gameObj.transform.rotation = obj.Rotation;
                Vector3 scale = obj.Scale;
                scale.z = 0; //fix Wm3 random object z-scale
                gameObj.transform.localScale = scale;
                gameObj.isStatic = true;
                success = true;

                //TODO: move to static helper class
                Renderer renderer = gameObj.GetComponent<Renderer>();
                Collider collider = gameObj.GetComponent<Collider>();

                if (obj.Flags.IsSet(Wm3Flags.Invisible))
                {
                    renderer.enabled = false;
                }

                if (obj.Flags.IsSet(Wm3Flags.Passable))
                {
                    collider.enabled = false;
                }
            }
            return success;
        }

        private static GameObject SpawnSpriteObject(Sprite sprite, Material material)
        {
            GameObject obj = new GameObject();

            SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sharedMaterial = material;

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

            return obj;
        }
    }
}
