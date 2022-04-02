using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MalodyInfoQuery.Model;
using MalodyInfoQuery.Utils;

namespace MalodyInfoQuery.Core
{
    public class Malody
    {
        private static string _xCsrfToken = "";

        private static string _sessionId = "";

        private static string _malodyBaseUrl = null!;
        
        private static DateTime _xCsrfTokenExpiredDate = DateTime.MinValue;
        
        private static DateTime _sessionIdExpiredDate = DateTime.MinValue;

        private static string _userAgent = null!;

        private static string _account = null!;

        private static string _password = null!;

        public Malody(string account,string password,string malodyBaseUrl= "https://m.mugzone.net",string userAgent = "Love MUG Forever")
        {
            _account = account;
            _password = MD5.GetMD5(password);
            _malodyBaseUrl = malodyBaseUrl;
            _userAgent = userAgent;
        }

        private async Task UpdateValidation()
        {
            if (_xCsrfToken == "")
            {
                await UpdateXCsrfToken();
            }
            if (_xCsrfTokenExpiredDate < DateTime.Now || _sessionIdExpiredDate < DateTime.Now)
            {
                await UpdateXCsrfToken();
                await LoginAccount();
            }
            
        }
        
        private async Task UpdateXCsrfToken()
        {
            Dictionary<string, string> headers = new() {{"User-Agent", _userAgent }};
            var response =await Network.HttpGet(_malodyBaseUrl+"/index/",headers);
            var cookies = response.Headers.GetValues("Set-Cookie");
            foreach (var cookie in cookies)
            {
                _xCsrfToken = cookie.Split(";")[0].Replace("csrftoken=","");
                _xCsrfTokenExpiredDate = DateTime.Parse(cookie.Split(";")[1].Replace(" expires=","")); 
            }
        }

