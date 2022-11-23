using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seeker : MonoBehaviour
{
    public int Attempts { get; private set; }

    [SerializeField] private int MaxAttempts = 5;
    
    void Start()
    {
        Attempts = MaxAttempts;
    }
}
