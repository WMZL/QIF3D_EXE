using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
{
"Type":"one",
    "isAlram": 1,
    "ID": "b2d70853c3c04976ae48e6f586c5cae4",
    "address": {
        "street": "科技园路.",
        "city": "江苏苏州",
        "country": "中国"
    },
    "links": [
        {
            "name": "Google",
            "url": "http://www.google.com"
        },
        {
            "name": "Baidu",
            "url": "http://www.baidu.com"
        },
        {
            "name": "SoSo",
            "url": "0b72ad2cf1874169a6e9f1a70522dcc1"
        }
    ]
}
 */

public class one
{
    public int isAlram;
    public string ID;
    public addressinfo address;
    public List<arr> links;

    public class addressinfo
    {
        public string street;
        public string city;
        public string country;

    }
    public class arr
    {
        public string name;
        public string url;
    }
}
