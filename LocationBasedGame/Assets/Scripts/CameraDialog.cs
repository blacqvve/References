using UnityEngine;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

public class CameraDialog : MonoBehaviour
{
    const int kDialogWidth = 300;
    const int kDialogHeight = 100;
    private bool windowOpen = true;
    public string _permType;

    
    void DoMyWindow(int windowID)
    {
        GUI.Label(new Rect(10, 20, kDialogWidth - 20, kDialogHeight - 50), "Arka planda camera ile objeleri görebilmeniz için kamera iznine ihtiyacımız var.");
        GUI.Button(new Rect(10, kDialogHeight - 30, 100, 20), "Hayır");
        if (GUI.Button(new Rect(kDialogWidth - 110, kDialogHeight - 30, 100, 20), "Evet"))
        {
#if PLATFORM_ANDROID
            Permission.RequestUserPermission(Permission.Camera);
#endif
            windowOpen = false;
        }
    }

    void OnGUI()
    {
        if (windowOpen)
        {
            Rect rect = new Rect((Screen.width / 2) - (kDialogWidth / 2), (Screen.height / 2) - (kDialogHeight / 2), kDialogWidth, kDialogHeight);
            GUI.ModalWindow(0, rect, DoMyWindow, "İzin Talebi");
        }
    }
}