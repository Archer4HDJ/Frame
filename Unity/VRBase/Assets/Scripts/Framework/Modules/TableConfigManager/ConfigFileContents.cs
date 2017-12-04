using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HDJ.Framework.Modules
{
    public class ConfigFileContents
    {

        public List<List<ConfigRowData>> configRowDataList = new List<List<ConfigRowData>>();

        public void RemoveFieldValue(string fieldName)
        {
            foreach (var item in configRowDataList)
            {
                foreach (var r in item)
                {
                    if (r.fieldName == fieldName)
                    {
                        item.Remove(r);
                        break;
                    }
                }
            }
        }

        public void AddRow(string fieldName, object value)
        {
            foreach (var item in configRowDataList)
            {
                bool isHave = false;
                foreach (var r in item)
                {
                    if (r.fieldName == fieldName)
                    {
                        isHave = true;
                        break;
                    }
                }

                if (!isHave)
                {
                    ConfigRowData c = new ConfigRowData();
                    c.fieldName = fieldName;
                    c.value = value;
                    item.Add(c);
                }
            }
        }
        public bool IsHaveRow(string fieldName)
        {
            foreach (var item in configRowDataList)
            {
                bool isHave = false;
                foreach (var r in item)
                {
                    if (r.fieldName == fieldName)
                    {
                        isHave = true;
                        break;
                    }
                }

                if (!isHave)
                {
                    return false;
                }
            }

            return true;
        }

        public void AddLine(TableConfigOtherInfo info)
        {
            List<ConfigRowData> data = new List<ConfigRowData>();

            foreach (var item in info.fieldInfoDic.Values)
            {
                ConfigRowData d = new ConfigRowData();
                d.fieldName = item.fieldName;
                d.value = item.defultValue;
                data.Add(d);
            }

            configRowDataList.Add(data);
        }
    }

    public class ConfigRowData
    {
        public string fieldName = "";
        public object value;
    }
}
