using System.Drawing;

namespace API_AutomationFramework.Helpers
{
    public class ComparisonResult
    {
        public bool Match { get; set; }

        public float DifferencePercentage { get; set; }

        public Image? DifferenceImage { get; set; }
    }
}
