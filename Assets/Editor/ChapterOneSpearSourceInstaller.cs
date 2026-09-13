using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class ChapterOneSpearSourceInstaller
{
    const string SourceUrl = "https://raw.githubusercontent.com/DevHuang1/embervale-godot/aa6608e466c377ef588a09eca993f89c0c92470e/assets/models/weapons/quaternius/Spear.fbx";
    const string ExpectedGitBlobSha = "47ccecfadd1779ae687a94d780b81a8934855b7d";
    const int ExpectedSize = 38556;
    const string AssetPath = "Assets/Game/Art/Characters/Source/Quaternius/MedievalWeapons/Spear.fbx";

    [MenuItem("The Troy Game/Characters/Install CC0 Chapter I Spear")]
    public static void Install()
    {
        if (File.Exists(AssetPath))
        {
            byte[] existing = File.ReadAllBytes(AssetPath);
            Validate(existing);
            AssetDatabase.ImportAsset(AssetPath, ImportAssetOptions.ForceSynchronousImport);
            ConfigureImporter();
            Debug.Log("Pinned Quaternius Chapter I spear source already installed and verified.");
            return;
        }

        byte[] bytes;
        using (HttpClient client = new HttpClient())
            bytes = client.GetByteArrayAsync(SourceUrl).GetAwaiter().GetResult();

        Validate(bytes);
        Directory.CreateDirectory(Path.GetDirectoryName(AssetPath));
        File.WriteAllBytes(AssetPath, bytes);
        AssetDatabase.ImportAsset(AssetPath, ImportAssetOptions.ForceSynchronousImport);
        ConfigureImporter();
        Debug.Log("Installed pinned CC0 Quaternius Medieval Weapons spear source: " + AssetPath);
    }

    public static GameObject LoadSpear()
    {
        return AssetDatabase.LoadAssetAtPath<GameObject>(AssetPath);
    }

    public static string GetAssetPath()
    {
        return AssetPath;
    }

    static void Validate(byte[] bytes)
    {
        if (bytes == null || bytes.Length != ExpectedSize)
            throw new InvalidDataException("Spear source size check failed. Expected " + ExpectedSize + " bytes, got " + (bytes == null ? 0 : bytes.Length) + ".");

        string actual = GitBlobSha(bytes);
        if (!string.Equals(actual, ExpectedGitBlobSha, StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException("Spear source integrity check failed. Expected " + ExpectedGitBlobSha + ", got " + actual + ".");
    }

    static void ConfigureImporter()
    {
        ModelImporter importer = AssetImporter.GetAtPath(AssetPath) as ModelImporter;
        if (importer == null) return;
        importer.importAnimation = false;
        importer.animationType = ModelImporterAnimationType.None;
        importer.importCameras = false;
        importer.importLights = false;
        importer.SaveAndReimport();
    }

    static string GitBlobSha(byte[] bytes)
    {
        byte[] header = Encoding.ASCII.GetBytes("blob " + bytes.Length + "\0");
        byte[] input = new byte[header.Length + bytes.Length];
        Buffer.BlockCopy(header, 0, input, 0, header.Length);
        Buffer.BlockCopy(bytes, 0, input, header.Length, bytes.Length);
        using (SHA1 sha = SHA1.Create())
        {
            byte[] hash = sha.ComputeHash(input);
            StringBuilder result = new StringBuilder(hash.Length * 2);
            foreach (byte b in hash) result.Append(b.ToString("x2"));
            return result.ToString();
        }
    }
}
