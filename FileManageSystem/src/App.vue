<template>
  <div id="app">
    <el-row class="Page">
      <el-col class="sider"  :span="4">
        <el-tree :data="treeData" :props="defaultProps" @node-click="handleNodeClick">
          <template class="custom-tree-node" slot-scope="{ node, data }">
            <el-button type="text"  @click="openTreeNode(data)">
              <i class="el-icon-folder" v-if="data.type=='FOLDER'">{{ node.label }}</i> 
              <i class="el-icon-document" v-else>{{ node.label }}</i> 
            </el-button>
          </template>
        </el-tree>
      </el-col>
      <el-col :span="20">
           <el-container>
              <el-header class="headerBar">
                    <el-button size="medium" icon="el-icon-back" circle 
                    :disabled="backNode==null" @click="onBackClick"></el-button>
                    <el-button size="medium" icon="el-icon-right" circle 
                    :disabled="forwardNode==null" @click="onForwardClick"></el-button>
                    <el-breadcrumb separator-class="el-icon-arrow-right" >
                      <el-breadcrumb-item v-for="(node,key) in path" :key="key">
                        <el-button type="text" @click="openHeaderNode(node)">{{node.name}}</el-button>
                      </el-breadcrumb-item>
                    </el-breadcrumb>
                    <el-button size="medium" icon="el-icon-help" @click="heplerVisible=true" circle></el-button>
                    <el-dropdown>
                      <el-button size="medium" icon="el-icon-more" circle></el-button>
                      <el-dropdown-menu slot="dropdown">
                        <el-dropdown-item>
                          <el-button size="medium" type="text" @click="addFolder">
                            <i class="el-icon-folder-add" > 新建文件夹</i>
                          </el-button>
                        </el-dropdown-item>
                        <el-dropdown-item>
                          <el-button size="medium" type="text" @click="addFile">
                            <i class="el-icon-document-add" > 新建文件</i>
                          </el-button>
                        </el-dropdown-item>
                        <el-dropdown-item>
                          <el-button size="medium" type="text" @click="formatDisk">
                            <i class="el-icon-brush" > 格式化</i>
                          </el-button>
                        </el-dropdown-item>
                        <el-dropdown-item>
                          <el-button size="medium" type="text" @click="displayDiskInfo">
                            <i class="el-icon-warning-outline" > 磁盘属性</i>
                          </el-button>
                        </el-dropdown-item>
                      </el-dropdown-menu>
                    </el-dropdown>
              </el-header>
              <el-main>
                <el-table :data="fileList" :height="tableHeight">
                  <template slot="empty">
	                  <p>该目录为空！</p>
                  </template>
                  <el-table-column prop="name" label="名称">
                    <template slot-scope="scope">
                      <el-button type="text" v-if="scope.row.type=='文件夹'" 
                      @click="openFile(scope.$index)">
                        <i class="el-icon-folder" >{{scope.row.name}}</i> 
                      </el-button>
                      <el-button type="text" v-else
                      @click="openFile(scope.$index)">
                        <i class="el-icon-document" >{{scope.row.name}}</i> 
                      </el-button>
                    </template>
                  </el-table-column>
                  <el-table-column prop="time" label="修改时间"></el-table-column>
                  <el-table-column prop="type" label="类型" width="80"></el-table-column>
                  <el-table-column prop="size" label="大小">
                      <template slot-scope="scope">
                        <div v-if="scope.row.type=='文件'">{{scope.row.size}}</div>
                      </template>
                  </el-table-column>
                  <el-table-column align="right" width="60">
                    <template slot-scope="scope">
                      <el-dropdown>
                        <el-button size="medium" icon="el-icon-more-outline" type="text"></el-button>
                        <el-dropdown-menu slot="dropdown">
                          <el-dropdown-item>
                            <el-button size="medium" type="text" @click="deleteFile(scope.$index)">
                              <i class="el-icon-delete" > 删除</i>
                            </el-button>
                          </el-dropdown-item>
                          <el-dropdown-item>
                            <el-button size="medium" type="text" @click="renameFile(scope.$index)">
                              <i class="el-icon-edit" > 重命名</i>
                            </el-button>
                          </el-dropdown-item>
                          <el-dropdown-item>
                            <el-button size="medium" type="text" @click="displayInfo(scope.$index)">
                              <i class="el-icon-warning-outline" > 属性</i>
                            </el-button>
                          </el-dropdown-item>
                        </el-dropdown-menu>
                      </el-dropdown>
                    </template>
                  </el-table-column>
                </el-table>
              </el-main>
           </el-container>
      </el-col>
      <el-dialog id="textEdit" :title="editFileName" 
      :visible.sync="textEditVisible" :before-close="handleEditClose">
        <el-input
          type="textarea"
          :rows="10"
          v-model="textarea">
        </el-input>
        <span slot="footer" class="dialog-footer">
          <el-button @click="cancelEdit">取 消</el-button>
          <el-button @click="saveFile" type="primary" >保 存</el-button>
        </span>
      </el-dialog>
      <el-dialog id="hepler"
      :visible.sync="heplerVisible" :before-close="handleHeplerClose">
      <span slot="title"><i class="el-icon-help"> 帮助</i></span>
        <p>欢迎使用文件管理系统模拟器！</p>
        <el-row type="flex" justify="center">
          <el-col :span="22" class="helper-text">
            <p>1.单击打开文件或文件夹</p>
            <p>2.通过文件右方下拉菜单进行删除、重命名和查看属性</p>
            <p>3.左侧的目录树中单击展开文件夹或单击文件夹名打开文件夹</p>
            <p>4.通过右上方下拉菜单进行新建文件、新建文件夹、格式化和查看磁盘属性</p>
            </el-col>
        </el-row>
        
      </el-dialog>
      <el-dialog id="fileInfo" :visible.sync="infoVisible">
        <span slot="title" class="dialog-title"><i class="el-icon-warning-outline"> 属性</i></span>
        <div v-if="displayNode!=null">
        <el-row>
          <el-col :span="8"><p class="infoKey">名称：</p></el-col>
          <el-col :span="16"><p>{{displayNode.name}}</p></el-col>
        </el-row>
        <el-row>
          <el-col :span="8"><p class="infoKey">修改时间：</p></el-col>
          <el-col :span="16"><p>{{displayNode.time}}</p></el-col>
        </el-row>
       <el-row>
          <el-col :span="8"><p class="infoKey">类型：</p></el-col>
          <el-col :span="16"><p>{{displayNode.type}}</p></el-col>
        </el-row>
        <el-row>
          <el-col :span="8"><p class="infoKey">大小：</p></el-col>
          <el-col :span="16"><p>{{displayNode.size}}</p></el-col>
        </el-row>
        </div>
      </el-dialog>
      <el-dialog id="diskInfo" :visible.sync="diskInfoVisible">
        <span slot="title" class="dialog-title"><i class="el-icon-warning-outline"> 磁盘属性</i></span>
        <div v-if="diskInfo!=null">
        <el-row>
          <el-col :span="8"><p class="infoKey">磁盘容量：</p></el-col>
          <el-col :span="16"><p>{{diskInfo.size}}B</p></el-col>
        </el-row>
        <el-row>
          <el-col :span="8"><p class="infoKey">块大小：</p></el-col>
          <el-col :span="16"><p>{{diskInfo.blocks}}B</p></el-col>
        </el-row>
       <el-row>
          <el-col :span="8"><p class="infoKey">磁盘块数：</p></el-col>
          <el-col :span="16"><p>{{diskInfo.blockNum}}</p></el-col>
        </el-row>
        <el-row>
          <el-col :span="8"><p class="infoKey">剩余块数：</p></el-col>
          <el-col :span="16"><p>{{diskInfo.remain}}</p></el-col>
        </el-row>
        </div>
      </el-dialog>
    </el-row>
  </div>
