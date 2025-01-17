using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleMobileAds.Api;

public class MenuBehaviour : MonoBehaviour
{
    [SerializeField] Animator[] mainMenuButtons;
    [SerializeField] Animator[] difficultyMenuButtons;
    [SerializeField] Animator[] campaignButtons;
    [SerializeField] Animator[] menuTitles;
    [SerializeField] Sprite[] lockedDifficultiesSprites;
    [SerializeField] Sprite[] unlockedDifficultiesSprites;
    [SerializeField] GameObject disabledMenu;
    [SerializeField] Animator disabledMenuAnimator;
    [SerializeField] Animator disabledDifficultyMenuAnimator;
    [SerializeField] Button disabledMenuButton;
    [SerializeField] Animator titleScreen;
    [SerializeField] Animator dialogSystem;
    [SerializeField] Animator mainMenuSettings;
    [SerializeField] Animator creditsScene;
    [SerializeField] Animator skipCredits;
    [SerializeField] Animator skipCreditsButton;
    private bool enableMenuButtons = false;
    private bool mainMenuHidden = false;
    private bool enterCampaignMenu = false;
    private bool enableDifficultyButtons = false;
    private bool disableDifficultyButtons = false;
    private bool enableCampaignButtons = false;
    private bool disableCampaignButtons = false;
    private bool enableDisabledScreenButton = false;
    private bool disableDisabledScreenButton = false;
    private bool disableTapScreen = false;
    private bool disableOptionsMenu = false;
    private int currentUnlockedDifficulty = 0;
    public bool skipLogo = false;
    private bool unlockLevel = false;
    private int levelToUnlock = 0;
    public bool resetDifficulty = false;
    private bool creditsDone = false;
    private bool skipCreditsDone = false;

    private void Awake()
    {
        /*if (resetDifficulty)
        {
            PlayerPrefs.SetInt("LevelToUnlock", 0);
        }*/

        unlockLevel = PlayerPrefs.GetInt("UnlockLevel") != 0;
        levelToUnlock = PlayerPrefs.GetInt("LevelToUnlock");
        skipLogo = PlayerPrefs.GetInt("SkipLogo") != 0;

        if (unlockLevel)
        {
            UnlockNextDifficulty();
            unlockLevel = false;
            PlayerPrefs.SetInt("UnlockLevel", (unlockLevel ? 1 : 0));
            if(levelToUnlock < 3)
            {
                levelToUnlock++;
            }
            Debug.LogError(levelToUnlock);
            PlayerPrefs.SetInt("LevelToUnlock", levelToUnlock);
        }
        else if (skipLogo)
        {
            SkipLogo();
        }
    }

    private void Start()
    {
        DifficultyButtonsCheck();
        StartCoroutine(LoadMainMenu());
    }

    private void InMenuBanner(AdPosition bannerPosition)
    {
        AdsManager.Instance.DestroyAd();
        AdsManager.Instance.bannerPosition = bannerPosition;
        AdsManager.Instance.ShowBanner();
    }

    private IEnumerator LoadMainMenu()
    {
        yield return new WaitUntil(() => disableTapScreen);
        titleScreen.gameObject.SetActive(false);
        StartCoroutine(EnterMainMenuButtons());
    }

