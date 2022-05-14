using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using MaterialSkin.Controls;
using MaterialSkin;
using FTP;
using System.IO;

namespace Memo
{
    public partial class Form1 : MaterialForm
    {
        ftpUtil FTP;
        public Form1()
        {
            InitializeComponent();

            FTP = new ftpUtil("", "", "*", "");
            /*
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Green800, Primary.Green900, Primary.Green500, Accent.Orange400, TextShade.WHITE);
            */
        }

        private bool GetNodes(ref TreeNode svrNode, string name)
        {
            string[] fileList = FTP.GetFileList(name);
            List<string> infoList = new List<string>();
            infoList = FTP.GetFilesDetailList(name);

            for (int i = 0; i < infoList.Count; i++)
            {
                string filePath = fileList[i].Split('/')[1];

                if (infoList[i][0] == 'd')
                {
                    TreeNode temp = new TreeNode(filePath, 0, 0);
                    GetNodes(ref temp, name + "/" + filePath);
                    svrNode.Nodes.Add(temp);
                }
                else
                {
                    svrNode.Nodes.Add(name + "/" + filePath, filePath, 1, 1);
                }
            }

            return true;
        }

        private void materialFlatButton1_Click(object sender, EventArgs e) // 새로고침 버튼
        {
            treeView.Nodes.Clear();

            // 이미지 다운로드
            FTP.Download(@Application.StartupPath + "/Images/folder.png", "memo/Image/folder.png");
            FTP.Download(@Application.StartupPath + "/Images/note.png", "memo/Image/note.png");
            Log("[Download] " + Application.StartupPath + "/Images/folder.png");
            Log("[Download] " + Application.StartupPath + "/Images/note.png");

            // 이미지 할당
            ImageList imgList = new ImageList();
            imgList.Images.Add(Bitmap.FromFile(Application.StartupPath + "/Images/folder.png"));
            imgList.Images.Add(Bitmap.FromFile(@Application.StartupPath + "/Images/note.png"));
            treeView.ImageList = imgList;

            // 서버 파일 가져오기
            TreeNode svrNode = new TreeNode("서버", 0, 0);
            bool temp = GetNodes(ref svrNode, "memo");

            if (temp)
            {
                Log("파일 목록 불러오기 성공");
            }
            else
            {
                Log("파일 목록 불러오기 실패");

            }


            treeView.Nodes.Add(svrNode);
            treeView.ExpandAll();
        }
        
        private void downLoad()
        {
            if (treeView.SelectedNode == null)
            {
                MessageBox.Show("다운로드할 파일을 선택해주세요");
            }
            else
            {
                var selectedNode = treeView.SelectedNode;

                if (selectedNode.Text == "")
                {
                    MessageBox.Show("폴더 다운로드 기능은 아직 준비중입니다");
                }
                else
                {
                    string localPath = @Application.StartupPath + "/Downloads/" + selectedNode.Text;

                    FTP.Download(localPath, selectedNode.Name); Log("[Download] " + localPath);
                    richTextBox.Clear();

                    if (selectedNode.Text.Contains(".png") || selectedNode.Text.Contains(".jpg")) // 이미지파일
                    {
                        Bitmap oBitmap = null;

                        oBitmap = (Bitmap)Bitmap.FromFile(Application.StartupPath + "/Downloads/" + selectedNode.Text);

                        Clipboard.Clear();

                        Clipboard.SetImage(oBitmap);

                        richTextBox.Paste();
                    }
                    else // 일반파일
                    {
                        richTextBox.AppendText(File.ReadAllText(localPath));
                    }
                    //FTP.Upload("asdasd", "asd");
                }
            }
        }

        private void materialFlatButton2_Click(object sender, EventArgs e) // 다운로드 버튼
        {
            downLoad();
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            downLoad();
        }

        private void Log(string str)
        {
            LogBox.AppendText("\n" + str);
        }

        private void materialFlatButton4_Click(object sender, EventArgs e)
        {
            if(materialFlatButton4.Text == "Hide Log")
            {
                materialFlatButton4.Text = "Show Log";
                this.Size = new Size(740, 468);
            }
            else
            {
                materialFlatButton4.Text = "Hide Log";
                this.Size = new Size(740, 729);
            }
        }
    }
}
