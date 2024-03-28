using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;

public class DBConnector : MonoBehaviour
{
    [SerializeField] private string IP = "127.0.0.1";
    [SerializeField] private int PORT = 3306;
    [SerializeField] private string ID = string.Empty;
    [SerializeField] private string PW = string.Empty;
    [SerializeField] private string DB_NAME = string.Empty;

    private static DBConnector single;

    private static MySqlConnection _connection = null;
    private static MySqlConnection connection
    {
        get
        {
            if(_connection == null)
            {
                try
                {
                    string formatSql = $"Server={single.IP}; Port={single.PORT}; Database={single.DB_NAME}; UserId={single.ID}; Password={single.PW}";
                    _connection = new MySqlConnection(formatSql);
                }catch(MySqlException e)
                {
                    Debug.LogError(e);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }

            return _connection;
                
        }
    }

    private void Awake()
    {
        single = this;
        DontDestroyOnLoad(single);
    }

    

    private static bool m_OnChange(string query)
    {
        bool result = false;
        try
        {
            MySqlCommand sqlCommand = new MySqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandText = query;

            connection.Open();

            sqlCommand.ExecuteNonQuery();

            connection.Close();

            result = true;
        }
        catch (Exception e)
        {   
            Debug.LogError(e.ToString());
        }

        connection.Close();
        return result;
    }

    private static DataSet m_OnLoad(string tableName, string query)
    {
        DataSet ds = null; ;
        try
        {
            connection.Open();   //DB 연결

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = query;

            MySqlDataAdapter sd = new MySqlDataAdapter(cmd);
            ds = new DataSet();
            sd.Fill(ds, tableName);
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }

        connection.Close();  //DB 연결 해제
        return ds;
    }

    /// <summary>
    /// 데이터 검색
    /// </summary>
    /// <param name="tableName">검색할 테이블</param>
    /// <param name="field">검색할 필드 (입력하지 않을 경우 전체 로드)</param>
    /// <param name="condition">조건</param>
    /// <returns></returns>
    public static XmlNodeList Select(string tableName, string field = "*", string condition = "")
    {
        DataSet dataSet = m_OnLoad(tableName, $"SELECT {field} FROM {tableName} {condition}");

        if (dataSet == null)
            return null;

        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(dataSet.GetXml());

        return xmlDocument.GetElementsByTagName(tableName);
    }

    /// <summary>
    /// 데이터 입력
    /// </summary>
    /// <param name="tableName">입력할 테이블</param>
    /// <param name="fieldName">입력할 필드 이름</param>
    /// <param name="value">입력할 값</param>
    /// <returns></returns>
    public static bool Insert(string tableName, string fieldName, string value)
    {
        return m_OnChange($"INSERT INTO {tableName} ({fieldName}) VALUES ('{value}')");
    }

    /// <summary>
    /// 데이터 입력 (순차적으로 전부 입력됨)
    /// </summary>
    /// <param name="tableName">입력할 테이블</param>
    /// <param name="values">입력할 값 (칼럼 순서대로 적용됨)</param>
    /// <returns></returns>
    public static bool Insert(string tableName, string[] values)
    {
        string strValues = string.Empty;

        foreach (string value in values)
        {
            if (strValues.Length > 0)
                strValues += ", ";
            strValues += $"'{value}'";
        }

        return m_OnChange($"INSERT INTO {tableName} VALUES ({strValues})");
    }

    /// <summary>
    /// 레코드 갱신
    /// </summary>
    /// <param name="tableName">입력할 테이블</param>
    /// <param name="fieldName">입력할 필드 이름</param>
    /// <param name="value">입력할 값</param>
    /// <param name="condition">조건</param>
    /// <returns></returns>
    public static bool Update(string tableName, string fieldName, string value, string condition)
    {
        return m_OnChange($"UPDATE {tableName} SET {fieldName}='{value}' WHERE {condition}");
    }

    /// <summary>
    /// 레코드 제거
    /// </summary>
    /// <param name="tableName">제거할 레코드가 포함된 테이블</param>
    /// <param name="condition">조건</param>
    /// <returns></returns>
    public static bool Delete(string tableName, string condition)
    {
        return m_OnChange($"DELETE FROM {tableName} WHERE {condition}");
    }
}

public static class DBConnecter_Expand
{
    public static T ParseXmlNode<T>(this XmlNode node, string fieldName)
    {
        return (T)System.Convert.ChangeType(node[fieldName].InnerText, typeof(T));
    }
}
