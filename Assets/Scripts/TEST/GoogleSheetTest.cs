using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GoogleSheetTest : MonoBehaviour
{
    public void Upload()
    {
        StartCoroutine("UploadIEnumerator");
    }

    IEnumerator UploadIEnumerator()
    {
        // Create the form object.
        WWWForm form = new WWWForm();
        // Add the method data to the form object. (read or write data)
        form.AddField("method", "write");

        // Add the data to the form object. (the data you want to pass to GAS)
        form.AddField("name", "Rempty");
        form.AddField("hp", 50);
        form.AddField("level", "88");
        form.AddField("atk", "100");

        // Sending the request to API url with form object.
        using (UnityWebRequest www = UnityWebRequest.Post("https://script.google.com/macros/s/AKfycbyIaYRi4fyA60FgoouvTOj4nv8RMuJZbOglNtCcelOK456FkxCGUae2vbkJk2f2j0fbvw/exec", form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                // Done and get the response text.
                print(www.downloadHandler.text);
                Debug.Log("Form upload complete!");
            }
        }
    }
}