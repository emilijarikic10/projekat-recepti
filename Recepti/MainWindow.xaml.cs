using Recepti.Models;
using Recepti.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.IO;

namespace Recepti
{
    public partial class MainWindow : Window
    {
        private User _currentUser;
        private RecipeService _recipeService = new RecipeService();
        private ObservableCollection<Recipe> _recipes;

        public MainWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;
            LoadRecipes();

            
            if (_currentUser.Role == UserRole.Visitor)
            {
                AddButton.Visibility = Visibility.Collapsed;
                DeleteButton.Visibility = Visibility.Collapsed;

                SelectColumn.Visibility = Visibility.Collapsed;
                SelectAllCheckbox.Visibility = Visibility.Collapsed;

                RecipeDataGrid.IsReadOnly = true;
            }
        }

    
        private void LoadRecipes()
        {
            var list = _recipeService.LoadRecipes() ?? new List<Recipe>();

            foreach (var r in list)
            {
                if (!string.IsNullOrEmpty(r.ImagePath))
                {
                    r.ImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, r.ImagePath);
                }
            }

            _recipes = new ObservableCollection<Recipe>(list);
            RecipeDataGrid.ItemsSource = _recipes;
        }

        
        private void SelectAll_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var r in _recipes)
                r.IsSelected = true;
            RecipeDataGrid.Items.Refresh();
        }

        private void SelectAll_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (var r in _recipes)
                r.IsSelected = false;
            RecipeDataGrid.Items.Refresh();
        }

        
        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Hyperlink link)
            {
                if (link.Tag == null)
                    return;

                int recipeId = (int)link.Tag;

                var recipe = _recipes.FirstOrDefault(r => r.Id == recipeId);
                if (recipe != null)
                {
                    if (_currentUser.Role == UserRole.Admin)
                    {
                        AddEditRecipeWindow editWindow = new AddEditRecipeWindow(recipe);
                        if (editWindow.ShowDialog() == true)
                            LoadRecipes();
                    }
                    else
                    {
                        DetailViewWindow detailWindow = new DetailViewWindow(recipe);
                        detailWindow.ShowDialog();
                    }
                }
            }
        }

        
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            AddEditRecipeWindow addWindow = new AddEditRecipeWindow();
            if (addWindow.ShowDialog() == true)
                LoadRecipes();
        }

        
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var selectedRecipes = _recipes.Where(r => r.IsSelected).ToList();
            if (selectedRecipes.Count == 0)
            {
                MessageBox.Show("Please select at least one recipe to delete.", "Delete",
                                 MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Are you sure you want to delete {selectedRecipes.Count} recipe(s)?",
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                foreach (var r in selectedRecipes)
                    _recipeService.DeleteRecipe(r.Id);

                LoadRecipes();
            }
        }

        
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow login = new LoginWindow();
            login.Show();
            this.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