</template>

<script>
import FCB from './scripts/FCB';
  export default {
    name: 'filemanagesystem',
    data () {
      return {
        title:'FileManageSystem',
        defaultProps: {
          children: 'children',
          label: 'label',
        },
        treeData:[],
        path:[],
        fileList:[],
        backNode:null,
        forwardNode:null,
        textEditVisible:false,
        textarea:'',
        editFileName:'',
        editFileNode:null,
        heplerVisible:true,
        infoVisible:false,
        displayNode:null,
        diskInfoVisible:false,
        diskInfo:{
          size:0,blocks:0,blockNum:0,remain:0
        },
        tableHeight: window.innerHeight * 0.82 
      }
    },
    methods:{
      handleNodeClick(data) {
        console.log(data);
      },
      changeNode(node){
        if(node!=fileSystem.currentRoot){
          this.backNode=fileSystem.currentRoot;
          fileSystem.currentRoot=node;
          this.forwardNode=null;
          this.updateData();
        }
      },
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
      },
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
      },
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
      },
      updateData(){
        this.generateTree();
        this.currentPath();
        this.getFileList();
        console.log('Update Data!');
      },
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
      },
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
      openFile(index){
        var node=this.fileList[index];
        var current=fileSystem.currentRoot;
        var name=node.name;
        if(node.type=='文件夹')
        {
          var result=fileSystem.category.searchChild(current,name,FOLDER);
          if(result!=null)
          {
            this.changeNode(result);
          }
        }
        else
        {
          var result=fileSystem.category.searchChild(current,name,TXTFILE);
          if(result!=null)
          {
            this.editFile(result);
          }
        }

      },
      openTreeNode(data){
        if(data.type=='FOLDER')
        {
          this.changeNode(data.node);
        }
        else
        {
          this.changeNode(data.node.parent);
        }
      },
      openHeaderNode(node){
        this.changeNode(node.node);
      },
      onBackClick(){
        if(this.backNode!=null){
          this.forwardNode=fileSystem.currentRoot;
          fileSystem.currentRoot=this.backNode;
          this.backNode=null;
          this.updateData();
        }
      },
      onForwardClick(){
        if(this.forwardNode!=null){
          this.backNode=fileSystem.currentRoot;
          fileSystem.currentRoot=this.forwardNode;
          this.forwardNode=null;
          this.updateData();
        }
      },
      editFile(node){
        this.editFileNode=node;
        this.textEditVisible=true;
        this.editFileName=node.fcb.fileName;
        this.textarea=unescape(fileSystem.MyDisk.getFileContent(node.fcb));
      },
      handleEditClose(done){
        this.$confirm('文件尚未保存,确认关闭？','关闭',{
          type: 'warning',
          center: true
        })
          .then(_ => {
            done();
            this.cancelEdit();
          })
          .catch(_ => {});
      },
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
      },
      cancelEdit(){
        this.textEditVisible=false;
        this.editFileNode=null;
      },
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
      },
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



      },
      displayInfo(index){
          this.displayNode=this.fileList[index];
          this.infoVisible=true;
      },
      handleHeplerClose(done){
        this.updateData();
        done();
      },
      displayDiskInfo(){
        this.diskInfo.size=fileSystem.MyDisk.size;
        this.diskInfo.blocks=fileSystem.MyDisk.blockSize;
        this.diskInfo.blockNum=fileSystem.MyDisk.blockNum;
        this.diskInfo.remain=fileSystem.MyDisk.remain;
        this.diskInfoVisible=true;
      },
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
    },
    mounted () {
        window.onresize = () => {
            return (() => {
                this.tableHeight=window.innerHeight * 0.82;
            })()
        }
    }
  
  }
</script>
