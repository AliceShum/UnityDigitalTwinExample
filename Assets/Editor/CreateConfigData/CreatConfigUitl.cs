﻿
using UnityEngine;
using System.IO;
using UnityEditor;

public class CreatConfigUitl {
    public static void CreatConfigFile(Object selectObj, string writePath)
    {
        string fileName = selectObj.name;
        string className = fileName;
        StreamWriter sw = new StreamWriter(Application.dataPath + writePath + className + ".cs");

        sw.WriteLine("using UnityEngine;\nusing System.Collections;\n");
        sw.WriteLine("public partial class " + className + " : GameConfigDataBase");
        sw.WriteLine("{");

        string filePath = AssetDatabase.GetAssetPath(selectObj);
        CsvStreamReader csr = new CsvStreamReader(filePath);
        for (int colNum = 1; colNum < csr.ColCount + 1; colNum++)
        {
            string fieldName = csr[2, colNum];
            string fieldType = csr[3, colNum];
            string fieldChinese = csr[1, colNum];
            sw.WriteLine("\t" + "public " + fieldType + " " + fieldName + ";" + " //" + fieldChinese);
        }
        sw.WriteLine("\t" + "protected override string getFilePath ()");
        sw.WriteLine("\t" + "{");
        //		filePath=filePath.Replace("Assets/Resources/","");
        //		filePath=filePath.Substring(0,filePath.LastIndexOf('.'));
        sw.WriteLine("\t\t" + "return " + "\"" + fileName + "\";");
        sw.WriteLine("\t" + "}");
        sw.WriteLine("}");

        sw.Flush();
        sw.Close();
        AssetDatabase.Refresh();        //这里是一个点
    }
}
