using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DataFormat
{
    public class FacilityData
    {
        public class Facility
        {
            public string code { get; set; }

            public string msg { get; set; }

            public string errMsg { get; set; }

            public string timestamp { get; set; }

            public Data data { get; set; }
        }

        public class Data 
        { 
            public string[] list { get; set; }

            public int totalCount { get; set; }

            public int totalPage { get; set; }

            public int pageNo { get; set; }

            public int pageSize { get; set; }

            public int offset { get; set; }
        }
    }

    public class RtspVideoData {
        public string cam_name { get; set; }
        public string video_path { get; set; }
    }

    public class InfoPanelData
    {
        public string name { get; set; }
        public string run_state { get; set; }
        public string event_des { get; set; }
        public string error_time { get; set; }
        public string facility_pos { get; set; }
        public string person_name { get; set; }
        public string phone_num { get; set; }
        public string video_path { get; set; }
    }

    public class NewFacilityInfoData {
        public string message { get; set; }
        public string index { get; set; }
        public string find_index { get; set; }
    }
}