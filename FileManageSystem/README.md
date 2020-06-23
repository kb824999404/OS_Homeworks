# 文件管理 - 文件系统
>​	**学号**				1851197
​	**姓名**				周楷彬
​	**指导老师**		王冬青老师
​	**上课时间**		周三五六节/周五一二节
​	**联系方式**		*email:* 824999404@qq.com
## 目录
[TOC]
## 项目需求

### 基本任务

​	在内存中开辟一个空间作为文件存储器，在其上实现一个简单的文件系统。

​	退出这个文件系统时，需要该文件系统的内容保存到磁盘上，以便下次可以将其恢复到内存中来。

### 功能描述

- 文件存储空间管理可采取显式链接（如FAT）或者其他方法。（即自选一种方法）

- 空闲空间管理可采用位图或者其他方法。如果采用了位图，可将位图和FAT表合二为一。

- 文件目录采用多级目录结构。至于是否采用索引节点结构，自选。目录项目中应包含：文件名、物理地址、长度等信息。同学可在这里增加一些其他信息。

- 文件系统提供的操作：

  - 格式化

  - 创建子目录

  - 删除子目录

  - 显示目录

  - 更改当前目录

  - 创建文件

  - 打开文件

  - 关闭文件

  - 写文件

  - 读文件

  - 删除文件

### 项目目的

- 熟悉文件存储空间的管理；
- 熟悉文件的物理结构、目录结构和文件操作；
- 熟悉文件系统管理实现；
- 加深对文件系统内部功能和实现过程的理解
## 开发环境

- **开发环境:** Windows 10

- **开发软件:** 

  1. **Visual Studio Code** *1.45.0*

- **开发语言:** html, javascript, css

- **主要引用:**
  1. Node.Js
  2. Electron
  3. Vue.js
  4. Electron-Vue
  5. Element UI

## 操作说明

* 解压`FileManageSystem.zip`，双击文件夹内的`filemanagesystem.exe`，进入文件系统模拟界面
![start](doc-imgs/start.png)
* 请详细阅读**帮助**了解模拟器功能, 点击帮助窗口外或关闭按钮关闭**帮助窗口**
* 单击文件列表或上方的路径的文件夹名跳转到相应的文件夹目录下
* 单击左上方的后退/前进按钮可在历史目录间跳转
![folder](doc-imgs/folder.png)
* 单击文件进行文本编辑，单击**保存**更新文件内容，单击**取消**退出文本编辑
![textEdit](doc-imgs/textEdit.png)
* 鼠标悬停在右上方图标上，出现下拉菜单，可选择**新建文件夹**、**新建文件**、**格式化**、**查看磁盘属性**操作
![menu](doc-imgs/menu.png)
* 鼠标悬停在文件列表右边的图标上，出现下拉菜单，可选择**删除**、**重命名**、**查看属性**操作
![fileDropDown](doc-imgs/fileDropDown.png)
## 系统分析
### 显示链接法

本文件系统中, 文件存储空间管理使用**显示链接**的方法，文件中的内容存放在磁盘不同的块中，每次创建文件时为文件分配数量合适的空闲块。每次写文件时按顺序将文件内容写在相应块中; 删除文件时将原先有内容的位置置为空即可。

### 位图、FAT表

磁盘空闲空间管理在**位图**的基础上进行改造，将存放磁盘上文件位置信息的**FAT表**与传统的位图进行结合，磁盘空闲的位置使用`EMPTY = -1`标识，放有文件的盘块存放文件所在的下一个盘块的位置，文件存放结束的盘块位置使用`END = -2`标识。


## 系统设计
### 界面设计

1. **整体设计**
* Electron
* Vue
* el-container
* el-table
* el-tree
* el-breadcrumb
* el-dropdown
![main](doc-imgs/main.png)
2. **帮助界面**
* el-dialog
![start](doc-imgs/start.png)
3. **文本编辑界面** 
* el-dialog
* el-input
![textEdit](doc-imgs/textEdit.png)

### 类设计

1. **目录项类：**
    ``` js
    /**
    * 目录项类
    * @param {文件名} name
    * @param {文件类型} type
    * @param {修改时间} time
    * @param {文件大小} size
    * @param {文件起始位置} start
    */
    function FCB(name,type,time,size,start)
    {
        if(arguments.length==4)         //函数重载
        {
            this.fileName = name;       //文件名
            this.type = type;           //文件类型
            this.lastModify = time;     //最近修改时间  
            this.size = size;           //文件大小
        }
        else if(arguments.length==5)
        {
            this.fileName = name;       //文件名
            this.type = type;           //文件类型
            this.lastModify = time;     //最近修改时间  
            this.size = size;           //文件大小
            this.start = start;         //文件在内存中初始存放的位置
        }
    }
    ```
