using System;
using UnityEngine;

public static class EnemyRuntimeBehaviorRegistry
{
    public static void Attach(GameObject enemyObject, string behaviorId)
    {
        if (enemyObject == null || string.IsNullOrWhiteSpace(behaviorId)) return;

        switch (behaviorId.Trim().ToLowerInvariant())
        {
            case "menelaus":
                if (enemyObject.GetComponent<MenelausBossController>() == null)
                    enemyObject.AddComponent<MenelausBossController>();
                return;
            default:
                throw new InvalidOperationException($"Unknown enemy runtime behavior id '{behaviorId}'. Add it to EnemyRuntimeBehaviorRegistry before authoring the encounter.");
        }
    }
}
