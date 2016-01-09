using UnityEngine;
using System;
using System.Reflection;

/// <summary>
/// Holds references to the prefabs of each challenge so they can be set through the designer.
/// </summary>
[Serializable]
public class LevelChallengePrefabs
{
    // variable name is important!
    // naming convention:
    //   {name of challenge script class}Front
    //   {name of challenge script class}Back

    [Header("Button Challenge")]
    public GameObject ButtonChallengeFront;
    public GameObject ButtonChallengeBack;

	[Header("Button Sequence Challenge")]
	public GameObject ButtonSequenceChallengeFront;
	public GameObject ButtonSequenceChallengeBack;

    public GameObject GetFrontPrefab(Type challengeType)
    {
        FieldInfo field = typeof(LevelChallengePrefabs).GetField(challengeType.Name + "Front");
        return (GameObject)field.GetValue(this);
    }

    public GameObject GetBackPrefab(Type challengeType)
    {
        FieldInfo field = typeof(LevelChallengePrefabs).GetField(challengeType.Name + "Back");
        return (GameObject)field.GetValue(this);
    }
}
