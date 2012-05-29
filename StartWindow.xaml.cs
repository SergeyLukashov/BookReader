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
        enum SortOrder { Asc, Desc };

        SortOrder titleOrder=SortOrder.Asc;
        SortOrder authorOrder = SortOrder.Asc;

        MainWindow mw;
        ObservableCollection<ListContent> content = new ObservableCollection<ListContent>();
        public StartWindow()
        {
            InitializeComponent();

            if (!File.Exists("AllBooks.xml"))
            {
                OnFirstStart();
            }

            listView1.ItemsSource=content;
            listView1.Focus();
            listView1.SelectedIndex = 0;
        }

        FolderBrowserDialog selectFolderDialog = new FolderBrowserDialog();
        private void OnFirstStart()
        {
            FirstStartWindow firstStartWindow = new FirstStartWindow();
            firstStartWindow.ShowDialog();

            selectFolderDialog.ShowNewFolderButton = false;//select books folder
            selectFolderDialog.RootFolder = Environment.SpecialFolder.MyComputer;
            DialogResult dr = selectFolderDialog.ShowDialog();

            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                XDocument tmp = new XDocument(new XElement("BookList"));
                tmp.Save("AllBooks.xml");

                Bypass(selectFolderDialog.SelectedPath);
            }
            else
            {
                Close();
            }
        }

        private void Bypass(string startFolder)
        {
            List<string> fb2Files = Directory.GetFiles(startFolder, "*.fb2", SearchOption.AllDirectories).ToList<string>();

            XDocument books = XDocument.Load("AllBooks.xml");
            books.Root.RemoveAll();
            content.Clear();
            foreach (string openedFile in fb2Files)
            {
                XDocument doc = XDocument.Load(openedFile);

                string title = ServiceClass.GetTitle(doc);
                string[] author = ServiceClass.GetAuthor(doc);
                string coverInStr = ServiceClass.GetCover(doc);
                string annotation = ServiceClass.GetAnnotation(doc);

                XElement book = new XElement("book", coverInStr);
                book.Add(new XAttribute("title", title));
                book.Add(new XAttribute("authorFN", author[0]));
                book.Add(new XAttribute("authorLN", author[1]));
                book.Add(new XAttribute("annotation", annotation));
                book.Add(new XAttribute("filePath", openedFile));

                books.Root.Add(book);

                content.Add(new ListContent(ServiceClass.ImageFromByte(coverInStr), title, author[0], author[1], openedFile, annotation));
            }
            books.Save("AllBooks.xml");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadBooks();
            Activate();
        }

        private void SortContent(Comparison<ListContent> sortFoo)
        {
            List<ListContent> temp_list = content.ToList();
            temp_list.Sort(sortFoo);

            content.Clear();
            foreach (var item in temp_list)
            {
                content.Add(item);
            }
        }

        private int AuthorAsc(ListContent ls1, ListContent ls2)
        {
            if (ls1.AuthorSurname[0] > ls2.AuthorSurname[0])
            {
                return 1;
            }
            else if (ls1.AuthorSurname[0] < ls2.AuthorSurname[0])
            {
                return -1;
            }

            return 0;
        }

        private int AuthorDesc(ListContent ls1, ListContent ls2)
        {
            if (ls1.AuthorSurname[0] < ls2.AuthorSurname[0])
            {
                return 1;
            }
            else if (ls1.AuthorSurname[0] > ls2.AuthorSurname[0])
            {
                return -1;
            }

            return 0;
        }

        private int TitleAsc(ListContent ls1, ListContent ls2)
        {
            if (ls1.Title[0] > ls2.Title[0])
            {
                return 1;
            }
            else if (ls1.Title[0] < ls2.Title[0])
            {
                return -1;
            }

            return 0;
        }

        private int TitleDesc(ListContent ls1, ListContent ls2)
        {
            if (ls1.Title[0] < ls2.Title[0])
            {
                return 1;
            }
            else if (ls1.Title[0] > ls2.Title[0])
            {
                return -1;
            }

            return 0;
        }

        private ObservableCollection<ListContent> LoadBooks()
        {
            XDocument document = XDocument.Load("AllBooks.xml");
            var q = from item in document.Root.Elements()
                    select item;

            if (content.Count != 0) content.Clear();
            foreach (var el in q)
            {
                content.Add(new ListContent(ServiceClass.ImageFromByte(el.Value), el.Attribute("title").Value, el.Attribute("authorFN").Value, el.Attribute("authorLN").Value, el.Attribute("filePath").Value,el.Attribute("annotation").Value));
            }

            return content;
        }

        private void listView1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListContent item = (ListContent)listView1.SelectedItem;

            if (item!=null)
            {
                string file = item.FilePath;
                mw = new MainWindow(file);
                mw.Show();
            }
        }

        private void listView1_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;

            ListContent item = (ListContent)listView1.SelectedItem;

            string file = item.FilePath;
            mw = new MainWindow(file);
            mw.Show();
        }

        private void listView1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListContent item = (ListContent)listView1.SelectedItem;

            if (item == null)
            {
                return;
            }

            image1.Source = item.Cover;
            textBlock1.Text = item.AuthorName+" "+item.AuthorSurname;
            textBlock2.Text = item.Title;
            //textBlock3.Text = item.Annotation;
            textBox1.Text = item.Annotation;
        }

        private void GridViewColumn_Title_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (titleOrder == SortOrder.Asc)
            {
                SortContent(TitleAsc);
                titleOrder = SortOrder.Desc;
            }
            else
            {
                SortContent(TitleDesc);
                titleOrder = SortOrder.Asc;
            }
        }

        private void GridViewColumn_Author_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (authorOrder == SortOrder.Asc)
            {
                SortContent(AuthorAsc);
                authorOrder = SortOrder.Desc;
            }
            else
            {
                SortContent(AuthorDesc);
                authorOrder = SortOrder.Asc;
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            selectFolderDialog.ShowNewFolderButton = false;//select books folder
            selectFolderDialog.RootFolder = Environment.SpecialFolder.MyComputer;
            DialogResult dr = selectFolderDialog.ShowDialog();

            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                XDocument tmp = new XDocument(new XElement("BookList"));
                tmp.Save("AllBooks.xml");

                Bypass(selectFolderDialog.SelectedPath);
            }
            else
            {
                Close();
            }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
