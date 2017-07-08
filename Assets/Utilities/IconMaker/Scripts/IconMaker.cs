#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Utilities{
    [ExecuteInEditMode]
    public class IconMaker : MonoBehaviour
    {
        public bool create;
        public RenderTexture render;
        public Camera bakeCam;

        public string spriteName;


        void Update() {
            if (create) {
                CreateIcon();
                create = false;
            }
        }

        void CreateIcon() {
            if (string.IsNullOrEmpty(spriteName)) {
                spriteName = "icon";
            }

            string path = SaveLocation();
            path += spriteName;

            bakeCam.targetTexture = render;

            RenderTexture curRT = RenderTexture.active;
            bakeCam.targetTexture.Release();
            RenderTexture.active = bakeCam.targetTexture;
            bakeCam.Render();

            Texture2D imgPng = new Texture2D(bakeCam.targetTexture.width, bakeCam.targetTexture.height, TextureFormat.ARGB32, false);
            imgPng.ReadPixels(new Rect(0, 0, bakeCam.targetTexture.width, bakeCam.targetTexture.height), 0, 0);
            imgPng.Apply();
            RenderTexture.active = curRT;
            byte[] bytesPng = imgPng.EncodeToPNG();
            System.IO.File.WriteAllBytes(path + ".png", bytesPng);

            Debug.Log(spriteName + " created");
        }

        string SaveLocation() {
            string saveLocation = Application.streamingAssetsPath + "/Icons/";

            if (!Directory.Exists(saveLocation)) {
                Directory.CreateDirectory(saveLocation);
            }
            return saveLocation;
        }
    }

}
#endif