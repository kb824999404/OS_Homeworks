import FCB from './FCB.js';
import {Category,Node }from './Category.js';
import VirtualDisk from './VirtualDisk';

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


export default FileManageSystem;