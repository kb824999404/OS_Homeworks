import FCB from './FCB.js';

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
    /*搜索孩子节点*/
    this.searchChild=function(pNode,fileName,type)
    {
        if (pNode == null)
        { return null; }
        var child=pNode.firstChild;
        while(child!=null)
        {
            if(child.fcb.fileName==fileName&&child.fcb.type==type)
                return child;
            child=child.nextBrother;
        }
        return null;
    }

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
        console.log(parentNode.firstChild.fcb);
    }

    /*在文件夹中创建文件*/
    this.createFile=function(parentNode,file)
    {

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

    /*获取路径*/
    this.getPath=function(pNode,rootNode)
    {
        if(pNode==null||rootNode==null) return null;
        var path=[];
        path.push(pNode);
        while(pNode!=rootNode)
        {
            if(pNode.parent!=null)
            {
                pNode=pNode.parent;
                path.unshift(pNode);
            }
            else
            {   break;  }
        }
        return path;
        
    }

}

export {Category,Node};