﻿namespace API_AutomationFramework.Helpers
{
    public class ComparisonOptions
    {
        public byte Threshold { get; set; }

        public bool CreateDifferenceImage { get; set; }

        public bool ShowCellValues { get; set; }

        public ComparisonOptions()
        {
            CreateDifferenceImage = true;
        }
    }
}
