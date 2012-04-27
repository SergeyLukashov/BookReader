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

        public MainWindow()
        {
            InitializeComponent();

            string fn="1.fb2";
            openedFile = PreOpen(fn);
            XDocument doc = XDocument.Load(openedFile);

            var query = from node in doc.Root.Descendants("p")
                        select node.Value;

            
            FlowDocument fd = new FlowDocument();
            fd.Background = Brushes.Beige;
            fd.ColumnRuleBrush = Brushes.Beige;
            fd.FontSize = 16;
            foreach (string parStr in query)
            {
                Paragraph p = new Paragraph();
                p.Inlines.Add(parStr);
                fd.Blocks.Add(p);   
            }
            fdr.Document = fd;
            fdr.Focus();
        }

        private string PreOpen(string fileName)
        {
            string tempFileName = fileName.Substring(0, fileName.Length - 4)+"_temp.fb2";

            StreamReader sr = new StreamReader(fileName, System.Text.Encoding.GetEncoding(1251));
            string temp = sr.ReadToEnd().Replace("xmlns=\"http://www.gribuser.ru/xml/fictionbook/2.0\"", "xmlns:xhtml=\"http://www.gribuser.ru/xml/fictionbook/2.0\"");
            sr.Close();

            StreamWriter sw = new StreamWriter(tempFileName, false, System.Text.Encoding.GetEncoding(1251));
            sw.Write(temp);
            sw.Close();

            return tempFileName;
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
        ~MainWindow()
        {
            File.Delete(openedFile);
        }
    }
}
