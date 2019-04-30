using System;
using System.Collections.Generic;
using System.IO;
using HtmlAgilityPack;
using Microsoft.Win32;

namespace eBayForm
{
    sealed class LogicController
    {
        // TODO: Check if the IE meta-tag at the beginn of the file
        public string ImportHtml()
        {
            // string which contain htmlCode
            string htmlCode;
            // FileDialog
            OpenFileDialog openFileDialog = new OpenFileDialog();
            // Only html files
            openFileDialog.Filter = "html files (*.html)|*.html";
            Nullable<bool> result = openFileDialog.ShowDialog();
            if (result == true)
            {
                // Read the file
                using (StreamReader reader = new StreamReader(openFileDialog.FileName))
                {
                    // Insert content into htmlCode
                    htmlCode = reader.ReadToEnd();
                }
                // Html agility pack
                // Class which gonna contain HtmlCode
                HtmlDocument document = new HtmlDocument();
                // Loading the Code
                document.LoadHtml(htmlCode);
                // To diplay the content rightly we need to check compatibility to InternetExplorer

                // Searching IE meta tag 
                foreach (var element in document.DocumentNode.SelectNodes("//meta/@http-equiv"))
                {
                    // If found we don't to change htmlCode
                    if (element.Attributes["content"].Value == "IE=10")
                    {
                        return htmlCode;
                    }
                }
                // Select the head-tag
                HtmlNode head = document.DocumentNode.SelectSingleNode("//head");
                // Creating meta-tag
                HtmlNode meta = document.CreateElement("meta");
                // Adding meta-tag at the beginn of head-tag
                head.PrependChild(meta);
                // Adding IE Attributes
                meta.SetAttributeValue("http-equiv", "X-UA-Compatible");
                meta.SetAttributeValue("content", "IE=10");
                // Returning the HtmlCode as string
                return document.DocumentNode.OuterHtml;
            }
            return null;
        }
    }
}
