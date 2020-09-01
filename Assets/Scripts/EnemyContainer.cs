using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyContainer : MonoBehaviour
{
    public EnemyController[] enemies;

    private void Awake()
    {
        enemies = GetComponentsInChildren<EnemyController>();
        Debug.Log("Tu ogarniam liste wrogów");
    }

    void Start()
    {
        foreach (var enemy in enemies)
            enemy.isAppearing = true;
    }



#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        int rowCount = 8;
        float tokenWidth = Settings.GetImmediate().tokens.size.x;
        Gizmos.color = Color.red;
        for (int i = 0; i < rowCount; i++)
            Gizmos.DrawWireCube(
                new Vector3( -tokenWidth * (0.5f * rowCount - 0.5f) + i * tokenWidth, 0), 
                new Vector3(0.9f * tokenWidth, 20));
    }
#endif

}
