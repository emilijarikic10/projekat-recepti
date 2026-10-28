
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Recepti
{
    public partial class DetailViewWindow : Window
    {
        public DetailViewWindow(Recipe recipe)
        {
            InitializeComponent();

            TitleText.Text = recipe.Title;
            ServingsText.Text = recipe.Servings.ToString();
            DateAddedText.Text = recipe.DateAdded.ToString("dd.MM.yyyy");

            string fullImagePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, recipe.ImagePath);

            if (File.Exists(fullImagePath))
            {
                RecipeImage.Source = new BitmapImage(new Uri(fullImagePath, UriKind.Absolute));
            }

            if (File.Exists(recipe.RtfPath))
            {
                TextRange range = new TextRange(InstructionsBox.Document.ContentStart, InstructionsBox.Document.ContentEnd);
                using (FileStream fs = new FileStream(recipe.RtfPath, FileMode.Open))
                    range.Load(fs, DataFormats.Rtf);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
