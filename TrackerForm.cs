using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Archery_Performance_Tracker.Utils;

namespace Archery_Performance_Tracker
{
    public partial class TrackerForm : Form
    {
        private const string nArrows = "Number of Arrows";
        private const string nLowScore = "Low Score";
        private const string nHighScore = "High Score";
        private const string nMeanScore = "Mean Score";
        
        public TrackerForm()
        {
            InitializeComponent();
            
            //clear the chart and add teh correct series
            this.chart.Series.Clear();
        }

        private void addOldValues()
        {
            
        }
        
        private void submitButton_Click(object sender, EventArgs e)
        {
            var d = dateTimePicker1.Value.ToOADate();
            int.TryParse(textBox2.Text, out var nS);

            float[] scores = null;
            
            if (!string.IsNullOrEmpty(textBox3.Text) && !string.IsNullOrWhiteSpace(textBox3.Text))
                scores = Utils.Utils.convertScoresToStringScoresSorted(textBox3.Text.Split(","));
            
            checkAndCreateSerises(scores != null);
            
            chart.Series[nArrows].Points.AddXY(d, nS);

            if (scores == null) return;
            
            //only run if scores were added
            chart.Series[nLowScore].Points.AddXY(d, scores[0]);
            chart.Series[nHighScore].Points.AddXY(d, scores[^1]);
            chart.Series[nMeanScore].Points.AddXY(d, Utils.Utils.calcualteMean(scores));
            
            Serialization.saveScores(d, nS, scores);
        }

        private void checkAndCreateSerises(bool scores = false)
        {
            if(chart.Series.FindByName(nArrows) == null)
                createSeries(nArrows, new DateTime(2020, 6, 1), DateTime.Now, Color.Aquamarine);

            if (!scores) return;
            
            if (chart.Series.FindByName(nLowScore) == null)
                createSeries(nLowScore, new DateTime(2020, 6, 1), DateTime.Now, Color.Red);
                
            if (chart.Series.FindByName(nHighScore) == null)
                createSeries(nHighScore, new DateTime(2020, 6, 1), DateTime.Now, Color.Green);
                
            if (chart.Series.FindByName(nMeanScore) == null)
                createSeries(nMeanScore, new DateTime(2020, 6, 1), DateTime.Now, Color.Orange);
        }

        private void createSeries(string sName, DateTime minDate, DateTime maxDate, Color lineColour)
        {
            //add a serise with the correct name
            this.chart.Series.Add(sName);
            
            //get the chart area
            var chart = this.chart.ChartAreas[0];
            
            //setup x-axis
            chart.AxisX.LabelStyle.Format = "dd/MM/yyyy";
            chart.AxisX.Interval = 2; // show the date every other interval
            chart.AxisX.IntervalType = DateTimeIntervalType.Days;
            chart.AxisX.IntervalOffset = 1;

            chart.AxisX.Minimum = minDate.AddSeconds(-1).ToOADate(); // remove a second so that the min date is usable
            chart.AxisX.Maximum = maxDate.ToOADate(); // add a second so the final time can be used

            //set the x axis properly
            this.chart.Series[sName].XValueType = ChartValueType.DateTime;
            
            this.chart.Series[sName].ChartType = SeriesChartType.Line;
            this.chart.Series[sName].Color = lineColour;
            this.chart.Series[sName].IsVisibleInLegend = true;
        }
    }
}