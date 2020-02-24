using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExcelDataReader;
using System;
using System.Data;
using System.IO;
using System.Text;
using UnityEngine.Events;

/// <summary>
/// Excel转Json类
/// </summary>
public static class XlsxReader
{
    private static string jsonOutputPath = Application.dataPath + "\\Outputs\\Resources\\Jsons";

    public static List<DataSet> Excute(string excelFile)
    {
        FileStream mStream = File.Open(excelFile, FileMode.Open, FileAccess.Read);
        IExcelDataReader mExcelReader = ExcelReaderFactory.CreateOpenXmlReader(mStream);
        DataSet excelDataSet = mExcelReader.AsDataSet();
        mStream.Close();
        //判断Excel文件中是否存在数据表
        if (excelDataSet.Tables.Count < 1)
            return null;

        List<DataSet> dataSets = new List<DataSet>();
        for (int sheetIndex = 0; sheetIndex < excelDataSet.Tables.Count; sheetIndex++)
        {   
            //防止表格没按照要求填写,则读取下一个表格
            try
            {
                //默认读取第一个数据表
                DataTable table = excelDataSet.Tables[sheetIndex];
                string jsonName = table.TableName;

                //重新构建一个DataSet
                DataSet newDataSet = new DataSet();
                newDataSet.DataSetName = jsonName;
                DataTable newTable = new DataTable();
                newDataSet.Tables.Add(newTable);

                //新构建的DataSet设置table名字
                newTable.TableName = "data";

                //判断数据表内是否存在数据
                if (table.Rows.Count < 2)
                    break;

                //读取数据表行数和列数
                int rowCount = table.Rows.Count;
                int columnCount = 0;
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    if (table.Rows[0][i] != null && !table.Rows[0][i].ToString().Equals(""))
                    {
                        columnCount++;
                    }
                    else
                    {
                        break;
                    }
                }

                //遍历每列来解析数类型
                for (int columnIndex = 0; columnIndex < columnCount; columnIndex++)
                {

                    //属性名字_类型
                    string temp = table.Rows[1][columnIndex].ToString();
                    string[] tempArry = temp.Split('_');
                    //属性名字
                    string pName = tempArry[0];
                    string typeName = tempArry[1];
                    table.Columns[columnIndex].ColumnName = pName;

                    //需要什么类型自己扩展
                    switch (typeName)
                    {
                        case "i":
                            newTable.Columns.Add(new DataColumn(pName, typeof(long)));
                            break;
                        case "s":
                            newTable.Columns.Add(new DataColumn(pName, typeof(string)));
                            break;
                        case "f":
                            newTable.Columns.Add(new DataColumn(pName, typeof(float)));
                            break;
                        case "b":
                            newTable.Columns.Add(new DataColumn(pName, typeof(bool)));
                            break;
                        default: break;
                    }

                }

                //思路来自：http://www.newtonsoft.com/json/help/html/SerializeDataSet.htm
                //读取数据
                for (int rowIndex = 2; rowIndex < rowCount; rowIndex++)
                {
                    DataRow m_newRow = newTable.NewRow();
                    for (int columnIndex = 0; columnIndex < columnCount; columnIndex++)
                    {
                        string content = table.Rows[rowIndex][columnIndex].ToString();
                        Type type = newTable.Columns[columnIndex].DataType;
                        if (type.Equals(typeof(long))
                            && (content.EndsWith("k")|| content.EndsWith("m")|| content.EndsWith("g")))
                        {
                            int unit = 0;
                            if (content.EndsWith("k"))
                            {
                                unit = 1000;
                            }
                            else if (content.EndsWith("m"))
                            {
                                unit = 1000000;
                            }
                            else if (content.EndsWith("g"))
                            {
                                unit = 1000000000;
                            }
                            float value = float.Parse(content.Remove(content.Length - 1));
                            m_newRow[table.Columns[columnIndex].ColumnName] = (long)(value * unit);
                        }
                        else
                        {
                            m_newRow[table.Columns[columnIndex].ColumnName] = table.Rows[rowIndex][columnIndex];
                        }
                    }
                    newTable.Rows.Add(m_newRow);
                }
                newDataSet.AcceptChanges();
                dataSets.Add(newDataSet);
            }
            catch (Exception e)
            {
                Dbg.LogException(e);
            }
        }

        if (dataSets != null)
        {
            ConvertDataSetToJson(dataSets);
        }
        return dataSets;
    }

    /// <summary>
    /// 将表单转为Json
    /// </summary>
    /// <param name="dataSets"></param>
    public static void ConvertDataSetToJson(List<DataSet> dataSets)
    {
        Encoding encoding = Encoding.UTF8;
        foreach (var dataSet in dataSets)
        {
            //生成Json字符串
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(dataSet, Newtonsoft.Json.Formatting.Indented);

            if (!Directory.Exists(jsonOutputPath))
            {
                Directory.CreateDirectory(jsonOutputPath);

            }
#if ENCRYPT
            string enJson = Utils.AESEncrypt(json);
#else 
            string enJson = json;
#endif

            //写入文件
            using (FileStream fileStream = new FileStream(jsonOutputPath + "/" +
                dataSet.DataSetName.ToLower() + ".json", FileMode.Create, FileAccess.Write))
            {
                using (TextWriter textWriter = new StreamWriter(fileStream, encoding))
                {
                    textWriter.Write(enJson);
                }
            }
        }
    }

}
