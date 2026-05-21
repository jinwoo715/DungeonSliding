using JW.DungeonSliding.GamePlay.Ability;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class AbilityDatabaseJsonImporter
{
    private const string StatJsonPath = "Assets/00.Resources/Data/Ability/StatAbilityData.json";
    private const string RuleStatJsonPath = "Assets/00.Resources/Data/Ability/RuleStatAbilityData.json";
    private const string RuleJsonPath = "Assets/00.Resources/Data/Ability/RuleAbilityData.json";

    private const string DatabasePath = "Assets/00.Resources/ScriptableObjects/Ability/AbilityDatabase.asset";
    private const string StatAssetFolder = "Assets/00.Resources/ScriptableObjects/Ability/Stat";
    private const string RuleStatAssetFolder = "Assets/00.Resources/ScriptableObjects/Ability/RuleStat";
    private const string RuleAssetFolder = "Assets/00.Resources/ScriptableObjects/Ability/Rule";

    [MenuItem("Tools/Ability/Rebuild Ability Database From JSON")]
    public static void RebuildFromJson()
    {
        EnsureFolders();

        var settings = new JsonSerializerSettings
        {
            ContractResolver = new EmptyStringValueTypeResolver()
        };

        var database = LoadOrCreateDatabase();
        database.StatAbilities.Clear();
        database.RuleStatAbilities.Clear();
        database.RuleAbilities.Clear();

        foreach (var data in LoadJson<StatAbilityData>(StatJsonPath, settings))
        {
            var asset = LoadOrCreateAsset<StatAbilityDataSO>(StatAssetFolder, data.UID);
            CopyBaseData(asset, data);
            asset.PlayerStatType = data.PlayerStatType;
            asset.ApplyType = data.ApplyType;
            asset.RatioType = data.RatioType;
            asset.StatValue = data.StatValue;

            EditorUtility.SetDirty(asset);
            database.StatAbilities.Add(asset);
        }

        foreach (var data in LoadJson<RuleStatAbilityData>(RuleStatJsonPath, settings))
        {
            var asset = LoadOrCreateAsset<RuleStatAbilityDataSO>(RuleStatAssetFolder, data.UID);
            CopyBaseData(asset, data);
            asset.PlayerStatType = data.PlayerStatType;
            asset.ApplyType = data.ApplyType;
            asset.RatioType = data.RatioType;
            asset.StatValue = data.StatValue;
            asset.GameTriggerType = data.GameTriggerType;
            asset.CreatureTriggerType = data.CreatureTriggerType;
            asset.StatType = data.StatType;
            asset.NextAttackType = data.NextAttackType;
            asset.NextAttackValue = data.NextAttackValue;
            asset.NeedStackCount = data.NeedStackCount;
            asset.IsResetEnabled = data.IsResetEnabled;
            asset.ResetGameTrigger = data.ResetGameTrigger;
            asset.ResetCreatureTrigger = data.ResetCreatureTrigger;
            asset.ResetThreshold = data.ResetThreshold;

            EditorUtility.SetDirty(asset);
            database.RuleStatAbilities.Add(asset);
        }

        foreach (var data in LoadJson<RuleAbilityData>(RuleJsonPath, settings))
        {
            var asset = LoadOrCreateAsset<RuleAbilityDataSO>(RuleAssetFolder, data.UID);
            CopyBaseData(asset, data);
            asset.GameTrigger = data.GameTrigger;
            asset.CreatureTrigger = data.CreatureTrigger;
            asset.AbilityName = data.AbilityName;
            asset.P1 = data.P1;
            asset.P2 = data.P2;
            asset.Notes = data.Notes;

            EditorUtility.SetDirty(asset);
            database.RuleAbilities.Add(asset);
        }

        EditorUtility.SetDirty(database);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"AbilityDatabase rebuilt. Stat: {database.StatAbilities.Count}, RuleStat: {database.RuleStatAbilities.Count}, Rule: {database.RuleAbilities.Count}");
    }

    private static List<T> LoadJson<T>(string path, JsonSerializerSettings settings)
    {
        if (!File.Exists(path))
        {
            Debug.LogWarning($"Ability json not found: {path}");
            return new List<T>();
        }

        return JsonConvert.DeserializeObject<List<T>>(File.ReadAllText(path), settings) ?? new List<T>();
    }

    private static AbilityDatabaseSO LoadOrCreateDatabase()
    {
        var database = AssetDatabase.LoadAssetAtPath<AbilityDatabaseSO>(DatabasePath);

        if (database != null)
            return database;

        database = ScriptableObject.CreateInstance<AbilityDatabaseSO>();
        AssetDatabase.CreateAsset(database, DatabasePath);
        return database;
    }

    private static T LoadOrCreateAsset<T>(string folder, string uid) where T : ScriptableObject
    {
        string path = $"{folder}/{uid}.asset";
        var asset = AssetDatabase.LoadAssetAtPath<T>(path);

        if (asset != null)
            return asset;

        asset = ScriptableObject.CreateInstance<T>();
        AssetDatabase.CreateAsset(asset, path);
        return asset;
    }

    private static void CopyBaseData(AbilityDataSOBase asset, AbilityDataBase data)
    {
        asset.UID = data.UID;
        asset.Name = data.Name;
        asset.Description = data.Description;
        asset.IconName = data.IconName;
        asset.Rank = data.Rank;
        asset.name = data.UID;
    }

    private static void EnsureFolders()
    {
        EnsureFolder("Assets/00.Resources/ScriptableObjects", "Ability");
        EnsureFolder("Assets/00.Resources/ScriptableObjects/Ability", "Stat");
        EnsureFolder("Assets/00.Resources/ScriptableObjects/Ability", "RuleStat");
        EnsureFolder("Assets/00.Resources/ScriptableObjects/Ability", "Rule");
    }

    private static void EnsureFolder(string parent, string folder)
    {
        string path = $"{parent}/{folder}";

        if (!AssetDatabase.IsValidFolder(path))
            AssetDatabase.CreateFolder(parent, folder);
    }
}