    #region Menu Loading Times
    private void LoadCurrentMenuTimes()
    {
        //Main Title Checker
        AnimatorStateInfo mainTitleAnimatorStateInfo = titleScreen.GetCurrentAnimatorStateInfo(0);

        if (mainTitleAnimatorStateInfo.IsName("Hide"))
        {
            float MTTime = mainTitleAnimatorStateInfo.normalizedTime;
            if (MTTime > 1)
            {
                disableTapScreen = true;
            }
        }

        //Main Menu Checker
        AnimatorStateInfo mainMenuAnimatorStateInfo = mainMenuButtons[mainMenuButtons.Length - 1].GetCurrentAnimatorStateInfo(0);

        if (mainMenuAnimatorStateInfo.IsName("Enter"))
        {
            float NTime = mainMenuAnimatorStateInfo.normalizedTime;
            if(NTime > 1)
            {
                enableMenuButtons = true;
            }
        }

        if (mainMenuAnimatorStateInfo.IsName("Back"))
        {
            float NTime = mainMenuAnimatorStateInfo.normalizedTime;
            if (NTime > 1)
            {
                enableMenuButtons = true;
                Debug.LogError("Menu");
            }
        }

        if (mainMenuAnimatorStateInfo.IsName("Exit"))
        {
            float NTime = mainMenuAnimatorStateInfo.normalizedTime;
            if(NTime > 1)
            {
                mainMenuHidden = true;
            }
        }
        else
        {
            mainMenuHidden = false;
        }

        //Options Menu Checker
        AnimatorStateInfo mainMenuOptionsAnimatorStateInfo = mainMenuSettings.GetCurrentAnimatorStateInfo(0);

        if (mainMenuOptionsAnimatorStateInfo.IsName("Exit Settings"))
        {
            float OpTime = mainMenuOptionsAnimatorStateInfo.normalizedTime;
            if (OpTime > 1)
            {
                disableOptionsMenu = true;
            }
        }
        else
        {
            disableOptionsMenu = false;
        }

        //Difficulty Menu Checker
        AnimatorStateInfo difficultyMenuAnimatorStateInfo = difficultyMenuButtons[difficultyMenuButtons.Length - 1].GetCurrentAnimatorStateInfo(0);

        if (difficultyMenuAnimatorStateInfo.IsName("Enter"))
        {
            float DTime = difficultyMenuAnimatorStateInfo.normalizedTime;
            if(DTime > 1)
            {
                enableDifficultyButtons = true;
            }
        }

        if (difficultyMenuAnimatorStateInfo.IsName("Back"))
        {
            float DTime = difficultyMenuAnimatorStateInfo.normalizedTime;
            if (DTime > 1)
            {
                enableDifficultyButtons = true;
            }
        }

        if (difficultyMenuAnimatorStateInfo.IsName("Exit"))
        {
            float DTime = difficultyMenuAnimatorStateInfo.normalizedTime;
            if (DTime > 1.0f)
            {
                enterCampaignMenu = true;
            }
        }

        if (difficultyMenuAnimatorStateInfo.IsName("Hide"))
        {
            float DTime = difficultyMenuAnimatorStateInfo.normalizedTime;
            if(DTime > 1.0f)
            {
                disableDifficultyButtons = true;
            }
        }

        //Campaign Menu Checker
        AnimatorStateInfo campaignMenuAnimatorStateInfo = campaignButtons[campaignButtons.Length - 1].GetCurrentAnimatorStateInfo(0);

        if (campaignMenuAnimatorStateInfo.IsName("Enter"))
        {
            float CTime = campaignMenuAnimatorStateInfo.normalizedTime;
            if (CTime > 1.0f)
            {
                enableCampaignButtons = true;
            }
        }

        if (campaignMenuAnimatorStateInfo.IsName("Hide"))
        {
            float CTime = campaignMenuAnimatorStateInfo.normalizedTime;
            if (CTime > 1.0f)
            {
                disableCampaignButtons = true;
            }
        }

        //Disabled Menu Checker
        AnimatorStateInfo disabledMenuAnimatorStateInfo = disabledMenuAnimator.GetCurrentAnimatorStateInfo(0);

        if (disabledMenuAnimatorStateInfo.IsName("Disabled Screen On"))
        {
            float DSTime = disabledMenuAnimatorStateInfo.normalizedTime;
            if (DSTime > 1.0f)
            {
                enableDisabledScreenButton = true;
                disabledMenuButton.interactable = true;
            }
        }

        if (disabledMenuAnimatorStateInfo.IsName("Disabled Screen Off"))
        {
            float DSTime = disabledMenuAnimatorStateInfo.normalizedTime;
            if (DSTime > 1.0f)
            {
                enableDisabledScreenButton = false;
                disabledMenu.SetActive(false);
            }
        }

        //Disabled Difficulty Menu Checker
        AnimatorStateInfo disabledDifficultyMenuAnimatorStateInfo = disabledDifficultyMenuAnimator.GetCurrentAnimatorStateInfo(0);

        if (disabledDifficultyMenuAnimatorStateInfo.IsName("Disabled Screen On"))
        {
            float DDTime = disabledDifficultyMenuAnimatorStateInfo.normalizedTime;
            if (DDTime > 1.0f)
            {
                disabledMenuButton.interactable = true;
            }
        }

        if (disabledDifficultyMenuAnimatorStateInfo.IsName("Disabled Screen Off"))
        {
            float DDTime = disabledDifficultyMenuAnimatorStateInfo.normalizedTime;
            if (DDTime > 1.0f)
            {
                disabledMenu.SetActive(false);
            }
        }

        //Credits Checker
        AnimatorStateInfo creditsAnimatorStateInfo = creditsScene.GetCurrentAnimatorStateInfo(0);

        if(creditsAnimatorStateInfo.IsName("Show Credits"))
        {
            float CTime = creditsAnimatorStateInfo.normalizedTime;
            if(CTime > 1.0f)
            {
                creditsDone = true;
            }
        }
        else
        {
            creditsDone = false;
        }

        //Skip Credits Checker
        AnimatorStateInfo skipCreditsAnimatorStateInfo = skipCredits.GetCurrentAnimatorStateInfo(0);

        if (skipCreditsAnimatorStateInfo.IsName("Skip Credits"))
        {
            float SKTime = skipCreditsAnimatorStateInfo.normalizedTime;
            if (SKTime > 1.0f)
            {
                skipCreditsDone = true;
            }
        }
        else
        {
            skipCreditsDone = false;
        }
    }
    #endregion

