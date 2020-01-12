using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEditor;
using UniRx.Async;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Ho.IrasutoyaSearch
{
#if UNITY_EDITOR
    static public class IrasutoyaSearcher
    {


        static string uri = "https://www.irasutoya.com/search?q=";
        static string savePath= "Assets/IrasutoyaSearch/Images";

        public static List<string> urlList;
        public static List<byte[]> dataList;
        public static List<Texture2D> imageList;

        //public static int maxImageNum = 10;

        static string ConvertTextToUTF16(string str)
        {
            byte[] data = Encoding.UTF8.GetBytes(str);
            string tmpStr = "";
            foreach (var s in data)
            {
                tmpStr += "%" + Convert.ToString(s, 16);
            }
            return tmpStr;
        }


        public static async void SearchImagesAsync(string keyword,int maxCount)
        {
            urlList = new List<string>();
            dataList = new List<byte[]>();
            imageList = new List<Texture2D>();
            var utf16str = ConvertTextToUTF16(keyword);
            var document = await ParceSearchResultAsync(utf16str);
            var boximElements = document.GetElementsByClassName("boxim");

            int cnt = 0;
            foreach (var e in boximElements)
            {


                var el = e.GetElementsByTagName("a");

                string imagePageUrl= el[0].GetAttribute("href");

                //Debug.Log(imagePageUrl);


                var doc=await ParceImagePageAsync(imagePageUrl);
                var postElements= doc.GetElementsByClassName("post");

                var separator = postElements[0].GetElementsByClassName("separator");

                if (separator.Length > 0)
                {
                    foreach(var sep in separator)
                    {



                        var img = sep.GetElementsByTagName("img");
                        foreach(var i in img)
                        {
                            if (cnt >= maxCount)
                            {
                                return;
                            }
                            var imageUrl = i.GetAttribute("src");
                            //Debug.Log(imageUrl);
                            await Task.Delay(100);
                            urlList.Add(imageUrl);
                            DownLoadImage(imageUrl, "image");
                            cnt++;
                        }
                                        
                    }

                }
                else
                {
                    var imgElements= postElements[0].GetElementsByClassName("entry")[0].GetElementsByTagName("img");

                    foreach(var img in imgElements) 
                    {
                        if (cnt >= maxCount)
                        {
                            return;
                        }
                        var imageUrl=img.GetAttribute("src");
                        //var filename = img.GetAttribute("alt");
                        //Debug.Log(imageUrl);
                        await Task.Delay(100);
                        urlList.Add(imageUrl);
                        DownLoadImage(imageUrl, "image");
                        cnt++;
                    }


                }



            }


        }
        static void DownLoadImage(string url,string filename)
        {
            float loading = 0.0f;
            UnityWebRequest webRequest = UnityWebRequest.Get(url);
            webRequest.SendWebRequest();
            while (!webRequest.isDone)
            {
                //Debug.Log(loading);
                loading = webRequest.downloadProgress;

            }
            Texture2D texture = new Texture2D(256, 256);
            texture.LoadImage(webRequest.downloadHandler.data);
            imageList.Add(texture);
            dataList.Add(webRequest.downloadHandler.data);
            string path = AssetDatabase.GenerateUniqueAssetPath(savePath + "/"+filename+".png");
            //Debug.Log(path);
            //AssetDatabase.CreateAsset(webRequest.downloadHandler.data, path);



            //ssetDatabase.Refresh();

        }

        public static void CreateAsset(byte[] data)
        {
            string path = AssetDatabase.GenerateUniqueAssetPath(savePath + "/" + "image" + ".png");
            File.WriteAllBytes(path, data);
            AssetDatabase.Refresh();
        }

        public static void CreateAsset(byte[] data,string path,bool isSprite=false)
        {
            path = AssetDatabase.GenerateUniqueAssetPath(path + "/" + "image" + ".png");
            File.WriteAllBytes(path, data);
            AssetDatabase.Refresh();
        }




        static async UniTask<IHtmlDocument> ParceSearchResultAsync(string word)
        {
            //Debug.Log(uri + word);
            using (HttpClient client=new HttpClient())
            using (var stream = await client.GetStreamAsync(new Uri(uri + word)))
            {
                await Task.Delay(100);
                var parser = new HtmlParser();
                return await parser.ParseDocumentAsync(stream);
            }
        }


        static async UniTask<IHtmlDocument> ParceImagePageAsync(string url)
        {
            using (HttpClient client = new HttpClient())
            using (var stream = await client.GetStreamAsync(new Uri(url)))
            {
                await Task.Delay(100);
                var parser = new HtmlParser();
                return await parser.ParseDocumentAsync(stream);
            }
        }



    }
#endif
}