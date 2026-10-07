using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// タイトル名UI用のスクリプト。
/// </summary>
public class TitleNameUIScript : MonoBehaviour
{
    private Image titleNameUI;//タイトル名UI。

    Vector3 baseScale = new Vector3(0.0f, 0.0f, 1.0f);//元の大きさ。
    Vector3 targetScale = new Vector3(0.0f, 0.0f, 1.0f);//ターゲットの大きさ。

    Vector4 baseColor = new Vector4(1.0f, 1.0f, 1.0f, 0.0f);//元の色。
    Vector4 targetColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);//ターゲットの色。

    float larpRate = 0.0f;//補完率。
    const float DIRECTION_SPEED = 1.0f;//演出速度。

    bool isDirectionFinish = false;//演出が終了したか？

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        titleNameUI = this.GetComponent<Image>();

        titleNameUI.transform.localScale = new Vector3(1.3f, 1.3f, 1.0f );
        titleNameUI.color = new Vector4(1.0f, 1.0f, 1.0f, 0.0f);

        baseScale = titleNameUI.transform.localScale;
        targetScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        larpRate += DIRECTION_SPEED * Time.deltaTime;

        if(larpRate > 1.0f)
        {
            isDirectionFinish = true;
            return;
        }

        //タイトル名UIの大きさを変える処理。
        TitleNameUIScaleChange(larpRate);

        //タイトル名UIの透明度を変える処理。
        TitleNameUIAlphaChange(larpRate);
    }

    //タイトル名UIの大きさを変える処理。
    void TitleNameUIScaleChange(float rete)
    {
        titleNameUI.transform.localScale = Vector3.Lerp(baseScale, targetScale, larpRate);
    }

    //タイトル名UIの透明度を変える処理。
    void TitleNameUIAlphaChange(float rate)
    {
        titleNameUI.color = Vector4.Lerp(baseColor, targetColor, larpRate);
    }

    //演出が終了したか？ trueなら終了している。
    public bool IsDirectionFinish()
    {
        return isDirectionFinish;
    }
}
