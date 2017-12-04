using UnityEngine;
using System.Collections;


[System.Reflection.Obfuscation(Exclude = true)]
public class ShowFPS : MonoSingleton<ShowFPS> {

    public FPSPosition showPosition = FPSPosition.Top_Left;
    // Update is called once per frame
    void Update()
    {
        UpdateTick();
    }

    void OnGUI()
    {
        DrawFps();
    }

    private Color guiColor;
    Vector2 size = new Vector2(64, 24);
    private void DrawFps()
    {
        if (mLastFps > 50)
        {
            guiColor = new Color(0, 1, 0);
        }
        else if (mLastFps > 40)
        {
            guiColor = new Color(1, 1, 0);
        }
        else
        {
            guiColor = new Color(1.0f, 0, 0);
        }
        GUIStyle GUI_style = new GUIStyle();
        GUI_style.fontSize = 28;
        GUI_style.normal.background = null;    //这是设置背景填充的
        GUI_style.normal.textColor = guiColor;   //设置字体颜色的
        GUI_style.alignment = TextAnchor.MiddleCenter;
        Rect rect = new Rect(GetPosition(showPosition, size), size);
        GUI.Label(rect, "fps: " + mLastFps, GUI_style);

    }

    private long mFrameCount = 0;
    private long mLastFrameTime = 0;
    static long mLastFps = 0;
    private void UpdateTick()
    {
        if (true)
        {
            mFrameCount++;
            long nCurTime = TickToMilliSec(System.DateTime.Now.Ticks);
            if (mLastFrameTime == 0)
            {
                mLastFrameTime = TickToMilliSec(System.DateTime.Now.Ticks);
            }

            if ((nCurTime - mLastFrameTime) >= 1000)
            {
                long fps = (long)(mFrameCount * 1.0f / ((nCurTime - mLastFrameTime) / 1000.0f));

                mLastFps = fps;

                mFrameCount = 0;

                mLastFrameTime = nCurTime;
            }
        }
    }
    public static long TickToMilliSec(long tick)
    {
        return tick / (10 * 1000);
    }
    private Vector2 GetPosition(FPSPosition pp,Vector2 size)
    {
        switch (pp)
        {
            case FPSPosition.Top_Left:
                return new Vector2(0, 0);
            case FPSPosition.Top_Middle:
                return new Vector2((Screen.width-size.x)/2, 0);
            case FPSPosition.Top_Right:
                return new Vector2(Screen.width - size.x*2, 0);
            case FPSPosition.Center:
                return new Vector2((Screen.width - size.x) / 2, (Screen.height-size.y)/2);
            case FPSPosition.Bottom_Left:
                return new Vector2(0, Screen.height-size.y);
            case FPSPosition.Bottom_Right:
                return new Vector2(Screen.width - size.x*2, Screen.height - size.y);

        }
        return Vector2.zero;
    }
    public enum FPSPosition
    {
        Top_Left,
        Top_Middle,
        Top_Right,
        Center,
        Bottom_Left,
        Bottom_Right,
    }
}
