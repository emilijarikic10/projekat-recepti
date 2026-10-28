using Microsoft.Win32;
using Recepti.Services;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Recepti
{
    public partial class AddEditRecipeWindow : Window
    {
        private Recipe _recipe;
        private RecipeService _recipeService = new RecipeService();
        private string _imagePath;

        public AddEditRecipeWindow(Recipe recipe = null)
        {

            this.Opacity = 0;

            var anim = new System.Windows.Media.Animation.DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300));
            this.BeginAnimation(Window.OpacityProperty, anim);

            InitializeComponent();
            _recipe = recipe;

            if (recipe != null)
                LoadRecipeData();

            InstructionsBox.TextChanged += InstructionsBox_TextChanged;
        }

        private void LoadRecipeData()
        {
            TitleBox.Text = _recipe.Title;
            ServingsBox.Text = _recipe.Servings.ToString();
            _imagePath = _recipe.ImagePath;

            string fullImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _recipe.ImagePath);

            _imagePath = fullImagePath;

            if (File.Exists(fullImagePath))
                ImagePreview.Source = new BitmapImage(new Uri(fullImagePath, UriKind.Absolute));

            string fullRtfPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _recipe.RtfPath);

            if (File.Exists(fullRtfPath))
            {
                TextRange range = new TextRange(InstructionsBox.Document.ContentStart, InstructionsBox.Document.ContentEnd);
                using (FileStream fs = new FileStream(fullRtfPath, FileMode.Open))
                    range.Load(fs, DataFormats.Rtf);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DoubleAnimation fade = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(1000)
            };

            this.BeginAnimation(Window.OpacityProperty, fade);
        }

        private void ChooseImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";

            if (dlg.ShowDialog() == true)
            {
                _imagePath = dlg.FileName;
                ImagePreview.Source = new BitmapImage(new Uri(_imagePath, UriKind.RelativeOrAbsolute));
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            bool isNewRecipe = false;
            TitleError.Text = "";
            ServingsError.Text = "";
            ImageError.Text = "";
            InstructionsError.Text = "";

            bool isValid = true;
            
            if (string.IsNullOrWhiteSpace(TitleBox.Text))
            {
                TitleError.Text = "Title is required";
                isValid = false;
            }

            //if (!int.TryParse(ServingsBox.Text, out int servings))
            //{
            // ServingsError.Text = "Servings must be a number";
            //isValid = false;
            // }

            int servings = 0;

            if (!string.IsNullOrWhiteSpace(ServingsBox.Text))
            {
                if (!int.TryParse(ServingsBox.Text, out servings))
                {
                    ServingsError.Text = "Servings must be a number";
                    isValid = false;
                }
            }

            if (string.IsNullOrEmpty(_imagePath))
            {
                ImageError.Text = "Image must be selected";
                isValid = false;
            }

            TextRange textRange = new TextRange(InstructionsBox.Document.ContentStart, InstructionsBox.Document.ContentEnd);
            if (string.IsNullOrWhiteSpace(textRange.Text.Trim()))
            {
                InstructionsError.Text = "Instructions cannot be empty";
                isValid = false;
            }

            if (!isValid)
                return;
            if (_recipe == null)
            {
                _recipe = new Recipe();
                _recipe.Id = _recipeService.GetNextRecipeId();
                _recipe.DateAdded = DateTime.Now;
                isNewRecipe = true;
            }

            _recipe.Title = TitleBox.Text;
            _recipe.Servings = servings;

            
            string imagesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
            if (!Directory.Exists(imagesFolder))
                Directory.CreateDirectory(imagesFolder);

            string imageExtension = Path.GetExtension(_imagePath);
            string imageFileName = Path.Combine(imagesFolder, $"recipe_{_recipe.Id}{imageExtension}");

         
            string sourceFullPath = Path.GetFullPath(_imagePath);
            string destinationFullPath = Path.GetFullPath(imageFileName);

            if (!sourceFullPath.Equals(destinationFullPath, StringComparison.OrdinalIgnoreCase))
            {
                File.Copy(sourceFullPath, destinationFullPath, true);
            }

            _recipe.ImagePath = $"Images/recipe_{_recipe.Id}{imageExtension}";

           
            string rtfFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RtfFiles");
            if (!Directory.Exists(rtfFolder))
                Directory.CreateDirectory(rtfFolder);

            string rtfFileName = Path.Combine(rtfFolder, $"recipe_{_recipe.Id}.rtf");
            TextRange range = new TextRange(InstructionsBox.Document.ContentStart, InstructionsBox.Document.ContentEnd);

            using (FileStream fs = new FileStream(rtfFileName, FileMode.Create))
                range.Save(fs, DataFormats.Rtf);

            _recipe.RtfPath = $"RtfFiles\\recipe_{_recipe.Id}.rtf";

            
            if (isNewRecipe)
                _recipeService.AddRecipe(_recipe);
            else
                _recipeService.UpdateRecipe(_recipe);

            MessageBox.Show("Recipe saved successfully!","Success",
                             MessageBoxButton.OK, MessageBoxImage.Information);
            this.DialogResult = true;
            this.Close();
        }
        private void Bold_Click(object sender, RoutedEventArgs e)
        {
            EditingCommands.ToggleBold.Execute(null, InstructionsBox);
        }

        private void Italic_Click(object sender, RoutedEventArgs e)
        {
            EditingCommands.ToggleItalic.Execute(null, InstructionsBox);
        }
        private void Underline_Click(object sender, RoutedEventArgs e)
        {
            EditingCommands.ToggleUnderline.Execute(null, InstructionsBox);
        }

        private void FontComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FontComboBox.SelectedItem == null) return;

            InstructionsBox.Focus();

            InstructionsBox.Selection.ApplyPropertyValue(
                TextElement.FontFamilyProperty,
                FontComboBox.SelectedItem);
        }

        private void FontSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FontSizeComboBox.SelectedItem == null) return;

            InstructionsBox.Focus();

            InstructionsBox.Selection.ApplyPropertyValue(
                TextElement.FontSizeProperty,
                FontSizeComboBox.SelectedItem);
        }
        private void ColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ColorComboBox.SelectedItem == null) return;

            InstructionsBox.Focus();

            var brush = (SolidColorBrush)new BrushConverter()
                .ConvertFromString(ColorComboBox.SelectedItem.ToString());

            InstructionsBox.Selection.ApplyPropertyValue(
                TextElement.ForegroundProperty,
                brush);
        }

        private void InstructionsBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            TextRange textRange = new TextRange(InstructionsBox.Document.ContentStart, InstructionsBox.Document.ContentEnd);
            string text = textRange.Text.Trim();

            int wordCount = string.IsNullOrEmpty(text) ? 0 : text.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;

            WordCountText.Text = $"Words: {wordCount}";
        }

        private void FontComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            FontComboBox.ItemsSource = Fonts.SystemFontFamilies;
        }

        private void FontSizeComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            FontSizeComboBox.ItemsSource = new List<double>
                      {
                         10, 12, 14, 16, 18, 20, 24, 28, 32
                      };
        }

        private void ColorComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            ColorComboBox.ItemsSource = new List<string>
                  {
                       "Black","Red","Blue","Green","Purple"
                  };
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
