/**
 * 作业类
 * @param {作业名称} name 
 * @param {申请/释放的空间大小} data 
 */
function Task(name,data)
{
    this.name=name;
    this.data=data;
    this.getName= function(){
        return this.name;
    }
    this.getData= function(){
        return this.data;
    }
}

/**
 * 分区类
 * @param {起始地址} start 
 * @param {长度} size
 * @param {占有标记} mark
 */
function Mem(start,size,mark)
{
    this.start=start;
    this.size=size;
    this.mark=mark;
    this.getStart=function(){
        return this.start;
    }
    this.getSize=function(){
        return this.size;
    }
    this.getMark=function(){
        return this.mark;
    }
}

/**
 * 消息类
 * @param {任务名} name
 * @param {任务申请内存空间} data
 * @param {起始地址}    start
 * @param {状态} status
 */
function Message(name,data,start,status)
{
    this.name=name;
    this.data=data;
    this.start=start;
    this.status=status;
    this.getName=function(){
        return this.name;
    }
    this.getData=function(){
        return this.data;
    }
    this.getStart=function(){
        return this.start;
    }
    this.getStatus=function(){
        return this.status;
    }
}

/**根据起始地址对Mem对象排序，用于首次适应算法 */
function compareStart(x, y) {
    if (x.getStart() < y.getStart()) {
        return -1;
    } else if (x.getStart() > y.getStart()) {
        return 1;
    } else {
        return 0;
    }
}
/**根据分区大小对Mem对象排序，用于最佳适应算法  */
function compareSize(x, y) {
    if (x.getSize() < y.getSize()) {
        return -1;
    } else if (x.getSize() > y.getSize()) {
        return 1;
    } else {
        return 0;
    }
}


/**
 * 下一步作业调度
 * 内存中添加作业
 * 日志中显示相应的信息
 */
function nextAssignment() {
    if (currentTask < taskList.length) {
        if(Adapt()){//可以装入作业
            currentTask++;
        }
    } else {        //所有作业都完成
        if(!app.isEnd)
        {
            logList.push(new Message('',0,0,TASKEND));
            app.isEnd=true;
            if(app.isContinuted)
            {
                if(myTimer!=null)
                {
                    clearInterval(myTimer);
                }
            }
        }
    }
    //日志信息滚动到底部
    var logbox=document.getElementById("logBox");
    logbox.scrollTop=logbox.scrollHeight;

}


/**适配算法 */
function Adapt() {

    var name=taskList[currentTask].getName();
    var data=taskList[currentTask].getData();

    if (data > 0)   //申请空间
    { 
        if(app.algorithm == 'first')        //首次适应算法
        {
            useableMem.sort(compareStart);
        }
        else if(app.algorithm == 'best')   //最佳适应算法
        {
            useableMem.sort(compareSize);
        }
        for (var i = 0; i < useableMem.length; ++i) {
            if (useableMem[i].getSize() > data)    //第一个能放下的位置
            { 
                start = useableMem[i].getStart();
                size = useableMem[i].getSize();

                occupyMem.push(new Mem(start, data,currentTask));
                occupyMem.sort(compareStart);

                useableMem[i].start += data;
                useableMem[i].size -= data;

                addTask(name, start, data);
                logList.push(new Message(name,data,start,ADDSUCCESS));

                return true;
            }
        }
    } 
    else            //释放空间
    { 
        var task = document.getElementById("task_" + name);
        document.getElementById("memory").removeChild(task); //清除作业块


        for (var i = 0; i < occupyMem.length; ++i) {
            if(taskList[occupyMem[i].getMark()].getName()==taskList[currentTask].getName()) 
            //在已分配区表中找到当前作业占用的分区
            {
                var start=occupyMem[i].getStart();
                var size=occupyMem[i].getSize();
                useableMem.push(new Mem(start, size,-1));   //在空闲区表添加一块新分区
                occupyMem.splice(i,1);
                break;
            }
        }

        updateUseable(); //对空闲分区重新整理

        logList.push(new Message(name,data,0,REMOVESUCCESS));

        return true;
    }

    logList.push(new Message(name,data,0,ADDFAILED));
    return false;
}

/**
 * 添加一个作业块
 * @param {作业名称} name 
 * @param {起始位置} start 
 * @param {作业数据信息} data 
 */
function addTask(name, start, data) {
    var mem=document.getElementById("memory");
    var task = document.createElement("div");
    task.classList.add('memory-task');
    task.id = "task_" + name;
    task.innerText = "\n" + name + "\n" + data + "\n"; //作业块内部显示作业名和作业大小
    task.style.background =  randomBrightness(0.95,1.); //随机配色
    task.style.width = String(data) + "px"; //作业块的宽度为作业大小

    /*实现以内存左端点为基准定位 */
    task.style.marginLeft = String(start) + "px";

    mem.appendChild(task);
}

/**对空闲分区重新整理 */
function updateUseable() {
    useableMem.sort(compareStart)

    var i = 0;
    while (i < useableMem.length) {
        while (i + 1 < useableMem.length &&             //当前空闲块和后面的空闲块可以合并,持续循环合并
            (useableMem[i].getStart() + useableMem[i].getSize() == useableMem[i + 1].getStart())) {
            useableMem[i].size += useableMem[i + 1].getSize();
            useableMem.splice(i + 1, 1);    //合并后删除后面的空闲块
        }
        ++i;
    }
}

//随机配色
function randomBrightness(l,r){
    var brightness=Math.random()*(r-l)+l;
    console.log(brightness);
    var hex=Math.floor(brightness*2003199).toString(16);
    while (hex.length < 6) { 
        hex = '0' + hex;
    }
    return '#'+hex;
}