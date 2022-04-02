using System;
using System.Collections.Generic;

namespace MalodyInfoQuery.Model
{
    public class MalodyUserInfoModel
    {
        public string UserName { get; set; }

        public DateTime JoinedTime { get; set; }

        public DateTime LastPlayedTime { get; set; }
        
        public string TotalPlayedTime { get; set; }

        public string Sex { get; set; }

        public string Age { get; set; }

        public string LiveIn { get; set; }

        public long CoinCount { get; set; }

        public long Income { get; set; }

        public string ChartBeingPlayedTime { get; set; }

        public int OnSaleChartsCount { get; set; }
        
        public int NoneOnSaleChartsCount { get; set; }

        public List<string> UserRole { get; set; }

        public List<MalodyUserRankModel> MalodyUserRanks { get; set; }

        public List<MalodyUserActivityModel> MalodyUserActivities { get; set; }

        public List<string> MalodyAchievePicUrls { get; set; }

        public MalodyUserInfoModel()
        {
            UserRole = new();
            MalodyUserRanks = new();
            MalodyUserActivities = new();
            MalodyAchievePicUrls = new();
        }
    }
}