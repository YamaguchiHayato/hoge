using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Aボタンを押してくださいを促すUI用のスクリプト。
/// </summary>
public class PressAButtonScript : MonoBehaviour
{
    private Image pressAButtonUI;//Aボタンを押してくださいを促すUI。

    Vector4 baseColor = new Vector4(1, 1, 1, 0);//元の色。
    Vector4 targetColor = new Vector4(1, 1, 1, 1);//ターゲットの色。

    float larpRate = 0.0f;//補完率。
    const float ALPHA_CHANGE_SPEED = 1.5f;//透明度を変える速度。

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pressAButtonUI = this.GetComponent<Image>();

        pressAButtonUI.color = new Vector4(1, 1, 1, 0);
    }

    // Update is called once per frame
    void Update()
    {
        //Aボタンを押してくださいを促すUIの透明度を変える処理。
        PressAButtonUIAlphaChange();
    }

    //Aボタンを押してくださいを促すUIの透明度を変える処理。
    void PressAButtonUIAlphaChange()
    {
        larpRate += ALPHA_CHANGE_SPEED * Time.deltaTime;
        pressAButtonUI.color = Vector4.Lerp(baseColor, targetColor, Mathf.Sin(larpRate));
    }
}