2. **目录节点类：**
    ``` js
    /**
    * 目录节点类
    * @param {FCB对象} file
    * @param {文件名} name
    * @param {文件类型} type
    */
    function Node()
    {
        this.fcb = new FCB();
        this.firstChild = null;      //左孩子
        this.nextBrother = null;     //右兄弟
        this.parent = null;          //父结点
        /* 一个参数，FCB file */
        if(arguments.length==1)    
        {
            var file=arguments[0];
            this.fcb=file;
        }
        /* 两个参数。String name,Number type */
        else if(arguments.length==2)
        {
            this.fcb.fileName = arguments[0]
            this.fcb.type = arguments[1];
        }
    }
    ```

3. **目录类：**
    ``` js
    /**
    * 目录类
    * @param {根节点目录项} rootFCB
    */
    function Category(rootFCB)
    {
        if(arguments.length==0)
        {
            this.root=null;
        }
        else if(arguments.length==1)
        {
            this.root = new Node(rootFCB);
        }

        /*清空某目录(当参数为root是为格式化)*/
        this.freeCategory=function(pNode)
        {...}
        /*搜索孩子节点*/
        this.searchChild=function(pNode,fileName,type)
        {...}

        /*搜索文件*/
        this.search=function(pNode,fileName,type)
        {...}
        /*在文件夹中创建文件*/
        this.createFileByName=function(parentName,file)
        {...}

        /*在文件夹中创建文件*/
        this.createFile=function(parentNode,file)
        {...}

        /*删除文件夹*/
        this.deleteFolder=function(currentNode)
        {...}

        /*删除文件*/
        this.deleteFile=function(currentNode)
        {...}

        /*判断同级目录下是否重名*/
        this.noSameName=function(name,pNode,type)
        {...}

        /*寻找该目录下根目录的名称*/
        this.currentRootName=function(pNode,name,type)
        {...}
        /*遍历树*/
        this.traverseNode=function(pNode)
        {...}

        /*获取路径*/
        this.getPath=function(pNode,rootNode)
        {...}
    }
    ```
4. **虚拟磁盘类：**
    ``` js
    /**
    * 虚拟磁盘类
    * @param {磁盘容量} size 
    * @param {块大小} blocksize
    */
    function VirtualDisk(size,blocksize)
    {
        this.size = size;                                   //磁盘容量
        this.blockSize = blocksize;                         //块大小
        this.blockNum = Math.ceil(size / blocksize);        //磁盘块数
        this.remain = this.blockNum;                        //磁盘剩余空间

        this.memory =new Array(this.blockNum);              //内存空间
        this.bitMap =new Array(this.blockNum);              //位图表
        for (var i = 0; i < this.blockNum; i++)
        {
            this.bitMap[i] = EMPTY;     //初始化位图表为全部可用
            this.memory[i] = "";        //初始化内存内容为空
        }

        /*给文件分配空间并添加内容*/
        this.giveSpace=function(fcb,content)
        {...}

        /*读取文件内容*/
        this.getFileContent=function(fcb)
        {...}

        /*删除文件内容*/
        this.deleteFileContent=function(start,size)
        {...}

        /*更新文件内容*/
        this.fileUpdate=function(nowFCB,newContent)
        {...}

        /*获取所占块数*/
        this.getBlockSize=function(size)
        {...}
    }
    ```

5. **文件管理系统类：**
    ``` js
    /**
    * 文件管理系统类
    */
    function FileManageSystem()
    {
        var root = new FCB("root", FOLDER, "", 1);
        this.category = new Category();                  //创建目录
        this.rootNode = new Node(root);                  //目录的根节点
        this.currentRoot = this.rootNode;                //当前根节点
        this.category.root = this.rootNode;
        this.MyDisk = new VirtualDisk(1000, 2);          //申请内存空间
        this.Init=function()
        {
            /*恢复文件管理系统*/
            this.readCategory();        //读取目录信息文件
            this.readBitMap();          //读取位图文件
            this.readMyDisk();          //读取虚拟磁盘内存文件
            this.readDiskInfo();        //读取虚拟磁盘信息文件
        }
        this.Save=function()
        {
            /*保存文件管理系统*/
            this.checkPath();       //检查目录是否存在
            this.writeCategory();   //写入目录信息文件
            this.writeBitMap();     //写入位图文件
            this.writeMyDisk();     //写入虚拟磁盘文件
            this.writeDiskInfo();    //写入虚拟磁盘信息文件
        }

    }
    ```

