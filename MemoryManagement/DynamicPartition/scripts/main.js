
currentTask=1;      //当前作业
// 作业列表
var taskList = [
    new Task("",0),
    new Task("作业1", 130),
    new Task("作业2", 60),
    new Task("作业3", 100),
    new Task("作业2", -60),
    new Task("作业4", 200),
    new Task("作业3", -100),
    new Task("作业1", -130),
    new Task("作业5", 140),
    new Task("作业6", 60),
    new Task("作业7", 50),
    new Task("作业6", -60),
]
//已分配区表
occupyMem=[];
// 空闲区表
useableMem=[
    new Mem(0,640,-1),
];
//日志列表
logList=[];


const ADDSUCCESS = 0;       //添加作业成功
const ADDFAILED = 1;        //添加作业失败
const REMOVESUCCESS = 2;    //释放作业成功
const TASKEND = 3;          //所有任务都完成



myTimer=null;   //定时器

//实例化Vue
app=new Vue({
    el:'#app',
    data:{
        title:"动态分区分配方式模拟",
        alogOptions:[
            {
                value:'first',
                label:'首次适应算法'
            },
            {
                value:'best',
                label:'最佳适应算法'
            }
        ],
        algorithm:'first',  //选择的算法
        memorySize:640,     //内存空间大小
        minMemorySize:200,  //最小内存大小
        maxMemorySize:1000, //最大内存大小
        newTaskName:'',
        newTaskSize:'',
        taskList:taskList,  //作业列表
        occupyMem:occupyMem,   //已分配区表
        useableMem:useableMem,  //空闲区表
        logList:logList,    //日志消息列表
        speed:1000,        //运行速度
        isStart:false,      //开始标志
        isEnd:false,        //结束标志
        isContinuted:true,  //运行方式，连续还是单步
        isPause:false       //暂停标志
    },
    methods:{
        deleteTask(index,row){      //删除作业事件  
            this.taskList.splice(index,1);
        },
        addTask(){                  //添加作业事件
            if(this.newTaskName!=""&&this.newTaskSize!=0)
            {
                this.taskList.push(new Task(this.newTaskName,Number(this.newTaskSize)));       
                this.newTaskName="";
                this.newTaskSize=""; 
            }
        },
        getStartBtnText(){          //根据运行状态改变开始/暂停/继续按钮的文字
            if(this.isStart)
            {
                if(this.isPause) return '继续';
                else return '暂停';
            }
            else return '开始';
        },
        getLogInfo(message){        //将日志消息列表的消息转化为文字
            if(message.getStatus()==ADDSUCCESS)
            {
                return String(message.getName()+'申请 '+message.getData()+' 内存空间成功');
            }
            else if(message.getStatus()==ADDFAILED)
            {
                return String(message.getName()+'申请 '+message.getData()+' 内存空间失败');
            }
            else if(message.getStatus()==REMOVESUCCESS)
            {
                return String(message.getName()+'释放 '+(-message.getData())+' 内存空间成功');
            }
            else 
            {
                return '作业已全部完成';
            }
        },
        onSizeChange(){                 //改变内存大小时动态调整内存模型的大小
            document.getElementById("memory").style.width=String(this.memorySize)+"px";
            useableMem[0].size=this.memorySize;
        },
        onStartClick(){                 //开始/暂停/继续按钮点击事件
            if(!this.isStart)
            {
                this.isStart=true;
                if(this.isContinuted)
                {
                    myTimer=setInterval(nextAssignment,this.speed);
                }
            }
            else
            {
                if(this.isContinuted)
                {
                    if(this.isPause)
                    {
                        this.isPause=false;
                        myTimer=setInterval(nextAssignment,this.speed);
                    }
                    else
                    {
                        this.isPause=true;
                        if(myTimer!=null)
                        {
                            clearInterval(myTimer);
                        }
                    }

                } 
            }

        },
        onNextClick(){          //下一步按钮点击事件
            if(this.isStart)
            {
                nextAssignment();
            }
        },
        onResetClick()          //重置按钮点击事件
        {
            this.isStart=false;
            this.isPause=false;
            this.isEnd=false;
            currentTask=1;
            logList.splice(0,logList.length);
            occupyMem.splice(0,occupyMem.length);
            useableMem.splice(0,useableMem.length);
            useableMem.push(new Mem(0,this.memorySize,-1));
            var mem=document.getElementById("memory");
            mem.innerHTML=''
            if(myTimer!=null)
            {
                clearInterval(myTimer);
            }
        }
    }
});