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
        //FlowDocumentPageViewer fdpv = new FlowDocumentPageViewer();
        //FlowDocumentReader fdr = new FlowDocumentReader();
        string openedFile;

        public MainWindow(string file)
        {
            InitializeComponent();

            //string fn="2.fb2";
            openedFile = ServiceClass.PreOpen(file);
            XDocument doc = XDocument.Load(openedFile);
            FlowDocument fd = new FlowDocument();
            fd.Background = Brushes.Beige;
            fd.ColumnRuleBrush = Brushes.Beige;
            fd.FontSize = 16;

            Title = ServiceClass.GetTitle(doc);

            var title_query = (from node in doc.Root.Descendants("title")
                               select node.Value).ToList<string>();

            foreach (string parStr in title_query)
            {
                Paragraph p = new Paragraph();
                p.Inlines.Add(new Bold(new Run(parStr)));
                fd.Blocks.Add(p);
            }

            //<coverpage><image l:href="#cover.jpg"/></coverpage>
            //<binary id="cover.jpg" content-type="image/jpeg">
            
            BitmapImage img = ServiceClass.ImageFromByte(ServiceClass.GetCover(doc));

            var p_query = from node in doc.Root.Descendants("p")
                          where node.Parent.Name.ToString() != "title"
                          select node.Value;

            fd.Blocks.Add(new BlockUIContainer(new Image() { Source = img, Width = img.PixelWidth, Height = img.PixelHeight }));
            foreach (string parStr in p_query)
            {
                Paragraph p = new Paragraph();
                p.Inlines.Add(parStr);
                fd.Blocks.Add(p);   
            }
            
            fdr.Document = fd;
            fdr.Focus();
        }

        

        Paragraph bookmark;
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            var paginator = ((IDocumentPaginatorSource)fdr.Document).DocumentPaginator as DynamicDocumentPaginator;
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
    }
}

