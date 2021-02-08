using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class TableLoader : Singleton<TableLoader>
{
    private List<Dictionary<string, string>> m_table = new List<Dictionary<string, string>>();
    
    public int Length { get { return m_table.Count; } }
    public void Clear()
    {
        m_table.Clear();
    }
    public string GetString(int index,string key)
    {
        return m_table[index][key];
    }
    public int GetInt(int index,string key)
    {
        return int.Parse(m_table[index][key]);
    }
    public byte[] ExcelDataLoad(string excelName)
    {
        var result = Resources.Load<TextAsset>("Prefab/ExcelDatas/" + excelName);
        return result.bytes;
    }
    public void Load(byte[] excel)
    {
        MemoryStream ms = new MemoryStream(excel);
        BinaryReader br = new BinaryReader(ms);

        int rowCount = br.ReadInt32();
        int colCount = br.ReadInt32();
        var strData = br.ReadString().Split('\t');
        int offset = 1;
        List<string> listKey = new List<string>();
        m_table.Clear();

        for(int i=0; i<rowCount; i++)
        {
            offset++;
            if(i == 0)
            {
                for(int j=0; j<colCount-1; j++)
                {
                    listKey.Add(strData[offset]);
                    offset++;
                }
            }
            else
            {
                Dictionary<string, string> dicData = new Dictionary<string, string>();
                for(int j=0; j<colCount-1; j++)
                {
                    dicData.Add(listKey[j], strData[offset]);
                    offset++;
                }
                m_table.Add(dicData);                
            }           
        }
        ms.Close();
        br.Close();
    }
}
