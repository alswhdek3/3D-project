using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableEffect : Singleton<TableEffect>
{
    public class Data
    {
        public int Id;
        public string Dummy;
        public string[] Prefab = new string[4];
    }

    public Dictionary<int, Data> m_dicData = new Dictionary<int, Data>();

    public void Load()
    {
        var excelData = TableLoader.Instance.ExcelDataLoad("Effect");
        TableLoader.Instance.Load(excelData);
        m_dicData.Clear();
        for(int i=0; i<TableLoader.Instance.Length; i++)
        {
            Data data = new Data();
            data.Id = TableLoader.Instance.GetInt(i, "Id");
            data.Dummy = TableLoader.Instance.GetString(i, "Dummy");
            for(int j=0; j<data.Prefab.Length; j++)
            {
                data.Prefab[j] = TableLoader.Instance.GetString(i, "Prefab_" + (j + 1));
            }
            m_dicData.Add(data.Id, data);
        }
        TableLoader.Instance.Clear();
    }
}
