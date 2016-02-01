using UnityEngine;
using System;
using System.Reflection;

/// <summary>
/// Holds references to the prefabs of each challenge so they can be set through the designer.
/// </summary>
[Serializable]
public class LevelChallengePrefabs
{
    // variable names are important!
    // naming convention:
    //   {name of challenge script class}Front
    //   {name of challenge script class}Back
    //   {name of challenge script class}DecoratedBack
    //   {name of challenge script class}TextFront
    //   {name of challenge script class}TextBack

    [Header("Button Challenge")]
    public GameObject ButtonChallengeFront;
    public GameObject ButtonChallengeBack;
    public bool ButtonChallengeDecoratedBack;
    public String ButtonChallengeTextFront;
    public String ButtonChallengeTextBack;

    [Header("Button Sequence Challenge")]
    public GameObject ButtonSequenceChallengeFront;
    public GameObject ButtonSequenceChallengeBack;
    public bool ButtonSequenceChallengeDecoratedBack;
    public String ButtonSequenceChallengeTextFront;
    public String ButtonSequenceChallengeTextBack;

    [Header("Interconnected Dials Challenge")]
    public GameObject InterconnectedDialChallengeFront;
    public GameObject InterconnectedDialChallengeBack;
    public bool InterconnectedDialChallengeDecoratedBack;
    public String InterconnectedDialChallengeTextFront;
    public String InterconnectedDialChallengeTextBack;

    [Header("Sliders & Knobs Challenge")]
    public GameObject SlidersKnobsChallengeFront;
    public GameObject SlidersKnobsChallengeBack;
    public bool SlidersKnobsChallengeDecoratedBack;
    public String SlidersKnobsChallengeTextFront;
    public String SlidersKnobsChallengeTextBack;

    [Header("Synchronous Sliders Challenge")]
    public GameObject SynchronousSlidersChallengeFront;
    public GameObject SynchronousSlidersChallengeBack;
    public bool SynchronousSlidersChallengeDecoratedBack;
    public String SynchronousSlidersChallengeTextFront;
    public String SynchronousSlidersChallengeTextBack;

    [Header("Accelerometer Challenge")]
	public GameObject AccelerometerChallengeFront;
	public GameObject AccelerometerChallengeBack;
	public bool AccelerometerChallengeDecoratedBack;
    public String AccelerometerChallengeTextFront;
    public String AccelerometerChallengeTextBack;

    [Header("Password Reels Challenge")]
    public GameObject PasswordReelsChallengeFront;
    public GameObject PasswordReelsChallengeBack;
    public bool PasswordReelsChallengeDecoratedBack;
    public String PasswordReelsChallengeTextFront;
    public String PasswordReelsChallengeTextBack;

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

    public bool HasDecoratedBack(Type challengeType)
    {
        FieldInfo field = typeof(LevelChallengePrefabs).GetField(challengeType.Name + "DecoratedBack");
        return (bool)field.GetValue(this);
    }

    public String GetFrontText(Type challengeType)
    {
        FieldInfo field = typeof(LevelChallengePrefabs).GetField(challengeType.Name + "TextFront");
        return (String)field.GetValue(this);
    }

    public String GetBackText(Type challengeType)
    {
        FieldInfo field = typeof(LevelChallengePrefabs).GetField(challengeType.Name + "TextBack");
        return (String)field.GetValue(this);
    }

}
