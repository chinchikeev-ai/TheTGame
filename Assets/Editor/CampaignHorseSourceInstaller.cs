using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class CampaignHorseSourceInstaller
{
    const string SourceUrl = "https://raw.githubusercontent.com/xxfrigorizxx/The-Legacy-Of-Seroka/a3aaca61260eee91a1ffb05904b90b03094d829d/_tmp_quaternius_animals/FBX/Horse.fbx";
    const string ExpectedGitBlobSha = "1849f3f762cc52e8badc63b7dd371239428c935d";
    const string AssetPath = "Assets/Game/Art/Vehicles/Source/Quaternius/Horse.fbx";

    [MenuItem("The Troy Game/Characters/Install CC0 Animated Horse")]
    public static void Install()
    {
        if (File.Exists(AssetPath))
        {
            ConfigureImporter();
            Debug.Log("Quaternius horse source already installed.");
            return;
        }

        byte[] bytes;
        using (HttpClient client = new HttpClient())
            bytes = client.GetByteArrayAsync(SourceUrl).GetAwaiter().GetResult();

        string actual = GitBlobSha(bytes);
        if (!string.Equals(actual, ExpectedGitBlobSha, StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException("Horse source integrity check failed: " + actual);

        Directory.CreateDirectory(Path.GetDirectoryName(AssetPath));
        File.WriteAllBytes(AssetPath, bytes);
        AssetDatabase.ImportAsset(AssetPath, ImportAssetOptions.ForceSynchronousImport);
        ConfigureImporter();
        Debug.Log("Installed pinned CC0 Quaternius animated horse source: " + AssetPath);
    }

    public static GameObject LoadHorse()
    {
        GameObject horse = AssetDatabase.LoadAssetAtPath<GameObject>(AssetPath);
        return horse;
    }

    static void ConfigureImporter()
    {
        ModelImporter importer = AssetImporter.GetAtPath(AssetPath) as ModelImporter;
        if (importer == null) return;
        importer.importAnimation = true;
        importer.animationType = ModelImporterAnimationType.Generic;
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
