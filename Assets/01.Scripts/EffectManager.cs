using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System;

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

    // 효과 타입별로 적용 함수를 매핑하여 switch문 제거 및 확장성 향상
    readonly Dictionary<EffectType, Action<Effect>> applyEffectByType = new Dictionary<EffectType, Action<Effect>>();

    void Awake()
    {
        // 초기 매핑 설정 (필요 시 타입 추가만으로 확장 가능)
        applyEffectByType[EffectType.HEALTH] = ApplyHealth;
        applyEffectByType[EffectType.COIN] = ApplyCoin;
        applyEffectByType[EffectType.AMMO] = ApplyAmmo;
    }

    public void Initialized()
    {
        //무작위 증강 선택
        // 버튼 개수와 원하는 옵션 수(3)를 고려하여 안전하게 개수 결정
        int optionCount = Mathf.Min(3, effectBtns.Count);
        List<Effect> eftlst = GetRandomElemets<Effect>(effects, optionCount);

        for (int i = 0; i < optionCount; i++)
        {
            EffectButton eftBtn = effectBtns[i].GetComponent<EffectButton>();

            eftBtn.nameText.text = eftlst[i].effectName;
            eftBtn.discriptionText.text = eftlst[i].effectDiscription;
            eftBtn.effectImg.sprite = eftlst[i].effectImg;
            eftBtn.effect = eftlst[i];

            effectBtns[i].GetComponent<Button>().onClick.RemoveAllListeners();
            // 클로저 이슈 방지: i가 아닌 현재 효과를 별도 변수로 캡처
            Effect capturedEffect = eftlst[i];
            effectBtns[i].GetComponent<Button>().onClick.AddListener(() =>
            {
                OnClick_Effect(capturedEffect);

                Time.timeScale = 1f;
                gameManager.effectPanel.SetActive(false);
            });
        }
    }

    // 리스트에서 중복없이 count개의 요소를 추출하는 함수
    List<T> GetRandomElemets<T>(List<T> list, int count)
    {
        List<T> shuffled = list.OrderBy(x => UnityEngine.Random.value).ToList();

        return shuffled.Take(count).ToList();
    }

    // 이펙트 선택 시 유저에게 이펙트를 적용하는 함수
    public void OnClick_Effect(Effect effect)
    {
        if (effect == null) return;
        // 딕셔너리에서 effectType와 같은 요소를 찾아서 apply로 받아옴.
        // apply는 delegate함수이기 때문에 해당 함수에 effect 전달해서 효과 적용
        if (applyEffectByType.TryGetValue(effect.effectType, out var apply))
        {
            apply(effect);
        }
        else
        {
            Debug.LogWarning($"[EffectManager] 적용 함수가 없는 효과 타입: {effect.effectType}");
        }

        SetEffectIcon(effect);
    }

    // 비활성 아이콘 슬롯을 찾아 스프라이트/레벨 설정 후 활성화
    public void SetEffectIcon(Effect effect)
    {
        foreach (var e in effectIcons)
        {
            if (e.activeSelf) continue;

            EffectIcon icon = e.GetComponent<EffectIcon>();
            icon.effectIconImg.sprite = effect.effectImg;
            icon.effectIconText.text = effect.effectLevel.ToString();

            e.SetActive(true);
            break;
        }
    }

    // HEALTH 효과 적용 로직
    void ApplyHealth(Effect effect)
    {
        int delta = (int)effect.effectValue * effect.effectMulti;
        gameManager.player.maxHealth += delta;
        gameManager.player.health = gameManager.player.maxHealth;
    }

    // COIN 효과 적용 로직
    void ApplyCoin(Effect effect)
    {
        float delta = effect.effectValue * effect.effectMulti;
        gameManager.player.coinMulti += delta;
    }

    // AMMO 효과 적용 로직
    void ApplyAmmo(Effect effect)
    {
        int delta = (int)effect.effectValue * effect.effectMulti;
        gameManager.player.maxAmmo += delta;
    }
}
