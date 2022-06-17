using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI cashText;
    [SerializeField] protected GameObject gameOverPanel;
    [SerializeField] protected Image[] healthSprites;
    [SerializeField] protected GameObject startMissionAnimation;
    [SerializeField] protected Slider thrusterSlider;
    [SerializeField] protected GameObject thrusterFill;
    [SerializeField] protected TextMeshProUGUI ammoCounter;

    private void OnEnable()
    {
        GameEvents.DisplayCash += SetCash;
        GameEvents.UpdateHealth += ChangeHealth;
        GameEvents.NewLife += DeductLives;
        GameEvents.GameOver += GameOverUI;
        GameEvents.ThrusterSupply += UpdateThrusterSlider;
        GameEvents.SetThrusterMax += SetThrusterValues;
        GameEvents.UpdateAmmo += UpdateAmmoCount;
    }

    private void OnDisable()
    {
        GameEvents.DisplayCash -= SetCash;
        GameEvents.UpdateHealth -= ChangeHealth;
        GameEvents.NewLife -= DeductLives;
        GameEvents.GameOver -= GameOverUI;
        GameEvents.ThrusterSupply -= UpdateThrusterSlider;
        GameEvents.SetThrusterMax -= SetThrusterValues;
        GameEvents.UpdateAmmo -= UpdateAmmoCount;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SectorStartAnimation());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Weapons

    private void UpdateAmmoCount(int currentAmmoCount)
    {
        ammoCounter.text = currentAmmoCount.ToString();
    }

    #endregion

    #region Thrusters

    private void UpdateThrusterSlider(float value)
    {
        thrusterSlider.value = value;
        
        if (thrusterSlider.value <= 0 && thrusterFill.activeInHierarchy)
        {
            thrusterFill.SetActive(false);
        }
        else if(!thrusterFill.activeInHierarchy && thrusterSlider.value > 0)
        {
            thrusterFill.SetActive(true);
        }
    }

    private void SetThrusterValues(float supply)
    {
        thrusterSlider.maxValue = supply;
    }

    #endregion

    #region Lives and Health

    private void SetLives(int health)
    {
        for (int i = 0; i < health; i++)
        {
            healthSprites[i].gameObject.SetActive(true);
        }
    }

    private void DeductLives(int livesLeft)
    {
        for (int i = 0; i < healthSprites.Length; i++)
        {
            if (i > livesLeft - 1)
            {
                healthSprites[i].gameObject.SetActive(false);
            }
        }
    }

    private void ChangeHealth(int health)
    {
        for (int i = 0; i < health; i++)
        {
            
        }
    }

    #endregion

    #region Cash

    private void SetCash(int cash)
    {
        cashText.text = cash.ToString("C2");
    }

    #endregion

    #region Game Start and Game Over

    private IEnumerator SectorStartAnimation()
    {
        yield return new WaitForSeconds(startMissionAnimation.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.length);

        startMissionAnimation.SetActive(false);

        GameEvents.SpawningStarted();
    }
    
    private void GameOverUI()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }
    
    public void NextLevel()
    {
        GameEvents.NextLevel();
    }

    public void Restart()
    {
        GameEvents.Restart();
    }

    public void Quit()
    {
        GameEvents.Quit();
    }

    #endregion
}

