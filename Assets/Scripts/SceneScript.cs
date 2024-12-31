using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneScript : MonoBehaviour
{
    [SerializeField] public Sprite[] catSprites;
    [SerializeField] public GameObject catSkinObject;
    [SerializeField] private GameObject lockUI;
    [SerializeField] private GameObject priceUI;
    [SerializeField] private GameObject redHighlight;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI buyOrSelectText; 

    [SerializeField] private Animator animator;

    [SerializeField] private GameObject selectedIndicatorUI;

    [SerializeField] private int priceOfSkin = 200;

    public CurrentData currentData;

    public SpriteRenderer spriteRenderer;

    private int selectedSkin = 0; // default white cat

    [SerializeField] private GameObject mainMenu;

    public void Start()
    {
        mainMenu.SetActive(true);
        currentData.LoadData();

        animator.SetInteger("selectedSkin", selectedSkin);

        selectedSkin = selectedSkin!=0 ? currentData.currentCat : 0;
    }

    //to be called on button click to view skins
    public void NextSkin()
    {
        selectedSkin++;
        if(selectedSkin == catSprites.Length)
        {
            selectedSkin = 0;
        }  

        spriteRenderer.sprite = catSprites[selectedSkin];

        UpdateSkinViewUI();
    }

    public void PreviousSkin()
    {
        selectedSkin--;
    
        if(selectedSkin < 0)
        {
            selectedSkin = catSprites.Length - 1;
        }  

        spriteRenderer.sprite = catSprites[selectedSkin];

        UpdateSkinViewUI();
    }

    //check whether user is viewing an unlocked skin
    private bool SkinIsUnlocked()
    {
        if(currentData.unlockedCats.Contains(selectedSkin) || selectedSkin == 0)
        {
            return true;
        }
        else{return false;}
    }

    //check whether user is viewing a selected skin
    public bool SkinIsSelected()
    {
        if(currentData.currentCat == selectedSkin)
        {
            return true;
        }
        else{return false;}
    }

    //to update the UI when viewing skin (for lock/selected)
    public void UpdateSkinViewUI()
    {
        animator.SetInteger("selectedSkin", selectedSkin);

        if(SkinIsUnlocked())
        {
            lockUI.SetActive(false);
            priceUI.SetActive(false);
            buyOrSelectText.text = "Select";
        }

        else
        {
            lockUI.SetActive(true);
            priceUI.SetActive(true);
            buyOrSelectText.text = "Buy";
        }

        if(SkinIsSelected())
        {
            selectedIndicatorUI.SetActive(true);
        }

        else
        {
            selectedIndicatorUI.SetActive(false);
        }
    }

    //to be called on button click (buy/select button)
    public void ConfirmSelection()
    {
        if(SkinIsUnlocked())
        {
            currentData.currentCat = selectedSkin;

            Debug.Log("Skin Selected");

            UpdateSkinViewUI();
            currentData.SaveData();
        }

        else
        {
            Debug.Log("Skin is locked.");
            BuySkin();

            UpdateSkinViewUI();
        }
    }

    //buy skin and updates total score that the user has left
    public void BuySkin()
    {
        if(currentData.totalScore >= priceOfSkin)
        {
            Debug.Log("Skin bought.");

            UpdateTotalScore(priceOfSkin);
            currentData.unlockedCats.Add(selectedSkin);

            UpdateSkinViewUI();

            currentData.SaveData();
        }
        
        else
        {
            StartCoroutine(ShowInsufficientPoints());
        }
    }

    //temporarily highlights the price in red
    private IEnumerator ShowInsufficientPoints()
    {
        redHighlight.SetActive(true);
        yield return new WaitForSeconds(0.8f);
        redHighlight.SetActive(false);
    }

    //updates the total score data. SAVES.
    public void UpdateTotalScore(int price)
    {
        currentData.totalScore -= price;
        currentData.SaveData();
        ShowScore();
    }

    //just shows the total score the user has in the character select screen
    public void ShowScore()
    {
        scoreText.text = currentData.totalScore.ToString();
    }



    // These are for Play/Quit Buttons
    public void PlayButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }

    public void QuitButton()
    {
        Application.Quit();
    }

}
