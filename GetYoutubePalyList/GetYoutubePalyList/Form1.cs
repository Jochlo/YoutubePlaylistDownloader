using System.Data;
using System.Security.Policy;
using System.Windows.Forms;

namespace GetYoutubePalyList
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            VideoInfo[] videoinfos = await GetDataFromWebsite(textBox1.Text);
            dataGridView1.Rows.Clear();

            foreach (VideoInfo vinfo in videoinfos)
            {
                dataGridView1.Rows.Add(vinfo.videoName, vinfo.videoURL);
            }
        }

        internal async Task<VideoInfo[]> GetDataFromWebsite(string url)
        {
            //strings used for traversing the website
            string startSearch = "else {window.addEventListener('script-load-dpj',";
            string endSearch = "</div>";
            string beforeEachEntry = "\"ADD_TO_QUEUE_TAIL\"";
            string beforeurl = "{\"webCommandMetadata\":{\"url\":\"/watch";
            string afterUrl = "\",\"webPageType\"";
            string filterTitleBetter = ",\"width\":336,\"height\":188}]}";
            string beforeTitle = ",\"title\":{\"runs\":[{\"text\":\"";
            string aftertitle = "\"";

            VideoInfo[] videoInfos;

            using (HttpClient client = new HttpClient())
            {
                //Get a response from Youtube
                HttpResponseMessage response = await client.GetAsync(url);

                //check if the Request was successful
                if (response.IsSuccessStatusCode)
                {
                    //Get the XML from the response
                    string xml = await response.Content.ReadAsStringAsync();

                    //narrow down the section where we search for useful data
                    string[] allData = xml.Split(startSearch)[1].Split(endSearch)[0].Split(beforeEachEntry);

                    //initialize the return object 
                    //the size is one less than allData.Length because the first entry contains no data
                    videoInfos = new VideoInfo[allData.Length - 1];

                    //iterate through the data
                    for (int i = 1; i < allData.Length; i++)
                    {
                        //narrow down the entry section for the title
                        string[] splitForVideoName = allData[i].Split(filterTitleBetter);
                        //split the entry string exactly to contain the video title
                        string videoName = splitForVideoName[splitForVideoName.Length - 1].Split(beforeTitle)[1].Split(aftertitle)[0];

                        //split the entry string exactly to contain the video url
                        string videoURL = "https://www.youtube.com/watch" + allData[i].Split(beforeurl)[1].Split(afterUrl)[0];

                        //add those information to our return object
                        videoInfos[i - 1] = new VideoInfo() { videoURL = videoURL, videoName = videoName };
                    }
                    client.Dispose();
                    return videoInfos;
                }
                else
                {
                    client.Dispose();
                    throw new Exception();
                }
            }
        }

        internal class VideoInfo
        {
            internal string videoURL;
            internal string videoName;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}