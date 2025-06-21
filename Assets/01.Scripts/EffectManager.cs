using System.Collections;
using System.Collections.Generic;
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
        int ran = 0;
        foreach(GameObject btn in effectBtns)
        {
            //무작위 증강 선택
            ran = Random.Range(0, effects.Count);

            //선택 된 증강을 버튼에 초기화
            EffectButton eftBtn = btn.GetComponent<EffectButton>(); //이거 왜 Null..?

            eftBtn.nameText.text = effects[ran].effectName;
            eftBtn.discriptionText.text = effects[ran].effectDiscription;
            eftBtn.effectImg.sprite = effects[ran].effectImg;
            eftBtn.effect = effects[ran];

            //버튼 클릭 이벤트에 effect의 이벤트 등록 (기존 등록된 리스너는 제거해야됨.)
            btn.GetComponent<Button>().onClick.RemoveAllListeners();
            btn.GetComponent<Button>().onClick.AddListener(() => {
                OnClick_Effect(eftBtn.effect); //이거 괜찮나..?

                Time.timeScale = 1f;
                gameManager.effectPanel.SetActive(false);
            });

        }
    }

    public void OnClick_Effect(Effect effect)
    {
        switch(effect.effectType)
        {
            case EffectType.HEALTH:
                gameManager.player.maxHealth += effect.effectLevel * effect.effectMulti;
            break;

            case EffectType.COIN:
                gameManager.player.coinMulti += effect.effectLevel * effect.effectMulti; 
            break;

            case EffectType.AMMO:
                gameManager.player.maxAmmo += effect.effectLevel * effect.effectMulti;
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
