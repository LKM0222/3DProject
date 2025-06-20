using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject menuCam;
    public GameObject gameCam;
    public Player player;
    public Boss boss;
    public GameObject itemShop;
    public GameObject weaponShop;
    public GameObject startZone;

    public int stage;
    public float playTime;
    public bool isBattle;

    public int enemyCntA;
    public int enemyCntB;
    public int enemyCntC;
    public int enemyCntD;    

    //PoolManager로 이동
    public PoolManager poolManager;
    public Transform[] enemyZones;
    
    public List<int> enemyList;

    public GameObject menuPanel;
    public GameObject gamePanel;
    public GameObject overPanel;

    public Text maxScoreText;

    public Text scoreText;
    public Text stageText;
    public Text playTimeText;

    public Text playerHelthText;
    public Text playerAmmoText;
    public Text playerCoinText;

    public Image weapon1Img;
    public Image weapon2Img;
    public Image weapon3Img;
    public Image weaponRImg;

    public Text enemyAText;
    public Text enemyBText;
    public Text enemyCText;

    public RectTransform bossHealthGroup;
    public RectTransform bossHealthBar;

    public RectTransform playerExpBar;
    public Text playerExpText;
    public Text curScoreText;
    public Text bestText;

    public GameObject effectPanel;
    public EffectManager effectManager;

    void Awake()
    {   
        enemyList = new List<int>();
        maxScoreText.text = string.Format("{0:n0}",PlayerPrefs.GetInt("MaxScore"));

        if(PlayerPrefs.HasKey("MaxScore"))
        {
            PlayerPrefs.SetInt("MaxScore", 0);
        }
    }

    void Update()
    {
        if(isBattle)
        {
            playTime += Time.deltaTime;
        }
    }

    void LateUpdate()
    {   
        //상단 UI
        scoreText.text = string.Format("{0:n0}", player.score);
        stageText.text = "STAGE " + stage;

        int hour = (int)(playTime / 3600);
        int min = (int)((playTime - hour * 3600) / 60);
        int sec = (int)(playTime % 60);
        playTimeText.text = $"{hour:00}:{min:00}:{sec:00}";

        //플레이어 UI
        playerHelthText.text = player.health + "/" + player.maxHealth;
        playerCoinText.text = string.Format("{0:n0}", player.coin);

        if(player.equipWeapon == null || player.equipWeapon.type == Weapon.Type.Melee)
            playerAmmoText.text = "-" + "/" + player.ammo;
        else
            playerAmmoText.text = player.equipWeapon.curAmmo + "/" + player.ammo;

        //하단 UI
        weapon1Img.color = new Color(1,1,1, player.hasWeapons[0] ? 1 : 0);
        weapon2Img.color = new Color(1,1,1, player.hasWeapons[1] ? 1 : 0);
        weapon3Img.color = new Color(1,1,1, player.hasWeapons[2] ? 1 : 0);
        weaponRImg.color = new Color(1,1,1, player.hasGrenades > 0 ? 1 : 0);

        enemyAText.text = enemyCntA.ToString();
        enemyBText.text = enemyCntB.ToString();
        enemyCText.text = enemyCntC.ToString();

        //EXP
        playerExpBar.localScale = new Vector3((float)player.exp / player.maxExp, 1, 1);
        playerExpText.text = $"{player.exp}/{player.maxExp}";

        // 보스 UI
        if(boss != null)
        {
            bossHealthGroup.anchoredPosition = Vector3.down * 30f;
            bossHealthBar.localScale = new Vector3((float)boss.curHealth / boss.maxHealth, 1, 1);
        }
        else
        {
            bossHealthGroup.anchoredPosition = Vector3.up * 200f;
        }


    }

    public void GameStart()
    {
        menuCam.SetActive(false);
        gameCam.SetActive(true);

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);

        player.gameObject.SetActive(true);
    }

    public void StageStart()
    {   
        itemShop.SetActive(false);
        weaponShop.SetActive(false);
        startZone.SetActive(false);

        foreach(Transform zone in enemyZones)
        {
            zone.gameObject.SetActive(true);
        }

        isBattle = true;
        StartCoroutine(InBattle());
    }

    public void StageEnd()
    {      
        player.transform.position = Vector3.up * 0.8f;

        itemShop.SetActive(true);
        weaponShop.SetActive(true);
        startZone.SetActive(true);

        foreach(Transform zone in enemyZones)
        {
            zone.gameObject.SetActive(false);
        }

        isBattle = false;
        stage++;
    }

    public void GameOver()
    {
        gamePanel.SetActive(false);

        overPanel.SetActive(true);
        curScoreText.text = scoreText.text;

        int maxScore = PlayerPrefs.GetInt("MaxScore");
        if(player.score > maxScore)
        {
            bestText.gameObject.SetActive(true);
            PlayerPrefs.SetInt("MaxScore", player.score);
        }
    }

    public void ReStart()
    {
        SceneManager.LoadScene(0);
    }

    IEnumerator InBattle()
    {   
        if(stage % 5 == 0)
        {
            enemyCntD++;
            // GameObject instantEnemy = Instantiate(enemies[3], enemyZones[0].position, enemyZones[0].rotation);

            // Enemy enemy = instantEnemy.GetComponent<Enemy>();
            Enemy enemy = poolManager.GetQueue(3).GetComponent<Enemy>();

            enemy.target = player.transform;
            enemy.manager = this;
            enemy.transform.position = enemyZones[0].position;
            enemy.IsEnable();

            boss = enemy.gameObject.GetComponent<Boss>();
        }
        else
        {
            for(int index = 0; index < stage; index++)
            {
                int ran = Random.Range(0,3);
                enemyList.Add(ran);

                switch(ran)
                {
                    case 0:
                        enemyCntA++;
                        break;
                    case 1:
                        enemyCntB++;
                        break;
                    case 2:
                        enemyCntC++;
                        break;
                }
            }

            while(enemyList.Count > 0)
            {
                int ranZone = Random.Range(0,4);
                // pool로 이동
                Enemy enemy = poolManager.GetQueue(enemyList[0]).GetComponent<Enemy>();
                
                enemy.target = player.transform;
                enemy.manager = this;
                enemy.transform.position = enemyZones[ranZone].position;
                enemy.IsEnable();

                enemyList.RemoveAt(0);
                yield return new WaitForSeconds(4f);
            }
        }

        while(enemyCntA + enemyCntB + enemyCntC + enemyCntD > 0)
        {
            yield return null;
        }

        yield return new WaitForSeconds(4f);
        
        boss = null;
        StageEnd();
    }
}