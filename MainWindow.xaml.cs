using System;
using System.Windows;

namespace ttn
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// add given range to database
        /// </summary>
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            //create database connection
            DocumentController docCtrl = InitializeDB();
            //create input data from user input
            InputData input = new InputData((MainWindow)MainWindow.GetWindow(this));
            //check if input data is correct for this operation
            if (!input.CheckAll())
            {
                ResultStatusBarItem.Content = "Ошибка! Некорректные данные. Не сохранено.";
                return;
            }
            //process given data
            try
            {
                docCtrl.AddRange(input.Series, input.From, input.To, input.Date);
                ResultStatusBarItem.Content = String.Format("Сохранено. Серия {0} с {1} по {2}", input.Series, input.From, input.To);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
            {
                MessageBox.Show(ex.Message);
                ResultStatusBarItem.Content = String.Format("Ошибка! Не сохранено. Серия {0} с {1} по {2}", input.Series, input.From, input.To);
                return;
            }
        }

        /// <summary>
        /// mark given range as used
        /// </summary>
        private void UsedButton_Click(object sender, RoutedEventArgs e)
        {
            //create database connection
            DocumentController docCtrl = InitializeDB();
            //create input data from user input
            InputData input = new InputData((MainWindow)MainWindow.GetWindow(this));
            //check if input data is correct for this operation
            if (!input.CheckSeriesFromTo())
            {
                ResultStatusBarItem.Content = "Ошибка! Некорректные данные. Не сохранено.";
                return;
            }
            //process given data
            try
            {
                docCtrl.UsedRange(input.Series, input.From, input.To, input.Date);
                ResultStatusBarItem.Content = String.Format("Использовано. Серия {0} с {1} по {2}", input.Series, input.From, input.To);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
            {
                MessageBox.Show(ex.Message);
                ResultStatusBarItem.Content = String.Format("Ошибка! Не сохранено. Серия {0} с {1} по {2}", input.Series, input.From, input.To);
                return;
            }
        }

        /// <summary>
        /// delete given range from database
        /// </summary>
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            //create database connection
            DocumentController docCtrl = InitializeDB();
            //create input data from user input
            InputData input = new InputData((MainWindow)MainWindow.GetWindow(this));
            //check if input data is correct for this operation
            if (!input.CheckSeriesFromTo())
            {
                ResultStatusBarItem.Content = "Ошибка! Некорректные данные. Не сохранено.";
                return;
            }
            //process given data
            try
            {
                docCtrl.DeleteRange(input.Series, input.From, input.To);
                ResultStatusBarItem.Content = String.Format("Удалено. Серия {0} с {1} по {2}", input.Series, input.From, input.To);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException ex)
            {
                MessageBox.Show("Document does not exist");
                ResultStatusBarItem.Content = String.Format("Ошибка! Не сохранено. Серия {0} с {1} по {2}", input.Series, input.From, input.To);
                return;
            }
        }

        /// <summary>
        /// mark given range as spoiled
        /// </summary>
        /// <param name="sender"></param>
        private void SpoiledButton_Click(object sender, RoutedEventArgs e)
        {
            //create database connection
            DocumentController docCtrl = InitializeDB();
            //create input data from user input
            InputData input = new InputData((MainWindow)MainWindow.GetWindow(this));
            //check if input data is correct for this operation
            if (!input.CheckSeriesFromTo())
            {
                ResultStatusBarItem.Content = "Ошибка! Некорректные данные. Не сохранено.";
                return;
            }
            //process given data
            try
            {
                docCtrl.SpoiledRange(input.Series, input.From, input.To);
                ResultStatusBarItem.Content = String.Format("Испорчено. Серия {0} с {1} по {2}", input.Series, input.From, input.To);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
            {
                MessageBox.Show(ex.Message);
                ResultStatusBarItem.Content = String.Format("Ошибка! Не сохранено. Серия {0} с {1} по {2}", input.Series, input.From, input.To);
                return;
            }

        }

        /// <summary>
        /// mark given range as normal if it was used
        /// </summary>
        private void UnusedButton_Click(object sender, RoutedEventArgs e)
        {
            //create database connection
            DocumentController docCtrl = InitializeDB();
            //create input data from user input
            InputData input = new InputData((MainWindow)MainWindow.GetWindow(this));
            //check if input data is correct for this operation
            if (!input.CheckSeriesFromTo())
            {
                ResultStatusBarItem.Content = "Ошибка! Некорректные данные. Не сохранено.";
                return;
            }
            //process given data
            try
            {
                docCtrl.UnusedRange(input.Series, input.From, input.To);
                ResultStatusBarItem.Content = String.Format("Не использовано. Серия {0} с {1} по {2}", input.Series, input.From, input.To);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
            {
                MessageBox.Show(ex.Message);
                ResultStatusBarItem.Content = String.Format("Ошибка! Не сохранено. Серия {0} с {1} по {2}", input.Series, input.From, input.To);
                return;
            }
        }

        /// <summary>
        /// mark given range as normal if it was spoiled
        /// </summary>
        private void UnspoiledButton_Click(object sender, RoutedEventArgs e)
        {
            //create database connection
            DocumentController docCtrl = InitializeDB();
            //create input data from user input
            InputData input = new InputData((MainWindow)MainWindow.GetWindow(this));
            if (!input.CheckSeriesFromTo())
            {
                ResultStatusBarItem.Content = "Ошибка! Некорректные данные. Не сохранено.";
                return;
            }

            try
            {
                docCtrl.UnspoiledRange(input.Series, input.From, input.To);
                ResultStatusBarItem.Content = String.Format("Не испорчено. Серия {0} с {1} по {2}", input.Series, input.From, input.To);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
            {
                MessageBox.Show(ex.Message);
                ResultStatusBarItem.Content = String.Format("Ошибка! Не сохранено. Серия {0} с {1} по {2}", input.Series, input.From, input.To);
                return;
            }
        }

        /// <summary>
        /// create from db and write report
        /// </summary>
        private void CreateReportButton_Click(object sender, RoutedEventArgs e)
        {
            //create database connection
            DocumentController docCtrl = InitializeDB();
            //create report workbook
            Report report = new Report();
            //create save path user dialog
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            //create report
            report.WriteList(docCtrl.GetLeftAt((DateTime)ReportFromDate.SelectedDate), "остаток на начало");
            report.WriteList(docCtrl.GetAddedIn((DateTime)ReportFromDate.SelectedDate, (DateTime)ReportToDate.SelectedDate), "приход");
            report.WriteList(docCtrl.GetUsedIn((DateTime)ReportFromDate.SelectedDate, (DateTime)ReportToDate.SelectedDate), "использовано всего");
            report.WriteList(docCtrl.GetSpoiledIn((DateTime)ReportFromDate.SelectedDate, (DateTime)ReportToDate.SelectedDate), "испорчено");
            report.WriteList(docCtrl.GetUsedInWitoutSpoiled((DateTime)ReportFromDate.SelectedDate, (DateTime)ReportToDate.SelectedDate), "использовано без испорченных");
            report.WriteList(docCtrl.GetLeftAt((DateTime)ReportToDate.SelectedDate), "остаток на конец");

            //get save path from user
            if (saveFileDialog.ShowDialog() == true)
            {
                report.WriteReport(saveFileDialog.FileName);
                //tell user that report was created
                ResultStatusBarItem.Content = String.Format("Создан отчет {0}", saveFileDialog.FileName);
            }
        }

        /// <summary>
        /// connect to database
        /// </summary>
        /// <returns>database controller</returns>
        private DocumentController InitializeDB()
        {
            return new DocumentController(DatabaseTextBox.Text);
        }

        /// <summary>
        /// get or create database file
        /// </summary>
        private void DatabaseButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog()
            {
                CheckFileExists = false,
                CheckPathExists = false,
                AddExtension = true,
                DefaultExt = ".db"
            };

            if (openFileDialog.ShowDialog() == true)
                DatabaseTextBox.Text = openFileDialog.FileName;
        }
    }
}
