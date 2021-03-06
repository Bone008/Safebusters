﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

/// <summary>
/// Controller of the level container that holds all safes.
/// </summary>
public class Level : MonoBehaviour
{

    public GameObject safePrefab;
    public LevelGenerationOptions generationOptions = new LevelGenerationOptions();
    public LevelGenerationOptions generationOptionsEasy = new LevelGenerationOptions();
    public LevelChallengePrefabs challengePrefabs = new LevelChallengePrefabs();
    
    public GameObject Player1Cam;
    public GameObject Player2Cam;
    public GameObject[] neuroToxinParticleSystem;
    public GameObject[] safeContentPrefabs;
    
    public Sprite[] iconSprites = new Sprite[0];
    public float[] iconSpriteRotations = new float[0];

    [SerializeField]
    List<UnityEngine.UI.Text> endGUIPlayer1;
    [SerializeField]
    List<UnityEngine.UI.Text> endGUIPlayer2;
    [SerializeField]
    GameObject endGUICanvas;
	[SerializeField]
	GameObject pauseCanvas;
    

    [HideInInspector]
    public List<Safe> safes;
    [HideInInspector]
    public int currentLifeCount;

    public Text player1Text;
    public Text player2Text;

    private List<string> fails = new List<string>();

    private float gameTime;
    private bool endOfGame = false;
    private float endOfLevelTimer = 15.0f;

    void Awake()
    {
        if(iconSprites.Length != iconSpriteRotations.Length)
        {
            Debug.LogError("Config error: Icon Sprites and Icon Sprite Rotations have to be the same length!");
            return;
        }

        if (PlayerPrefs.GetInt("EnableTutorial", 1) == 0)
        {
            foreach (var go in GameObject.FindGameObjectsWithTag("Tutorial"))
                go.SetActive(false);
        }

        // if started with easy difficulty, replace generation options with easy variant
        if(PlayerPrefs.GetInt("Difficulty") == 1)
        {
            generationOptions = generationOptionsEasy;
        }

        currentLifeCount = generationOptions.maxLives;
        GenerateLevel();
    }

    void Update() {
		// back to main menu
		if (Input.GetKeyDown (KeyCode.Escape)) {
			Application.LoadLevel("Menu");
		}

		// pause
		if (Input.GetKeyDown (KeyCode.Pause)) {
			Time.timeScale = 1 - Time.timeScale;
			pauseCanvas.SetActive(!pauseCanvas.activeSelf);
		}

        gameTime += Time.deltaTime;
        if(endOfGame){
            if (endOfLevelTimer > 0.0f)
            {
                endOfLevelTimer -= Time.deltaTime;
                endGUIPlayer1[4].text = endOfLevelTimer.ToString("F1") + " seconds";
                endGUIPlayer2[4].text = endOfLevelTimer.ToString("F1") + " seconds";
            }
            else {
                Application.LoadLevel(0);
            }
        }
    }

    private void GenerateLevel()
    {
		// get safe-bounds
        Bounds bounds = safePrefab.GetComponent<Safe>().GetFrameBounds();
        float safeWidth = bounds.size.x;
        float safeHeight = bounds.size.y;

		// calculate base offset
        int rows = Mathf.CeilToInt(generationOptions.safesToGenerate / (float)generationOptions.safesPerRow);
        Vector3 baseOffset = new Vector3((-generationOptions.safesPerRow + 1) * safeWidth / 2, rows * safeHeight - safeHeight / 2, 0);
		Vector3 offset;
        
		// create safe gameobjects and logic
		safes = new List<Safe>();
        int currentlyActiveSafeCount = 0;
        int i = 0;
        foreach(Type challengeType in EnumerateRandomChallenges().Take(generationOptions.safesToGenerate))
        {
			// create GameObject
            var go = Instantiate(safePrefab, Vector3.zero, Quaternion.identity) as GameObject;
            go.transform.SetParent(transform);
            go.name = "Safe " + i;

            // offset from top left position
            offset = new Vector3(safeWidth * (i % generationOptions.safesPerRow), -safeHeight * (int)(i / generationOptions.safesPerRow), 0);
            go.transform.localPosition = baseOffset + offset;

            //spawn in a content for the safe
            GameObject.Instantiate(safeContentPrefabs[UnityEngine.Random.Range(0,safeContentPrefabs.Length)], go.transform.FindChild("safeContent").position,go.transform.FindChild("safeContent").rotation);

			// instantiate the challenge
            AbstractChallenge challenge = (AbstractChallenge)go.AddComponent(challengeType);
            Debug.Log(challenge.GetHumanName());

			// find challenge-prefabs
            GameObject frontPrefab = challengePrefabs.GetFrontPrefab(challengeType);
            GameObject backPrefab = challengePrefabs.GetBackPrefab(challengeType);
            bool decoratedBack = challengePrefabs.HasDecoratedBack(challengeType);

			// initialize safe logic
			Safe safe = go.GetComponent<Safe>();
			safe.challenge = challenge;   
            safe.SetMaxTimer(UnityEngine.Random.Range(generationOptions.minTimer, generationOptions.maxTimer));
            safe.SpawnChallengeObjects(frontPrefab, backPrefab, decoratedBack);

            safe.SetBackwards(i % 2 == 1);
            // this is a possible alternative algorithm where the left half of the wall is front-facing
            // and the right half is back-facing (uneven safes-per-row numbers alternate front/back in the middle
            //int spr = generationOptions.safesPerRow;
            //safe.SetBackwards(i % spr >= (spr / 2) || (spr % 2 == 1 && i % spr == (spr / 2) && (i / spr % 2) == 1));

            bool shouldActivate;
            if (generationOptions.activateRandomly)
            {
                float activeProbability = generationOptions.safesActiveAtStart / (float)generationOptions.safesToGenerate;
                shouldActivate = (currentlyActiveSafeCount < generationOptions.safesActiveAtStart && UnityEngine.Random.value < activeProbability);
            }
            else
                shouldActivate = (i < generationOptions.safesActiveAtStart);


            if (shouldActivate)
            {
                safe.SetActive(true);
                //print(go.name+" is active at start");
                currentlyActiveSafeCount++;
            }
            else {
                safe.SetActive(false);
            }

            // get random colorgroup (should be improved at some point)
            Color color = generationOptions.colorGroups[UnityEngine.Random.Range(0, generationOptions.colorGroups.Length)];
            safe.SetDisplayColor(color);

			// set 1 safe to activate after solved challenge (should be improved at some point)
            safe.SetNumberOfSafesToActivate(1);

            safes.Add(safe);
            i++;
        }

        if (generationOptions.activateRandomly)
        {
            //make sure we do actually have the specified amount of safes active
            while (currentlyActiveSafeCount < generationOptions.safesActiveAtStart)
            {
                int rI = UnityEngine.Random.Range(0, safes.Count);
                if (!safes[rI].IsActive)
                {
                    safes[rI].SetActive(true);
                    //print(safes[rI].gameObject.name + " is active at start (by force)");
                    currentlyActiveSafeCount++;
                }
            }
        }
        //print("currently active safes count: " + currentlyActiveSafeCount);
    }

