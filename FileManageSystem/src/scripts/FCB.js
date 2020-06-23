const TXTFILE = 0;   //文本文件标识
const FOLDER = 1;    //文件夹标识

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

export default FCB;