### 状态设计

1. **文本文件标识:** `global.TXTFILE = 0;`
2. **文件夹标识:** `global.FOLDER = 1;`
3. **当前块为空:** `global.EMPTY = -1;`
4. **文件结束:** `global.END = -2;`                                                      

## 系统实现
### 	虚拟磁盘

#### 	给文件分配空间并添加内容

- 获取FCB指向的文件大小
- 该文件大小大于剩余空间 => 无法存储下该文件, 直接返回
- 该文件大小小于剩余空间 => 通过位图找到第一个`EMPTY`位置, 从此位置开始存放 => 将FCB中的起始位置信息更新 
- 从此位置开始逐项的存储信息 => 剩余空间减一 => 并以链接的方式存储信息
- 当处理到最后一个数据时 => 将位图中填入`END`标识作为文件尾
``` js
/*给文件分配空间并添加内容*/
this.giveSpace=function(fcb,content)
{
    var blocks = this.getBlockSize(fcb.size);

    if (blocks <= this.remain)
    {
        /*找到该文件开始存放的位置*/
        var start = 0;
        for (var i = 0; i < this.blockNum; i++) 
        {
            if (this.bitMap[i] == EMPTY)
            {
                this.remain--;
                start = i;
                fcb.start = i;
                this.memory[i] = content.substr(0, this.blockSize);

                break;
            }
        }

        /*从该位置往后开始存放内容*/
        for (var j = 1, i = start + 1; j < blocks && i < this.blockNum; i++)
        {
            if (this.bitMap[i] == EMPTY)
            {
                this.remain--;

                this.bitMap[start] = i;  //以链接的方式存储每位数据
                start = i;

                if (blocks != 1)
                {
                    if (j != blocks - 1)
                    {
                        this.memory[i] = content.substr(j * this.blockSize, this.blockSize);
                    }
                    else
                    {
                        this.bitMap[i] = END;    //文件尾
                        if (fcb.size % this.blockSize != 0)
                        {
                            this.memory[i] = content.substr(j * this.blockSize, 
                                content.length - j * this.blockSize);
                        }
                        else
                        {
                            this.memory[i] = content.substr(j * this.blockSize, this.blockSize);
                        }
                    }

                }
                else
                    { this.memory[i] = content; }

                j++;   //找到一个位置
            }
        }
        return true;
    }
    else { return false; }
}
```

#### 	读取文件内容

- FCB中的起始位置为`EMPTY`状态 => 该文件无内容, 直接返回
- FCB中的起始位置不为`EMPTY`状态  => 首先获取FCB指向的文件所占块的数量和起始地址
- 从起始位置开始逐一读取内存中的数据, 并拼接起来, 之后跳转到下一数据位置继续读取
``` js
/*读取文件内容*/
this.getFileContent=function(fcb)
{
    if (fcb.start == EMPTY)
        { return ""; }
    else
    {
        var content = "";
        var start = fcb.start;
        var blocks = this.getBlockSize(fcb.size);

        var count = 0, i = start;
        while(i < this.blockNum)
        {
            if (count == blocks)
            {
                break;
            }
            else
            {
                content += this.memory[i];       //内容拼接内存的一个单元的数据
                i = this.bitMap[i];              //跳转到位图指向的下一个存储单元
                count++;
            }
        }

        return content;
    }
}
```

#### 	删除文件内容

- 首先获取到被删除内容所占块的数量
- 从该内容的其实块位置开始, 逐个内存单元的清空虚拟磁盘上的信息
- 在删除操作中要首先记录即将跳转的位置(防止清空后找不到接下来的位置)
- 之后清空该位, 并跳转到下一个数据位置继续删除
``` js
/*删除文件内容*/
this.deleteFileContent=function(start,size)
{
    var blocks = this.getBlockSize(size);

    var count = 0, i = start;
    while(i < this.blockNum)
    {
        if (count == blocks)
        {
            break;
        }
        else
        {
            this.memory[i] = "";             //逐内存单元的清空
            this.remain++;

            var next = this.bitMap[i];       //先记录即将跳转的位置
            this.bitMap[i] = EMPTY;          //清空该位
            i = next;

            count++;
        }
    }
}
```

#### 	更新文件内容

