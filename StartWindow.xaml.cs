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
using System.Windows.Shapes;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Collections.ObjectModel;

namespace Kursovoj
{
    /// <summary>
    /// Логика взаимодействия для StartWindow.xaml
    /// </summary>
    public partial class StartWindow : Window
    {
        MainWindow mw;
        ObservableCollection<ListContent> content = new ObservableCollection<ListContent>();
        public StartWindow()
        {
            InitializeComponent();

            listView1.ItemsSource=content;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            mw = new MainWindow();
            mw.Show();
        }

        FolderBrowserDialog selectFolderDialog = new FolderBrowserDialog();
        private void button2_Click(object sender, RoutedEventArgs e)//проход
        {
            selectFolderDialog.ShowNewFolderButton = false;
            selectFolderDialog.RootFolder = Environment.SpecialFolder.MyComputer;
            selectFolderDialog.ShowDialog();

            Bypass(selectFolderDialog.SelectedPath);
        }

        private void Bypass(string startFolder)
        {
            List<string> fb2Files = Directory.GetFiles(startFolder, "*.fb2", SearchOption.AllDirectories).ToList<string>();

            XDocument books = XDocument.Load("AllBooks.xml");
            books.Root.RemoveAll();
            content.Clear();
            foreach (string file in fb2Files)
            {
                string openedFile = ServiceClass.PreOpen(file);
                XDocument doc = XDocument.Load(openedFile);

                string title = ServiceClass.GetTitle(doc);
                string[] author = ServiceClass.GetAuthor(doc);
                string coverInStr = ServiceClass.GetCover(doc);

                File.Delete(openedFile);

                XElement book = new XElement("book", coverInStr);
                book.Add(new XAttribute("title", title));
                book.Add(new XAttribute("authorFN", author[0]));
                book.Add(new XAttribute("authorLN", author[1]));

                books.Root.Add(book);

                if (coverInStr != "Error. No image available")
                    content.Add(new ListContent(ServiceClass.ImageFromByte(coverInStr), title, author[0], author[1]));
                else
                {
                    BitmapImage noCover = new BitmapImage();
                    noCover.BeginInit();
                    noCover.UriSource = new Uri("noCover.png", UriKind.Relative);
                    noCover.CacheOption = BitmapCacheOption.OnLoad;
                    noCover.EndInit();
                    content.Add(new ListContent(noCover, title, author[0], author[1]));
                }
            }
            books.Save("AllBooks.xml");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadBooks();
        }

        private ObservableCollection<ListContent> LoadBooks()
        {
            XDocument document = XDocument.Load("AllBooks.xml");
            var q = from item in document.Root.Elements()
                    select item;

            if (content.Count != 0) content.Clear();
            foreach (var el in q)
            {
                if (el.Value != "Error. No image available")
                    content.Add(new ListContent(ServiceClass.ImageFromByte(el.Value), el.Attribute("title").Value, el.Attribute("authorFN").Value, el.Attribute("authorLN").Value));
                else
                {
                    BitmapImage noCover = new BitmapImage();
                    noCover.BeginInit();
                    noCover.UriSource = new Uri("noCover.png", UriKind.Relative);
                    noCover.CacheOption = BitmapCacheOption.OnLoad;
                    noCover.EndInit();
                    content.Add(new ListContent(noCover, el.Attribute("title").Value, el.Attribute("authorFN").Value, el.Attribute("authorLN").Value));
                }
            }

            return content;
        }
    }
}
