using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Conversation", menuName = "LudumDare-51/Conversation", order = 0)]
public class Conversation : ScriptableObject {
    [SerializeField] private List<Statement> conversation;
    public List<Statement> GetConversation() {
        return new List<Statement>(conversation);
    }
}

[Serializable] public struct Statement {
    public string text;
    public string name;
    public Sprite portrait;
}
