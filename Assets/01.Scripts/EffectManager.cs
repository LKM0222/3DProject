using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum EffectType
{
    NONE = 0,
    HEALTH = 1,
    COIN = 2,
    AMMO = 3,
}

public class EffectManager : MonoBehaviour
{
    public GameManager gameManager;

    [SerializeField] List<Effect> effects;
    [SerializeField] List<GameObject> effectBtns;
    [SerializeField] List<GameObject> effectIcons;


    public void Initialized()
    {
        //무작위 증강 선택
        List<Effect> eftlst = GetRandomElemets<Effect>(effects, 3);

        for(int i = 0; i < effectBtns.Count; i++)
        {
            EffectButton eftBtn = effectBtns[i].GetComponent<EffectButton>();

            eftBtn.nameText.text = eftlst[i].effectName;
            eftBtn.discriptionText.text = eftlst[i].effectDiscription;
            eftBtn.effectImg.sprite = eftlst[i].effectImg;
            eftBtn.effect = eftlst[i];

            effectBtns[i].GetComponent<Button>().onClick.RemoveAllListeners();
            effectBtns[i].GetComponent<Button>().onClick.AddListener(() => {
                OnClick_Effect(eftBtn.effect); //이거 괜찮나..?

                Time.timeScale = 1f;
                gameManager.effectPanel.SetActive(false);
            });
        }
    }

    List<T> GetRandomElemets<T>(List<T> list, int count)    // 중복없이 n개 추출
    {
        List<T> shuffled = list.OrderBy(x => UnityEngine.Random.value).ToList();

        return shuffled.Take(count).ToList();
    }

    public void OnClick_Effect(Effect effect)
    {
        switch(effect.effectType)
        {
            case EffectType.HEALTH:
                gameManager.player.maxHealth += (int)effect.effectValue * effect.effectMulti;
                gameManager.player.health = gameManager.player.maxHealth;
            break;

            case EffectType.COIN:
                gameManager.player.coinMulti += effect.effectValue * effect.effectMulti; 
            break;

            case EffectType.AMMO:
                gameManager.player.maxAmmo += (int)effect.effectValue * effect.effectMulti;
            break;
        }
        // Debug.Log($"Effect : {effect.effectName}/{effect.effectDiscription}");
        SetEffectIcon(effect);
    }

    public void SetEffectIcon(Effect effect)
    {
        foreach(var e in effectIcons)
        {
            if(e.activeSelf) continue;

            EffectIcon icon = e.GetComponent<EffectIcon>();
            icon.effectIconImg.sprite = effect.effectImg;
            icon.effectIconText.text = effect.effectLevel.ToString();

            e.SetActive(true);
            break;
        }
    }
}
