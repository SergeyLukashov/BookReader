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

        public MainWindow(string file)
        {
            InitializeComponent();

            openedFile = ServiceClass.PreOpen(file);
            XDocument doc = XDocument.Load(openedFile);
            

            fdr.Document = Parse(doc);
            fdr.Focus();
        }

        private FlowDocument Parse(XDocument doc)
        {
            bool coverFlag = true;

            FlowDocument fd = new FlowDocument();
            fd.Background = new SolidColorBrush(Color.FromRgb(255, 253, 225));
            fd.FontSize = 16;

            var all_elements = from el in doc.Root.Element("body").Descendants()
                               select el;

            foreach (var element in all_elements)
            {
                //string s = element.Name.ToString();
                switch (element.Name.ToString())
                {
                    case "title":
                        if (coverFlag)
                        {
                            BitmapImage img = ServiceClass.ImageFromByte(ServiceClass.GetCover(doc));
                            fd.Blocks.Add(new BlockUIContainer(new Image() { Source = img, Width = img.PixelWidth, Height = img.PixelHeight }));
                            coverFlag = false;
                        }
                        var p = from e in element.Descendants()
                                where e.Name=="p"
                                select e;
                        foreach (var par in p)
                        {
                            Paragraph title_par = new Paragraph();
                            title_par.Inlines.Add(new Bold(new Run(par.Value)));
                            title_par.TextAlignment = TextAlignment.Center;
                            fd.Blocks.Add(title_par);
                        }
                        break;

                    case "p":
                        if (coverFlag)
                        {
                            BitmapImage img = ServiceClass.ImageFromByte(ServiceClass.GetCover(doc));
                            fd.Blocks.Add(new BlockUIContainer(new Image() { Source = img, Width = img.PixelWidth, Height = img.PixelHeight }));
                            coverFlag = false;
                        }
                        if (element.Parent.Name != "title")
                        {
                            Paragraph paragraf = new Paragraph();
                            paragraf.Inlines.Add(element.Value);
                            fd.Blocks.Add(paragraf);
                        }
                        break;
                }
            }

            return fd;
        }

        Paragraph bookmark;
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            var paginator = ((IDocumentPaginatorSource)fdr.Document).DocumentPaginator as DynamicDocumentPaginator;
            //paginator
            //ser
            var position = paginator.GetPagePosition(paginator.GetPage(fdr.PageNumber )) as TextPointer;
            bookmark = position.Paragraph;
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            if(bookmark!=null)
            bookmark.BringIntoView();
        }

        ~MainWindow()
        {
            File.Delete(openedFile);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }
    }
}