        private async Task LoginAccount()
        {
            Dictionary<string, string> headers = new();
            headers.Add("User-Agent",_userAgent);
            headers.Add("x-csrftoken",_xCsrfToken);
            headers.Add("referer",$"{_malodyBaseUrl}/accounts/login/?next=/page/all");
            headers.Add("cookie",$"csrftoken={_xCsrfToken};");
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"email", _account},
                {"psw", _password}
            });
            var response =await Network.HttpPost(_malodyBaseUrl+"/accounts/login",content,headers);
            var cookies = response.Headers.GetValues("set-cookie");
            foreach (var cookie in cookies)
            {
                if (cookie.Contains("csrftoken="))
                {
                    _xCsrfToken = cookie.Split(";")[0].Replace("csrftoken=","");
                    _xCsrfTokenExpiredDate = DateTime.Parse(cookie.Split(";")[1].Replace(" expires=","")); 
                }
                if (cookie.Contains("sessionid="))
                {
                    _sessionId = cookie.Split(";")[0].Replace("sessionid=","");
                    _sessionIdExpiredDate = DateTime.Parse(cookie.Split(";")[1].Replace(" expires=","")); 
                }
            }
        }
        private async Task<string> GetPageContent(string url)
        {
            await UpdateValidation();
            Dictionary<string, string> headers = new()
            {
                {"User-Agent", _userAgent},
                {"Cookie", $"csrftoken={_xCsrfToken}; sessionid={_sessionId}"}
            };
            var content = await Network.HttpGetContent(url, headers);
            return content;
        }

        private MalodyPlayJudge GetSongPlayingJudge(string judge)
        {
            switch (judge)
            {
                case "A":
                    return MalodyPlayJudge.A;
                case "B":
                    return MalodyPlayJudge.B;
                case "C":
                    return MalodyPlayJudge.C;
                case "D":
                    return MalodyPlayJudge.D;
                case "E":
                    return MalodyPlayJudge.E;
                default:
                    return MalodyPlayJudge.None;
            }
        }
        
        private MalodyPlayMode GetSongPlayingMode(string songNameRawString)
        {
            if (songNameRawString.Contains("mode-0.png"))
            {
                return MalodyPlayMode.Key;
            }
            if (songNameRawString.Contains("mode-1.png"))
            {
                return MalodyPlayMode.StepMania;
            }
            if (songNameRawString.Contains("mode-2.png"))
            {
                return MalodyPlayMode.DJ;
            }
            if (songNameRawString.Contains("mode-3.png"))
            {
                return MalodyPlayMode.Catch;
            }
            if (songNameRawString.Contains("mode-4.png"))
            {
                return MalodyPlayMode.Jubeat;
            }
            if (songNameRawString.Contains("mode-5.png"))
            {
                return MalodyPlayMode.Taiko;
            }
            if (songNameRawString.Contains("mode-6.png"))
            {
                return MalodyPlayMode.Ring;
            }
            if (songNameRawString.Contains("mode-7.png"))
            {
                return MalodyPlayMode.Slide;
            }
            return MalodyPlayMode.None;
        }

        public async Task<MalodyUserInfoModel> GetUserInfo(int malodyId)
        {
            string html = Html.EscToHtml(await GetPageContent($"{_malodyBaseUrl}/accounts/user/" + malodyId));
            string userName = Regex.Match(html, @"<p class=""name"">\n<span.*\n").ToString().Replace("<p class=\"name\">\n", "").Replace("<span>","").Replace("</span>","");
            var rankContent = html.Substring(html.IndexOf("<div class=\"rank g_rblock\">"), html.IndexOf("<div class=\"ach g_rblock\">") - html.IndexOf("<div class=\"rank g_rblock\">"));
            
            string activitiesContent;
            if (html.Contains("<div class=\"chart \">"))
            {
                activitiesContent = html.Substring(html.IndexOf("<div class=\"active curr\">"), html.IndexOf("<div class=\"chart \">") - html.IndexOf("<div class=\"active curr\">"));
            }
            else
            {
                activitiesContent = html.Substring(html.IndexOf("<div class=\"active curr\">"), html.IndexOf("<span class=\"clear\"></span>") - html.IndexOf("<div class=\"active curr\">"));
            }
            
            var modes = Regex.Matches(rankContent, @"<img src=""/static/img/mode/mode-.*>");//(?<=A).*(?=B)
            MalodyUserInfoModel malodyUserInfoModel = new ();
            var achievePicUrls = Regex.Matches(html, "<img src=\"/static/img/achieve/.*(?=\" />)");
            var ranks = Regex.Matches(rankContent, @"(?<=<p class=""rank"">).*(?=</p>)");//
            var exps = Regex.Matches(rankContent, @"(?<=Exp.).*(?=</span>)");
            var combos = Regex.Matches(rankContent, @"(?<=Combo:).*(?=</span>)");
            var accs = Regex.Matches(rankContent, @"(?<=Acc.).*(?=</span>)");
            var playCounts = Regex.Matches(rankContent, @"(?<=Playcount:).*(?=</span>)");
            var activitiesGetTimes = Regex.Matches(activitiesContent, @"(?<=<span>).*(?=</span>)");
            var activities = Regex.Matches(activitiesContent, $"<a class=\"textfix\" href=.*(?=</a>)" );
            malodyUserInfoModel = new()
            {
                JoinedTime = DateTime.Parse(Regex.Match(html, @"(?<=Joined since:).*(?=</span>)").Value),
                LastPlayedTime = DateTime.Parse(Regex.Match(html, @"(?<=Last play:).*(?=</span>)").ToString().Trim()),
                TotalPlayedTime = Regex.Match(html, @"(?<=Played: ).*(?=</span>)").ToString(),
                Sex = Regex.Match(html, @"(?<=Gender: ).*(?=</span>)").ToString().Split("</span>")[0],
                Age = Regex.Match(html, @"(?<=Age: ).*(?=</span>)").ToString().Split("</span>")[0],
                LiveIn = Regex.Match(html, @"(?<=Location: ).*(?=</span>)").ToString(),
                CoinCount = Convert.ToInt64(Regex.Match(html, @"(?<=Gold:).*(?=</span>)").ToString().Trim()),
                Income = Convert.ToInt64(Regex.IsMatch(html, @"(?<=Income:).*(?=</span>)") ? Regex.IsMatch(html, @"(?<=Income:).*(?=</span>)").ToString().Trim() : "0"),
                ChartBeingPlayedTime = Regex.Match(html, @"Charts been played.*(?=</span>)").ToString(),
                OnSaleChartsCount = Convert.ToInt32(Regex.Match(html, @"(?<=Stable charts:).*(?=</span>)").ToString().Trim()),
                NoneOnSaleChartsCount = Convert.ToInt32(Regex.Match(html, @"(?<=Unstable charts:).*(?=</span>)").ToString().Trim()),
                UserName = userName.Contains("<span style=") ? userName.Split(">")[1].Replace("\n","") : userName.Replace("\n","")
            };
            for (int i = 0; i < modes.Count; i++)
            {
                MalodyUserRankModel malodyUserRank = new()
                {
                    Mode = GetSongPlayingMode(modes[i].Value),
                    Acc = Convert.ToSingle(accs[i].Value.Replace("%","")),
                    Combo = Convert.ToInt32(combos[i].Value),
                    Exp = Convert.ToInt32(exps[i].Value),
                    PlayCount = Convert.ToInt32(playCounts[i].Value),
                    Rank = Convert.ToInt32(ranks[i].Value.Replace("#",""))
                };
                malodyUserInfoModel.MalodyUserRanks.Add(malodyUserRank);
            }
            for (int i = 0; i < activities.Count; i++)
            {
                MalodyUserActivityModel malodyUserActivityModel = new()
                {
                    ActivityName = activities[i].Value.Split(">")[1],
                    ActivityTime = activitiesGetTimes[i].Value
                };
                malodyUserInfoModel.MalodyUserActivities.Add(malodyUserActivityModel);
            }
            foreach (Match achievePicUrl in achievePicUrls)
            {
                malodyUserInfoModel.MalodyAchievePicUrls.Add(_malodyBaseUrl+achievePicUrl.Value.Replace("<img src=\"", ""));
            }
            return malodyUserInfoModel;
        }
        
        public async Task<List<MalodyUserQueryModel>> MalodyUserQuery(string playerName)
        {
            var content = await GetPageContent($"{_malodyBaseUrl}/page/search?keyword={playerName}");
            if (Regex.IsMatch(content, @"<a class=""textfix"" href=""/accounts/user/.*\n"))
            {
                var playerInfos = Regex.Matches(content, @"<a class=""textfix"" href=""/accounts/user/.*\n");
                List<MalodyUserQueryModel> queryUsersList = new();
                foreach (Match playerInfo in playerInfos)
                {
                    MalodyUserQueryModel userQueryModel = new()
                    {
                        UserId = Convert.ToInt32(playerInfo.Value.Split(">")[0].Split("/")[3].Replace("\"", "")),
                        UserName = playerInfo.Value.Split(">")[1].Replace("</a", "")
                    };
                    queryUsersList.Add(userQueryModel);
                }
                return queryUsersList;
            }
            return new List<MalodyUserQueryModel>();
        }

        public async Task<List<MalodySongModel>> SearchRecent(int malodyId)
        {
            string html = Html.EscToHtml(await GetPageContent(_malodyBaseUrl+"/accounts/user/" + malodyId));
            string recentHtml = html.Split("<div class=\"active curr\">")[0];
            string userName = Regex.Match(html, @"<p class=""name"">\n<span.*\n").ToString().Replace("<p class=\"name\">\n", "").Replace("<span>","").Replace("</span>","");
            List<MalodySongModel> songModels = new ();
            List<string> recentCombos = new ();
            var songs = Regex.Matches(recentHtml, @"<p class=""textfix title""><img src=""/static/img/mode/mode-.*>");
            var picUrls = Regex.Matches(recentHtml, @"<div class=""cover"" style=""background-image:url.*>");
            var scores = Regex.Matches(recentHtml, @"(?<=Score:).*");
            var combos = Regex.Matches(recentHtml, @"(?<=Combo: ).*");
            var accs = Regex.Matches(recentHtml, @"(?<=Acc. ):.*");
            var judges = Regex.Matches(recentHtml, @"(?<=Judge :).*");
            var times = Regex.Matches(recentHtml, @"<span class=""time"">.*");
            foreach (Match combo in combos)
            {
                if (!combo.Value.Contains("</span>"))
                {
                    recentCombos.Add(combo.Value);
                }
            }
            for (int i = 0; i < songs.Count; i++)
            {
                MalodySongModel songModel = new ()
                {
                    SongName = songs[i].Value.Split('>')[3].Replace("</a", ""),
                    Acc = Convert.ToSingle(accs[i].Value.Replace("%","").Replace(":","").Trim()),
                    Score = Convert.ToInt32(scores[i].Value.Trim()),
                    Combo = Convert.ToInt32(recentCombos[i].Trim()),
                    Judge = GetSongPlayingJudge(judges[i].Value.Split('>')[1].Replace("</em","")),
                    PlayedTime = times[i].Value.Replace(@"<span class=""time"">","").Replace("</span>",""),
                    CoverPicUrl = picUrls[i].Value.Replace("<div class=\"cover\" style=\"background-image:url(", "").Replace(")\"></div>", ""),
                    Mode = GetSongPlayingMode(songs[i].Value),
                };
                songModels.Add(songModel);
            }
            if (userName.Contains("<span style="))
            {
                userName = userName.Split(">")[1];
            }
            return songModels;
        }

        public string GetXCsrfToken()
        {
            return _xCsrfToken;
        }
        
        public string GetSessionId()
        {
            return _sessionId;
        }
        
        public DateTime GetXCsrfTokenExpireDate()
        {
            return _xCsrfTokenExpiredDate;
        }
        
        public DateTime GetSessionIdExpireDate()
        {
            return _sessionIdExpiredDate;
        }
    }
}