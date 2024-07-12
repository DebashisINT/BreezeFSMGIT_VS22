#region======================================Revision History=========================================================
//Written By : Debashis Talukder On 02/07/2024
//Purpose: LMS Info Details.Row: 945,949 & 950
#endregion===================================End of Revision History==================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopAPI.Models
{
    public class TopicListInput
    {
        public long user_id { get; set; }
    }

    public class TopicListOutputModel
    {
        public string status { get; set; }
        public string message { get; set; }
        public long user_id { get; set; }
        public List<TopiclistOutput> topic_list { get; set; }
    }

    public class TopiclistOutput
    {
        public long topic_id { get; set; }
        public string topic_name { get; set; }
        public int video_count { get; set; }
    }

    public class TopicWiseListsInput
    {
        public long topic_id { get; set; }
    }

    public class TopicWiseListsOutput
    {
        public string status { get; set; }
        public string message { get; set; }
        public long topic_id { get; set; }
        public string topic_name { get; set; }
        public List<ContentlistOutput> content_list { get; set; }
    }

    public class ContentlistOutput
    {
        public long content_id { get; set; }
        public string content_url { get; set; }
        public string content_title { get; set; }
        public string content_description { get; set; }
        public long content_play_sequence { get; set; }
        public bool isAllowLike { get; set; }
        public bool isAllowComment { get; set; }
        public bool isAllowShare { get; set; }
        public List<QuestionlistOutput> question_list { get; set; }
    }

    public class QuestionlistOutput
    {
        public long topic_id { get; set; }
        public long content_id { get; set; }
        public long question_id { get; set; }
        public string question { get; set; }
        public string question_description { get; set; }
        public List<OptionlistOutput> option_list { get; set; }
    }

    public class OptionlistOutput
    {
        public long question_id { get; set; }
        public long option_id { get; set; }
        public string option_no_1 { get; set; }
        public long option_point_1 { get; set; }
        public bool isCorrect_1 { get; set; }
        public string option_no_2 { get; set; }
        public long option_point_2 { get; set; }
        public bool isCorrect_2 { get; set; }
        public string option_no_3 { get; set; }
        public long option_point_3 { get; set; }
        public bool isCorrect_3 { get; set; }
        public string option_no_4 { get; set; }
        public long option_point_4 { get; set; }
        public bool isCorrect_4 { get; set; }
    }

    public class UserWiseLMSModulesInfoSaveInput
    {
        public long user_id { get; set; }
        public List<Userlmsinfolists> user_lms_info_list { get; set; }
    }

    public class Userlmsinfolists
    {
        public string module_name { get; set; }
        public int count_of_use { get; set; }
        public string time_spend { get; set; }
        public string last_current_loc_lat { get; set; }
        public string last_current_loc_long { get; set; }
        public string last_current_loc_address { get; set; }
        public string date_time { get; set; }
        public string phone_model { get; set; }
    }
    public class UserWiseLMSModulesInfoSaveOutput
    {
        public string status { get; set; }
        public string message { get; set; }
    }

    public class TopicContentDetailsSaveInput
    {
        public long user_id { get; set; }
        public long topic_id { get; set; }
        public string topic_name { get; set; }
        public long content_id { get; set; }
        public bool like_flag { get; set; }
        public int share_count { get; set; }
        public string content_length { get; set; }
        public string content_watch_length { get; set; }
        public bool content_watch_completed { get; set; }
        public string content_last_view_date_time { get; set; }
        public List<Commentlists> comment_list { get; set; }
    }

    public class Commentlists
    {
        public long topic_id { get; set; }
        public long content_id { get; set; }
        public string comment_id { get; set; }
        public string comment_description { get; set; }
        public string comment_date_time { get; set; }
    }

    public class TopicContentDetailsSaveOutput
    {
        public string status { get; set; }
        public string message { get; set; }
    }

    public class LearningContentListsInput
    {
        public long user_id { get; set; }
    }

    public class LearningContentListsOutput
    {
        public string status { get; set; }
        public string message { get; set; }
        public long user_id { get; set; }
        public List<LearningContentInfolistOutput> learning_content_info_list { get; set; }
    }

    public class LearningContentInfolistOutput
    {
        public long topic_id { get; set; }
        public string topic_name { get; set; }
        public long content_id { get; set; }
        public string content_url { get; set; }
        public string content_title { get; set; }
        public string content_description { get; set; }
        public string content_length { get; set; }
        public string content_watch_length { get; set; }
        public bool content_watch_completed { get; set; }
        public string content_last_view_date_time { get; set; }
    }
}