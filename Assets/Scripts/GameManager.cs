using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public GameObject[] deckPrefab;
    public Vector3 deckLocation;
    public GameObject CardPosLeft;
    public GameObject CardPosRight;
    public GameObject LowButton;
    public GameObject HighButton;
    public float buttonPressDelayAfterDealing = 1.5f;
    public float gameStartDelay = 2.0f;
    

    private List<GameObject> currentDeck;
    private GameObject card1;
    private GameObject card2;

    private bool aceOfSpadesDrawn = false;
    private bool heartsAvailable = true;
    public bool canGuess = false;

    public AudioClip winSound;
    public AudioClip loseSound;
    public AudioClip dealSound;
    public AudioClip gameOverSound;

    private void Awake()
    {
        NewGame();
    }

    // Start is called before the first frame update
    void Start()
    { 
        StartCoroutine(CustomDelay(gameStartDelay));
        DealHand();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /* Check first if the deck has cards in it, then go through and 
     * initialize all prefab cards in the deck */
    private void NewGame()
    {
        if(currentDeck != null)
        {
            currentDeck.Clear();
            for (int i = 0; i < deckPrefab.Length; i++)
            {
                GameObject tempCard = Instantiate(deckPrefab[Random.Range(0, deckPrefab.Length)], deckLocation, Quaternion.identity);
                currentDeck.Add(tempCard);
            }
            /*foreach (GameObject card in deckPrefab)
            {
                GameObject tempCard = Instantiate(card, deckLocation, Quaternion.identity);
                currentDeck.Add(tempCard);
                
            }*/

        }
        else
        {
            currentDeck = new List<GameObject>();
            for (int i = 0; i < deckPrefab.Length; i++)
            {
                GameObject tempCard = Instantiate(deckPrefab[Random.Range(0, deckPrefab.Length)], deckLocation, Quaternion.identity);
                currentDeck.Add(tempCard);
            }

            /*foreach (GameObject card in deckPrefab)
            {
                GameObject tempCard = Instantiate(card, deckLocation, Quaternion.identity);
                currentDeck.Add(tempCard);
                //Debug.Log("Added " + tempCard.GetComponent<Card>().value + " of " + tempCard.GetComponent<Card>().suit);
            }*/
        }

        heartsAvailable = currentDeck.Exists(c => c.GetComponent<Card>().suit == "Hearts");
        aceOfSpadesDrawn = !currentDeck.Exists(c => c.GetComponent<Card>().suit == "Spades" && c.GetComponent<Card>().value == 1);
    }

    /*Assign 2 cards returned from the draw method, set the face value of 
     * the first card's sprite renderer and animate the cards'movement
     * to the desitination poisition*/

    private void DealHand()
    {
        card1 = DrawCard();
        card2 = DrawCard();
        ScoreManager.instance.UpdateCardFrequency(card1);
        ScoreManager.instance.UpdateCardFrequency(card2);
        card1.GetComponent<Card>().FlipCard();
        // Animate card1 moving to the left position
        
        iTween.MoveTo(card1, iTween.Hash(
            "position", CardPosLeft.transform.position,
            "time", 1.0f, // Time in seconds to complete the move
            "easetype", iTween.EaseType.easeInOutExpo
        ));
        //card1.transform.position = CardPosLeft.transform.position;
        //card2.transform.position = CardPosRight.transform.position;
        // Animate card2 moving to the right position
        
        iTween.MoveTo(card2, iTween.Hash(
        "position", CardPosRight.transform.position,
        "time", 1.0f,
        "delay", 0.5f,  // Delay before this animation starts
        "easetype", iTween.EaseType.easeInOutExpo
        ));
        StartCoroutine(CardSoundDelay(.5f));

        StartCoroutine(PressDelay());
    }

 
    /* Useful delay function to make the game flow better*/
    private IEnumerator CustomDelay(float value)
    {
        yield return new WaitForSeconds(value);
    }

    /*Specific delay function that changes the canGuess flag to true.
     * This is to insure all animations have played, and both card are in
     * poision before the user can click the button*/
    private IEnumerator PressDelay()
    {
        yield return new WaitForSeconds(buttonPressDelayAfterDealing);
        canGuess = true;
    }

    /*Aligns the sounds more appropriately with the animation of the
     * card movement*/
    private IEnumerator CardSoundDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        AudioSource.PlayClipAtPoint(dealSound, new Vector3(0, 0, 0), 20f);
        yield return new WaitForSeconds(delay);
        AudioSource.PlayClipAtPoint(dealSound, new Vector3(0, 0, 0), 20f);
    }

    /*Function that returns a card with the specified probability of the ace
     * of spades being 3 times as likely to be drawn and a heart card being
     * twice as likely to be drawn. Flags are checked and set appropriately
     * so that the probability stays the same in edge cases like the ace of spades
     * being the last card drawn, or there being only hearts left in the deck. The
     * max range is changed before each drawn according to the remaining cards in the
     * deck */
    public GameObject DrawCard()
    {
        if (currentDeck.Count == 0) return null;

        bool normalCardsAvailable = currentDeck.Any(c => c.GetComponent<Card>().suit != "Hearts" && !(c.GetComponent<Card>().value == 1 && c.GetComponent<Card>().suit == "Spades"));
        int maxRange;

        if (aceOfSpadesDrawn)
        {
            maxRange = heartsAvailable? (normalCardsAvailable? 30 : 20) : (normalCardsAvailable? 10 : 0);
        }
        else
        {
            maxRange = heartsAvailable ? (normalCardsAvailable ? 60 : 90) : (normalCardsAvailable ? 40 : 1);
        }

        int randomNumber = Random.Range(1, maxRange + 1);
        List<GameObject> selectionPool;

        if (randomNumber <= 10 && normalCardsAvailable)
        {
            selectionPool = currentDeck.Where(c => c.GetComponent<Card>().suit != "Hearts" && !(c.GetComponent<Card>().value == 1 && c.GetComponent<Card>().suit == "Spades")).ToList();
        }
        else if (randomNumber <= 30 && heartsAvailable)
        {
            selectionPool = currentDeck.Where(c => c.GetComponent<Card>().suit == "Hearts").ToList();
        }
        else if (randomNumber <= maxRange && !aceOfSpadesDrawn)
        {
            selectionPool = currentDeck.Where(c => c.GetComponent<Card>().value == 1 && c.GetComponent<Card>().suit == "Spades").ToList();
        }
        else
        {
            Debug.Log("failsafe hit");
            selectionPool = new List<GameObject>(currentDeck); // falesafe incase ranges exceed current conditions
        }

        if (selectionPool.Count > 0)
        {
            int index = Random.Range(0, selectionPool.Count);
            GameObject selectedCard = selectionPool[index];
            currentDeck.Remove(selectedCard);

            // Update flags based on the drawn card
            if (selectedCard.GetComponent<Card>().suit == "Spades" && selectedCard.GetComponent<Card>().value == 1)
            {
                //updated to allow for duplicate values of  the ace of spades in a deck
                aceOfSpadesDrawn = !currentDeck.Exists(c => c.GetComponent<Card>().suit == "Spades" && c.GetComponent<Card>().value == 1);
            }

            if (selectedCard.GetComponent<Card>().suit == "Hearts")
            {
                heartsAvailable = currentDeck.Exists(c => c.GetComponent<Card>().suit == "Hearts");
            }

            //Debug.Log("Added " + selectedCard.GetComponent<Card>().value + " of " + selectedCard.GetComponent<Card>().suit);
            return selectedCard;
        }

        return null; // Falesafe if there was no suitable pool is found
    }

    public void GuessHigh()
    {
        //Make sure to check to see if player is able to guess (left card is flipped and animations
        //are complete before submitting guess)
        if(canGuess)
        {
            canGuess = false;
            //Debug.Log("Guess High Button Pressed!");
            bool result = CompareCards(card1, card2, "higher");
            card2.GetComponent<Card>().AnimateFlipCard();
            StartCoroutine(ProcessTurn(result));
        }
    }

    public void GuessLow()
    {
        if (canGuess)
        {
            canGuess = false;
            //Debug.Log("Guess Low Button Pressed!");
            bool result = CompareCards(card1, card2, "lower");
            card2.GetComponent<Card>().AnimateFlipCard();
            StartCoroutine(ProcessTurn(result));
        }
    }

    void ProcessResult(bool win)
    {
        if (win)
        {
            ScoreManager.instance.AddScore(card1.GetComponent<Card>().value, card2.GetComponent<Card>().value);
            ScoreManager.instance.UpdateStreakCounter(1);
            AudioSource.PlayClipAtPoint(winSound, new Vector3(0, 0, 0), 0.5f);
        }  
        else
        {
            ScoreManager.instance.UpdateStreakCounter(0);
            AudioSource.PlayClipAtPoint(loseSound, new Vector3(0, 0, 0), 0.5f);
        }
    }

    private bool CompareCards(GameObject first, GameObject second, string guess)
    {
        if (first.GetComponent<Card>().value == second.GetComponent<Card>().value) // If values are the same, check the suits
        {
            //maybe output a TIE UI screen here and then show which one wins
            //Debug.Log("TIE, Calling CompareSuits Function!");
            return CompareSuits(first, second, guess);
        }
        else
        {
            if (guess == "higher")
            {
                //Debug.Log("Guessed higher and " + second.GetComponent<Card>().value + " of " + second.GetComponent<Card>().suit + " is greater than other card?: " + (second.GetComponent<Card>().value > first.GetComponent<Card>().value));
                return second.GetComponent<Card>().value > first.GetComponent<Card>().value;
            }
            else
            {
                return second.GetComponent<Card>().value < first.GetComponent<Card>().value;
            }
        }
    }

    private bool CompareSuits(GameObject first, GameObject second, string guess)
    {
        //Spades > Hearts > Diamonds > Clubs
        int firstSuitValue = GetSuitValue(first.GetComponent<Card>().suit);
        int secondSuitValue = GetSuitValue(second.GetComponent<Card>().suit);

        //Now that there are duplicate cards, we give the player a free win if its a true tie
        
        if(firstSuitValue == secondSuitValue)
        {
            return true;
        }

        if (guess == "higher")
        {
            return secondSuitValue > firstSuitValue;
        }
        else
        {
            return secondSuitValue < firstSuitValue;
        }
    }

    private int GetSuitValue(string suit)
    {
        switch (suit)
        {
            case "Spades": return 4;
            case "Hearts": return 3;
            case "Diamonds": return 2;
            case "Clubs": return 1;
            default: return 0;
        }
    }

    private IEnumerator ProcessTurn(bool result)
    {
        yield return new WaitForSeconds(1);
        ProcessResult(result);
        yield return new WaitForSeconds(2);
        Destroy(card1);
        Destroy(card2);

        if (currentDeck.Count > 0)
        {
            DealHand();
        }
        else
        {
            LowButton.SetActive(false);
            HighButton.SetActive(false);
            AudioSource.PlayClipAtPoint(gameOverSound, new Vector3(0, 0, 0), 0.5f);
            ScoreManager.instance.DisplayEndGameScore();    
        }
    }
}

