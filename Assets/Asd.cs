using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Asd : MonoBehaviour
{
    public int i = 0;//调用的摄像头的序号
    public RawImage display; // 用于显示摄像头图像的 UI 组件
    public Texture2D photo;
    private WebCamTexture webcamTexture;
    private string photoPath; // 用于保存照片路径的变量
    private bool f;


    private void OnEnable()
    {
        //f = true;
        Initialize();
    }



    private void Update()
    {

        if (Input.GetMouseButtonDown(2))
        {
            CapturePhoto();
            display.enabled = false;
            enabled = false;
        }
    }

    private void Initialize()
    {
        // 获取可用的摄像头
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length > 0)
        {
            // 尝试从第一个摄像头开始
            for (; i < devices.Length; i++)
            {
                // 创建 WebCamTexture 并检查该摄像头是否正在被使用
                webcamTexture = new WebCamTexture(devices[i].name);

                // 如果当前摄像头没有在播放（即未被占用），则启动该摄像头
                if (!webcamTexture.isPlaying)
                {
                    display.texture = webcamTexture;
                    Debug.Log(222);
                    webcamTexture.Play();
                    break; // 成功启动一个摄像头后跳出循环
                }
            }
        }
        else
        {
            Debug.LogError("没有可用的摄像头！");
        }

        // 定义照片的保存路径
        string ima_name = "photo" + i + ".png";
        photoPath = Path.Combine(Application.persistentDataPath, ima_name);
        Time.timeScale = 0;
    }

    public void CapturePhoto()
    {
        Debug.Log(webcamTexture.isPlaying);
        if (webcamTexture != null && webcamTexture.isPlaying)
        {
            // 创建一个 Texture2D 并从 WebCamTexture 读取像素
            photo = new Texture2D(webcamTexture.width, webcamTexture.height);
            photo.SetPixels(webcamTexture.GetPixels());
            photo.Apply();
            //display.texture = photo;
            // 保存 Texture2D 到文件
            File.WriteAllBytes(photoPath, photo.EncodeToPNG());
            Debug.Log("照片已保存到：" + photoPath);
            Time.timeScale = 1;
        }
        else
        {
            Debug.LogError("摄像头未启动或不可用！");
        }
    }

    public void DeletePhoto()
    {
        // 检查文件是否存在
        if (File.Exists(photoPath))
        {
            // 删除文件
            File.Delete(photoPath);
            Debug.Log("照片已删除：" + photoPath);
        }
        else
        {
            Debug.LogWarning("照片文件不存在，无法删除！");
        }
    }

    private void OnDestroy()
    {
        // 停止摄像头
        if (webcamTexture != null && webcamTexture.isPlaying)
        {
            webcamTexture.Stop();
        }
    }
}