    public void HideTitle()
    {
        titleScreen.SetTrigger("Hide");
    }

    private void Update()
    {
        LoadCurrentMenuTimes();
    }

    #region Main Menu
    private IEnumerator EnterMainMenuButtons()
    {
        InMenuBanner(AdPosition.Bottom);
        menuTitles[0].SetTrigger("TitleOn");

        for (int i = 0; i < mainMenuButtons.Length; i++)
        {
            mainMenuButtons[i].SetTrigger("Enter");
            yield return new WaitForSeconds(0.75f);
        }

        yield return new WaitUntil(() => enableMenuButtons);
        //mainMenuButtons[0].transform.parent.GetComponent<VerticalLayoutGroup>().enabled = true;
        foreach (Animator button in mainMenuButtons)
        {
            switch (button.name)
            {
                case "Campaign":
                    button.gameObject.GetComponent<Button>().interactable = true;
                    break;
                case "GOAT":
                    button.gameObject.GetComponent<Button>().interactable = true;
                    break;
                case "Hangar":
                    button.gameObject.GetComponent<Button>().interactable = true;
                    break;
                case "Tutorial":
                    button.gameObject.GetComponent<Button>().interactable = true;
                    break;
                case "Options":
                    button.gameObject.GetComponent<Button>().interactable = true;
                    break;
                case "Credits":
                    button.gameObject.GetComponent<Button>().interactable = true;
                    break;
            }
        }

        enableMenuButtons = false;
    }

    private IEnumerator BackMainMenuButtons()
    {
        InMenuBanner(AdPosition.Bottom);
        menuTitles[0].SetTrigger("TitleOn");

        for (int i = 0; i < mainMenuButtons.Length; i++)
        {
            mainMenuButtons[i].SetTrigger("Back");
            yield return new WaitForSeconds(0.75f);
        }

        yield return new WaitUntil(() => enableMenuButtons);
        //mainMenuButtons[0].transform.parent.GetComponent<VerticalLayoutGroup>().enabled = true;
        foreach (Animator button in mainMenuButtons)
        {
            switch (button.name)
            {
                case "Campaign":
                    button.gameObject.GetComponent<Button>().interactable = true;
                    break;
                case "GOAT":
                    button.gameObject.GetComponent<Button>().interactable = true;
                    break;
                case "Hangar":
                    button.gameObject.GetComponent<Button>().interactable = true;
                    break;
                case "Tutorial":
                    button.gameObject.GetComponent<Button>().interactable = true;
                    break;
                case "Options":
                    button.gameObject.GetComponent<Button>().interactable = true;
                    break;
                case "Credits":
                    button.gameObject.GetComponent<Button>().interactable = true;
                    break;
            }
        }
        enableMenuButtons = false;
    }

    private void BackToMainMenu()
    {
        StartCoroutine(BackMainMenuButtons());
    }

