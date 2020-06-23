
const EMPTY = -1;        //当前块为空
const END = -2;          //结束标识
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

    this.memory =new Array(this.blockNum);                   //内存空间
    this.bitMap =new Array(this.blockNum);                   //位图表
    for (var i = 0; i < this.blockNum; i++)
    {
        this.bitMap[i] = EMPTY;     //初始化位图表为全部可用
        this.memory[i] = "";        //初始化内存内容为空
    }

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

    /*获取所占块数*/
    this.getBlockSize=function(size)
    {
        return Math.ceil(size / this.blockSize);
    }
}

export default VirtualDisk;