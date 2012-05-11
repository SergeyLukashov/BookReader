using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Windows.Media.Imaging;

namespace Kursovoj
{
    class ServiceClass
    {
        public static string PreOpen(string fileName)
        {
            string tempFileName = fileName.Substring(0, fileName.Length - 4) + "_temp.fb2";

            StreamReader sr = new StreamReader(fileName, System.Text.Encoding.GetEncoding(1251));
            string temp = sr.ReadToEnd().Replace("xmlns=\"http://www.gribuser.ru/xml/fictionbook/2.0\"", "xmlns:xhtml=\"http://www.gribuser.ru/xml/fictionbook/2.0\"");
            sr.Close();

            StreamWriter sw = new StreamWriter(tempFileName, false, System.Text.Encoding.GetEncoding(1251));
            sw.Write(temp);
            sw.Close();

            return tempFileName;
        }

        public static string GetCover(XDocument doc)
        {
            var image_query = (from node in doc.Root.Descendants("binary")
                               select node.Value).ToList<string>();

            if (image_query.Count != 0) return image_query[0];
            else return "Error. No image available";
        }

        public static BitmapImage ImageFromByte(string strByte)
        {
            if (strByte == "Error. No image available")
            {
                BitmapImage noCover = new BitmapImage();
                noCover.BeginInit();
                noCover.UriSource = new Uri("noCover.png", UriKind.Relative);
                noCover.CacheOption = BitmapCacheOption.OnLoad;
                noCover.EndInit();

                return noCover;
            }
            else
            {
                byte[] byteImg = Convert.FromBase64String(strByte);
                MemoryStream ms = new MemoryStream(byteImg);
                BitmapImage img = new BitmapImage();
                img.BeginInit();
                img.CacheOption = BitmapCacheOption.OnLoad;
                img.StreamSource = ms;
                img.EndInit();

                return img;
            }
        }

        public static string GetTitle(XDocument doc)
        {
            List<string> b_title = (from node in doc.Root.Descendants("book-title")
                                    select node.Value).ToList<string>();

            if (b_title.Count != 0) return b_title[0];
            else return "Error. No title available.";
        }

        public static string[] GetAuthor(XDocument doc)
        {
            string[] author = new string[2];
            List<string> q_firstName = (from node in doc.Root.Descendants("first-name")
                                        select node.Value).ToList<string>();
            if (q_firstName.Count != 0) author[0] = q_firstName[0];

            List<string> q_lastName = (from node in doc.Root.Descendants("last-name")
                                       select node.Value).ToList<string>();
            if (q_lastName.Count != 0) author[1] = q_lastName[0];

            return author;
        }
    }
}
