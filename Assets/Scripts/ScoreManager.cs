using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{

    public static ScoreManager sharedInstance { get; set; }

    public Canvas currentCanvas;

    public int current_score = 0;

    public  TextMeshProUGUI SCORE_TEXT;
    public  TextMeshProUGUI UPDATE_TEXT_PREFAB;

    private TextMeshProUGUI temp_UpdateText;

    private bool UpdatingScore      = false;
    private bool UpdatingAnimation  = false;

    private void Awake()
    {
        sharedInstance = this;
    }

    void Start()
    {
        SCORE_TEXT.text = current_score.ToString();
    }

    private void Update()
    {
        if (UpdatingAnimation && temp_UpdateText != null)
        {
            temp_UpdateText.transform.position += new Vector3(0, 700 * Time.deltaTime, 0);
            if(temp_UpdateText.transform.position.y > 2000f)
            {
                Destroy(temp_UpdateText.gameObject);
                temp_UpdateText = null;
            }
        }
    }

    public void OnRowClear()
    {
        if (!UpdatingScore)
        {
            int clear_row_score = 10;

            current_score += clear_row_score;
            SCORE_TEXT.text = current_score.ToString();

            AnimateUpdateScore(clear_row_score);

            UpdatingScore = true;
        }
    }

    public void OnColumnClear()
    {
        if (!UpdatingScore)
        {
            int clear_column_score = 30;

            current_score += clear_column_score;
            SCORE_TEXT.text = current_score.ToString();

            AnimateUpdateScore(clear_column_score);

            UpdatingScore = true;
        }
    }

    public void OnFinishUpdateScore()
    {
        UpdatingScore           = false;
        UpdatingAnimation       = false;
    }

    private void AnimateUpdateScore(int _score)
    {
        UpdatingAnimation = true;

        if (temp_UpdateText != null)
            Destroy(temp_UpdateText.gameObject);

        UPDATE_TEXT_PREFAB.text = "+" + _score.ToString();
        temp_UpdateText = Instantiate(UPDATE_TEXT_PREFAB, SCORE_TEXT.transform.position, Quaternion.identity, currentCanvas.transform);
    }
}
