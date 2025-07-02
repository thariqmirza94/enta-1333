using UnityEngine;

public static class CombatAIUtility
{
    public static GameObject FindClosestEnemy(Vector3 origin, string enemyTag, float maxRange)
    {
        GameObject best = null;
        float bestSq = maxRange * maxRange;
        foreach (var go in GameObject.FindGameObjectsWithTag(enemyTag))
        {
            float sq = (go.transform.position - origin).sqrMagnitude;
            if (sq < bestSq) { bestSq = sq; best = go; }
        }
        return best;
    }
}