- 由于更新后的文件size与原文件不同, 因此不能采用直接覆盖的方式
- 首先调用方法删除原内容
- 再在虚拟磁盘上开辟新的块, 并按顺序在新的块中添加内容

``` js
/*更新文件内容*/
this.fileUpdate=function(nowFCB,newContent)
{
    var oldSize = nowFCB.size;
    var oldStart = nowFCB.start;

    nowFCB.size = newContent.length;
    nowFCB.lastModify = new Date().toLocaleString();

    //在内存上给文件分配空间
    if (nowFCB.size>0)
    {
        if(this.remain<nowFCB.size)
        {
            return false;
        }
        else
        {
            if(oldStart==-1)    //如果该文本文件之前为空(第一次修改)
            {
                this.giveSpace(nowFCB, newContent);
            }
            else                //更新
            {
                this.deleteFileContent(oldStart, oldSize);   //删除原内容
                this.giveSpace(nowFCB, newContent);          //开辟新的块并添加内容
            }
        }
        return true;                
    }
    return false;

}
```

### 目录

#### 清空某目录(当参数为root是为格式化)

*<u>当参数传递的为root时 <=> 格式化磁盘</u>*

- 如果当前目录为空 => 不需要删除, 直接返回
- 孩子不为空 => 递归调用清空方法删除孩子的子树 => 获取孩子的兄弟节点
- 将自己设置为空
``` js
/*清空某目录(当参数为root是为格式化)*/
this.freeCategory=function(pNode)
{
    if (pNode == null)
        { return; }
    var child=pNode.firstChild;
    pNode.firstChild=null;
    while(child!=null)
    {
        this.freeCategory(child);
        child=child.nextBrother;
    }
}
```
#### 搜索文件

- 如果该目录下没有文件 => 搜索失败, 直接犯规
- 如果左孩子不为空 且 左孩子文件类型匹配 且 左孩子文件名匹配 => 返回左孩子
- 如果右兄弟不为空 且 右兄弟文件类型匹配 且 右兄弟文件名匹配 => 返回右兄弟
- 递归的搜索左孩子子树 => 找到了 => 返回该结点
- 递归的搜索右兄弟子树 => 返回搜索结果(找到了/没找到)
``` js
/*搜索文件*/
this.search=function(pNode,fileName,type)
{
    if (pNode == null)
        { return null; }
    if (pNode.fcb.fileName == fileName && pNode.fcb.type == type)
        { return pNode; }
    if (pNode.firstChild == null && pNode.nextBrother == null)
        { return null; }
    else
    {
        var firstChild = this.search(pNode.firstChild, fileName, type); //递归的搜索左孩子的子树
        if (firstChild != null)
            { return firstChild; }
        else
            { return this. search(pNode.nextBrother, fileName, type); }   //递归的搜索右兄弟的子树
    }
}
```

#### 在文件夹中创建文件

- 如果是在root目录下新建文件 => 直接创建
- 如果实在其他目录下创建文件 => 获取到该目录的父文件夹
  - 如果父节点为空 => 无法创建
  - 如果父节点左孩子为空 => 该文件夹为空 => 新创建的文件为第一个文件, 放到左孩子的位置 => 将新加入的文件父节点设置为当前目录结点
  - 该文件夹中已经有很多文件 => 顺序找到该文件夹下最后一个文件存储的位置 => 将文件添加在最后的位置 => 将父节点设置为当前目录
``` js
/*在文件夹中创建文件*/
this.createFileByName=function(parentName,file)
{
    var parentNode = this.search(this.root, parentName, FOLDER);    //找到父文件夹

    if (this.root == null||parentNode == null)
        { return; }

    if (parentNode.firstChild == null)  //该文件夹为空
    {
        parentNode.firstChild = new Node(file);     //新创建的文件为第一个文件, 放到左孩子的位置
        parentNode.firstChild.parent = parentNode;
    }
    else
    {

        var temp = parentNode.firstChild;
        while (temp.nextBrother != null)        //顺序找到该文件夹下最后一个文件存储的位置
            temp = temp.nextBrother;

        temp.nextBrother = new Node(file);
        temp.nextBrother.parent = parentNode;

    }
}
```

#### 	删除文件夹/文件

- 调用`search`方法找到要删除的文件夹
- 获取要删除文件夹的父节点
- 如果要删除的文件夹是父文件夹中的第一项 => 更新父文件夹的左孩子
- 如果要删除的文件夹不是父文件夹中的第一项 => 寻找到该结点的哥哥结点 => 让其指向自己的弟弟
- 判断要删除的结点类型:
  - 如果待删的是文件夹 => 最后删除当前结点下的所有文件
  - 如果待删的是文件 => 将该文件结点释放即可

