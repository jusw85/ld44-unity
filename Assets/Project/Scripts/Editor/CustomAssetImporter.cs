using UnityEngine;
using UnityEditor;

public class CustomAssetImporter : AssetPostprocessor {

    public void OnPreprocessTexture() {
        var importer = (TextureImporter)assetImporter;

        TextureImporterSettings settings = new TextureImporterSettings();
        importer.ReadTextureSettings(settings);
        settings.textureType = TextureImporterType.Sprite;
        settings.spritePixelsPerUnit = 64f;
        settings.filterMode = FilterMode.Point;
        importer.SetTextureSettings(settings);

        TextureImporterPlatformSettings platformSettings = importer.GetDefaultPlatformTextureSettings();
        platformSettings.textureCompression = TextureImporterCompression.Uncompressed;
        importer.SetPlatformTextureSettings(platformSettings);
    }

    public void OnPostprocessTexture(Texture2D texture) {
    }

    public void OnPostprocessSprites(Texture2D texture, Sprite[] sprites) {
    }

}
