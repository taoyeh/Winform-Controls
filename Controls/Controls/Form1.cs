using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace Controls
{
    public partial class Form1 : Form
    {
        string status = "";
        public Form1()
        {
            InitializeComponent();
            //TreeNode node = new TreeNode("绍兴文理学院");
            //treeView1.Nodes.Add(node);
            //TreeNode node2 = new TreeNode("计算机系");
            //TreeNode node3 = new TreeNode("机械系");
            //TreeNode node4 = new TreeNode("自动化系");
            //node.Nodes.Add(node2); node.Nodes.Add(node3); node.Nodes.Add(node4);

            doc.Load("TreeXml.xml"); //我是把xml放到debug里面了.你的路径就随便啦.不过这样方便一些.
            RecursionTreeControl(doc.DocumentElement, treeView1.Nodes);//将加载完成的XML文件显示在TreeView控件中
            treeView1.ExpandAll();//展开TreeView控件中的所有项
            textBoxClose();
            FindDepartment();
        }

        XmlDocument doc = new XmlDocument();
        StringBuilder sb = new StringBuilder();
        //XML每行的内容
        private string xmlLine = "";
        /// <summary>
        /// 通过xml生成treeview
        /// </summary>
        /// <param name="xmlNode"></param>
        /// <param name="nodes"></param>
        private void RecursionTreeControl(XmlNode xmlNode, TreeNodeCollection nodes)
        {
            foreach (XmlNode node in xmlNode.ChildNodes)//循环遍历当前元素的子元素集合
            {
                TreeNode new_child = new TreeNode();//定义一个TreeNode节点对象
                if(node.Attributes["Id"].Value=="")
                {
                    new_child.Text = node.Attributes["Name"].Value;
                }

                else 
                {
                    Student s = new Student(node.Attributes["Id"].Value, node.Attributes["Name"].Value, node.Attributes["Address"].Value, node.Attributes["Department"].Value,  node.Attributes["Sex"].Value=="True"?true:false);
                    new_child.Tag = s;
                    new_child.Text = s.Id;
                }
                nodes.Add(new_child);//向当前TreeNodeCollection集合中添加当前节点
                RecursionTreeControl(node, new_child.Nodes);//调用本方法进行递归
            }
        }
        //保存
        private void SaveXml()
        {
            try
            {

                //写文件头部内容
                //下面是生成RSS的OPML文件
                sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                sb.Append("<Tree>");

                //遍历根节点
                foreach (TreeNode node in treeView1.Nodes)
                {
                    xmlLine = GetRSSText(node);
                    sb.Append(xmlLine);

                    //递归遍历节点
                    parseNode(node, sb);

                    sb.Append("</Node>");
                }
                sb.Append("</Tree>");

                StreamWriter sr = new StreamWriter("TreeXml.xml", false, System.Text.Encoding.UTF8);
                sr.Write(sb.ToString());
                sr.Close();
            }
            catch (Exception ex)
            {
  
            }
        }
        //成生RSS节点的XML文本行
        private string GetRSSText(TreeNode node)
        {
            //根据Node属性生成XML文本
            Student s = node.Tag as Student;
            string rssText = "";
            if (s == null)
                //<Node Id="" Name="计算机系" Address="" Department="" Sex="" ></Node>
                //<Node  Id="" Name="" Address="" Department="" Sex="">
                rssText = "<Node  Id=\"\" " +"Name=\"" + node.Text + "\" Address=\"\" Department=\"\" Sex=\"\"" + ">";
            else
            {
                rssText = "<Node  Id=\""+s.Id  + "\" Name =\"" + s.Name + "\" Address =\"" + s.Address + "\" Department =\"" + s.Department + "\" Sex =\"" + s.Sex +"\"" + ">";
            }
           
            return rssText;
        }
        //递归遍历节点内容,最关键的函数
        private void parseNode(TreeNode tn, StringBuilder sb)
        {
            IEnumerator ie = tn.Nodes.GetEnumerator();

            while (ie.MoveNext())
            {
                TreeNode ctn = (TreeNode)ie.Current;
                xmlLine = GetRSSText(ctn);
                sb.Append(xmlLine);
                //如果还有子节点则继续遍历
                if (ctn.Nodes.Count > 0)
                {
                    parseNode(ctn, sb);
                }
                sb.Append("</Node>");
            }


        }





        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
             if(e.Node.Level==2) GiveStudent(e.Node);

        }

         
        /// <summary>
        /// menustrip操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 插入ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.Text = textBox2.Text = textBox3.Text = comboBox1.Text = "";

            textBoxOpen();

            status = "插入";
        }
        private void 修改ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(treeView1.SelectedNode.Level == 2)
            {
                status = "修改";
                textBoxOpen();
                textBox1.Enabled = false;
            }
            else
            {
                MessageBox.Show("请选择一个学生节点！！！");
            }
        }
        Form form;
        TextBox textBox;
        Button button;
        private void 查找ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            form = new Form();
            form.Width = 500;form.Height = 250;
            form.StartPosition = FormStartPosition.Manual;
            form.Top = Height/2;form.Left=Left+Width/4;
            Label label = new Label();
            label.Top = 50; label.Left = 50;label.Width=150;label.Font = label1.Font;label.Text = "请输入学号：";

            textBox = new TextBox();
            textBox.Top = 50; textBox.Left = 200; textBox.Width = 200; textBox.Font = textBox1.Font;

            button = new Button();
            button.Top = 125; button.Left = 150; button.Width = 150; button.Font = label1.Font; button.Text = "立即查找";button.BackColor =finish.BackColor;button.Size = finish.Size;
            button.Click+= FindStudent;
            form.Controls.Add(label); form.Controls.Add(textBox); form.Controls.Add(button);
            form.ShowDialog();
        }


        private void FindStudent(object v, EventArgs eventArgs)
        {
            TreeNode tnRet = null;

            foreach (TreeNode tn in treeView1.Nodes)
            {
                tnRet = FindNode(tn,textBox.Text);
                if (tnRet != null) break;
            }
            if(tnRet!=null)
            {
                GiveStudent(tnRet);
                treeView1.SelectedNode = tnRet;
            }
                
            else MessageBox.Show("查无此人");
            form.Close();
            return;
        }
        /// <summary>
        /// 把tag赋给textbox等信息
        /// </summary>
        /// <param name="tnRet"></param>

        private void GiveStudent(TreeNode tnRet)
        {
            Student s = tnRet.Tag as Student;
            if (s == null) return;
            textBox1.Text = s.Id;
            textBox2.Text = s.Name;
            textBox3.Text = s.Address;
            comboBox1.Text = s.Department;
            if (s.Sex == true)
                radioButton1.Checked = true;
            else
                radioButton3.Checked = true;
        }
        


        /// <summary>
        /// 遍历Treeview并且查找学生id
        /// </summary>
        /// <param name="tnParent"></param>
        /// <param name="strValue"></param>
        /// <returns></returns>
        private TreeNode FindNode(TreeNode tnParent, string strValue)
        {
            if (tnParent == null) return null;
            if (tnParent.Text == strValue) return tnParent;   
             
            TreeNode tnRet = null;
            foreach (TreeNode tn in tnParent.Nodes)
            {
                tnRet = FindNode(tn, strValue);
                if (tnRet != null) break;
            }
            return tnRet;
        }
        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode.Level == 2)
            {
                if (MessageBox.Show("你确认删除此节点嘛？", "Confirm", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    treeView1.Nodes.Remove(treeView1.SelectedNode);
                }
            }
            else MessageBox.Show("此操作只适用于学生节点");
        }
        /// <summary>
        /// 所有按钮可以输入
        /// </summary>
        private void textBoxOpen()
        {
            textBox1.Enabled = true;
            textBox2.Enabled = true;
            textBox3.Enabled = true;
            comboBox1.Enabled = true;
            finish.Enabled = true;
            radioButton1.Enabled = true;
            radioButton3.Enabled = true; 
        }

        /// <summary>
        /// 所有按钮不能输入
        /// </summary>
        private void textBoxClose()
        {
            textBox1.Enabled = false;
            textBox2.Enabled = false;
            textBox3.Enabled = false;
            comboBox1.Enabled = false;
            finish.Enabled = false;
            radioButton1.Enabled = false;
            radioButton3.Enabled = false;  
        }

        private void finish_Click(object sender, EventArgs e)
        {
            switch (status)
            {
                case "插入":  if(insertStudent()==true) textBoxClose(); break;
                case "修改":  ChangeStudent();textBoxClose(); break;

            }
        }
        /// <summary>
        /// 插入学生信息
        /// </summary>
        private bool insertStudent()
        {

            TreeNode node = new TreeNode(textBox1.Text);
            bool sex = radioButton1.Checked ? true : false;
            Student s = new Student(textBox1.Text, textBox2.Text, textBox3.Text, comboBox1.Text, sex);
            node.Tag = s;
            if (treeView1.SelectedNode.Level == 1)
            {
                treeView1.SelectedNode.Nodes.Add(node);
                treeView1.SelectedNode.Expand();
                return true;
            }
            else
            {
                MessageBox.Show("必须选择一个系节点");
                return false;
            }
        }

        /// <summary>
        /// 修改学生信息
        /// </summary>
        private void ChangeStudent()
        {
            Student s = treeView1.SelectedNode.Tag as Student;
            if (s.Id == textBox1.Text)
            {
                s.Name = textBox2.Text;
                s.Address = textBox3.Text;
                s.Department = comboBox1.Text;
                s.Sex = radioButton1.Checked ? true : false;
            }
        }


        /// <summary>
        /// 寻找院系并且赋给comboBox
        /// </summary>
        private void FindDepartment()
        {
            comboBox1.Items.Clear();
            foreach (TreeNode n in treeView1.Nodes)
            {
                foreach (TreeNode ans in n.Nodes)
                {
                    comboBox1.Items.Add(ans.Text);
                }
            }
        }
        /// <summary>
        /// 关闭窗体前保存数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            SaveXml();
        }

        /// <summary>
        /// 验证输入信息
        /// </summary>
        int flag = 0;
        private void Checked()
        {
            if (flag == 0)
            {
                finish.Enabled = false; finish.BackColor = Color.Salmon;
            }
            else
            {
                finish.Enabled = true; finish.BackColor = SystemColors.ActiveCaption;
            }
           
        }

        private void textBox1_Validated(object sender, EventArgs e)
        {
            if (textBox1.TextLength == 0)
            {
                errorProvider1.SetError(textBox1, "学号输入不能为空"); flag &= 1;
                return;
            }
            else if(textBox1.TextLength > 0)
            {
                TreeNode tnRet = null;

                foreach (TreeNode tn in treeView1.Nodes)
                {
                    tnRet = FindNode(tn, textBox1.Text);
                    if (tnRet != null) break;
                }
                if(tnRet!=null)
                {
                    errorProvider1.SetError(textBox1, "学号重复"); flag &= 1;
                }
                return;
            }
            errorProvider1.SetError(textBox1, ""); flag |= 1;
            Checked();
        }

        private void textBox2_Validated(object sender, EventArgs e)
        {
            if (textBox2.TextLength == 0)
            {
                errorProvider1.SetError(textBox2, "姓名输入不能为空"); flag &= 2;
            }
            else
            {
                errorProvider1.SetError(textBox2, ""); flag |= 2;
            }
            Checked();
        }

        private void comboBox1_Validated(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == -1)
            {
                errorProvider1.SetError(comboBox1, "院系输入不能为空"); flag &= 8;
            }
            else
            {
                errorProvider1.SetError(comboBox1, ""); flag |= 8;
            }
            Checked();
        }

        private void textBox3_Validated(object sender, EventArgs e)
        {
            if (textBox3.TextLength == 0)
            {
                errorProvider1.SetError(textBox3, "住址输入不能为空"); flag &= 4;
            }
            else
            {
                errorProvider1.SetError(textBox3, ""); flag |= 4;
            }
            Checked();
        }

        /// <summary>
        /// 院系操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 删除ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode.Level == 1)
            {
                if (MessageBox.Show("你确认删除此节点嘛？", "Confirm", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    treeView1.Nodes.Remove(treeView1.SelectedNode);
                }
            }
            else MessageBox.Show("此操作只适用于院系操作");
        }

        private void 新增ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            form = new Form();
            form.Width = 500; form.Height = 250;
            form.StartPosition = FormStartPosition.Manual;
            form.Top = Height / 2; form.Left = Left + Width / 4;
            Label label = new Label();
            label.Top = 50; label.Left = 50; label.Width = 150; label.Font = label1.Font; label.Text = "请输入名称：";

            textBox = new TextBox();
            textBox.Top = 50; textBox.Left = 200; textBox.Width = 200; textBox.Font = textBox1.Font;

            button = new Button();
            button.Top = 125; button.Left = 150; button.Width = 150; button.Font = label1.Font; button.Text = "添加院系"; button.BackColor = finish.BackColor; button.Size = finish.Size;
            button.Click += InsertDepartment;
            form.Controls.Add(label); form.Controls.Add(textBox); form.Controls.Add(button);
            form.ShowDialog();
        }

        private void InsertDepartment(object sender, EventArgs e)
        {
            TreeNode node = new TreeNode(textBox.Text);
            node.Text = textBox.Text;
            foreach (TreeNode n in treeView1.Nodes)
            {
                n.Nodes.Add(node);
            }
            FindDepartment();
            form.Close();
        }


        /// <summary>
        /// treeviewItem控制鼠标移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            TreeNode dragNode = e.Item as TreeNode;
            DoDragDrop(dragNode, DragDropEffects.Move);
        }
        private void treeView_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.AllowedEffect;
        }
        private void treeView_DragOver(object sender, DragEventArgs e)
        {
            TreeView tv = sender as TreeView;
            tv.SelectedNode = tv.GetNodeAt(tv.PointToClient(new Point(e.X, e.Y)));
        }
        private void treeView_DragDrop(object sender, DragEventArgs e)
        {
            TreeView tv = sender as TreeView;
            //取得被拖拽的节点
            TreeNode dragNode = e.Data.GetData(typeof(TreeNode)) as TreeNode;
            if (dragNode.Equals(tv.SelectedNode))
                return;
            if (tv.SelectedNode.Level != dragNode.Level - 1) return; 
            if (e.Effect == DragDropEffects.Move)
            {
                if (tv.SelectedNode == null)
                {
                    tv.Nodes.Add(dragNode.Clone() as TreeNode);
                    dragNode.Remove();
                    return;
                }
                dragNode.Remove();
                tv.SelectedNode.Nodes.Add(dragNode);
            }
            treeView1.ExpandAll();
        }


        /// <summary>
        /// contextMenuStrip 添加删除操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 添加ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBoxClose();
            if (treeView1.SelectedNode.Level==0)
            {

                新增ToolStripMenuItem_Click(null, null);
            }
            else if(treeView1.SelectedNode.Level == 1)
            {
                textBoxOpen(); 
                插入ToolStripMenuItem_Click(null, null);
            }
            else if(treeView1.SelectedNode.Level == 2)
            {
;
            }
        }

        private void 删除ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode.Level == 0)
            { 
                
            }
            else if (treeView1.SelectedNode.Level == 1)
            {
                新增ToolStripMenuItem_Click(null, null);
            }
            else if (treeView1.SelectedNode.Level == 2)
            {
                删除ToolStripMenuItem_Click(null, null);
            }
        }
    }
}
