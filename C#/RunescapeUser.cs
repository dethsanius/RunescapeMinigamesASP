using System;
using System.Net;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SQL;

namespace RunscapeUser {
    class Clan {
        public Clan(string clan) {
            string clanPage=MakeAsyncRequest("http://www.runeclan.com/clan/" + clan + "/members", "text/html");
			while (clanPage.Contains("database error")) {
				clanPage = MakeAsyncRequest("http://www.runeclan.com/clan/" + clan + "/members", "text/html");
			}
            foreach (string username in getUsernames(clanPage)) {
                Console.WriteLine(username);
            }
            //Number of pages for any runeclan website
            Console.WriteLine(getInnerPage(clanPage, "<div class=\"pagination_page\">Page 1 of ", "</div><div class=\"pagination_select\"><span class=\"disabled\">"));
            
        }
        private string getInnerPage(string page, string start, string end) {
            // Console.WriteLine(page);
            // Console.WriteLine(start);
            // Console.WriteLine(end);
            // Console.WriteLine(page.IndexOf(start));
            // Console.WriteLine(page.IndexOf(end));
            if (page.IndexOf(start)>=0 && page.IndexOf(end) >= 0) {
                return page.Substring(page.IndexOf(start) + start.Length).Remove(page.IndexOf(end) - page.IndexOf(start) - start.Length);
            } else {
                return "Error";
            }
        }
        private string[] splittList(string list) {
            return list.Split(new string[] {"tr>"}, StringSplitOptions.None);
        }
        private List<string> getUsernames(string clanPage) {
            List<string> usernames = new List<string>();
            string userlist = getInnerPage(clanPage, "<table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" class=\"regular\">", "<div class=\"adlgleaderboard\" style=\"margin-top:15px;\">");
            // Console.WriteLine(userlist);
            foreach (string userCollumn in splittList(userlist)) {
                String username = getInnerPage(userCollumn, "style=\"background-image:url(http://www.runeclan.com/images//chat_head.php?a=", ");");
                if (username != "Error") { 
                    usernames.Add(username.Replace("+"," "));
                    // Console.WriteLine(username.Replace("+"," "));
                }
            }
            return usernames;
        }
        private void getUserInfo(string username) {/*
            string eventPage = page.Substring(page.IndexOf("<div class=\"events_wrap\">")).Remove(page.IndexOf("<div class=\"page_footer\">") - page.IndexOf("<div class=\"events_wrap\">"));
			int Sub = eventPage.IndexOf("class=\"regular\">"); 
			int Remove = eventPage.LastIndexOf("<div class=\"pagination\">");
			string isss = "<td class=\"events_table2\">";
			int iss = eventPage.IndexOf(isss);
			int isend = eventPage.IndexOf("\" /><span class=\"competition_skillname");
			string skill = eventPage.Substring(iss + isss.Length).Remove(isend - iss- isss.Length).Substring(eventPage.IndexOf("alt=\"") + 5 - iss - isss.Length);
			foreach (string i in eventPage.Substring(Sub).Remove(Remove - Sub).Split(new string[] {"tr>"}, StringSplitOptions.None)) {
				if (i.Contains("td")) {
					string isns = "<a href=\"/user/";
					int isn = i.IndexOf(isns);
					int ien = i.IndexOf("\" name=");
					string username = i.Substring(isn + isns.Length).Remove(ien - isn - isns.Length).Replace("+"," ");
					// Console.WriteLine(username);
					string ises = "class=\"clan_td clan_xpgain_trk\">";
					int ise = i.IndexOf(ises);
					int iee = i.IndexOf("</td></");
					int  eventXP = Int32.Parse(i.Substring(ise + ises.Length).Remove(iee - ise - ises.Length).Replace(",",""));
					String dsUser = MakeAsyncRequest("http://www.runeclan.com/user/"+ username.Replace(" ","+"), "text/html").Result;
					while (dsUser.Contains("database error")) {
						dsUser = MakeAsyncRequest("http://www.runeclan.com/user/"+ username.Replace(" ","+"), "text/html").Result;
					}
					// Console.WriteLine(dsUser.IndexOf("alt=\"Private Profile\""));
					if (dsUser.IndexOf("alt=\"Private Profile\"") != -1) {
						// userInfo.Add(username + ": Skill: " + skill + " PRIVATE PROFILE");
						Console.WriteLine("{0} PRIVATE PROFILE",username);
					} else if (dsUser.IndexOf("We are not currently tracking this users stats.") != -1) {
						Console.WriteLine("{0} Not Tracking",username);
					} else {
						string isdss = "onmousemove=\"xpTrackerBox('" + skill.ToLower();
						int isds = dsUser.IndexOf(isdss);
						int ieds = dsUser.LastIndexOf("<div class=\"adlgleaderboard");
						string[] skillArray;
						try {skillArray = dsUser.Substring(isds + isdss.Length).Remove(ieds - isds - isdss.Length).Split(new string[] {"td>"}, StringSplitOptions.None);}
						catch {
							Console.WriteLine(dsUser);
						}
						skillArray = dsUser.Substring(isds + isdss.Length).Remove(ieds - isds - isdss.Length).Split(new string[] {"td>"}, StringSplitOptions.None);
                    	// Console.Write(skillArray.Length + ": " + username);
						// if (skillArray.Length == 1) {Console.Write(": "+skillArray[0]);}
						// Console.WriteLine(": " + skillArray[1] + ": " + isds + ": " + ieds);
						int totalXP = Int32.Parse(skillArray[1].Replace("<td class=\"xp_tracker_cxp\">", "").Trim('/').Trim('<').Replace(",",""));
						usersInfo.Add(new user(username, skill, totalXP, eventXP, points(skill, totalXP, eventXP), getLevel(totalXP)));
						usernames.Add(username);
					}
				}
            }*/
        }
        public string MakeAsyncRequest(string url, string contentType)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = contentType;
            request.Proxy = null;

            Task<WebResponse> task = Task.Factory.FromAsync(
                request.BeginGetResponse,
                asyncResult => request.EndGetResponse(asyncResult),
                (object)null);

            return task.ContinueWith(t => ReadStreamFromResponse(t.Result)).Result;
        }
        private string ReadStreamFromResponse(WebResponse response)
        {
            using (Stream responseStream = response.GetResponseStream())
            using (StreamReader sr = new StreamReader(responseStream))
            {
                //Need to return this response 
                string strContent = sr.ReadToEnd();
                return strContent;
            }
        }
    }
}