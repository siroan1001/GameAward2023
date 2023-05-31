using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EffectType
{
    E_EFFECT_KIND_JET = 0,
    E_EFFECT_KIND_EXPLOSION,
    E_EFFECT_KIND_FIRE,
    E_EFFECT_KIND_SPARK,
    E_EFFECT_KIND_MAX
}

[System.Serializable]
public class EffectInfo
{
    public EffectType type;
    public GameObject effectPrefab;
}



public class EffectManager_iwata : MonoBehaviour
{
    public static EffectManager_iwata instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    [SerializeField] public List<EffectInfo> effectList = new List<EffectInfo>();

    public static Dictionary<EffectType, string> EffectTypeNames = new Dictionary<EffectType, string>()
    {
        { EffectType.E_EFFECT_KIND_JET,         "JET" },
        { EffectType.E_EFFECT_KIND_EXPLOSION,   "EXPLOSION" },
        { EffectType.E_EFFECT_KIND_MAX,         "MAX" },
    };

    public static string EffectTypeToString(EffectType effectType)
    {
        if (EffectTypeNames.ContainsKey(effectType))
        {
            return EffectTypeNames[effectType];
        }
        else
        {
            return effectType.ToString();
        }
    }

    public static void PlayEffect(EffectType type, Vector3 position, Transform parent, Vector3 size = default(Vector3))
    {
        //// 引数が指定されていない場合はデフォルト値を使用
        //if (size == default(Vector3))
        //{
        //    size = new Vector3(3.0f, 3.0f, 3.0f);
        //}

        EffectInfo effectInfo = instance.effectList.Find(effect => effect.type == type);
         
        if (effectInfo != null)
        {
            GameObject effectInstance = Instantiate(effectInfo.effectPrefab, position, Quaternion.identity);
            if(size != default) effectInstance.transform.localScale = size;
            effectInstance.transform.parent = parent;
        }
        else
        {
            Debug.LogWarning("No effect of type " + type + " was found.");
        }
    }
}
