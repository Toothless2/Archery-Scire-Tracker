using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Archery_Performance_Tracker.Enums;
using Archery_Performance_Tracker.JSONStuff;
using Archery_Performance_Tracker.Utils;

namespace Archery_Performance_Tracker
{
    public partial class TrackerForm : Form
    {
        private const string nArrows = "Number of Arrows";
        private const string nLowScore = "Low Score";
        private const string nHighScore = "High Score";
        private const string nMeanScore = "Mean Score";

        private ERound currentRound = ERound.ROUND_18M_30CM;
        
        public TrackerForm()
        {
            InitializeComponent();
            
            comboBox1.Items.AddRange(Enums.Constatnts.ROUND_NAMES);
            comboBox1.SelectedIndex = 0;
            
            reloadChart(Serialization.loadScores());
        }

        private void reloadChart(JSONFile d)
        {
            chart.Series.Clear();
            loadSavedValues(d);
        }
        
        private void loadSavedValues(JSONFile d)
        {
            if (d == null) return;

            var oldest = d.getOldestScore(currentRound);
            
            checkAndCreateSerises(oldest);
            
            foreach (var score in d.getRoundRoundScores(currentRound))
                addScore(score.date, score.nShots, score.scores);
        }
        
        private void submitButton_Click(object sender, EventArgs e)
        {
            var d = dateTimePicker1.Value.ToOADate();
            int.TryParse(textBox2.Text, out var nS);

            float[] scores = null;
            
            //create scores array
            if (!string.IsNullOrEmpty(textBox3.Text) && !string.IsNullOrWhiteSpace(textBox3.Text))
                scores = Utils.Utils.convertScoresToStringScoresSorted(textBox3.Text.Split(","));
            
            //create score/shot series if needed
            checkAndCreateSerises();

            if (Serialization.saveScores(d, nS, scores, currentRound) && d > Serialization.data.getRoundRoundScores(currentRound)[0].date) // rebuild whole chart if new oldest value was added or and old value was updated
                addScore(d, nS, scores);
            else
                reloadChart(Serialization.data);

        }

        private void addScore(double date, int nShots, float[] scores)
        {
            chart.Series[nArrows].Points.AddXY(date, nShots);
            
            if (scores == null) return;
            
            //only run if scores were added
            chart.Series[nLowScore].Points.AddXY(date, scores[0]);
            chart.Series[nHighScore].Points.AddXY(date, scores[^1]);
            chart.Series[nMeanScore].Points.AddXY(date, Utils.Utils.calcualteMean(scores));
        }

        private void checkAndCreateSerises(DateTime oldestDate = default)
        {
            if (oldestDate == default)
                oldestDate = new DateTime(2020, 6, 1);
            
            if(chart.Series.FindByName(nArrows) == null)
                createSeries(nArrows, oldestDate, DateTime.Now, Color.Aquamarine);

            if (chart.Series.FindByName(nLowScore) == null)
                createSeries(nLowScore, oldestDate, DateTime.Now, Color.Red);
                
            if (chart.Series.FindByName(nHighScore) == null)
                createSeries(nHighScore, oldestDate, DateTime.Now, Color.Green);
                
            if (chart.Series.FindByName(nMeanScore) == null)
                createSeries(nMeanScore, oldestDate, DateTime.Now, Color.Orange);
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
            this.chart.Series[sName].BorderWidth = 3;
            this.chart.Series[sName].MarkerStyle = MarkerStyle.Circle;
            this.chart.Series[sName].MarkerSize = 8;
            this.chart.Series[sName].IsVisibleInLegend = true;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentRound = (ERound) ((ComboBox) sender).SelectedIndex;
            
            reloadChart(Serialization.loadScores());
        }

        private void chart_Click(object sender, EventArgs e)
        {
            var hit = chart.HitTest(((MouseEventArgs) e).X, ((MouseEventArgs) e).Y);

            pointInformation.Text = "";
            
            if (hit.PointIndex >= 0)
            {
                var score = Serialization.getScore(currentRound, hit.PointIndex);

                var sInfo = $"Date: {DateTime.FromOADate(score.date).ToShortDateString()}" +
                                    $"\n\n# Shots: {score.nShots}" +
                                    $"\n\nMean: {score.scores.Sum() / score.scores.Length}" +
                                    $"\n\nScores: {string.Join(",", score.scores.Select(s => ((int)s).ToString())).Replace(",", $"\n{"".PadLeft(13)}")}";

                pointInformation.Text = sInfo;
            }
        }
    }
}