``` js
/*删除文件夹*/
this.deleteFolder=function(currentNode)
{
    var parentNode = currentNode.parent;

    if (parentNode.firstChild == currentNode)   //如果要删除的文件夹是父文件夹中的第一项
        { parentNode.firstChild = currentNode.nextBrother; }    //更新父文件夹的左孩子
    else
    {
        var temp = parentNode.firstChild;
        while (temp.nextBrother != currentNode)
        {
            temp = temp.nextBrother;
        }

        temp.nextBrother = currentNode.nextBrother;     //找到该文件的哥哥, 让其指向自己的弟弟
    }

    this.freeCategory(currentNode);        //删除当前结点下的所有文件
}

/*删除文件*/
this.deleteFile=function(currentNode)
{
    var parentNode = currentNode.parent;

    if (parentNode.firstChild == currentNode)   //如果要删除的文件是父文件夹中的第一项
    { parentNode.firstChild = currentNode.nextBrother; }    //更新父文件夹的左孩子    
    else
    {
        var temp = parentNode.firstChild;
        while (temp.nextBrother != currentNode)
        {
            temp = temp.nextBrother;
        }

        temp.nextBrother = currentNode.nextBrother;
    }
    currentNode = null;
}
```

#### 	判断同级目录下是否重名

- 如果该结点的左孩子为空 => 该目录中无任何文件夹和文件 => 无重名文件并返回true
- 如果该结点的左孩子不为空 且 左孩子与当前文件类型相同 且 左孩子与当前文件同名 => 发现重名文件并返回false 
- 沿着右兄弟逐一寻找是否有某个结点与当前文件类型相同 且 与当前文件同名:
  - 发现 => 发现重名文件并返回false
- 找到目录结尾 => 无重名文件并返回true
``` js
/*判断同级目录下是否重名*/
this.noSameName=function(name,pNode,type)
{
    //pNode为该级目录的根节点
    pNode = pNode.firstChild;
    if (pNode == null)  //该目录中无任何文件夹和文件
        { return true; }
    if (pNode.fcb.fileName == name && pNode.fcb.type == type)   //第一个文件夹/文件重名
        { return false; }
    else
    {

        var temp = pNode.nextBrother;
        while (temp != null)
        {
            if (temp.fcb.fileName == name && temp.fcb.type == type)
                { return false; }
            temp = temp.nextBrother;
        }
        return true; //不重名
    }
}
```

