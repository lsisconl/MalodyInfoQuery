using System;

namespace MalodyInfoQuery.Model
{
    public class MalodySongModel
    {
        public string SongName { get; set; }
        public string CoverPicUrl { get; set; }
        public int Score { get; set; }
        public int Combo { get; set; }
        public float Acc { get; set; }
        public MalodyPlayJudge Judge { get; set; }
        public string PlayedTime { get; set; }
        public MalodyPlayMode Mode { get; set; }
    }
}