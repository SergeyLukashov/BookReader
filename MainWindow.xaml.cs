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

namespace Kursovoj
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        FlowDocumentPageViewer fdpv = new FlowDocumentPageViewer();
        //FlowDocumentReader fdr = new FlowDocumentReader();
        
        public MainWindow()
        {
            InitializeComponent();


            Paragraph p = new Paragraph();
            XmlDocument xmld = new XmlDocument();
            xmld.Load("2.fb2");
            //if (xmld.InnerXml!="")///!!!
            {
                p.Inlines.Add(xmld.InnerXml);
                FlowDocument fd = new FlowDocument();
                fd.Blocks.Add(p);
                fd.Background = Brushes.Beige;
                fd.ColumnRuleBrush = Brushes.Beige;
                fd.FontSize = 16;

                fdr.Document = fd;

                fdr.Focus();
            }
        }

        Paragraph bookmark;
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            var paginator = ((IDocumentPaginatorSource)fdr.Document).DocumentPaginator as DynamicDocumentPaginator;
            var position = paginator.GetPagePosition(paginator.GetPage(fdr.PageNumber - 1)) as TextPointer;
            bookmark = position.Paragraph;
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            bookmark.BringIntoView();
        }
    }
}
