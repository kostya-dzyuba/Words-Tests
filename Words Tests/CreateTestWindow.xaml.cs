﻿using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Words_Tests
{
    public partial class CreateTestWindow : Window
    {
        public CreateTestWindow()
        {
            InitializeComponent();
            for (int i = 0; i < 7; i++)
            {
                AddQuestion(null, null);
            }
        }

        private void AddQuestion(object sender, RoutedEventArgs e)
        {
            TextBlock textBlockQuestion = new TextBlock { Text = $"Вопрос {listBoxQuestions.Items.Count + 1}:", };
            Grid.SetColumn(textBlockQuestion, 0);
            Grid.SetRow(textBlockQuestion, listBoxQuestions.Items.Count);

            TextBox textBoxQuestion = new TextBox();
            Grid.SetColumn(textBoxQuestion, 1);
            Grid.SetRow(textBoxQuestion, listBoxQuestions.Items.Count);

            TextBlock textBlockAnswer = new TextBlock { Text = $"Ответ {listBoxQuestions.Items.Count + 1}:", };
            Grid.SetColumn(textBlockAnswer, 2);
            Grid.SetRow(textBlockAnswer, listBoxQuestions.Items.Count);

            TextBox textBoxAnswer = new TextBox();
            Grid.SetColumn(textBoxAnswer, 3);
            Grid.SetRow(textBoxAnswer, listBoxQuestions.Items.Count);

            CheckBox checkBoxRemove = new CheckBox { IsTabStop = false };
            checkBoxRemove.Click += (s, ea) =>
                ((ListBoxItem)listBoxQuestions.ItemContainerGenerator.ContainerFromItem((Grid)checkBoxRemove.Parent)).IsSelected = checkBoxRemove.IsChecked.Value;
            Grid.SetColumn(checkBoxRemove, 4);
            Grid.SetRow(checkBoxRemove, listBoxQuestions.Items.Count);

            Grid grid = new Grid();

            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            grid.Children.Add(textBlockQuestion);
            grid.Children.Add(textBoxQuestion);
            grid.Children.Add(textBlockAnswer);
            grid.Children.Add(textBoxAnswer);
            grid.Children.Add(checkBoxRemove);

            listBoxQuestions.Items.Add(grid);
        }

        private void RemoveQuestions(object sender, RoutedEventArgs e)
        {
            for (int i = listBoxQuestions.SelectedItems.Count - 1; i >= 0; i--)
                listBoxQuestions.Items.Remove(listBoxQuestions.SelectedItems[i]);
            for (int i = 0; i < listBoxQuestions.Items.Count; i++)
            {
                Grid grid = (Grid)listBoxQuestions.Items[i];
                ((TextBlock)grid.Children[0]).Text = $"Вопрос {i + 1}:";
                ((TextBlock)grid.Children[2]).Text = $"Ответ {i + 1}:";
            }
        }

        private void ListBoxQuestions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (Grid grid in listBoxQuestions.Items)
                ((CheckBox)grid.Children[4]).IsChecked = ((ListBoxItem)listBoxQuestions.ItemContainerGenerator.ContainerFromItem(grid)).IsSelected;
        }

        private void SaveTest(object sender, RoutedEventArgs e)
        {
            var pairs = new List<(string answer, string question)>();
            foreach (Grid grid in listBoxQuestions.Items)
            {
                TextBox textBoxQuestion = (TextBox)grid.Children[1];
                TextBox textBoxAnswer = (TextBox)grid.Children[3];
                if (textBoxQuestion.Text == "" || textBoxAnswer.Text == "")
                    continue;
                pairs.Add((textBoxQuestion.Text, textBoxAnswer.Text));
            }
            if (pairs.Count == 0)
            {
                MessageBox.Show("Нет ни одной действительной пары вопрос-ответ", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            string name = Interaction.InputBox("Введите имя теста");
            if (name == "")
                return;
            string newFilePath = Path.Combine(App.testsDir, name) + ".xml";
            if (File.Exists(newFilePath))
            {
                MessageBox.Show("Тест с таким названием уже существует, попробуйте другое имя", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!Directory.Exists(App.testsDir))
                Directory.CreateDirectory(App.testsDir);
            using (FileStream fs = File.OpenWrite(newFilePath))
                App.serializer.Serialize(fs, pairs);
            MessageBox.Show("Тест сохранён", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);

            Close();
        }

        private void Window_Closing(object sender, EventArgs e)
        {
            new MainWindow
            {
                Left = Left,
                Top = Top
            }.Show();
        }
    }
}
