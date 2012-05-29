using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Xml;
using System.Xml.Linq;
using System.IO;
//using System.Resources.

namespace Kursovoj
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //FlowDocumentPageViewer fdpv = new FlowDocumentPageViewer();
        //FlowDocumentReader fdr = new FlowDocumentReader();
        string openedFile;

        public MainWindow(string openedFile)
        {
            InitializeComponent();

            //openedFile = ServiceClass.PreOpen(file);
            //openedFile = file;

            if (!File.Exists(openedFile))
            {
                System.Windows.MessageBox.Show("Файл не существует.");
                //Close();
            }
            else
            {
                XDocument doc = XDocument.Load(openedFile);
                fdr.Document = Parse(doc);
                //LoadBookmark();
                fdr.Focus();
            }
        }

        private FlowDocument Parse(XDocument doc)
        {
            bool coverFlag = true;

            FlowDocument fd = new FlowDocument();
            fd.Background = new SolidColorBrush(Color.FromRgb(255, 253, 225));
            fd.FontSize = 16;

            XNamespace ns = doc.Root.GetDefaultNamespace();

            //doc.Root.SetAttributeValue("SelectionNamespaces", "xmlns:xhtml=\"http://www.gribuser.ru/xml/fictionbook/2.0\"");
            
            //doc.Root.

            var all_elements = from el in doc.Root.Element(ns+"body").Descendants()
                               select el;

            foreach (var element in all_elements)
            {
                string s = element.Name.ToString();
                XName n = element.Name;
                switch (element.Name.ToString())
                {
                    case "{http://www.gribuser.ru/xml/fictionbook/2.0}title":
                        if (coverFlag)
                        {
                            BitmapImage img = ServiceClass.ImageFromByte(ServiceClass.GetCover(doc));
                            fd.Blocks.Add(new BlockUIContainer(new Image() { Source = img, Width = img.PixelWidth, Height = img.PixelHeight }));
                            coverFlag = false;
                        }
                        var p = from e in element.Descendants()
                                where e.Name == "{http://www.gribuser.ru/xml/fictionbook/2.0}p"
                                select e;
                        foreach (var par in p)
                        {
                            Paragraph title_par = new Paragraph();
                            title_par.Inlines.Add(new Bold(new Run(par.Value)));
                            title_par.TextAlignment = TextAlignment.Center;
                            fd.Blocks.Add(title_par);
                        }
                        break;

                    case "{http://www.gribuser.ru/xml/fictionbook/2.0}p":
                        if (coverFlag)
                        {
                            BitmapImage img = ServiceClass.ImageFromByte(ServiceClass.GetCover(doc));
                            fd.Blocks.Add(new BlockUIContainer(new Image() { Source = img, Width = img.PixelWidth, Height = img.PixelHeight }));
                            coverFlag = false;
                        }
                        if (element.Parent.Name != "{http://www.gribuser.ru/xml/fictionbook/2.0}title")
                        {
                            Paragraph paragraph = new Paragraph();
                            paragraph.Inlines.Add(element.Value);
                            //if (paragraph.Inlines == LoadBookmark().Inlines) System.Windows.MessageBox.Show("adawdadawd");
                            fd.Blocks.Add(paragraph);
                        }
                        break;
                }
            }
            return fd;
        }

        
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            //var paginator = ((IDocumentPaginatorSource)fdr.Document).DocumentPaginator as DynamicDocumentPaginator;
            //var position = paginator.GetPagePosition(paginator.GetPage(fdr.PageNumber )) as TextPointer;
            //bookmark = position.Paragraph;
            LoadBookmark();
            var paginator = ((IDocumentPaginatorSource)fdr.Document).DocumentPaginator as DynamicDocumentPaginator;
            var position = paginator.GetPagePosition(paginator.GetPage(fdr.PageNumber)) as TextPointer;
            //FindWordFromPosition(position, "народ").Paragraph.BringIntoView();

        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            //if(bookmark!=null)
            //bookmark.BringIntoView();
            //fdr.Document.Background

            SaveBookmark();
        }

        ~MainWindow()
        {
            
        }

        //Paragraph bookmark;
        private void SaveBookmark()
        {
            if (fdr.Document == null) return;

            Paragraph bookmark;
            var paginator = ((IDocumentPaginatorSource)fdr.Document).DocumentPaginator as DynamicDocumentPaginator;
            var position = paginator.GetPagePosition(paginator.GetPage(fdr.PageNumber)) as TextPointer;
            if (position == null)
            {
                return;
            }

            bookmark = position.Paragraph;

            string serializeFileName = openedFile.Substring(0, openedFile.Length - 9) + ".s";
            ServiceClass.Serialize(bookmark, serializeFileName);
        }

        TextPointer FindWordFromPosition(TextPointer position, string word)
        {
            while (position != null)
            {
                if (position.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    string textRun = position.GetTextInRun(LogicalDirection.Forward);

                    // Find the starting index of any substring that matches "word".
                    int indexInRun = textRun.IndexOf(word);
                    if (indexInRun >= 0)
                    {
                        position = position.GetPositionAtOffset(indexInRun);
                        break;
                    }
                }
                else
                    position = position.GetNextContextPosition(LogicalDirection.Forward);
            }

            // position will be null if "word" is not found.
            return position;
        }

        private Paragraph LoadBookmark()
        {
            string serializeFileName = openedFile.Substring(0, openedFile.Length - 4) + ".s";
            if (File.Exists(serializeFileName))
            {
                Paragraph bookmark = ServiceClass.Deserialize(serializeFileName);
                if(fdr.Document.Blocks.Contains(bookmark)) System.Windows.MessageBox.Show("adawdadawd");
                
                //var q = from i in fdr.Document.Blocks
                //        where i is Paragraph 
                //        select i;

                //foreach (var item in q)
                //{
                //    Paragraph p = (Paragraph)item;
                //    if (p.Inlines == bookmark.Inlines) System.Windows.MessageBox.Show("adawdadawd");
                //}

                if(bookmark != null)
                {
                    bookmark.BringIntoView();
                }
                return bookmark;
            }
            else
            {
                return null;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //SaveBookmark();
            //File.Delete(openedFile);
        }
    }
}

