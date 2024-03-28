using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class Demo : MonoBehaviour
{
    private const string DB_TABLE_USER_INFO = "userInfo";

    // Start is called before the first frame update
    private void Start()
    {
        void View()
        {
            XmlNodeList userInfoList = DBConnector.Select(DB_TABLE_USER_INFO);

            // how to use?
            foreach (XmlNode node in userInfoList)
            {
                Debug.Log(node.ParseXmlNode<int>("uID"));
                Debug.Log(node.ParseXmlNode<string>("id"));
            }

            Debug.Log("=================================================");
        }

        View();

        if (DBConnector.Insert(DB_TABLE_USER_INFO, "id", "ssg88"))
        {
            Debug.Log("입력 성공");
            View();
        }

        if (DBConnector.Insert(DB_TABLE_USER_INFO, new string[] { "0", "ssg9" }))
        {
            Debug.Log("입력 성공");
            View();
        }

        if(DBConnector.Update(DB_TABLE_USER_INFO, "id", "ssg101", "uID = 4"))
        {
            Debug.Log("변경 성공");
            View();
        }

        if (DBConnector.Delete(DB_TABLE_USER_INFO, "id = 'ssg88'"))
        {
            Debug.Log("제거 성공");
            View();
        }
    }
}
