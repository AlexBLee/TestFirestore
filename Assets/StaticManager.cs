using System;
using System.Collections.Generic;
using Firebase.Firestore;
using UnityEngine;

public static class FirestoreManager
{
    public enum State
    {
        Idle,
        Initting,
        Inited,
        Error,
    }

    static FirebaseFirestore database;
    static ListenerRegistration listener;

    private static State _state = State.Idle;
    public static bool IsInit { get { return _state == State.Inited; } }


    public static void InitializeDB()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                database = FirebaseFirestore.DefaultInstance;
                Debug.Log("Firestore has initialized succesfully!");
                _state = State.Inited;

                Query query = database.Collection("newspaper");
                GetDocuments(query, () => _state = State.Inited);
            }
            else
            {
                Debug.LogErrorFormat("Could not resolve all Firebase dependencies: {0}", dependencyStatus);
                _state = State.Error;
            }
        });
    }

    public static void AddDocument(string strainName, Dictionary<string, object> strain)
    {
        if (_state != State.Inited)
        {
            return;
        }

        string collectionName = "newspaperDev";

        DocumentReference docRef = database.Collection(collectionName).Document(strainName);
        docRef.SetAsync(strain).ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                _state = State.Error;
                Debug.LogError(task.Exception.Message);
            }
            else if (task.IsCompleted)
            {
                Debug.LogFormat("Added new entry {0}", strainName);
            }
        });
    }

    public static void GetDocuments(Query query, Action onItemChanged)
    {
        if (listener != null)
        {
            listener.Stop();
        }

        listener = query.Listen(snapshot =>
        {
            Debug.Log("change");
            foreach (DocumentChange change in snapshot.GetChanges())
            {
                if (change.ChangeType == DocumentChange.Type.Added)
                {
                    Debug.Log(String.Format(change.Document.Id));
                }
                else if (change.ChangeType == DocumentChange.Type.Modified)
                {
                    Debug.Log(change.Document.Metadata);
                    Debug.Log(String.Format("Modified city: {0}", change.Document.Id));
                }
                else if (change.ChangeType == DocumentChange.Type.Removed)
                {
                    Debug.Log(String.Format("Removed city: {0}", change.Document.Id));
                }
            }
        });
    }

    public static void DeleteDocument(string docName)
    {
        string collectionName = "newspaperDev";

        DocumentReference docRef = database.Collection(collectionName).Document(docName);

        listener.Stop();
        docRef.DeleteAsync();

        Debug.LogFormat("Removed entry: {0}", docName);

    }
}