    private IEnumerator ExitMainMenuButtons()
    {
        //mainMenuButtons[0].transform.parent.GetComponent<VerticalLayoutGroup>().enabled = false;

        foreach (Animator button in mainMenuButtons)
        {
            button.gameObject.GetComponent<Button>().interactable = false;
        }

        menuTitles[0].SetTrigger("TitleOff");

        for (int i = 0; i < mainMenuButtons.Length; i++)
        {
            mainMenuButtons[i].SetTrigger("Exit");
            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitUntil(() => mainMenuHidden);
        GetInDifficultyButtons();
    }

    private IEnumerator ExitMainMenuButtonsToOptions()
    {
        //mainMenuButtons[0].transform.parent.GetComponent<VerticalLayoutGroup>().enabled = false;

        foreach (Animator button in mainMenuButtons)
        {
            button.gameObject.GetComponent<Button>().interactable = false;
        }

        menuTitles[0].SetTrigger("TitleOff");

        for (int i = 0; i < mainMenuButtons.Length; i++)
        {
            mainMenuButtons[i].SetTrigger("Exit");
            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitUntil(() => mainMenuHidden);

        InMenuBanner(AdPosition.BottomRight);
        //Show Options Menu
        mainMenuSettings.gameObject.SetActive(true);
        mainMenuSettings.SetTrigger("EnterMenu");
    }

    public void MoveOutPreviousButtons()
    {
        StartCoroutine(ExitMainMenuButtons());
    }

    public void ShowOptionsMenu()
    {
        StartCoroutine(ExitMainMenuButtonsToOptions());
    }

    IEnumerator ClosingOptionsMenu()
    {
        mainMenuSettings.SetTrigger("ExitMenu");
        yield return new WaitUntil(() => disableOptionsMenu);
        mainMenuSettings.gameObject.SetActive(false);
        BackToMainMenu();
    }

    public void CloseOptionsMenu()
    {
        StartCoroutine(ClosingOptionsMenu());
    }

    private IEnumerator EnterBackMainMenuButtons()
    {
        menuTitles[1].SetTrigger("TitleOff");

        foreach (Animator button in difficultyMenuButtons)
        {
            button.gameObject.GetComponent<Button>().enabled = false;
        }

        for (int i = 0; i < difficultyMenuButtons.Length; i++)
        {
            difficultyMenuButtons[i].SetTrigger("Hide");
            yield return new WaitForSeconds(0.75f);
        }

        yield return new WaitUntil(() => disableDifficultyButtons);
        BackToMainMenu();
    }

    public void GetBackMainMenu()
    {
        StartCoroutine(EnterBackMainMenuButtons());
    }

    private void SkipLogo()
    {
        titleScreen.gameObject.SetActive(false);
        StartCoroutine(EnterMainMenuButtons());

    }
    #endregion

    #region Difficulty Menu
    private IEnumerator EnterDifficultyButtons()
    {
        InMenuBanner(AdPosition.BottomRight);
        menuTitles[1].SetTrigger("TitleOn");

        for (int i = 0; i < difficultyMenuButtons.Length; i++)
        {
            difficultyMenuButtons[i].SetTrigger("Enter");
            yield return new WaitForSeconds(0.75f);
        }

        yield return new WaitUntil(() => enableDifficultyButtons);
        DifficultyButtonsBehaviour();

        enableDifficultyButtons = false;
    }

    private void DifficultyButtonsBehaviour()
    {
        foreach (Animator button in difficultyMenuButtons)
        {
            switch (button.name)
            {
                case "Pussycat":
                    button.gameObject.GetComponent<Button>().enabled = true;
                    break;
                case "Average":
                    button.gameObject.GetComponent<Button>().enabled = true;
                    break;
                case "Foolish":
                    button.gameObject.GetComponent<Button>().enabled = true;
                    break;
                case "Insane":
                    button.gameObject.GetComponent<Button>().enabled = true;
                    break;
                case "Back_D":
                    button.gameObject.GetComponent<Button>().enabled = true;
                    break;
            }
        }
    }

    private void GetInDifficultyButtons()
    {
        StartCoroutine(EnterDifficultyButtons());
    }

    private IEnumerator ExitDifficultyMenuButtons()
    {
        //difficultyMenuButtons[0].transform.parent.GetComponent<VerticalLayoutGroup>().enabled = false;

        foreach (Animator button in difficultyMenuButtons)
        {
            button.gameObject.GetComponent<Button>().enabled = false;
        }

        menuTitles[1].SetTrigger("TitleOff");

        for (int i = 0; i < difficultyMenuButtons.Length; i++)
        {
            difficultyMenuButtons[i].SetTrigger("Exit");
            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitUntil(() => enterCampaignMenu);
        GetInCampaignButtons();
    }

    public void MoveOutDifficultyButtons()
    {
        StartCoroutine(ExitDifficultyMenuButtons());
    }

    private IEnumerator BackDifficultyButtons()
    {
        menuTitles[1].SetTrigger("TitleOn");

        for (int i = 0; i < difficultyMenuButtons.Length; i++)
        {
            difficultyMenuButtons[i].SetTrigger("Back");
            yield return new WaitForSeconds(0.75f);
        }

        yield return new WaitUntil(() => enableDifficultyButtons);
        DifficultyButtonsBehaviour();

        enableDifficultyButtons = false;
    }

    private void BackToDifficultyMenu()
    {
        StartCoroutine(BackDifficultyButtons());
    }

    private IEnumerator EnterBackDifficultyButtons()
    {
        menuTitles[2].SetTrigger("TitleOff");

        foreach (Animator button in campaignButtons)
        {
            button.gameObject.GetComponent<Button>().interactable = false;
        }

        for (int i = 0; i < campaignButtons.Length; i++)
        {
            campaignButtons[i].SetTrigger("Hide");
            yield return new WaitForSeconds(0.75f);
        }

        yield return new WaitUntil(() => disableCampaignButtons);
        BackToDifficultyMenu();
    }

    public void GetBackDifficulty()
    {
        StartCoroutine(EnterBackDifficultyButtons());
    }
    #endregion

    #region Campaign Menu
    private IEnumerator EnterCampaignButtons()
    {
        menuTitles[2].SetTrigger("TitleOn");

        for (int i = 0; i < campaignButtons.Length; i++)
        {
            campaignButtons[i].SetTrigger("Enter");
            yield return new WaitForSeconds(0.75f);
        }

        yield return new WaitUntil(() => enableCampaignButtons);
        //campaignButtons[0].transform.parent.GetComponent<VerticalLayoutGroup>().enabled = true;
        foreach (Animator button in campaignButtons)
        {
            if(button.name == "System01_T" && button.name == "Back")
            {
                button.gameObject.GetComponent<Button>().interactable = true;
            }

            switch (button.name)
            {
                case "System01_T":
                    button.gameObject.GetComponent<Button>().interactable = true;
                    break;
                case "System02 (1)":
                    button.gameObject.GetComponent<Button>().interactable = true;
                    break;
                case "System03 (1)":
                    button.gameObject.GetComponent<Button>().interactable = true;
                    break;
                case "System04 (1)":
                    button.gameObject.GetComponent<Button>().interactable = true;
                    break;
                case "System05 (1)":
                    button.gameObject.GetComponent<Button>().interactable = true;
                    break;
                case "System06 (1)":
                    button.gameObject.GetComponent<Button>().interactable = true;
                    break;
                case "System07 (1)":
                    button.gameObject.GetComponent<Button>().interactable = true;
                    break;
                case "Back":
                    button.gameObject.GetComponent<Button>().interactable = true;
                    break;
            }
        }

        enableCampaignButtons = false;
    }

    private void GetInCampaignButtons()
    {
        StartCoroutine(EnterCampaignButtons());
    }
    #endregion

    #region Not Available Menu Screen

    //Enable Disabled Menu Screen
    public void OnDisableMenuButton()
    {
        disabledMenu.SetActive(true);
        disabledMenuAnimator.SetTrigger("DisabledScreenOn");
    }

    public void OffDisableMenuButton()
    {
        disabledMenuButton.interactable = false;
        if(disabledMenuAnimator.GetCurrentAnimatorStateInfo(0).IsName("Disabled Screen On"))
        {
            disabledMenuAnimator.SetTrigger("DisabledScreenOff");
        }
    }

    public void TurnOffDisabledScreen()
    {
        if (enableDisabledScreenButton)
        {
            OffDisableMenuButton();
        }
        else
        {
            OffDisableDifficultyMenuButton();
        }
    }

    public void OnDisableDifficultyMenuButton()
    {
        disabledMenu.SetActive(true);
        disabledDifficultyMenuAnimator.SetTrigger("DisabledScreenOn");
    }

    private void OffDisableDifficultyMenuButton()
    {
        disabledMenuButton.interactable = false;
        if (disabledDifficultyMenuAnimator.GetCurrentAnimatorStateInfo(0).IsName("Disabled Screen On"))
        {
            disabledDifficultyMenuAnimator.SetTrigger("DisabledScreenOff");
        }
    }
    #endregion

    #region Unlocked Difficulties
    private void DifficultyButtonsCheck()
    {
        foreach (Animator button in difficultyMenuButtons)
        {
            switch (button.name)
            {
                case "Average":
                    if (!PlayerPrefs.HasKey("PussycatDone") || PlayerPrefs.GetInt("PussycatDone") == 0)
                    {
                        button.gameObject.GetComponent<Button>().image.sprite = lockedDifficultiesSprites[0];
                    }
                    else
                    {
                        button.gameObject.GetComponent<Button>().image.sprite = unlockedDifficultiesSprites[0];
                    }
                    break;
                case "Foolish":
                    if (!PlayerPrefs.HasKey("AverageDone") || PlayerPrefs.GetInt("AverageDone") == 0)
                    {
                        button.gameObject.GetComponent<Button>().image.sprite = lockedDifficultiesSprites[1];
                    }
                    else
                    {
                        button.gameObject.GetComponent<Button>().image.sprite = unlockedDifficultiesSprites[1];
                    }
                    break;
                case "Insane":
                    if (!PlayerPrefs.HasKey("FoolishDone") || PlayerPrefs.GetInt("FoolishDone") == 0)
                    {
                        button.gameObject.GetComponent<Button>().image.sprite = lockedDifficultiesSprites[2];
                    }
                    else
                    {
                        button.gameObject.GetComponent<Button>().image.sprite = unlockedDifficultiesSprites[2];
                    }
                    break;
            }
        }
    }

    private void UnlockNextDifficulty()
    {
        //Hide Title Screen
        titleScreen.gameObject.SetActive(false);
        //Show Difficulty Menu
        GetInDifficultyButtons();
        //Show unlocked difficulty dialogue
        StartCoroutine(NewDifficultyUnlocked());
    }

    IEnumerator NewDifficultyUnlocked()
    {
        yield return new WaitUntil(() => enableDifficultyButtons);
        DialogueIndex.Instance.SetDialogue(DialogueIndex.Dialogue.Unlock_Difficulty);
        dialogSystem.gameObject.SetActive(true);
        dialogSystem.SetTrigger("OpenDialogue");
        yield return new WaitUntil(() => dialogSystem.gameObject.GetComponent<DialogueSystem>().isDialogueClosed);
        dialogSystem.GetComponent<DialogueSystem>().isDialogueClosed = false;
        dialogSystem.gameObject.SetActive(false);
    }

    public void SetCurrentDifficultyData(int level)
    {
        PlayerPrefs.SetInt("Difficulty", level);

        switch (level)
        {
            case 0:
                DataControl.Instance.SetPussycatDifficultyData();
                MoveOutDifficultyButtons();
                break;
            case 1:
                if(PlayerPrefs.GetInt("PussycatDone") == 1)
                {
                    DataControl.Instance.SetAverageDifficultyData();
                    MoveOutDifficultyButtons();
                }
                else
                {
                    OnDisableDifficultyMenuButton();
                }
                break;
            case 2:
                if (PlayerPrefs.GetInt("AverageDone") == 1)
                {
                    DataControl.Instance.SetFoolishDifficultyData();
                    MoveOutDifficultyButtons();
                }
                else
                {
                    OnDisableDifficultyMenuButton();
                }
                break;
            case 3:
                if (PlayerPrefs.GetInt("FoolishDone") == 1)
                {
                    DataControl.Instance.SetInsaneDifficultyData();
                    MoveOutDifficultyButtons();
                }
                else
                {
                    OnDisableDifficultyMenuButton();
                }
                break;
            default:
                DataControl.Instance.SetPussycatDifficultyData();
                break;
        }
    }
    #endregion

    #region Credits Titles
    public void ShowCredits()
    {
        StartCoroutine(StartCredits());
    }

    IEnumerator StartCredits()
    {
        foreach (Animator button in mainMenuButtons)
        {
            button.gameObject.GetComponent<Button>().interactable = false;
        }

        menuTitles[0].SetTrigger("TitleOff");

        for (int i = 0; i < mainMenuButtons.Length; i++)
        {
            mainMenuButtons[i].SetTrigger("Exit");
            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitUntil(() => mainMenuHidden);
        InMenuBanner(AdPosition.BottomRight);
        skipCredits.SetTrigger("TurnOnCredits");
        creditsScene.SetBool("CreditsOff", false);
        creditsScene.SetTrigger("ShowCredits");
        skipCreditsButton.SetTrigger("Enter");
        skipCreditsButton.gameObject.GetComponent<Button>().enabled = true;

        //Wait Until Credits Finish
        yield return new WaitUntil(() => creditsDone);
        skipCreditsButton.SetTrigger("Hide");
        creditsScene.SetBool("CreditsOff", true);
        BackToMainMenu();
    }

    public void SkipCreditsButton()
    {
        skipCreditsButton.SetTrigger("Hide");
        skipCreditsButton.gameObject.GetComponent<Button>().enabled = false;
        skipCredits.SetTrigger("Skip");
        //Load main menu
        StartCoroutine(Skip());
    }

    IEnumerator Skip()
    {
        yield return new WaitUntil(() => skipCreditsDone);
        creditsScene.SetBool("CreditsOff", true);
        BackToMainMenu();
    }
    #endregion
}
