using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// <para>Static class so we dont need to create 10000 instances of it and hold it on the scene.</para>
/// Just contains whatever important variables we have, that we might need throughout the runtime of the game.
/// </summary>
public static class Database
{
    public static TextAsset currentJson;

}
