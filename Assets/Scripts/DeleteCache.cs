using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections;

public class DeleteCache : MonoBehaviour
{
    public void ClearCache()
    {
        StartCoroutine(ClearCacheCoroutine());
    }
        
    private IEnumerator ClearCacheCoroutine()
    {
        Debug.Log("Menghapus cache Addressables...");

        // Hapus semua resource yang di-load oleh Addressables
        Addressables.ClearResourceLocators();

        // Tunggu sebentar sebelum membersihkan cache
        yield return new WaitForSeconds(1f);

        // Hapus cache Unity
        if (Caching.ClearCache())
        {
            Debug.Log("Cache berhasil dihapus.");
        }
        else
        {
            Debug.Log("Gagal menghapus cache atau tidak ada cache yang bisa dihapus.");
        }
    }
}
