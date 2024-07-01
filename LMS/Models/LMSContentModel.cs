using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.Models
{
    public class LMSContentModel
    {
        public string Is_PageLoad { get; set; }
        public string Is_ContentId { get; set; }
        public string Is_ContentTitle { get; set; }
        public int TopicId { get; set; }
        public List<TopicList> TopicList { get; set; }

        public Int64 TotalContents { get; set; }
        public Int64 ActiveContents { get; set; }
        public Int64 InactiveContents { get; set; }
        
    }

    public class LMSContentAddModel
    {
        public string Action { get; set; }
        public string ContentID { get; set; }
        public string ContentTitle { get; set; }
        public string ContentDesc { get; set; }
        public string TopicIds { get; set; }
        public int PlaySequence { get; set; }
        public string Status { get; set; }
        public string AllowLike { get; set; }
        public string AllowComments { get; set; }
        public string AllowShare { get; set; }
        public string RETURN_VALUE { get; set; }
        public string RETURN_DUPLICATEMAPNAME { get; set; }
        public string SelectedQuestionMapList { get; set; }
    }

    public class LMSContentEditModel
    {
        public string Action { get; set; }
        public string ContentID { get; set; }
        public string ContentTitle { get; set; }
        public string ContentDesc { get; set; }
        public string TopicIds { get; set; }
        public string PlaySequence { get; set; }
        public bool Status { get; set; }
        public bool AllowLike { get; set; }
        public bool AllowComments { get; set; }
        public bool AllowShare { get; set; }
        public string RETURN_VALUE { get; set; }
        public string RETURN_DUPLICATEMAPNAME { get; set; }
    }

    public class TopicList
    {
        public String TOPIC_ID { get; set; }
        public String TOPIC_NAME { get; set; }
    }
    public class CategoryList
    {
        public String CATEGORYID { get; set; }
        public String CATEGORYNAME { get; set; }
    }

    public class UploadContentFiles
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public Nullable<int> FileSize { get; set; }
        public string FilePath { get; set; }
        public string Filetype { get; set; }
        public string FileDescription { get; set; }
        public string FilePathIcon { get; set; }
        public string IsActive { get; set; }

    }

    public class ContentListingData
    {
        public string CONTENTID { get; set; }
        public string CONTENTTITLE { get; set; }
        public string CONTENTDESC { get; set; }
        public string CONTENTSTATUS { get; set; }
        public string CONTENT_FILESIZE { get; set; }
        public string CONTENT_FILENAME { get; set; }
        public string CONTENT_FILEPATH { get; set; }
        public string CONTENT_FILETYPE { get; set; }
        public string CONTENT_FILEDURATION { get; set; }
        public string CONTENT_ALLOWLIKE { get; set; }
        public string CONTENT_ALLOWCOMMENTS { get; set; }
        public string CONTENT_ALLOWSHARE { get; set; }

        public string cnt_TotalContents { get; set; }
        public string cnt_ActiveContents { get; set; }
        public string cnt_InactiveContents { get; set; }

    }

    public class QuestionListForMap
    {
        public string ID { get; set; }
        public string NAME { get; set; }
        public bool selected { get; set; }
    }

}