using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DamageNumbersPro;

public class ScoreManager : MonoBehaviour
{

    //During Game
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;

    //Game Over Screen
    public TextMeshProUGUI scoreGO;
    public TextMeshProUGUI accuracyGO;
    public TextMeshProUGUI LongestStreakGO;


    public Sprite[] streak;
    public GameObject streakObject;
    public GameObject gameOverScorePanel;

    public GameObject textPrefab;
    public Transform contentPanel;

    private SpriteRenderer streakRenderer;
    private int score = 0;
    private int highScore = 0;
    private int currentStreak;
    private int longestStreak;
    private int correctCounter = 0;

    //Score Numbers
    public DamageNumber numberPrefabRight;
    public DamageNumber numberPrefabLeft;
    public DamageNumber numberPrefabStreak;
    public float BonusStreakDelay = 0.5f;

    public static ScoreManager instance;

    private Dictionary<string, int> cardFrequencies = new Dictionary<string, int>();
    private Dictionary<string, string> cardNames = new Dictionary<string, string>
    {
        {"1 of Hearts", "Ace of Hearts"},
        { "1 of Diamonds", "Ace of Diamonds" },
        { "1 of Clubs","Ace of Clubs" },
        { "1 of Spades","Ace of Spades" },

        { "11 of Hearts","Jack of Hearts" },
        { "11 of Diamonds", "Jack of Diamonds" },
        { "11 of Clubs", "Jack of Clubs" },
        { "11 of Spades","Jack of Spades" },

        { "12 of Hearts","Queen of Hearts" },
        { "12 of Diamonds","Queen of Diamonds" },
        { "12 of Clubs","Queen of Clubs" },
        { "12 of Spades","Queen of Spades" },

        { "13 of Hearts","King of Hearts" },
        { "13 of Diamonds","King of Diamonds" },
        {"13 of Clubs", "King of Clubs"},
        { "13 of Spades", "King of Spades" }
    };


    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        highScore = PlayerPrefs.GetInt("highscore", 0);
        scoreText.text = score.ToString();
        highScoreText.text = highScore.ToString();
        streakRenderer = streakObject.GetComponent<SpriteRenderer>();
        UpdateStreakCounter(0);
        cardFrequencies.Clear();
    }

    public void AddScore(int card1Value, int card2Value)
    {
        score += (card1Value + card2Value);
        DamageNumber scoreAmount = numberPrefabRight.Spawn(numberPrefabRight.transform.position, (float)card2Value);
        DamageNumber scoreAmount1 = numberPrefabLeft.Spawn(numberPrefabLeft.transform.position, (float)card1Value);
        scoreText.text = score.ToString();

        if (highScore <= score)
        {
            highScore = score;
            PlayerPrefs.SetInt("highscore", score);
            highScoreText.text = highScore.ToString();
        }
    }

    public void UpdateStreakCounter(int value)
    {
        if (value == 1)
        {
            //Needed to avoid giving points on a zero streak
            int bonusAmount = currentStreak > 3 ? 3 : currentStreak;
            if (bonusAmount > 0)
            {
                StartCoroutine(StreakBonusDelay(BonusStreakDelay, bonusAmount));
            }
            streakRenderer.sprite = streak[bonusAmount];
            correctCounter++;
            currentStreak++;
            longestStreak = Mathf.Max(longestStreak, currentStreak); 
        }
        else
        {
            currentStreak = 0;
            streakRenderer.sprite = streak[0];
        }
    }

    public void DisplayEndGameScore()
    {

        scoreGO.text = score.ToString();
        LongestStreakGO.text = longestStreak.ToString();
        accuracyGO.text = ((double)correctCounter / 26.0).ToString("F2") + "%";

        gameOverScorePanel.SetActive(true);

        // Clear previous entries to avoid duplicate listings
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }

        foreach (KeyValuePair<string, int> card in cardFrequencies)
        {
            GameObject textGO = Instantiate(textPrefab, contentPanel.transform);
            textGO.GetComponent<TextMeshProUGUI>().text = $"{GetNameOfCard(card.Key)} appeared {card.Value} {TimeOrTimes(card.Value)}";
        }
    }

    private string TimeOrTimes(int amount)
    {
        string result = amount > 1 ? "times" : "time";
        return result;
    }

    private string GetNameOfCard(string card)
    {
        if(cardNames.TryGetValue(card, out string result))
        {
            return result;
        }

        return card;
    }

    public void UpdateCardFrequency(GameObject card1)
    {
        Card tempcard1 = card1.GetComponent<Card>();
    
        if(cardFrequencies.TryGetValue($"{tempcard1.value} of {tempcard1.suit}", out int timesAppeared))
        {
            cardFrequencies[$"{tempcard1.value} of {tempcard1.suit}"] = timesAppeared + 1;
            //Debug.Log($"card found: {tempcard1.value} of {tempcard1.suit} : " + cardFrequencies[$"{ tempcard1.value} of { tempcard1.suit}"]);
        }
        else
        {
            cardFrequencies[$"{tempcard1.value} of {tempcard1.suit}"] = 1;
            //Debug.Log($"no card found, adding {tempcard1.value} of {tempcard1.suit}");
        }
    }

    IEnumerator StreakBonusDelay(float Delay, int number)
    {
        yield return new WaitForSeconds(Delay);

        DamageNumber bonusAmount = numberPrefabStreak.Spawn(numberPrefabStreak.transform.position, (float)number);
        score += number;
        scoreText.text = score.ToString();
        if (highScore <= score)
        {
            highScore = score;
            PlayerPrefs.SetInt("highscore", score);
            highScoreText.text = highScore.ToString();
        }
    }
}
