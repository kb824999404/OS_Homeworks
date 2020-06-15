const EMPTY=-1;
const HIT=1;        //命中，不缺页
const REMEMPTY=2;   //调页，内存未满
const ADJUST=3;    //调页，置换页

/**
 * 内存类
 * @param {内存大小} MemSize
 * @param {指令数} CommandSize
 */
function Memory(MemSize,CommandSize)
{
    this.CommandSize=CommandSize;       
    this.MemSize=MemSize;
    this.block=new Array(MemSize);      //内存块
    this.LRU_Queue=[];                  //最近最少使用队列
    this.runTime=0;                     //运行次数
    this.adjustTime=0;                  //调页次数
    this.commandList=[];                //最近执行的指令
    this.logList=new Array(MemSize);    //日志消息

}


// 返回[low, high]间的随机指令
function getRand(low, high)
{
	// if (high - low == -1) { return high; }		//消除作业中指令访问次序产生high比low小1的问题
	return Math.floor( Math.random() * (high - low) )+ low;
}

//初始化
function Init()
{
    for(var i=0;i<memory.MemSize;i++)
    {
        memory.block[i]=EMPTY;
    }
    memory.block.unshift(memory.block.shift());     //为了更新页面
    memory.runTime=0;                     
    memory.adjustTime=0; 
    memory.LRU_Queue.length=0;
    memory.logList.length=0;
    memory.commandList.length=0;
}


// 请求调页存储管理方式模拟
function Simulate()
{

    if(memory.runTime>=memory.CommandSize)
    {
        app.isEnd=true;
        if(app.isContinuted)
        {
            if(myTimer!=null)
            {
                clearInterval(myTimer);
            }
        }
    }
    else
    {
        nextStep();
        //日志信息滚动到底部
        var logbox=document.getElementById("logBox");
        logbox.scrollTop=logbox.scrollHeight;
    }


}

//生成一条指令并执行
function nextStep()
{
    var aim;
    if(memory.runTime==0)
    {
        //随机选取一个起始指令
        aim = getRand(0, memory.CommandSize - 1);
    }
    else
    {
        aim=memory.commandList[0].address;
        var type=memory.runTime%4;
        switch(type)
        {
            case 1:
            case 3:
                aim++;  //顺序执行下一条指令
                break;
            case 2:
                aim = getRand(0, aim - 1);   //跳转到前地址部分
                break;
            case 0:
                aim = getRand(aim + 1, memory.CommandSize- 1);    //跳转到后地址部分
                break;
        }
    }
    memory.commandList.unshift({id:memory.runTime,address:aim});
    if(memory.commandList.length>4)
    {
        memory.commandList.pop();
    }
    execute(aim); 
    memory.block.unshift(memory.block.shift());     //为了更新页面
    memory.runTime++;
}

/* 执行一条指令
 * @param {待执行指令} aim
*/
function execute(aim)
{

	var page = Math.floor(aim / 10);	//计算页号
	var pos = 0;

    addPosMessage(aim,memory.logList);

    // 在内存中查找该页
    for(pos=0;pos<memory.MemSize;pos++){
        if(memory.block[pos] == page)
        {
            if (app.algorithm == 'LRU')     //LRU算法，页面被访问时，将该页面调整到队列尾部
            {
                var index;
                for(var i=0;i<memory.LRU_Queue.length;i++){
                    if(memory.LRU_Queue[i]==pos)
                    {
                        index=i;
                        break;
                    }
                }
                memory.LRU_Queue.splice(index,1);
                memory.LRU_Queue.push(pos);
            }
            addLoadMessage(page,page,pos,HIT,memory.logList);
            return;
        }
    }
    //缺页
	memory.adjustTime++;		//更新调页次数

	/*检测内存中有无空闲块*/
	for (pos=0;pos<memory.MemSize;pos++)
	{
		if (memory.block[pos] == EMPTY)
		{
			memory.block[pos] = page;

			if (app.algorithm == 'LRU')
			{
				memory.LRU_Queue.push(pos);		//将其压入最近最少使用队列
            }
            
            addLoadMessage(page,page,pos,REMEMPTY,memory.logList);
			return;
		}
	}
    //内存已满，调页置换
	var old = adjust(page);
}


/* 请求调页
 * @returnValue {要被替换掉的页号}
*/
function adjust(page)
{

    var old,pos;
	if (app.algorithm == 'FIFO')
	{
        old=memory.block.shift();             //最先进入的页
        memory.block.push(page);              //新调入的页面
        pos=memory.MemSize-1;
	}
	else
	{
		pos = memory.LRU_Queue.shift();		//将原来页面从队列中删除，重新添加到队列尾部
		memory.LRU_Queue.push(pos);			//将其压入队尾

        old = memory.block[pos];
        
    }
    memory.block[pos]=page;

    addLoadMessage(old,page,pos,ADJUST,memory.logList);

    return old;
}

// 添加指令地址信息
function addPosMessage(aim,logList)
{   
    var mess='第'+memory.runTime+'条指令：物理地址为：'+aim+'，地址空间页号为：'+Math.floor(aim/10)+'，页内第'+aim%10+'条指令';
    logList.push(mess);
}

//添加是否调页信息
function addLoadMessage(oldPage,newPage,pos,status,logList)
{   
    var mess;
    if(status==HIT){                //命中
        mess=oldPage+'号页已经在内存中第'+pos+'号块中了, 未发生调页';
    }
    else if(status==REMEMPTY){       //缺页，内存未满
        mess='缺页，内存未满，将'+newPage+'号页放在内存中第'+pos+'号块中';
    }
    else{
        mess='缺页，内存已满，调出内存中第'+pos+'块中第'+oldPage+'号页, 调入第'+newPage+'号页';
    }
    mess='第'+memory.runTime+'条指令：'+mess;
    logList.push(mess);
}