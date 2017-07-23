using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Heartcatch.Editor
{
    [CreateAssetMenu(menuName = "Heartcatch/Sprite Atlas Description")]
    public sealed class SpriteAtlasDescription : ScriptableObject
    {
        [SerializeField] private string path;
        [SerializeField] private string packingTag = "Atlas";
        [SerializeField] private string assetBundle;
        [SerializeField] private bool mipmapEnabled = false;
        [SerializeField] private TextureImporterCompression compression = TextureImporterCompression.CompressedHQ;

        public void Apply()
        {
            var guids = AssetDatabase.FindAssets("t:Texture", new string[]{ path });
            foreach (var guid in guids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var importer = (TextureImporter) AssetImporter.GetAtPath(assetPath);
                var dirty = false;
                if (importer.assetBundleName != assetBundle)
                {
                    importer.SetAssetBundleNameAndVariant(assetBundle, null);
                }
                if (importer.mipmapEnabled != mipmapEnabled)
                {
                    importer.mipmapEnabled = mipmapEnabled;
                    dirty = true;
                }
                if (importer.spritePackingTag != packingTag)
                {
                    importer.spritePackingTag = packingTag;
                    dirty = true;
                }
                if (importer.isReadable)
                {
                    importer.isReadable = false;
                    dirty = true;
                }
                if (importer.textureCompression != compression)
                {
                    importer.textureCompression = compression;
                    dirty = true;
                }
                if (importer.crunchedCompression)
                {
                    importer.crunchedCompression = false;
                    dirty = true;
                }
                if (dirty)
                {
                    importer.SaveAndReimport();
                }
            }
        }
    }
}
