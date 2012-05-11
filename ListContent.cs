using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Kursovoj
{
    class ListContent
    {
        public ListContent(BitmapImage _cover,string _title, string _authorName, string _authorSurname,string _filePath)
        {
            Cover = _cover;
            Title = _title;
            AuthorName = _authorName;
            AuthorSurname = _authorSurname;
            FilePath = _filePath;
        }

        public BitmapImage Cover {set ; get;}
        public string Title { set; get; }
        public string AuthorName { set; get; }
        public string AuthorSurname { set; get; }
        public string FilePath { set; get; }
    }
}
