namespace JW.EditorUtility
{
    using UnityEditor;
    using UnityEngine;

    public class LoadAsset : MonoBehaviour
    {
        //texture에 이미지가 여러장 있을 경우
        public static Texture2D GetMultiTexture(string path, string spriteName)
        {
            string spriteSheetPath = "Assets/" + path;

            Object[] loadedObjects = AssetDatabase.LoadAllAssetRepresentationsAtPath(spriteSheetPath);
            Sprite[] sprites = new Sprite[loadedObjects.Length];

            //Object배열로 불러온 것은 배열을 통으로 캐스팅할 수 없기 때문에 개별적으로 캐스팅해서 넣어준다.
            for (int i = 0; i < loadedObjects.Length; i++)
            {
                sprites[i] = loadedObjects[i] as Sprite;
            }

            //단일 sprite 시트
            Sprite sprite = System.Array.Find(sprites, sprite => sprite.name == spriteName);

            //새로운 texture 생성
            //sprite의 width값과 height값으로 크기 지정
            //sprite의 texture에서 pixel들의 컬러값을 받아와서 지정
            Texture2D newText = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
            Color[] newColors2 = sprite.texture.GetPixels((int)sprite.rect.x, (int)sprite.rect.y, (int)sprite.rect.width, (int)sprite.rect.height);

            newText.SetPixels(newColors2);
            newText.Apply();

            return newText;
        }
        public static Texture2D GetSingleTexture(string path)
        {
            string assetPath = "Assets/" + path;
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
            return sprite.texture;
        }

        public static string GetJsonData(string path)
        {
            string assetPath = "Assets/" + path;
            TextAsset ta = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
            return ta.text;
        }
    }
}