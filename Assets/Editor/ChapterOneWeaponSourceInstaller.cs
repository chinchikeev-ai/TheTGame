using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class ChapterOneWeaponSourceInstaller
{
    const string SourceUrl = "https://raw.githubusercontent.com/jameskane05/three-game/571253b6098f269956f0137125d0920cc9cda4da/prototypes/tower-defense/assets/quaternius/bow.fbx";
    const string ExpectedGitBlobSha = "a836e493f3dbebc75a619e5aa99118f34f700718";
    const int ExpectedSize = 28540;
    const string AssetPath = "Assets/Game/Art/Characters/Source/Quaternius/MedievalWeapons/Bow.fbx";

    [MenuItem("The Troy Game/Characters/Install CC0 Chapter I Bow")]
    public static void Install()
    {
        if (File.Exists(AssetPath))
        {
            byte[] existing = File.ReadAllBytes(AssetPath);
            Validate(existing);
            AssetDatabase.ImportAsset(AssetPath, ImportAssetOptions.ForceSynchronousImport);
            ConfigureImporter();
            Debug.Log("Pinned Quaternius Chapter I bow source already installed and verified.");
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
        Debug.Log("Installed pinned CC0 Quaternius Medieval Weapons bow source: " + AssetPath);
    }

    public static GameObject LoadBow()
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
            throw new InvalidDataException("Bow source size check failed. Expected " + ExpectedSize + " bytes, got " + (bytes == null ? 0 : bytes.Length) + ".");

        string actual = GitBlobSha(bytes);
        if (!string.Equals(actual, ExpectedGitBlobSha, StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException("Bow source integrity check failed. Expected " + ExpectedGitBlobSha + ", got " + actual + ".");
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
