using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.AddressableAssets.ResourceLocators;
using TMPro;
using UnityEngine.ResourceManagement.ResourceLocations;

public class DLCManager : MonoBehaviour
{
    // Nama scene yang sudah di-tag sebagai Addressable
    private string dlcSceneKey = "Triangle";
    [SerializeField] private Slider dlcDownloadProgressSlider;
    [SerializeField] private TextMeshProUGUI dlcSizeText;
    public Button downDLC;
    public Button openDLC;
    public Button deleteDLC;

    private void Start()
    {
        CheckDLCStatus();
    }

    public void CheckDLCStatus()
    {
        StartCoroutine(CheckDLCStatusCoroutine());
    }

    private IEnumerator CheckDLCStatusCoroutine()
    {
        Debug.Log("Memeriksa status DLC...");

        AsyncOperationHandle<long> checkHandle = Addressables.GetDownloadSizeAsync(dlcSceneKey);
        yield return checkHandle;

        if (checkHandle.Status == AsyncOperationStatus.Succeeded)
        {
            long dlcSize = checkHandle.Result;
            if (dlcSize > 0)
            {
                Debug.Log($"DLC tersedia untuk diunduh, ukuran: {FormatSize(dlcSize)}.");
                dlcSizeText.text = $"DLC Size: {FormatSize(dlcSize)}";
                downDLC.gameObject.SetActive(true);
                openDLC.gameObject.SetActive(false);
                deleteDLC.gameObject.SetActive(false);
            }
            else
            {
                Debug.Log("DLC sudah tersedia. Menampilkan tombol Load Scene.");
                downDLC.gameObject.SetActive(false);
                openDLC.gameObject.SetActive(true);
                deleteDLC.gameObject.SetActive(true);
            }
        }
        else
        {
            Debug.LogError("Gagal memeriksa status DLC.");
            downDLC.gameObject.SetActive(true);
            openDLC.gameObject.SetActive(false);
        }
    }

    public void DownloadandLoad()
    {
        StartCoroutine(DownloadAndLoadDLC());
    }

    IEnumerator DownloadAndLoadDLC()
    {
        Debug.Log("Memulai download DLC...");

        // Pastikan slider aktif dan dimulai dari 0
        dlcDownloadProgressSlider.gameObject.SetActive(true);
        dlcDownloadProgressSlider.value = 0f;

        AsyncOperationHandle downloadHandle = Addressables.DownloadDependenciesAsync(dlcSceneKey);

        while (!downloadHandle.IsDone)
        {
            dlcDownloadProgressSlider.value = downloadHandle.PercentComplete; // Update progress bar
            yield return null; // Tunggu frame berikutnya
        }

        if (downloadHandle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("Download DLC selesai!");

            // Sembunyikan slider setelah selesai
            dlcDownloadProgressSlider.gameObject.SetActive(false);
            dlcSizeText.gameObject.SetActive(false);
            CheckDLCStatus();
        }
        else
        {
            Debug.LogError("Gagal mendownload DLC!");
            dlcDownloadProgressSlider.gameObject.SetActive(false);
        }
    }

    public void OpenDownloadedDLC()
    {
        StartCoroutine(CheckAndLoadDLC());
    }

    private IEnumerator CheckAndLoadDLC()
    {
        AsyncOperationHandle<long> checkHandle = Addressables.GetDownloadSizeAsync(dlcSceneKey);
        yield return checkHandle;

        if (checkHandle.Status == AsyncOperationStatus.Succeeded)
        {
            if (checkHandle.Result > 0)
            {
                Debug.LogError("DLC tidak ditemukan secara lokal! Silakan download terlebih dahulu.");
            }
            else
            {
                Debug.Log("DLC ditemukan di cache, memuat scene...");
                AsyncOperationHandle<SceneInstance> sceneHandle = Addressables.LoadSceneAsync(dlcSceneKey);
                yield return sceneHandle;

                if (sceneHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    Debug.Log("Scene DLC berhasil dimuat!");
                }
                else
                {
                    Debug.LogError("Gagal memuat scene DLC!");
                }
            }
        }
        else
        {
            Debug.LogError("Gagal mengecek ukuran DLC!");
        }
    }

    public void ClearCache()
    {
        StartCoroutine(ClearCacheCoroutine());
    }

    private IEnumerator ClearCacheCoroutine()
    {
        Debug.Log("Menghapus cache Addressables...");

        var handle = Addressables.ClearDependencyCacheAsync(dlcSceneKey, false);
        yield return handle;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("Cache dependensi Addressables berhasil dihapus.");
        }
        else
        {
            Debug.LogError("Gagal menghapus cache dependensi Addressables.");
        }

        yield return new WaitForSeconds(1f);

        if (Caching.ClearAllCachedVersions(dlcSceneKey))
        {
            Debug.Log("Semua versi DLC yang tersimpan di cache telah dihapus.");
            Debug.Log("Silahkan Restart Aplikasi");
            openDLC.gameObject.SetActive(false);
            deleteDLC.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("Tidak ada cache DLC yang bisa dihapus.");
        }

        yield return new WaitForSeconds(1f);

        Addressables.ClearResourceLocators();
        Application.Quit();

        yield return new WaitForSeconds(1f);
    }

    private string FormatSize(long sizeInBytes)
    {
        float size = sizeInBytes;
        string[] units = { "B", "KB", "MB", "GB" };
        int unitIndex = 0;

        while (size >= 1024 && unitIndex < units.Length - 1)
        {
            size /= 1024;
            unitIndex++;
        }

        return $"{size:F2} {units[unitIndex]}";
    }
}
