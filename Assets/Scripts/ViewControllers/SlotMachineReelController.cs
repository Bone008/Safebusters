using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class SlotMachineReelController : MonoBehaviour
{

    private Text[] letterDisplays;
    private char[] letters;

    private Quaternion initialRotation;
    private float currentAngle = 0;
    private float rotationSpeed = 0;

    void Awake()
    {
        letterDisplays = GetComponentsInChildren<Text>();
        letters = new char[letterDisplays.Length];
    }

    void Start()
    {
        initialRotation = transform.localRotation;
    }

    void Update()
    {
        currentAngle = (currentAngle + rotationSpeed * Time.deltaTime + 360.0f) % 360.0f;
        transform.localRotation = Quaternion.Euler(-currentAngle, 0, 0) * initialRotation;
    }

    public int GetSlotCount()
    {
        return letterDisplays.Length;
    }

    public void SetRotationSpeed(float rotationSpeed)
    {
        this.rotationSpeed = rotationSpeed;
    }

    public void SetLetters(char[] letters)
    {
        if (letters.Length != letterDisplays.Length)
            throw new ArgumentException("letters do not fit the display");

        this.letters = letters;
        for (int i = 0; i < letterDisplays.Length; i++)
            letterDisplays[i].text = "" + letters[i];
    }

    public int GetActiveLetterIndex()
    {
        float anglePerSlot = 360.0f / letters.Length;
        return Mathf.RoundToInt(currentAngle / anglePerSlot) % letters.Length;
    }
    public char GetActiveLetter()
    {
        return letters[GetActiveLetterIndex()];
    }

    public void SetActiveLetterIndex(int index)
    {
        float anglePerSlot = 360.0f / letters.Length;
        currentAngle = index * anglePerSlot;
    }

}