    private IEnumerable<Type> EnumerateRandomChallenges()
    {
        List<Type> remainingChallenges = generationOptions.availableChallenges.ToList();

        while(true)
        {
            // when all challenges have been consumed, refill the list with all of them again
            if (remainingChallenges.Count == 0)
                remainingChallenges.AddRange(generationOptions.availableChallenges);

            // pick a random challenge from the list and return it
            int index = UnityEngine.Random.Range(0, remainingChallenges.Count);
            Type element = remainingChallenges[index];
            remainingChallenges.RemoveAt(index);
            yield return element;
        }
    }

    // activates [amount] new safes of the same color as [color]
    public void ActivateNewSafes(Color color, int amount)
    {
        if(generationOptions.activateRandomly)
        {
            List<Safe> candidates = safes.Where(s => !s.IsActive && !s.IsOpen && s.GetDisplayColor() == color).ToList();
            // activate [amount] random safes, but not more than still available
            int count = Mathf.Min(amount, candidates.Count);
            for(int i=0; i<count; i++)
            {
                int index = UnityEngine.Random.Range(0, candidates.Count);
                candidates[index].SetActive(true);
                print(candidates[index].gameObject.name + " was activated!");
                candidates.RemoveAt(index);
            }
        }
        else
        {
            // take the first (up to) [amount] inactive safes of that color and activate them
            var toActivate = safes.Where(s => !s.IsActive && !s.IsOpen && s.GetDisplayColor() == color).Take(amount);
            foreach (Safe safe in toActivate)
            {
                safe.SetActive(true);
                print(safe.gameObject.name + " was activated!");
            }
        }
    }

    public void RecordFail(string cause, bool wasTimer)
    {
        currentLifeCount--;
        fails.Add(cause + (wasTimer ? " (timer)" : ""));
    }

    public void EndGame(bool won) {
        // deactivate all safes
        foreach (Safe s in safes)
        {
            if (s.IsActive) s.SetActive(false);
        }

        endGUICanvas.SetActive(true);
        endOfGame = true;
        if (won)
        {
            endGUIPlayer1[0].text = "You WIN!";
            endGUIPlayer2[0].text = "You WIN!";
        }
        else {
            endGUIPlayer1[0].text = "You LOSE!";
            endGUIPlayer2[0].text = "You LOSE!";
        }
        endGUIPlayer1[1].text = safes.Count(s => s.IsOpen) + "/" + safes.Count;
        endGUIPlayer2[1].text = safes.Count(s => s.IsOpen) + "/" + safes.Count;

        endGUIPlayer1[2].text = string.Join("\n", fails.ToArray());
        endGUIPlayer2[2].text = string.Join("\n", fails.ToArray());

        string minSec = string.Format("{0}:{1:00}", (int)gameTime / 60, (int)gameTime % 60);
        endGUIPlayer1[3].text = minSec;
        endGUIPlayer2[3].text = minSec;
    }

}
