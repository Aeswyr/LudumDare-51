using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class HUDHandler : Singleton<HUDHandler>
{
    [Header("HUD")]
    [SerializeField] private Transform energyHolder;
    [SerializeField] private GameObject energyPrefab;
    [SerializeField] private TextMeshProUGUI countDown;
    [Header("TextBox")]
    [SerializeField] private GameObject textBox;
    [SerializeField] private TextMeshProUGUI convoText;
    [SerializeField] private TextMeshProUGUI convoName;
    [SerializeField] private Image portrait;

    private List<Statement> currentConversation;
    private UnityAction conversationCallback;
    bool typing = false;
    int index = 0;

    private List<GameObject> energy = new List<GameObject>();

    public void AddEnergy() {
        energy.Add(Instantiate(energyPrefab, energyHolder));
    }

    public void RemoveEnergy() {
        if (energy.Count < 1)
            return;
        Destroy(energy[0]);
        energy.RemoveAt(0);
    }

    public void UpdateCountDown(int count) {
        countDown.text = count.ToString();
    }

    public void StartConversation(Conversation conversation, UnityAction callback = null) {
        textBox.SetActive(true);
        convoText.text = "";
        this.currentConversation = conversation.GetConversation();
        this.conversationCallback = callback;
        typing = true;
        index = 0;

        GameHandler.Instance.GetPlayer().GetComponent<PlayerHandler>().DisableInputs();
    }

    void FixedUpdate() {
        if (currentConversation != null)
            TypeWriter();
    }

    private void TypeWriter() {
        if (typing) {
            convoText.text = currentConversation[0].text.Substring(0, index + 1);

            index++;
            if (index == currentConversation[0].text.Length)
                typing = false;
        }

        if (InputHandler.Instance.any.pressed) {
            if (typing) {
                typing = false;
                index = currentConversation[0].text.Length;

                convoText.text = currentConversation[0].text;

            } else {
                AudioHandler.Instance.Play(AudioType.SELECT);
                index = 0;
                currentConversation.RemoveAt(0);
                if (currentConversation.Count > 0) {
                    typing = true;
                    index = 0;
                } else {
                    EndConversation();
                }
            }
        }
    }

    public void EndConversation() {
        textBox.SetActive(false);
        conversationCallback?.Invoke();

        this.currentConversation = null;
        this.conversationCallback = null;
        typing = false;

        GameHandler.Instance.GetPlayer().GetComponent<PlayerHandler>().EnableInputs();
    }
}