#### 遍历树
- 根据当前节点目录项设置当前目录信息
- 递归调用函数递归当前节点子树 => 获得的目录信息加入到当前目录信息的子目录信息中
- 返回当前目录信息
``` js
/*遍历树*/
this.traverseNode=function(pNode){
    var data={};
    data.label=pNode.fcb.fileName;
    if(pNode.fcb.type==FOLDER)
    {
        data.type='FOLDER';
    }
    else
    {
        data.type='FILE';
    }
    data.node=pNode;
    var children=[];
    var childNode=pNode.firstChild;
    while(childNode!=null)
    {
        children.push(this.traverseNode(childNode));
        childNode=childNode.nextBrother;
    }
    if(children.length!=0)
    {
        data.children=children;
    }
    return data;

}
```
### 主窗口
#### 加载文件系统
* 生成目录树
* 获取根节点到当前节点的路径
* 获取当前目录下文件列表
``` js
// 生成目录树
generateTree(){
    var data=[];
    var root=fileSystem.category.root;
    var childNode=root.firstChild;
    while(childNode!=null)
    {
        data.push(fileSystem.category.traverseNode(childNode));
        childNode=childNode.nextBrother;
    }
    this.treeData=data;          
}
// 获取根节点到当前节点的路径
currentPath(){
    var root=fileSystem.category.root;
    var current=fileSystem.currentRoot;
    var path=[];
    var pathNode=fileSystem.category.getPath(current,root);
    if(pathNode!=null||pathNode.length>0)
    {
        for(var i=0;i<pathNode.length;i++)
        {
        path.push({node:pathNode[i],name:pathNode[i].fcb.fileName});
        }
    }
    this.path=path;
}
// 获取当前目录下文件列表
getFileList(){
    var files=[];
    var current=fileSystem.currentRoot;
    var childNode=current.firstChild;
    while(childNode!=null)
    {
        var node={name:childNode.fcb.fileName,time:childNode.fcb.lastModify,size:childNode.fcb.size+'B'};
        if(childNode.fcb.type==FOLDER)
        {
        node.type='文件夹';
        }
        else
        {
        node.type='文件';
        }
        files.push(node);
        childNode=childNode.nextBrother;
    }
    this.fileList=files;
}
```
#### 新建文件/文件夹
- 用户成功输入文件夹/文件名
- 如果当前路径下没有重名的文件 => 获取时间信息 => 创建文件项 => 将文件项加入目录中
- 如果当前路径下有重名的文件 => 弹出对话框提醒用户已经有同名的文件 => 创建失败
``` js
// 新建文件夹
addFolder(){
    this.$prompt('请输入文件夹名称', '新建文件夹', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
    }).then(({ value }) => {
        console.log('Add Folder!');

        if (fileSystem.category.noSameName(value, fileSystem.currentRoot, FOLDER))
        {
            var time=new Date().toLocaleString();
            console.log(time);
            fileSystem.category.createFile(fileSystem.currentRoot, 
            new FCB(value, FOLDER, time, 0));  //文件夹加入到目录中
            this.$message({
            type: 'success',
            message: '新建文件夹 ' + value +' 成功！',
            duration:1000,
            });
            fileSystem.Save();
            this.updateData();
        }
        else
        {
        this.$message({
            type: 'error',
            message: '已存在名为 ' + value +' 的文件夹，创建失败！',
            duration:1000,
        });
        }

    }).catch((err) => {
        this.$message({
        type: 'info',
        message: '取消新建文件夹',
        duration:1000,
        });  
        console.log(err);     
    });
}
// 新建文件
addFile(){
    this.$prompt('请输入文件名称', '新建文件', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
    }).then(({ value }) => {
        console.log('Add File!');
        console.log(fileSystem.category);
        if (fileSystem.category.noSameName(value, fileSystem.currentRoot, TXTFILE))
        {
            var time=new Date().toLocaleString();
            console.log(time);
            fileSystem.category.createFile(fileSystem.currentRoot, 
            new FCB(value, TXTFILE, time, 0));  //文件夹加入到目录中
            this.$message({
            type: 'success',
            message: '新建文件 ' + value +' 成功！',
            duration:1000,
            });
            fileSystem.Save();
            this.updateData();
        }
        else
        {
        this.$message({
            type: 'error',
            message: '已存在名为 ' + value +' 的文件，创建失败！',
            duration:1000,
        });
        }

    }).catch((err) => {
        this.$message({
        type: 'info',
        message: '取消新建文件',
        duration:1000,
        });  
        console.log(err);     
    });
},
```
#### 	删除文件夹/文件

