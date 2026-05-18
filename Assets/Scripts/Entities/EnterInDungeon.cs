using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class EnterInDungeon : MonoBehaviour
{
    public void DoTeleport()
    {
        GameManager.ChangeScene("FirstDungeonLevel");
        Debug.Log("DoTeleport");
    }
}
