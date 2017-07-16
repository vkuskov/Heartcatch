using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Heartcatch.Design.Models
{
    [CreateAssetMenu(menuName = "Heartcatch/UI Asset Bundle")]
    public sealed class UiAssetBundleDescriptionModel : ScriptableObject, IAssetBundleDescriptionModel
    {
        [SerializeField] private string name;
        [SerializeField] private bool includeToStreamingAssets;
        [SerializeField] private string path;
        [SerializeField] private string atlas = "ui";
        [SerializeField] private bool mipmaps = false;
        [SerializeField] private TextureImporterCompression compression = TextureImporterCompression.CompressedHQ;

        public string Name
        {
            get { return name; }
        }

        public bool IncludeToStreamingAssets
        {
            get { return includeToStreamingAssets; }
        }

        public void ApplySetting()
        {
            var guids = AssetDatabase.FindAssets("t:Texture", new string[] {this.path});
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var importer = (TextureImporter) TextureImporter.GetAtPath(path);
                var reimport = false;
                if (importer.spritePackingTag != atlas)
                {
                    reimport = true;
                    importer.spritePackingTag = atlas;
                }
                if (importer.mipmapEnabled != mipmaps)
                {
                    reimport = true;
                    importer.mipmapEnabled = mipmaps;
                }
                if (importer.textureCompression != compression)
                {
                    reimport = true;
                    importer.textureCompression = compression;
                }
                if (importer.isReadable)
                {
                    importer.isReadable = false;
                    reimport = true;
                }
                if (reimport)
                {
                    importer.SaveAndReimport();
                }
            }
        }

        public IEnumerable<AssetPath> GetAssetPaths()
        {
            var guids = AssetDatabase.FindAssets("t:Texture", new string[]{ this.path });
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                yield return new AssetPath()
                {
                    Name = Path.GetFileNameWithoutExtension(path),
                    HiDefAssetPath = path
                };
            }
            
        }
    }
}
