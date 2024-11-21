using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Asd : MonoBehaviour
{
    public int i = 0;//���õ�����ͷ�����
    public RawImage display; // ������ʾ����ͷͼ��� UI ���
    public Texture2D photo;
    private WebCamTexture webcamTexture;
    private string photoPath; // ���ڱ�����Ƭ·���ı���
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
        // ��ȡ���õ�����ͷ
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length > 0)
        {
            // ���Դӵ�һ������ͷ��ʼ
            for (; i < devices.Length; i++)
            {
                // ���� WebCamTexture ����������ͷ�Ƿ����ڱ�ʹ��
                webcamTexture = new WebCamTexture(devices[i].name);

                // �����ǰ����ͷû���ڲ��ţ���δ��ռ�ã���������������ͷ
                if (!webcamTexture.isPlaying)
                {
                    display.texture = webcamTexture;
                    Debug.Log(222);
                    webcamTexture.Play();
                    break; // �ɹ�����һ������ͷ������ѭ��
                }
            }
        }
        else
        {
            Debug.LogError("û�п��õ�����ͷ��");
        }

        // ������Ƭ�ı���·��
        string ima_name = "photo" + i + ".png";
        photoPath = Path.Combine(Application.persistentDataPath, ima_name);
        Time.timeScale = 0;
    }

    public void CapturePhoto()
    {
        Debug.Log(webcamTexture.isPlaying);
        if (webcamTexture != null && webcamTexture.isPlaying)
        {
            // ����һ�� Texture2D ���� WebCamTexture ��ȡ����
            photo = new Texture2D(webcamTexture.width, webcamTexture.height);
            photo.SetPixels(webcamTexture.GetPixels());
            photo.Apply();
            //display.texture = photo;
            // ���� Texture2D ���ļ�
            File.WriteAllBytes(photoPath, photo.EncodeToPNG());
            Debug.Log("��Ƭ�ѱ��浽��" + photoPath);
            Time.timeScale = 1;
        }
        else
        {
            Debug.LogError("����ͷδ�����򲻿��ã�");
        }
    }

    public void DeletePhoto()
    {
        // ����ļ��Ƿ����
        if (File.Exists(photoPath))
        {
            // ɾ���ļ�
            File.Delete(photoPath);
            Debug.Log("��Ƭ��ɾ����" + photoPath);
        }
        else
        {
            Debug.LogWarning("��Ƭ�ļ������ڣ��޷�ɾ����");
        }
    }

    private void OnDestroy()
    {
        // ֹͣ����ͷ
        if (webcamTexture != null && webcamTexture.isPlaying)
        {
            webcamTexture.Stop();
        }
    }
}
