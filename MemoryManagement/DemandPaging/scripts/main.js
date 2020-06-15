

memory=new Memory(4,320);
Init();


myTimer=null;   //定时器

//实例化Vue
app=new Vue({
    el:'#app',
    data:{
        title:"请求调页存储管理方式模拟",
        alogOptions:[
            {
                value:'LRU',
                label:'LRU算法'
            },
            {
                value:'FIFO',
                label:'FIFO算法'
            }
        ],
        algorithm:'LRU',  //选择的算法
        logList:memory.logList,    //日志消息列表
        memBlocks:memory.block,     //内存块
        cmdList:memory.commandList, 
        speed:10,
        simulate:Simulate,
        isStart:false,      //开始标志
        isEnd:false,        //结束标志
        isContinuted:true,  //运行方式，连续还是单步
        isPause:false       //暂停标志
    },
    methods:{
        getMissRate(){              //计算缺页率
            if(memory.runTime==0)
                return 0;
            else
                return (memory.adjustTime*100/memory.runTime).toFixed(2);
        },
        getMissNum(){
            return memory.adjustTime;
        },
        getPage(address){
            return Math.floor(address/10);
        },
        getRange(page){                 //获取地址范围
            if(page==-1)
                return '无';
            else
            {
                return '['+page*10+','+(page+1)*10+')';
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
        onStartClick(){                 //开始/暂停/继续按钮点击事件
            if(!this.isStart)
            {
                this.isStart=true;
                if(this.isContinuted)
                {
                    myTimer=setInterval(this.simulate,this.speed);
                }
            }
            else
            {
                if(this.isContinuted)
                {
                    if(this.isPause)
                    {
                        this.isPause=false;
                        myTimer=setInterval(this.simulate,this.speed);
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
                this.simulate();
            }
        },
        onResetClick()          //重置按钮点击事件
        {
            this.isStart=false;
            this.isPause=false;
            this.isEnd=false;
            Init();
            if(myTimer!=null)
            {
                clearInterval(myTimer);
            }
        },
        tableRowClassName({row, rowIndex}) 
        {
            if (rowIndex === 0) {
                return 'currentCmd';
            } else {
                return '';
            }
        }
    }
});