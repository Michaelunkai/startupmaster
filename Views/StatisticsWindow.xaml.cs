using StartupMaster.Models;
using StartupMaster.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace StartupMaster.Views
{
    public partial class StatisticsWindow : Window
    {
        private readonly List<StartupItem> _items;
        private readonly StartupImpactCalculator _calculator;
        private readonly PerformanceAnalyzer _analyzer;

        public StatisticsWindow(List<StartupItem> items)
        {
            InitializeComponent();
            _items = items;
            _calculator = new StartupImpactCalculator();
            _analyzer = new PerformanceAnalyzer();
            
            LoadStatistics();
        }

        private void LoadStatistics()
        {
            // Summary cards
            TotalItemsText.Text = _items.Count.ToString();
            EnabledText.Text = _items.Count(i => i.IsEnabled).ToString();
            
            var highImpact = _calculator.GetHighImpactItems(_items);
            HighImpactText.Text = highImpact.Count.ToString();
            
            var bootTime = _calculator.EstimateBootTimeSeconds(_items);
            BootTimeText.Text = $"{bootTime}s";

            // Performance rating
            var rating = GetPerformanceRating(_items.Count(i => i.IsEnabled));
            RatingText.Text = rating.rating;
            RatingText.Foreground = new System.Windows.Media.SolidColorBrush(
                (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(rating.color));
            RatingDescription.Text = rating.description;

            // By location
            var byLocation = _items.GroupBy(i => i.LocationDisplay)
                .Select(g => $"{g.Key}: {g.Count()} items ({g.Count(i => i.IsEnabled)} enabled)")
                .ToList();
            LocationList.ItemsSource = byLocation;

            // Recommendations
            var report = _analyzer.AnalyzeStartupItems(_items);
            RecommendationsList.ItemsSource = report.Recommendations;

            // High impact items with suggestions
            var highImpactData = highImpact.Select(item => new
            {
                Impact = $"{_calculator.GetImpactEmoji(_calculator.CalculateImpact(item))} {_calculator.CalculateImpact(item)}/10",
                item.Name,
                item.Command,
                Suggestion = _calculator.GetOptimizationSuggestion(item)
            }).ToList();
            HighImpactGrid.ItemsSource = highImpactData;

            // Issues
            IssuesList.ItemsSource = report.PotentialIssues;
        }

        private (string rating, string color, string description) GetPerformanceRating(int enabledCount)
        {
            if (enabledCount <= 10)
                return ("Excellent", "#00CC00", "Your startup configuration is optimal. Boot times should be very fast.");
            if (enabledCount <= 15)
                return ("Good", "#00AA00", "Decent configuration with room for minor improvements.");
            if (enabledCount <= 20)
                return ("Fair", "#FFA500", "Above average item count. Consider optimizing further.");
            return ("Poor", "#FF0000", "Too many startup items. Significant optimization recommended.");
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
