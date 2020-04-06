一、框架概述 
-------------
1. NBCZ_Admin_Vue是一个前后端分离通用权限系统， 用vs2017+sqlserver2012开发工具。
2. 后端标准三层结构：
   1. Repository（DAL仓储层）使用Dapper.Contrib+Dapper开发。
   2. api使用asp.net webapi,jwt身份认证。
3. 前端
   1. 基于vue的iview框架。
--------  

二、配置使用
-------------------
1. 项目文件基本配置
    1. git clone项目，修改文件夹及sln、csproj、user文件名称为项目命名空间。

    2. 修改sln、csproj内容 将NBCZ修改为项目命名空间。

    3. 用vs打开项目，整个解决方案替换将NBCZ修改为项目命名空间。

2. 数据库配置
    1. 还原数据库db→NBCZ
    2. 配置 DBUtility项目下的DbEntity.ttinclude
    3. 配置web.config项目下的数据库连接

3. 代码生成
    1. 按Model、DAL、BLL的顺序, 分别保存项目文件 T4.DapperExt→后缀为.tt文件。
	
4. 前端
    1. 进入NBCZ.Web.Admin目录，运行命令：npm install、npm run dev。

