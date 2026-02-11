using System;
using System.Windows;
using System.Windows.Controls;
using Labb3.Models;
using Labb3.ViewModels;

namespace Labb3.Views
{
    public partial class TriviaImport : Window
    {
        public TriviaImport()
        {
            InitializeComponent();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void Import_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int amount = int.Parse(tbAmount.Text);

                string category = "";
                ComboBoxItem selectedCategory = cbCat.SelectedItem as ComboBoxItem;
                if (selectedCategory != null)
                {
                    if (selectedCategory.Tag != null)
                    {
                        category = selectedCategory.Tag.ToString();
                    }
                }

                string difficulty = "";
                ComboBoxItem selectedDifficulty = cbDiff.SelectedItem as ComboBoxItem;
                if (selectedDifficulty != null)
                {
                    if (selectedDifficulty.Tag != null)
                    {
                        difficulty = selectedDifficulty.Tag.ToString();
                    }
                }

                TriviaFetcher fetcher = new TriviaFetcher();
                var results = await fetcher.GetFromApi(amount, category, difficulty);

                if (results != null && results.Count > 0)
                {
                    MainWindowViewModel vm = this.DataContext as MainWindowViewModel;

                    if (vm != null)
                    {
                        if (vm.ActivePack != null)
                        {
                            for (int i = 0; i < results.Count; i++)
                            {
                                var item = results[i];

                                string questionText = System.Net.WebUtility.HtmlDecode(item.question);
                                string correctAnswer = System.Net.WebUtility.HtmlDecode(item.correct_answer);

                                string wrong1 = "";
                                if (item.incorrect_answers.Count > 0)
                                {
                                    wrong1 = System.Net.WebUtility.HtmlDecode(item.incorrect_answers[0]);
                                }

                                string wrong2 = "";
                                if (item.incorrect_answers.Count > 1)
                                {
                                    wrong2 = System.Net.WebUtility.HtmlDecode(item.incorrect_answers[1]);
                                }

                                string wrong3 = "";
                                if (item.incorrect_answers.Count > 2)
                                {
                                    wrong3 = System.Net.WebUtility.HtmlDecode(item.incorrect_answers[2]);
                                }

                                Question newQuestion = new Question(questionText, correctAnswer, wrong1, wrong2, wrong3);
                                vm.ActivePack.Questions.Add(newQuestion);
                            }

                            string message = "Successfully imported " + results.Count + " questions!";
                            MessageBox.Show(message, "Import Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("No questions found. Try different settings.", "No Results", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                this.Close();
            }
            catch (Exception ex)
            {
                string errorMessage = "Error importing questions: " + ex.Message;
                MessageBox.Show(errorMessage, "Import Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}