``` js
deleteFile(index){
    var node=this.fileList[index];
    var current=fileSystem.currentRoot;
    var name=node.name;
    this.$confirm('是否要删除该'+node.type+'？','删除'+node.type,{
    type: 'warning',
    center: true
    })
    .then(_ => {

        if(node.type=='文件夹')
        {
        var result=fileSystem.category.searchChild(current,name,FOLDER);
        if(result!=null)
        {
            fileSystem.category.deleteFolder(result);
        }
        }
        else
        {
        var result=fileSystem.category.searchChild(current,name,TXTFILE);
        if(result!=null)
        {
            fileSystem.category.deleteFile(result);
        }
        }
        fileSystem.Save();
        this.updateData();
        this.$message({
        type: 'success',
        message: '删除'+node.type+'成功！',
        duration:1000,
        });
    })
    .catch(_ => {});
}
```
#### 目录跳转
* 获取相应的节点
* 跳转至该节点的目录下
* 保存历史节点信息
* 更新界面
``` js
// 跳转至节点
changeNode(node){
    if(node!=fileSystem.currentRoot){
        this.backNode=fileSystem.currentRoot;
        fileSystem.currentRoot=node;
        this.forwardNode=null;
        this.updateData();
    }
}
```
#### 历史目录跳转
* 保存历史节点信息
* 跳转目录
* 更新界面
``` js
// 后退
onBackClick(){
    if(this.backNode!=null){
        this.forwardNode=fileSystem.currentRoot;
        fileSystem.currentRoot=this.backNode;
        this.backNode=null;
        this.updateData();
    }
}
// 前进
onForwardClick(){
    if(this.forwardNode!=null){
        this.backNode=fileSystem.currentRoot;
        fileSystem.currentRoot=this.forwardNode;
        this.forwardNode=null;
        this.updateData();
    }
}
```
#### 格式化
- 用户点击格式化按钮 => 弹出对话框再次提醒用户是否确定删除
- 确定 => 删除目录项 => 清空虚拟内存 => 位图置为空 => 将剩余空间重新置为最大 => 更新界面 => 将信息保存至文件
``` js
formatDisk(){
    this.$confirm('是否要将磁盘格式化？','格式化',{
        type: 'warning',
        center: true
    })
        .then(_ => {
        fileSystem.category.freeCategory(fileSystem.rootNode);
        for (var i = 0; i < fileSystem.MyDisk.blockNum; i++)
        {
            fileSystem.MyDisk.memory[i] = "";
            fileSystem.MyDisk.bitMap[i] = EMPTY;
            fileSystem.MyDisk.remain = fileSystem.MyDisk.blockNum;
        }
        fileSystem.currentRoot=fileSystem.rootNode;
        fileSystem.Save();
        this.updateData();
        this.backNode=null;
        this.forwardNode=null;
        this.$message({
            type: 'success',
            message: '格式化成功！',
            duration:1000,
        });
        })
        .catch(_ => {});
}
```
#### 编辑文件
* 打开文本编辑界面
* 从文件读取信息，将文本域内容设置为文件内容
* 用户对文本内容进行修改 => 用户点击保存按钮
* 从文本域获取新文本内容 =>将文件内容更新为新的文本
* 更新界面，将信息保存至文件
``` js
// 打开文本编辑界面
editFile(node){
    this.editFileNode=node;
    this.textEditVisible=true;
    this.editFileName=node.fcb.fileName;
    this.textarea=unescape(fileSystem.MyDisk.getFileContent(node.fcb));
}
// 保存修改文件
saveFile(){
    if(this.textEditVisible&&this.editFileNode!=null){
        console.log(escape(this.textarea));
        if(fileSystem.MyDisk.fileUpdate(this.editFileNode.fcb,escape(this.textarea))) //保存成功
        {
        this.cancelEdit();
        this.$message({
            type: 'success',
            message: '保存文件 ' + this.editFileName+' 成功！',
            duration:1000,
        });
        fileSystem.Save();
        this.updateData();
        }
    }
}
```
#### 文件/文件夹重命名
- 用户成功输入文件夹/文件名
- 如果当前路径下没有重名的文件 => 获取时间信息 => 修改文件名和最后修改时间
- 如果当前路径下有重名的文件 => 弹出对话框提醒用户已经有同名的文件 => 重命名失败
``` js
renameFile(index){
    var node=this.fileList[index];
    var current=fileSystem.currentRoot;
    var name=node.name;
    this.$prompt('请输入名称', '重命名', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
    }).then(({ value }) => {
        console.log(value);
        var result=null;
        if(node.type=='文件夹')
        {
        result=fileSystem.category.searchChild(current,name,FOLDER);

        }
        else
        {
        result=fileSystem.category.searchChild(current,name,TXTFILE);
        }
        if(result!=null)
        {
        var newNode=fileSystem.category.searchChild(current,value,result.fcb.type);
        if(newNode==null)
        {
            var time=new Date().toLocaleString();
            result.fcb.fileName=value;
            result.fcb.lastModify=time;
            this.$message({
            type: 'success',
            message: '重命名成功！',
            duration:1000,
            });
            fileSystem.Save();
            this.updateData();
            
        }
        else
        {
            this.$message({
            type: 'error',
            message: '已存在名为 ' + value +' 的'+node.type+'，重命名失败！',
            duration:1000,
        });
        }
        }
    }).catch((err) => {
        this.$message({
        type: 'info',
        message: '取消重命名',
        duration:1000,
        });  
        console.log(err);     
    });
}
```

