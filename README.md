﻿# 操作系统课程项目

[TOC]

------

## 项目说明

### 进程管理-电梯调度

1. 每个电梯里面设置必要功能键：如**数字键**、**关门键**、**开门键**、**上行键**、**下行键**、**报警键**、当前电梯的**楼层数**、**上升及下降状态**等。
2. 每层楼的每部电梯门口，应该有**上行和下行按钮**和当前**电梯状态的数码显示器**。
3. 五部电梯门口的**按钮是互联结的**，即当一个电梯按钮按下去时，其他电梯的相应按钮也就同时点亮，表示也按下去了。
4. 所有电梯初始状态都在第一层。每个电梯如果在它的上层或者下层没有相应请求情况下，则应该**在原地保持不动**。

![eleStart](https://cdn.jsdelivr.net/gh/kb824999404/OS_Homeworks/Elevator/Imgs/start.PNG)

![elemain](https://cdn.jsdelivr.net/gh/kb824999404/OS_Homeworks/Elevator/Imgs/mainScene.PNG)

![eleEle](https://cdn.jsdelivr.net/gh/kb824999404/OS_Homeworks/Elevator/Imgs/elevator.PNG)

![elefloor](https://cdn.jsdelivr.net/gh/kb824999404/OS_Homeworks/Elevator/Imgs/floor.PNG)

### 内存管理 - 动态分区分配方式模拟

假设初始态下，可用内存空间为640K，并有下列请求序列，请分别用首次适应算法和最佳适应算法进行内存块的分配和回收，并显示出每次分配和回收后的空闲分区链的情况来。
|   作业        |
| :-----------:|
| 作业1申请130K |
| 作业2申请 60K |
| 作业3申请100k |
| 作业2释放 60K |
| 作业4申请200K |
| 作业3释放100K |
| 作业1释放130K |
| 作业5申请140K |
| 作业6申请 60K |
| 作业7申请 50K |
| 作业6释放 60K |

![DynamicPartition](https://cdn.jsdelivr.net/gh/kb824999404/OS_Homeworks/DynamicPartition/doc-imgs/main.png)

### 内存管理 - 请求调页存储管理方式模拟

- 假设每个页面可存放10条指令，分配给一个作业的内存块为4。模拟一个作业的执行过程，该作业有320条指令，即它的地址空间为32页，目前所有页还没有调入内存。


- 在模拟过程中，如果所访问指令在内存中，则显示其物理地址，并转到下一条指令；如果没有在内存中，则发生缺页，此时需要记录缺页次数，并将其调入内存。如果4个内存块中已装入作业，则需进行页面置换。
- 所有320条指令执行完成后，计算并显示作业执行过程中发生的缺页率。

- 置换算法可以选用FIFO或者LRU算法

- 作业中指令访问次序可以按照下面原则形成：  

  ​    50%的指令是顺序执行的，25%是均匀分布在前地址部分，25％是均匀分布在后地址部分

![DemandPaging](https://cdn.jsdelivr.net/gh/kb824999404/OS_Homeworks/DemandPaging/doc-imgs/main.png)