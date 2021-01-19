using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Extensions;
using Firebase.Firestore;
using UnityEngine;

public class HA : MonoBehaviour
{
    private void Start()
    {
        FirestoreManager.InitializeDB();


    }
}
