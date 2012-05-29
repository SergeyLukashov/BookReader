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


namespace Kursovoj
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string openFileName;

        public MainWindow(string openedFile)
        {
            InitializeComponent();

            if (!File.Exists(openedFile))
            {
                System.Windows.MessageBox.Show("Файл не существует.");
            }
            else
            {
                openFileName = openedFile;
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
                            fd.Blocks.Add(paragraph);
                        }
                        break;
                }
            }
            return fd;
        }
        
        ~MainWindow()
        {
            
        }

        private void SaveBookmark()
        {
            if (openFileName != null)
            {
                string path = openFileName.Substring(0, openFileName.Length - 4) + ".s";
                ServiceClass.Serialize(fdr.PageNumber, path);
            }
        }

        private void LoadBookmark()
        {
            string path = openFileName.Substring(0, openFileName.Length - 4) + ".s";
            if (!File.Exists(path))
            {
                return;
            }
            int page = ServiceClass.Deserialize(path);
            var paginator = ((IDocumentPaginatorSource)fdr.Document).DocumentPaginator as DynamicDocumentPaginator;
            var position = paginator.GetPagePosition(paginator.GetPage(page)) as TextPointer;

            position.Paragraph.BringIntoView();
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
            SaveBookmark();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (openFileName != null)
            {
                LoadBookmark();
            }
        }
    }
}

