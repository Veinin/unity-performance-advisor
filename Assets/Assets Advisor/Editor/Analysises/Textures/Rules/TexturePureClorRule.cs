using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UPA
{
    // [Rule(AssetType.Texture)]
    public class TexturePureColorRule : AnalysistRule
    {
        public TexturePureColorRule()
        {
            name = "纯色纹理";
            desc = "纯色纹理，建议使用其他方式代替材质属性。";
        }

        public override bool OnAnalysis(AssetItem item)
        {
            var texture = item.obj as Texture2D;

            var importer = item.importer as TextureImporter;
            importer.isReadable = true;
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(texture));
            
            var isPureColor = true;
            var color = texture.GetPixel(0, 0);
            for (var i = 0; i < texture.width; i++)
            {
                for (var j = 0; j < texture.height; j++)
                {
                    if (i == 0 && j == 0)
                    {
                        continue;
                    }

                    if (texture.GetPixel(i, j) != color)
                    {
                        isPureColor = true;
                        break;
                    }
                }

                if (!isPureColor)
                {
                    break;
                }
            }

            importer.isReadable = false;
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(texture));
            
            return isPureColor;
        }
    }
}