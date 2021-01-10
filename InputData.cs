using System;
using System.Windows;

namespace ttn
{
    /// <summary>
    /// user input data container
    /// </summary>
    class InputData
    {
        public string Series; //series of documents
        public int From; //first document in range
        public int To; //last document in range
        public DateTime Date; //operation date


        public InputData(MainWindow window)
        {
            Series = window.SeriesTextBox.Text;
            int.TryParse(window.FromTextBox.Text, out From);
            int.TryParse(window.ToTextBox.Text, out To);
            if (window.OperationDate.SelectedDate != null)
            {
                Date = (DateTime)window.OperationDate.SelectedDate;
            }
            else
            {
                Date = default;
            }
        }

        /// <summary>
        /// check if series of documents, first and last document was given
        /// </summary>
        /// <returns>true if series of documents, first and last document was given</returns>
        public bool CheckSeriesFromTo()
        {
            if (Series == default)
            {
                MessageBox.Show("Введите серию");
                return false;
            }
            if (From == default)
            {
                MessageBox.Show("Введите первый номер диапазона");
                return false;
            }
            if (To == default)
            {
                MessageBox.Show("Введите последний номер диапазона");
                return false;
            }
            return true;
        }

        /// <summary>
        /// check if all user input data was given
        /// </summary>
        /// <returns>true if all user input data was given</returns>
        public bool CheckAll()
        {
            if (CheckSeriesFromTo() && Date != default)
            {
                return true;
            }
            else
            {
                MessageBox.Show("Введите дату");
                return false;
            }
        }
    }
}
