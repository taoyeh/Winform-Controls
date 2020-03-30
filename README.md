# 前言
&emsp;&emsp;其实我觉的学校作业可以稍微少花点时间，把时间放到自己认为重要的地方去。(｀・ω・´)

# 问题描述

实现一个小型的学生信息管理系统，实现学生信息的输入，浏览，编辑、查找等基本功能，要求：

 - 学生信息包括学号、姓名、性别、系别、家庭住址等基本情况，选择合适的控件尽可能方便用户的输入；

 - 程序需保证用户的输入有效（即进行必要的验证操作）；

 - 用户可通过TreeView控件或其他合适的界面进行所有学生信息的浏览，系统能够体现数据的层次化特性,并提供对学生信息的查找、修改和删除功能；

 - 数据可永久保存，可保存在数据库或文件中

 - 其他相关功能，如院系增加、调整、节点拖放等
 
 - 在treeview节点上右键同样可以进行一系列操作
 
先看个效果吧，想要多了解点直接下载代码去看吧
![image.png](https://i.loli.net/2020/03/30/DxoLbtqOg2Ej5Im.png)

# 解决思路
### 通过 xml 本地保存treeview 上面的信息

&emsp;&emsp;其实我觉的直接连数据库也是很方便的，你们可以尝试下（~~别尝试xml了，对身体不好~~），我的xml放在debug文件下，我们先看下xml文件内容
```
<?xml version="1.0" encoding="UTF-8"?>
<Tree>
  <Node  Id="" Name="绍兴文理学院" Address="" Department="" Sex="">
    <Node  Id="" Name="计算机系" Address="" Department="" Sex="">
      <Node  Id="17145129" Name ="陶烨豪" Address ="河西校区" Department ="计算机系" Sex ="True"></Node>
    </Node><Node  Id="" Name="机械系" Address="" Department="" Sex=""></Node>
    <Node  Id="" Name="自动化系" Address="" Department="" Sex=""></Node>
  </Node>
</Tree>
```
node代表一个节点，里面id，name，address都是一些内容可以根据自己的需要进行，然后再根据

	new_child.Text = node.Attributes["Name"].Value;
代表的意思就是把Name这个数值拿出来赋给这个new_child.Text，通过这种方法可以把xml所有信息拿出来，再通过递归生成treeview形式
递归方法如下
```
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
```

这样我们就完成了读取操作，接下来我们来实现保存操作

---

想法其实很简单，就是退出的时候把treeview上面的内容通过递归的方式再次编程xml的形式
比如
```
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
```
其中\"代表是一个**引号 ”** 的意思（为了解决字符串拼接问题），把所需要的内容拼接成xml的形式，再次存入xml文件中就可以的，所以程序运行的过程中，增删改查都不会影响xml文件，xml文件只与treeview数据有关，可能这一点比数据库方便一些。（~~所以我还是强推使用数据库~~）


### menustrip,contextmenustrip菜单栏
&emsp;&emsp;就是一个菜单栏，可以添加一些事件，还是比较方便，推荐使用。
其他也没什么大问题了，百度都能解决，注意问题多考虑一下，少写点bug代码就行了 = v =


### 源码附上 [项目传送门](https://github.com/taoyeh/Winform-Controls)
&emsp;&emsp;友情提示，应该有一些小问题，别喷 (｀・ω・´)
