import Vue from 'vue'
import ElementUI from 'element-ui'
import 'element-ui/lib/theme-chalk/index.css'
import axios from 'axios'
import App from './App'
import fs from 'fs'
import readline from 'readline'
import './styles/style.css'
import FileManageSystem from './scripts/FileManageSystem'
import FCB from './scripts/FCB'

if (!process.env.IS_WEB) Vue.use(require('vue-electron'))
Vue.http = Vue.prototype.$http = axios
Vue.config.productionTip = false

Vue.use(ElementUI);


global.EMPTY = -1;        //当前块为空
global.END = -2;          //结束标识
global.TXTFILE = 0;   //文本文件标识
global.FOLDER = 1;    //文件夹标识

var fileManageSystem=new FileManageSystem();
global.fileSystem=fileManageSystem;


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

fileManageSystem.checkPath=function()
{
  var path="Data";
  fs.access(path,function(err){
    if(err){
        console.log(path+' No Exists!');
        fs.mkdirSync(path);
    }
    else
    {
        console.log(path+' Exists!');
    }
  });
}


//初始化
fileManageSystem.Init()




/* eslint-disable no-new */
new Vue({
  components: { App },
  template: '<App/>'
}).$mount('#app');

