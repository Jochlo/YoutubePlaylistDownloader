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

            foreach (VideoInfo vinfo in videoinfos)
            {
                dataGridView1.Rows.Add(vinfo.videoName, vinfo.videoURL);
            }
        }

        internal async Task<VideoInfo[]> GetDataFromWebsite(string url)
        {
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
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string xml = await response.Content.ReadAsStringAsync();
                    string[] allData = xml.Split(startSearch)[1].Split(endSearch)[0].Split(beforeEachEntry);

                    videoInfos = new VideoInfo[allData.Length - 1];

                    for (int i = 1; i < allData.Length; i++)
                    {
                        if (i == 11)
                        {
                            int justforme = 1;
                        }

                        string[] splitForVideoName = allData[i].Split(filterTitleBetter);
                        string videoName = splitForVideoName[splitForVideoName.Length - 1].Split(beforeTitle)[1].Split(aftertitle)[0];
                        string videoURL = "https://www.youtube.com/watch" + allData[i].Split(beforeurl)[1].Split(afterUrl)[0];



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