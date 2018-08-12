XWidget.JobQueue
=====
[![NuGet](https://img.shields.io/nuget/v/XWidget.JobQueue.svg?style=flat-square)](https://www.nuget.org/packages/XWidget.JobQueue/)
[![NuGet](https://img.shields.io/nuget/dt/XWidget.JobQueue.svg?style=flat-square)](https://www.nuget.org/packages/XWidget.JobQueue/)
[![GitHub](https://img.shields.io/github/license/XuPeiYao/XWidget.svg?style=flat-square)](https://github.com/XuPeiYao/XWidget/blob/master/LICENSE)

提供工作列隊操作

## 快速上手

### 1.建立工作者(即單一工作線程)或工作管理者(多個工作線程)

```csharp
IWorker worker = new Worker();
```
或
```csharp
IWorker worker = new WorkerManager(2); // 設定兩個工作線程，至少為1
```

### 2.加入工作佇列

```csharp
worker.Add(new Job<int>(job=>{
    Thread.Sleep(1000);
    Console.WriteLine(1);
})); // 當工作加入時，立刻執行
worker.Add(new Job<int>(job => {
    throw new NotImplementedException();
}).Subscribe(
    (int x) => { },
    (Exception e) => {
        Console.WriteLine(e);
    },
    () => { })
);
```

### 3.等候工作者工作完成
```csharp
worker.WaitForIdle();
```