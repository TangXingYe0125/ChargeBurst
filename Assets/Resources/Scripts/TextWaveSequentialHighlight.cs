using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class TextWaveSequentialHighlight : MonoBehaviour
{
    [Header("Text")]
    public TMP_Text text;

    [Header("Jump Settings")]
    public float jumpHeight = 20f;       // 跳跃高度
    public float jumpDuration = 0.3f;    // 每个字符跳跃时间
    public bool leftToRight = true;      // 跳动方向

    [Header("Color Settings")]
    public Color normalColor = Color.white;
    public Color highlightColor = Color.yellow;

    private TMP_TextInfo textInfo;
    private float timer = 0f;
    private int currentChar = 0;

    void Awake()
    {
        if (text == null) text = GetComponent<TMP_Text>();
    }

    void Update()
    {
        text.ForceMeshUpdate();
        textInfo = text.textInfo;

        if (textInfo.characterCount == 0) return;

        // 跳过不可见字符（空格或换行）
        int visibleCharCount = textInfo.characterCount;
        int checkedChar = 0;
        while (!textInfo.characterInfo[currentChar].isVisible && checkedChar < visibleCharCount)
        {
            if (leftToRight)
                currentChar = (currentChar + 1) % visibleCharCount;
            else
                currentChar = (currentChar - 1 + visibleCharCount) % visibleCharCount;
            checkedChar++;
        }

        // 如果全是不可见字符就直接退出
        if (!textInfo.characterInfo[currentChar].isVisible) return;

        TMP_CharacterInfo charInfo = textInfo.characterInfo[currentChar];
        int meshIndex = charInfo.materialReferenceIndex;
        int vertexIndex = charInfo.vertexIndex;
        Vector3[] vertices = textInfo.meshInfo[meshIndex].vertices;
        Color32[] colors = textInfo.meshInfo[meshIndex].colors32;

        // 计算跳跃偏移
        timer += Time.deltaTime;
        float progress = timer / jumpDuration;
        float yOffset = 0f;
        float colorLerp = 0f;

        if (progress < 0.5f)
        {
            // 上升阶段
            yOffset = Mathf.Lerp(0, jumpHeight, progress / 0.5f);
            colorLerp = Mathf.Lerp(0, 1, progress / 0.5f);
        }
        else if (progress < 1f)
        {
            // 下降阶段
            yOffset = Mathf.Lerp(jumpHeight, 0, (progress - 0.5f) / 0.5f);
            colorLerp = Mathf.Lerp(1, 0, (progress - 0.5f) / 0.5f);
        }
        else
        {
            // 跳完切换到下一个字符
            timer = 0f;
            if (leftToRight)
                currentChar = (currentChar + 1) % visibleCharCount;
            else
                currentChar = (currentChar - 1 + visibleCharCount) % visibleCharCount;
            yOffset = 0f;
            colorLerp = 0f;
        }

        Vector3 offset = new Vector3(0, yOffset, 0);

        // 移动字符顶点
        vertices[vertexIndex + 0] = charInfo.bottomLeft + offset;
        vertices[vertexIndex + 1] = charInfo.topLeft + offset;
        vertices[vertexIndex + 2] = charInfo.topRight + offset;
        vertices[vertexIndex + 3] = charInfo.bottomRight + offset;

        // 设置字符颜色渐变
        Color charColor = Color.Lerp(normalColor, highlightColor, colorLerp);
        colors[vertexIndex + 0] = charColor;
        colors[vertexIndex + 1] = charColor;
        colors[vertexIndex + 2] = charColor;
        colors[vertexIndex + 3] = charColor;

        // 更新网格
        text.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices | TMP_VertexDataUpdateFlags.Colors32);
    }
}