### 读写文件
#### 读/写目录信息文件
CategoryInfo.json中保存了目录树按层次遍历的所有目录项信息，包括
- 当前目录下的根节点
- 文件名称
- 文件类型
- 上次修改时间
- 文件大小
- 文件起始位置
``` js
//读取目录文件
fileManageSystem.readCategory=function(){
  var path = "Data/CategoryInfo.json";
  fs.access(path,function(err){
      if(err){
          console.log(path+' No Exists!');

      }
      else
      {
          var data=JSON.parse(fs.readFileSync(path));
          for(var i=0;i<data.length;i++)
          {
            var item=data[i];
            var now = new FCB(item.name, item.type, item.lastModify, item.size,item.start);
            fileManageSystem.category.createFileByName(item.parentName, now);   //把文件结点的内容加到目录中
            console.log(fileManageSystem.rootNode.firstChild.fcb.start);
        }

      }
  });
}

//把目录写入文件中
fileManageSystem.writeCategory=function(pNode)
{
    function convertData(pNode,parentNode)    //目录项转化为JSON对象
    {
        var item={
          parentName:parentNode.fcb.fileName,   //父结点的名字
          name:pNode.fcb.fileName,              //文件的名字
          type:pNode.fcb.type,                  //文件的类型
          lastModify:pNode.fcb.lastModify,      //最后修改的时间
          size:pNode.fcb.size,                  //文件的大小
        }
        if (pNode.fcb.type == TXTFILE)                //文件的开始位置
        {
          item.start=pNode.fcb.start;
        }
        else if (pNode.fcb.type == FOLDER)            //若为文件夹则写入-1
        {
          item.start=-1;
        }
        return item;
    }
    
    var path = "Data/CategoryInfo.json";
    var q=[],data=[],parent,current;
    q.push(fileManageSystem.category.root);
    while(q.length!=0)
    {
      parent=q.shift();
      current=parent.firstChild;
      while(current!=null)
      {
        q.push(current);
        data.push(convertData(current,parent));
        current=current.nextBrother;
      }
    }
    fs.writeFileSync(path,JSON.stringify(data));


}
```
#### 读/写位图文件
``` js
/*读取位图文件*/
fileManageSystem.readBitMap=function()
{
  var path = "Data/BitMapInfo.json";
  fs.access(path,function(err){
      if(err){
          console.log(path+' No Exists!');
      }
      else
      {
          var data=JSON.parse(fs.readFileSync(path));
          fileManageSystem.MyDisk.bitMap=data;
      }
  });
}

/*写入位图文件*/
fileManageSystem.writeBitMap=function()
{
  var path = "Data/BitMapInfo.json";
  fs.writeFileSync(path,JSON.stringify(fileManageSystem.MyDisk.bitMap));
}
```
#### 读/写虚拟磁盘内存文件
``` js

/*读取虚拟磁盘内存文件*/
fileManageSystem.readMyDisk=function()
{
  var path = "Data/Memory.json";
  fs.access(path,function(err){
      if(err){
          console.log(path+' No Exists!');
      }
      else
      {
          var data=JSON.parse(fs.readFileSync(path));
          fileManageSystem.MyDisk.memory=data;
      }
  });
}

/*写入虚拟磁盘内存文件*/
fileManageSystem.writeMyDisk=function()
{
  var path = "Data/Memory.json";
  fs.writeFileSync(path,JSON.stringify(fileManageSystem.MyDisk.memory));
}
```
#### 读/写虚拟磁盘信息文件
``` js
/*读取虚拟磁盘信息文件*/
fileManageSystem.readDiskInfo=function()
{
  var path = "Data/DiskInfo.json";
  fs.access(path,function(err){
      if(err){
          console.log(path+' No Exists!');
      }
      else
      {
          var data=JSON.parse(fs.readFileSync(path));
          fileManageSystem.MyDisk.size=data.size;
          fileManageSystem.MyDisk.blockSize=data.blockSize;
          fileManageSystem.MyDisk.blockNum=data.blockNum;
          fileManageSystem.MyDisk.remain=data.remain;
      }
  });
}

/*写入虚拟磁盘信息文件*/
fileManageSystem.writeDiskInfo=function()
{
  var path = "Data/DiskInfo.json";
  var data={
    size:fileManageSystem.MyDisk.size,blockSize:fileManageSystem.MyDisk.blockSize,
    blockNum:fileManageSystem.MyDisk.blockNum,remain:fileManageSystem.MyDisk.remain
  };
  fs.writeFileSync(path,JSON.stringify(data));
}
```
 
## 功能实现截屏展示
### 帮助界面
![start](doc-imgs/start.png)
### 新建文件夹
![newfolder](doc-imgs/addfolder.png)
### 新建文件
![addfile](doc-imgs/addfile.png)
### 格式化
![format](doc-imgs/format.png)
### 删除文件
![delete](doc-imgs/deletefile.png)
### 重命名
![rename](doc-imgs/rename.png)
### 文件编辑
![textedit](doc-imgs/textedit.png)
### 文件属性
![fileinfo](doc-imgs/fileinfo.png)
### 磁盘属性
![diskinfo](doc-imgs/diskinfo.png)