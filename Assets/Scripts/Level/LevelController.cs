using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [SerializeField] private Level level;
    [SerializeField] private Transform objectHolder;

    List<SpawnObject> spawns;
    private int blockIndex = 0;

    bool levelStarted;
    float blockStartTime;

    int score, baseScore;



    void Start() {
        GameHandler.Instance.GetPlayer().GetComponent<PlayerHandler>().DisableInputs();
        StartCoroutine(StartupSequence());
    }

    private IEnumerator StartupSequence() {
        VFXHandler.Instance.ScreenBlackout(true);
        VFXHandler.Instance.FadeIn();
        yield return new WaitForSeconds(1.5f);
        StartSequence();
    }

    void FixedUpdate() {
        if (levelStarted) {
            HUDHandler.Instance.UpdateCountDown(Mathf.CeilToInt(10f - (Time.time - blockStartTime)));

            if (Time.time - blockStartTime > 10 && blockIndex < level.phases.Count - 1) {
                AudioHandler.Instance.Play(AudioType.EMP);
                EndBlock();
                EndSequence();
                return;
            }

            while (spawns.Count > 0 && spawns[0].time <= Time.time - blockStartTime) {
                Instantiate(spawns[0].obj, objectHolder).transform.position = spawns[0].position;
                VFXHandler.Instance.ParticleBuilder(ParticleType.VFX_SPAWN, spawns[0].position, true);
                AudioHandler.Instance.Play(AudioType.TELEPORT);

                spawns.RemoveAt(0);
            }
        }
    }

    public void StartSequence() {
        var convo = level.phases[blockIndex].conversation;
        if (convo != null) {
            HUDHandler.Instance.StartConversation(convo, StartBlock);
        } else {
            StartBlock();
        }
    }

    public void StartBlock() {
        GameHandler.Instance.GetPlayer().GetComponent<PlayerHandler>().RefillEnergy();
        levelStarted = true;
        blockStartTime = Time.time;

        spawns = level.phases[blockIndex].GetSpawns();
    }

    public void EndBlock() {
        levelStarted = false;
        foreach (Transform child in objectHolder) {
            if (child.TryGetComponent(out CombatController combatController))
                combatController.OnHit(default, default);
            else
                Destroy(child.gameObject);
        }
        foreach (HitboxController obj in FindObjectsOfType<HitboxController>()) {
            Destroy(obj.gameObject);
        }
    }

    public void EndSequence() {
        AddScore(200 * GameHandler.Instance.GetPlayer().GetComponent<PlayerHandler>().GetEnergy());
        baseScore = score;
        HUDHandler.Instance.SetScore(score);
        blockIndex++;

        if (blockIndex < level.phases.Count) {
            StartSequence();
        } else {
            
        }
    }

    public void StopBlock() {
        levelStarted = false;
    }

    public void AddScore(int amt) {
        score += amt;
        HUDHandler.Instance.SetScore(score);
    }

    public void ResetScore() {
        score = baseScore;
        HUDHandler.Instance.SetScore(score);
    }

    public void ResetAndSubtractScore(int amt) {
        score -= amt;
        if (score < 0)
            score = 0;
        baseScore = Mathf.Min(baseScore, score);
        score = baseScore;
        HUDHandler.Instance.SetScore(score);
    }
}
