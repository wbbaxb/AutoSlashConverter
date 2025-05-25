using System;

namespace AutoSlashConverter.Framework.Models
{
    public class ConversionHistory
    {
        public string OriginalPath { get; set; }
        public string ConvertedPath { get; set; }
        public DateTime ConvertedTime { get; set; }
        
        public ConversionHistory(string originalPath, string convertedPath)
        {
            OriginalPath = originalPath;
            ConvertedPath = convertedPath;
            ConvertedTime = DateTime.Now;
        }
        
        public string FormattedTime => ConvertedTime.ToString("yyyy-MM-dd HH:mm:ss");
    }
} 