using System.Windows.Controls;

namespace ProjectTimeTracker
{
    public partial class TimeTrackerWindowControl : UserControl
    {
        public TimeTrackerWindowControl()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            var timeData = TimeDataHelper.LoadTimeData();
            TimeDataGrid.ItemsSource = timeData;
        }
    }
}
