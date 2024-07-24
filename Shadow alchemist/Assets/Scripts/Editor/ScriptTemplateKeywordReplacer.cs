using UnityEditor;
using System.IO;
using UnityEngine;
using System.Text.RegularExpressions;

public class ScriptTemplateKeywordReplacer : UnityEditor.AssetModificationProcessor
{
    //If there would be more than one keyword to replace, add a Dictionary

    public static void OnWillCreateAsset(string metaFilePath)
    {
        string fileName = Path.GetFileNameWithoutExtension(metaFilePath);

        if (!fileName.EndsWith(".cs"))
            return;


        string actualFilePath = $"{Path.GetDirectoryName(metaFilePath)}{Path.DirectorySeparatorChar}{fileName}";
        string fileNameNoExtensions = Path.GetFileNameWithoutExtension(fileName); ;

        string content = File.ReadAllText(actualFilePath);
        string newcontent = content.Replace("#PROJECTNAME#", PlayerSettings.productName);
       
        string myText = "Controller";
        string myText2 = "State";
        int index2 = fileNameNoExtensions.IndexOf(myText2);
        int index=fileNameNoExtensions.IndexOf(myText);
        if(index != -1)
        {
            newcontent = newcontent.Replace("#ENEMYNAME#", fileNameNoExtensions.Substring(0, index));
        }
        if(index2 != -1) 
        {
            newcontent = newcontent.Replace("#ENEMYNAME#", fileNameNoExtensions.Substring(0, index2));
        }
        if (content != newcontent)
        {
            File.WriteAllText(actualFilePath, newcontent);
            AssetDatabase.Refresh();
        }
    }
}