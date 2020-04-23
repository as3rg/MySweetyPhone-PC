using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MySweetyPhone_PC.Forms
{
    /// <summary>
    /// Логика взаимодействия для InputDialog.xaml
    /// </summary>
    public partial class InputDialog : Window
    {
        Boolean isDone = false;
        Type type;
        public String GetResult()
        {
            return isDone ? Result.Text : null;
        }

        public enum Type
        {
            FILENAME,
            CODE
        }
        public InputDialog(String Title, String Hint, Type type, String Default = "")
        {
            InitializeComponent();
            this.type = type;
            this.Title = Title;
            this.Hint.Text = Hint;
            this.Result.Text = Default;
            this.Result.GotFocus += delegate
            {
                ErrorBorder.Visibility = Visibility.Collapsed;
            };
        }

        public void Done(object sender, RoutedEventArgs e)
        {
            switch (type)
            {
                case Type.FILENAME:
                    if (Result.Text == "")
                    {
                        ErrorBorder.Visibility = Visibility.Visible;
                        Error.Text = "Введено пустое имя!";
                    }
                    else if (Result.Text[0] == ' ' || Result.Text[Result.Text.Length - 1] == ' ')
                    {
                        ErrorBorder.Visibility = Visibility.Visible;
                        Error.Text = "Имя не может начинаться и заканчиваться пробелом";
                    }
                    else if (Result.Text.Contains('\\')
                        || Result.Text.Contains('/')
                        || Result.Text.Contains(':')
                        || Result.Text.Contains('*')
                        || Result.Text.Contains('?')
                        || Result.Text.Contains('"')
                        || Result.Text.Contains('<')
                        || Result.Text.Contains('>')
                        || Result.Text.Contains('|'))
                    {
                        ErrorBorder.Visibility = Visibility.Visible;
                        Error.Text = "Введены недопустимые символы";
                    }
                    else
                    {
                        isDone = true;
                        Close();
                    }
                    break;
                case Type.CODE:
                    if (!Regex.IsMatch(Result.Text, @"\d{6}"))
                    {
                        ErrorBorder.Visibility = Visibility.Visible;
                        Error.Text = "Неверный формат кода!";
                    }
                    else
                    {
                        isDone = true;
                        Close();
                    }
                    break;
            }
      
        }
    }
}
